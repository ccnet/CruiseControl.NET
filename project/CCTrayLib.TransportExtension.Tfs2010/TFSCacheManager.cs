/*
 * TFSCacheManager.cs
 *
 * Created by Benjamin Gavin <ben@virtual-olympus.com>.
 *
 * Copyright (c) 2010 Benjamin Gavin
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 *
 * Redistributions of source code must retain the above copyright notice,
 * this list of conditions and the following disclaimer.
 *
 * Redistributions in binary form must reproduce the above copyright
 * notice, this list of conditions and the following disclaimer in the
 * documentation and/or other materials provided with the distribution.
 *
 * Neither the name of the project's author nor the names of its
 * contributors may be used to endorse or promote products derived from
 * this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
 * HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
 * TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 */

using System;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using Microsoft.TeamFoundation.Build.Client;

namespace CCTray.TransportExtension.Tfs2010
{
    internal class TFSCacheManager
    {
        private const int CACHE_PROJECT_LIST_TIMEOUT_SECONDS = 15 * 60;
        private const int CACHE_PROJECT_STATUS_TIMEOUT_SECONDS = 2 * 60;
        private const int CHECK_QUEUED_BUILDS_TIMEOUT_SECONDS = 2 * 60;

        private TFSServerManager _manager;
        private DateTime _lastProjectListUpdate = DateTime.MinValue;
        private Dictionary<string, DateTime> _lastQueuedBuildsCheck;
        private DateTime _lastBuildQueueCheck;

        private List<CCTrayProject> _projectListCache;
        private QueueSetSnapshot _buildQueue;

        private List<IBuildDetail> _trackedBuilds;
        private object _trackedBuildLock = new object();

        private Dictionary<string, ProjectStatus> _projectStatusCache;
        private Dictionary<string, DateTime> _projectStatusLastUpdate;

        private object _statusCacheLock = new object();

        public TFSCacheManager(TFSServerManager manager)
        {
            _manager = manager;
            _projectListCache = new List<CCTrayProject>();
            _projectStatusCache = new Dictionary<string, ProjectStatus>();
            _projectStatusLastUpdate = new Dictionary<string, DateTime>();
            _trackedBuilds = new List<IBuildDetail>();
            _lastQueuedBuildsCheck = new Dictionary<string, DateTime>();
        }

        public CruiseServerSnapshot GetCruiseServerSnapshot()
        {
            CheckProjectListCache();
            return BuildCruiseServerSnapshot();
        }

        public void UpdateProjectStatus(IBuildDetail buildDetail)
        {
            // update the project status
            lock (_statusCacheLock)
            {
                ProjectStatus oldStatus = new ProjectStatus();
                if (_projectStatusCache.ContainsKey(buildDetail.BuildDefinition.Name))
                {
                    oldStatus = _projectStatusCache[buildDetail.BuildDefinition.Name];
                    _projectStatusCache.Remove(buildDetail.BuildDefinition.Name);
                    _projectStatusLastUpdate.Remove(buildDetail.BuildDefinition.Name);
                }

                ProjectStatus newStatus = _manager.GetProjectStatus(buildDetail, oldStatus);
                _projectStatusCache.Add(buildDetail.BuildDefinition.Name, newStatus);
                _projectStatusLastUpdate.Add(buildDetail.BuildDefinition.Name, DateTime.Now);
            }

            if (buildDetail.Status == BuildStatus.InProgress)
            {
                DetachFromBuild(buildDetail.BuildDefinition.Name);
                AttachToBuild(buildDetail);
            }
        }

        private void AttachToBuild(IBuildDetail buildDetail)
        {
            lock (_trackedBuildLock)
            {
                // attach to the build
                _trackedBuilds.Add(buildDetail);
                buildDetail.StatusChanged += new StatusChangedEventHandler(Build_StatusChanged);
                buildDetail.Connect();
            }
        }

        private void DetachFromBuild(string buildDefinition)
        {
            lock (_trackedBuildLock)
            {
                if (_trackedBuilds != null)
                {
                    List<IBuildDetail> tmpList = new List<IBuildDetail>(_trackedBuilds);

                    tmpList.ForEach(build =>
                        {
                            if (StringComparer.InvariantCultureIgnoreCase.Compare(build.BuildDefinition.Name, buildDefinition) == 0)
                            {
                                DetachFromBuild(build);
                            }
                        });
                }
            }
        }

        private void DetachFromBuild(IBuildDetail buildDetail)
        {
            lock (_trackedBuildLock)
            {
                buildDetail.StatusChanged -= new StatusChangedEventHandler(Build_StatusChanged);
                buildDetail.Disconnect();
                _trackedBuilds.Remove(buildDetail);
            }
        }

        private void ClearEventHandlers()
        {
            lock (_trackedBuildLock)
            {
                if (_trackedBuilds != null)
                {
                    _trackedBuilds.ForEach(build =>
                        {
                            build.StatusChanged -= new StatusChangedEventHandler(Build_StatusChanged);
                            build.Disconnect();
                        });

                    _trackedBuilds.Clear();
                }
            }
        }

        private CCTrayProject[] AddEventHandlers(IBuildDefinition[] buildDefinitions)
        {
            IQueuedBuildsView queuedBuildsView = _manager.TfsBuildServer.CreateQueuedBuildsView(_manager.Settings.TeamProject);

            List<CCTrayProject> retVal = new List<CCTrayProject>();
            for (int i = 0; i < buildDefinitions.Length; i++)
            {
                //for (int j = 0; j < queuedBuildsView.QueuedBuilds.Length; j++)
                //{
                //    IQueuedBuild queuedBuild = queuedBuildsView.QueuedBuilds[j];
                //    if (queuedBuild.BuildDefinition.Name == buildDefinitions[i].Name && queuedBuild.Build != null)
                //    {
                //        AttachToBuild(queuedBuild.Build);

                //        if (!_lastQueuedBuildsCheck.ContainsKey(buildDefinitions[i].Name))
                //        {
                //            _lastQueuedBuildsCheck.Add(buildDefinitions[i].Name, DateTime.Now);
                //        }
                //        else
                //        {
                //            _lastQueuedBuildsCheck[buildDefinitions[i].Name] = DateTime.Now;
                //        }
                //    }
                //}

                IQueuedBuildSpec queueSpec = _manager.TfsBuildServer.CreateBuildQueueSpec(_manager.Settings.TeamProject);
                queueSpec.DefinitionSpec.Name = buildDefinitions[i].Name;
                queueSpec.QueryOptions = QueryOptions.All;
                queueSpec.Status = QueueStatus.All;

                IQueuedBuildQueryResult queryResult = _manager.TfsBuildServer.QueryQueuedBuilds(queueSpec);
                if (queryResult != null && queryResult.QueuedBuilds.Length > 0)
                {
                    IBuildDetail buildDetail = queryResult.QueuedBuilds[0].Build;
                    if (buildDetail != null)
                    {
                        AttachToBuild(buildDetail);

                        if (!_lastQueuedBuildsCheck.ContainsKey(buildDefinitions[i].Name))
                        {
                            _lastQueuedBuildsCheck.Add(buildDefinitions[i].Name, DateTime.Now);
                        }
                        else
                        {
                            _lastQueuedBuildsCheck[buildDefinitions[i].Name] = DateTime.Now;
                        }
                    }
                }

                retVal.Add(new CCTrayProject(_manager.Configuration, buildDefinitions[i].Name));
            }

            return retVal.ToArray();
        }

        void Build_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            IBuildDetail buildDetail = sender as IBuildDetail;
            if (buildDetail != null && e.Changed)
            {
                // something has changed so we need to update the project status
                if (buildDetail.Status != BuildStatus.InProgress)
                {
                    // we're in some sort of completion state
                    DetachFromBuild(buildDetail);
                }

                lock (_statusCacheLock)
                {
                    ProjectStatus oldStatus = new ProjectStatus();
                    if (_projectStatusCache.ContainsKey(buildDetail.BuildDefinition.Name))
                    {
                        oldStatus = _projectStatusCache[buildDetail.BuildDefinition.Name];
                        _projectStatusCache.Remove(buildDetail.BuildDefinition.Name);
                        _projectStatusLastUpdate.Remove(buildDetail.BuildDefinition.Name);
                    }

                    ProjectStatus newStatus = _manager.GetProjectStatus(buildDetail, oldStatus);
                    _projectStatusCache.Add(buildDetail.BuildDefinition.Name, newStatus);
                    _projectStatusLastUpdate.Add(buildDetail.BuildDefinition.Name, DateTime.Now);
                }

                if (buildDetail.Status != BuildStatus.InProgress)
                {
                    AttachToNextBuild(buildDetail.BuildDefinition.Name);
                }
            }
        }

        private bool AttachToNextBuild(string buildDefinition)
        {
            IQueuedBuildSpec queueSpec = _manager.TfsBuildServer.CreateBuildQueueSpec(_manager.Settings.TeamProject);
            queueSpec.DefinitionSpec.Name = buildDefinition;
            queueSpec.QueryOptions = QueryOptions.All;
            queueSpec.Status = QueueStatus.All;

            // IQueuedBuildsView queuedBuildsView = _manager.TfsBuildServer.CreateQueuedBuildsView(_manager.Settings.TeamProject);

            IQueuedBuildQueryResult queryResult = _manager.TfsBuildServer.QueryQueuedBuilds(queueSpec);
            // if (queuedBuildsView != null && queuedBuildsView.QueuedBuilds.Length > 0)
            if (queryResult != null && queryResult.QueuedBuilds.Length > 0)
            {
                //for (int i = 0; i < queuedBuildsView.QueuedBuilds.Length; i++)
                //{
                //    IQueuedBuild queuedBuild = queuedBuildsView.QueuedBuilds[i];
                //    if (queuedBuild.BuildDefinition.Name == buildDefinition && queuedBuild.Build != null)
                //    {
                //        AttachToBuild(queuedBuild.Build);
                //        return true;   
                //    }
                //}

                IBuildDetail buildDetail = queryResult.QueuedBuilds[0].Build;
                if (buildDetail != null)
                {
                    AttachToBuild(buildDetail);
                    return true;
                }
            }

            return false;
        }

        private void CheckProjectListCache()
        {
            if (_lastProjectListUpdate.AddSeconds(CACHE_PROJECT_LIST_TIMEOUT_SECONDS) < DateTime.Now)
            {
                lock (_statusCacheLock)
                {
                    _projectListCache.Clear();
                    _projectStatusCache.Clear();
                    _projectStatusLastUpdate.Clear();

                    _projectListCache.AddRange(AddEventHandlers(_manager.GetTFSProjectList()));
                    _lastProjectListUpdate = DateTime.Now;
                }
            }
        }

        private void CheckProjectStatusCache()
        {
            lock (_statusCacheLock)
            {
                string[] keys = new string[_projectStatusLastUpdate.Keys.Count];
                _projectStatusLastUpdate.Keys.CopyTo(keys, 0);

                foreach (string key in keys)
                {
                    if (_projectStatusLastUpdate[key].AddSeconds(CACHE_PROJECT_STATUS_TIMEOUT_SECONDS) < DateTime.Now)
                    {
                        _projectStatusLastUpdate.Remove(key);
                        _projectStatusCache.Remove(key);
                    }
                }
            }
        }

        private void CheckBuildQueueCache()
        {
            lock (_statusCacheLock)
            {
                if (_lastBuildQueueCheck.AddSeconds(CHECK_QUEUED_BUILDS_TIMEOUT_SECONDS) < DateTime.Now)
                {
                    if (_buildQueue != null)
                    {
                        _buildQueue.Queues.Clear();
                    }
                }
            }
        }

        private CruiseServerSnapshot BuildCruiseServerSnapshot()
        {
            List<ProjectStatus> projectStatus = new List<ProjectStatus>();

            CheckProjectStatusCache();

            _projectListCache.ForEach(project =>
                {
                    lock (_statusCacheLock)
                    {
                        if (!_projectStatusCache.ContainsKey(project.ProjectName))
                        {
                            _projectStatusCache.Add(project.ProjectName, _manager.GetProjectStatus(project.ProjectName));
                            _projectStatusLastUpdate.Add(project.ProjectName, DateTime.Now);
                        }

                        if (!_lastQueuedBuildsCheck.ContainsKey(project.ProjectName) || _lastQueuedBuildsCheck[project.ProjectName].AddSeconds(CHECK_QUEUED_BUILDS_TIMEOUT_SECONDS) < DateTime.Now)
                        {
                            if (!IsBuildDefinitionTracked(project.ProjectName))
                            {
                                if (AttachToNextBuild(project.ProjectName))
                                {
                                    if (!_lastQueuedBuildsCheck.ContainsKey(project.ProjectName))
                                    {
                                        _lastQueuedBuildsCheck.Add(project.ProjectName, DateTime.Now);
                                    }
                                    else
                                    {
                                        _lastQueuedBuildsCheck[project.ProjectName] = DateTime.Now;
                                    }
                                }
                            }
                        }
                    }

                    projectStatus.Add(_projectStatusCache[project.ProjectName]);
                });

            CheckBuildQueueCache();
            lock (_statusCacheLock)
            {
                if (_buildQueue == null)
                {
                    _buildQueue = new QueueSetSnapshot();
                }

                if (_buildQueue.Queues.Count <= 0)
                {
                    IBuildController[] controllers = _manager.GetBuildControllers();
                    
                    for (int i = 0; i < controllers.Length; i++)
                    {
                        IQueuedBuild[] builds = _manager.GetBuildQueue(controllers[i]);
                        QueueSnapshot queue = new QueueSnapshot(controllers[i].Name);
                        _buildQueue.Queues.Add(queue);

                        if (builds.Length > 0)
                        {
                            for (int j = 0; j < builds.Length; j++)
                            {
                                queue.Requests.Add(new QueuedRequestSnapshot(builds[j].BuildDefinition.Name, TFSServerManager.GetProjectActivity(builds[j].Status), builds[j].QueueTime));
                            }
                        }
                    }

                    _lastBuildQueueCheck = DateTime.Now;
                }
            }

            return new CruiseServerSnapshot(projectStatus.ToArray(), _buildQueue);
        }

        private bool IsBuildDefinitionTracked(string buildDefinition)
        {
            if (_trackedBuilds != null && _trackedBuilds.Count > 0)
            {
                return _trackedBuilds.Exists(
                            build => 
                                StringComparer.InvariantCultureIgnoreCase.Compare(
                                        (build.BuildDefinition != null)?build.BuildDefinition.Name : String.Empty, 
                                        buildDefinition
                                ) == 0
                            );
            }

            return false;
        }
    }
}
