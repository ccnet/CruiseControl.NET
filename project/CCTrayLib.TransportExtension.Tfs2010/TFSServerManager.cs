/*
 * TFSServerManager.cs
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
using System.Linq;
using CCTray.TransportExtension.Tfs2010.Configuration;
using Microsoft.TeamFoundation.Server;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.Remote;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Build.Client;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using Microsoft.TeamFoundation;

namespace CCTray.TransportExtension.Tfs2010
{
    public class TFSServerManager : ICruiseServerManager
    {
        private const int MAX_BUILDS_TO_QUERY = 10;

        private readonly BuildServer _buildServer;
        private TfsConnection _tfsConnection;
        private TfsTeamProjectCollection _teamProjectCollection;
        private IBuildServer _tfsBuildServer;
        private TFSCacheManager _cacheManager;

        private readonly object _tfsServerLock = new object();

        public TFSServerManager(BuildServer buildServer)
        {
            if (buildServer.Transport != BuildServerTransport.Extension ||
                String.IsNullOrEmpty(buildServer.ExtensionSettings))
            {
                throw new ArgumentException("You must specify the TFS Settings to utilize this Server Manager");
            }

            _buildServer = buildServer;
            Settings = TFSServerManagerSettings.GetSettings(buildServer.ExtensionSettings);
        }

        public string ServerUrl
        {
            get { return _buildServer.Url; }
        }

        public string DisplayName
        {
            get { return _buildServer.DisplayName; }
        }

        public BuildServerTransport Transport
        {
            get { return BuildServerTransport.Extension; }
        }

        public void CancelPendingRequest(string projectName)
        {
            throw new NotImplementedException("Cancel request not currently implemented for Team Foundation Servers.");
        }

        public CruiseServerSnapshot GetCruiseServerSnapshot()
        {
            EnsureTfsCacheManager();
            return _cacheManager.GetCruiseServerSnapshot();
        }

        public BuildServer Configuration
        {
            get { return _buildServer; }
        }

        public string SessionToken
        {
            get { return String.Empty; }
        }

        public CCTrayProject[] GetProjectList()
        {
            IBuildDefinitionSpec defSpec = TfsBuildServer.CreateBuildDefinitionSpec(Settings.TeamProject);
            IBuildDefinitionQueryResult query = TfsBuildServer.QueryBuildDefinitions(defSpec);

            if (query != null)
            {
                CCTrayProject[] projects = new CCTrayProject[query.Definitions.Length];

                for (int i = 0; i < query.Definitions.Length; i++)
                {
                    projects[i] = new CCTrayProject()
                    {
                        BuildServer = Configuration,
                        ExtensionName = Configuration.ExtensionName,
                        ExtensionSettings = Configuration.ExtensionSettings,
                        ProjectName = query.Definitions[i].Name,
                        SecuritySettings = Configuration.SecuritySettings,
                        SecurityType = Configuration.SecurityType,
                        ServerUrl = Configuration.Url,
                        ShowProject = query.Definitions[i].Enabled
                    };
                }

                return projects;
            }

            return null;
        }

        public bool Login()
        {
            TfsConnection.Authenticate();
            return TfsConnection.HasAuthenticated;
        }

        public void Logout()
        {
            // no-op
        }

        internal TfsConnection TfsConnection
        {
            get
            {
                EnsureTfsConnection();
                return _tfsConnection;
            }
            set { lock (_tfsServerLock) { _tfsConnection = value; } }
        }
        private void EnsureTfsConnection()
        {
            if (_tfsConnection == null)
            {
                lock (_tfsServerLock)
                {
                    if (_tfsConnection == null)
                    {
                        Uri serverUri = new Uri(Settings.ServerUrl);

                        if (Settings.ServerVersion == TfsServerVersion.Tfs2010)
                        {

                            TfsConfigurationServer configurationServer = TfsConfigurationServerFactory.GetConfigurationServer(serverUri);
                            _tfsConnection = configurationServer;

                            //_teamProjectCollection = new TfsTeamProjectCollection(configurationServer);

                            _teamProjectCollection = new TfsTeamProjectCollection(new Uri(Settings.ServerUrl));

                            //_teamProjectCollection = configurationServer.GetTeamProjectCollection(configurationServer.InstanceId);
                            

                        }
                        if (_tfsConnection == null)
                        {
                            throw new InvalidOperationException("Unable to locate TFS Server");
                        }
                    }
                }
            }
        }

        internal TfsTeamProjectCollection TfsTeamProjectCollection
        {
            get
            {
                EnsureTfsConnection();
                return _teamProjectCollection;
            }
        }

        internal IBuildServer TfsBuildServer
        {
            get {
                EnsureBuildServer();
                return _tfsBuildServer; 
            }
        }
        private void EnsureBuildServer()
        {
            EnsureTfsConnection();
            lock (_tfsServerLock)
            {
                if (_tfsBuildServer == null)
                {
                    _tfsBuildServer = _teamProjectCollection.GetService<IBuildServer>(); 

                    if (_tfsBuildServer == null)
                    {
                        throw new InvalidOperationException("Unable to Retrieve Build Server from TFS");
                    }
                }
            }
        }

        internal TFSServerManagerSettings Settings
        {
            get;
            set;
        }

        internal IBuildDefinition[] GetTFSProjectList()
        {
            IBuildDefinitionSpec defSpec = TfsBuildServer.CreateBuildDefinitionSpec(Settings.TeamProject);
            IBuildDefinitionQueryResult query = TfsBuildServer.QueryBuildDefinitions(defSpec);

            if (query != null)
            {
                return query.Definitions;
            }

            return null;
        }

        internal IBuildController[] GetBuildControllers()
        {
            // QueryBuildControllers without a spec has issues connecting to TFS server 2008 - http://social.msdn.microsoft.com/Forums/en-US/tfsbuild/thread/a8988205-bdbc-491c-8df8-5d69f323f2b4/
            if (TfsBuildServer.BuildServerVersion == BuildServerVersion.V2)
            {
                IBuildControllerSpec bcSpec = TfsBuildServer.CreateBuildControllerSpec();
                bcSpec.Name = String.Format(@"{0}\*", Settings.TeamProject);
                bcSpec.IncludeAgents = true;
                IBuildControllerQueryResult bcQueryResult = TfsBuildServer.QueryBuildControllers(bcSpec);
                return bcQueryResult.Controllers;
            }
            else
            {
                return TfsBuildServer.QueryBuildControllers();
            }
        }

        internal IQueuedBuild[] GetBuildQueue(IBuildController controller)
        {
            List<IQueuedBuild> queuedBuilds = new List<IQueuedBuild>(GetBuildQueue());
            return queuedBuilds.Where(build => build.BuildControllerUri == controller.Uri).ToArray();
        }

        internal IQueuedBuild[] GetBuildQueue()
        {
            IQueuedBuildSpec queueSpec = TfsBuildServer.CreateBuildQueueSpec(Settings.TeamProject);
            IQueuedBuildQueryResult result = TfsBuildServer.QueryQueuedBuilds(queueSpec);

            return result.QueuedBuilds;
        }

        public ProjectStatusSnapshot GetProjectStatusSnapshot(string projectName)
        {
            IBuildDetailSpec querySpec = TfsBuildServer.CreateBuildDetailSpec(
                                            Settings.TeamProject,
                                            projectName);
            querySpec.MaxBuildsPerDefinition = MAX_BUILDS_TO_QUERY;
            querySpec.QueryOrder = BuildQueryOrder.FinishTimeDescending;

            IBuildQueryResult queryResult = TfsBuildServer.QueryBuilds(querySpec);
            if (queryResult != null)
            {
                IBuildDetail[] buildDetails = queryResult.Builds;
                if (buildDetails.Length > 0)
                {
                    IBuildDetail successfulBuild = GetLastSuccessfulBuild(buildDetails);
                    IBuildDetail currentBuild = GetLastNonStoppedBuild(buildDetails);

                    ProjectStatusSnapshot tmpStatus = new ProjectStatusSnapshot()
                    {
                        Description = GetProjectCategory(projectName),
                        Error = (currentBuild.Status == BuildStatus.Failed || currentBuild.Status == BuildStatus.Failed) ? GetBuildStage(currentBuild) : String.Empty,
                        Name = projectName,
                        Status = GetItemBuildStatus(currentBuild.Status),
                        TimeCompleted = currentBuild.FinishTime,
                        TimeOfSnapshot = DateTime.Now,
                        TimeStarted = currentBuild.StartTime
                    };

                    return tmpStatus;

                }
            }

            return new ProjectStatusSnapshot() { Name = projectName, Status = ItemBuildStatus.Unknown };
        }
        
        public ProjectStatus GetProjectStatus(string projectName)
        {
            IBuildDetailSpec querySpec = TfsBuildServer.CreateBuildDetailSpec(
                                            Settings.TeamProject,
                                            projectName);
            querySpec.MaxBuildsPerDefinition = MAX_BUILDS_TO_QUERY;
            querySpec.QueryOrder = BuildQueryOrder.FinishTimeDescending;

            IBuildQueryResult queryResult = TfsBuildServer.QueryBuilds(querySpec);
            if (queryResult != null)
            {
                IBuildDetail[] buildDetails = queryResult.Builds;
                if(buildDetails.Length > 0)
                {
                    IBuildDetail successfulBuild = GetLastSuccessfulBuild(buildDetails);
                    IBuildDetail currentBuild = GetLastNonStoppedBuild(buildDetails);

                    ProjectStatus tmpStatus = new ProjectStatus(
                        projectName,
                        GetProjectCategory(projectName),
                        GetProjectActivity(currentBuild.Status),
                        GetIntegrationStatus(currentBuild.Status),
                        GetProjectIntegratorState(currentBuild.Status),
                        GetBuildStatusUrl(currentBuild),
                        GetBuildTime(currentBuild),
                        currentBuild.BuildNumber,
                        (successfulBuild != null) ? successfulBuild.BuildNumber : String.Empty,
                        GetNextBuildDate(currentBuild),
                        GetBuildStage(currentBuild),
                        "", // TODO: need the build agent name?
                        0
                        );

                    tmpStatus.Messages = GetBuildMessages(buildDetails[0]);

                    return tmpStatus;

                }
            }

            return new ProjectStatus(projectName, IntegrationStatus.Unknown, DateTime.MinValue);
        }

        public ProjectStatus GetProjectStatus(IBuildDetail buildDetail, ProjectStatus previousStatus)
        {
            bool successfulBuild = (buildDetail.Status == BuildStatus.Succeeded);

            string queue = (buildDetail.BuildController == null) ? buildDetail.BuildControllerUri.ToString() : buildDetail.BuildController.Name;
            
            ProjectStatus tmpStatus = new ProjectStatus(
                buildDetail.BuildDefinition.Name,
                GetProjectCategory(buildDetail.BuildDefinition.Name),
                GetProjectActivity(buildDetail.Status),
                GetIntegrationStatus(buildDetail.Status),
                GetProjectIntegratorState(buildDetail.Status),
                GetBuildStatusUrl(buildDetail),
                GetBuildTime(buildDetail),
                buildDetail.BuildNumber,
                (successfulBuild) ? buildDetail.BuildNumber : previousStatus.LastSuccessfulBuildLabel,
                GetNextBuildDate(buildDetail),
                GetBuildStage(buildDetail),
                queue,
                0
                );

            tmpStatus.Messages = GetBuildMessages(buildDetail);

            return tmpStatus;
        }

        public void UpdateProjectStatus(IBuildDetail buildDetail)
        {
            EnsureTfsCacheManager();
            _cacheManager.UpdateProjectStatus(buildDetail);
        }

        private void EnsureTfsCacheManager()
        {
            EnsureTfsConnection();

            lock (_tfsServerLock)
            {
                if (_cacheManager == null)
                {
                    _cacheManager = new TFSCacheManager(this);
                }
            }
        }

        private static IntegrationStatus GetIntegrationStatus(BuildStatus buildStatus)
        {
            switch (buildStatus)
            {
                case BuildStatus.Failed: return IntegrationStatus.Failure;
                case BuildStatus.InProgress: return IntegrationStatus.Unknown;
                case BuildStatus.NotStarted: return IntegrationStatus.Unknown;
                case BuildStatus.PartiallySucceeded: return IntegrationStatus.Exception;
                case BuildStatus.Stopped: return IntegrationStatus.Unknown;
                case BuildStatus.Succeeded: return IntegrationStatus.Success;

                default: return IntegrationStatus.Unknown;
            }
        }

        private static string GetProjectCategory(string buildLabel)
        {
            string normLabel = buildLabel.ToLower();
            if (normLabel.Contains(" ci"))
            {
                return "Continuous Integration";
            }
            else if (normLabel.Contains("msi"))
            {
                return "Installer (MSI)";
            }
            else
            {
                return "Unknown";
            }
        }

        internal static ProjectActivity GetProjectActivity(BuildStatus buildStatus)
        {
            switch (buildStatus)
            {
                case BuildStatus.Failed:
                case BuildStatus.NotStarted:
                case BuildStatus.PartiallySucceeded:
                case BuildStatus.Stopped:
                case BuildStatus.Succeeded:
                    return ProjectActivity.Sleeping;

                case BuildStatus.InProgress:
                    return ProjectActivity.Building;

                default:
                    return ProjectActivity.Pending;

            }
        }

        internal static ProjectActivity GetProjectActivity(QueueStatus queueStatus)
        {
            switch (queueStatus)
            {
                case QueueStatus.Completed:
                case QueueStatus.Canceled:
                case QueueStatus.Postponed:
                    return ProjectActivity.Sleeping;

                case QueueStatus.InProgress:
                    return ProjectActivity.Building;

                default:
                    return ProjectActivity.Pending;

            }
        }

        private static ItemBuildStatus GetItemBuildStatus(BuildStatus buildStatus)
        {
            switch (buildStatus)
            {
                case BuildStatus.Failed:
                    return ItemBuildStatus.CompletedFailed;

                case BuildStatus.NotStarted:
                    return ItemBuildStatus.Pending;

                case BuildStatus.PartiallySucceeded:
                    return ItemBuildStatus.CompletedFailed;

                case BuildStatus.Stopped:
                    return ItemBuildStatus.Cancelled;

                case BuildStatus.Succeeded:
                    return ItemBuildStatus.CompletedSuccess;

                case BuildStatus.InProgress:
                    return ItemBuildStatus.Running;

                default:
                    return ItemBuildStatus.Unknown;

            }
        }
        
        private static ProjectIntegratorState GetProjectIntegratorState(BuildStatus buildStatus)
        {
            switch (buildStatus)
            {
                case BuildStatus.Failed:
                case BuildStatus.NotStarted:
                case BuildStatus.PartiallySucceeded:
                case BuildStatus.Stopped:
                case BuildStatus.Succeeded:
                    return ProjectIntegratorState.Stopped;

                case BuildStatus.InProgress:
                    return ProjectIntegratorState.Running;

                default:
                    return ProjectIntegratorState.Stopping;

            }
        }

        private static string GetBuildStage(IBuildDetail buildDetail)
        {
            if(buildDetail.Information.Nodes.Length > 0)
            {
                return buildDetail.Information.Nodes[buildDetail.Information.Nodes.Length - 1].ToString();
            }
            else
            {
                return "No Information Available";
            }
        }

        private static Message[] GetBuildMessages(IBuildDetail buildDetail)
        {
            List<Message> retVal = new List<Message>();
            List<IBuildStep> buildSteps = InformationNodeConverters.GetBuildSteps(buildDetail);

            if (buildSteps != null)
            {
                buildSteps.ForEach(step => retVal.Add(new Message(GetBuildStepMessage(step))));
            }

            return retVal.ToArray();
        }

        private static string GetBuildStepMessage(IBuildStep buildStep)
        {
            return String.Format("{0}: {1:dd-MMM-yyyy HH:mm:ss} - {2}",
                                 buildStep.Status,
                                 buildStep.StartTime,
                                 buildStep.Message);
        }

        private string GetBuildStatusUrl(IBuildDetail buildDetail)
        {
            ILinking linking = _teamProjectCollection.GetService<ILinking>();

            return linking.GetArtifactUrlExternal(buildDetail.Uri.ToString());
        }

        private static IBuildDetail GetLastSuccessfulBuild(IBuildDetail[] buildDetails)
        {
            for (int i = 0; i < buildDetails.Length; i++)
            {
                if (buildDetails[i].Status == BuildStatus.Succeeded)
                {
                    return buildDetails[i];
                }
            }

            return null;
        }

        private static DateTime GetNextBuildDate(IBuildDetail buildDetail)
        {
            IBuildDefinition def = buildDetail.BuildDefinition;
            DateTime nextDate;

            if (def.ContinuousIntegrationType == ContinuousIntegrationType.Individual)
            {
                nextDate = DateTime.Now;
            }
            else if (def.ContinuousIntegrationType == ContinuousIntegrationType.Batch)
            {
                if (def.ContinuousIntegrationQuietPeriod > 0)
                {
                    if (buildDetail.Status != BuildStatus.InProgress)
                    {
                        nextDate = buildDetail.FinishTime.AddMinutes(def.ContinuousIntegrationQuietPeriod);
                    }
                    else
                    {
                        nextDate = buildDetail.StartTime.AddMinutes(def.ContinuousIntegrationQuietPeriod);
                    }
                }
                else
                {
                    nextDate = DateTime.Now;
                }
            }
            else if (def.ContinuousIntegrationType == ContinuousIntegrationType.None)
            {
                nextDate = DateTime.MaxValue;
            }
            else
            {
                // find the next place in the schedule when the build will run...
                if (def.Schedules != null && def.Schedules.Count > 0)
                {
                    DateTime minDate = DateTime.MaxValue;
                    def.Schedules.ForEach(schedule =>
                        {
                            DateTime startDate = GetNextScheduledDay(schedule.DaysToBuild);
                            startDate.AddSeconds(schedule.StartTime);

                            if (minDate > startDate)
                            {
                                minDate = startDate;
                            }
                        });

                    nextDate = minDate;
                }
                else
                {
                    nextDate = DateTime.MaxValue;
                }
            }

            if (nextDate < DateTime.Now)
            {
                return DateTime.Now;
            }
            else
            {
                return nextDate;
            }
        }

        private static DateTime GetNextScheduledDay(ScheduleDays scheduledDays)
        {
            DateTime nextDate = DateTime.Now.Date;
            if((scheduledDays & ScheduleDays.All) == ScheduleDays.All)
            {
                return nextDate;
            }

            // check for a week
            for (int i = 0; i < 7; i++)
            {
                if (i != 0)
                {
                    nextDate = nextDate.AddDays(1);
                }

                ScheduleDays curDay = ConvertToScheduleDays(nextDate.DayOfWeek);
                if ((scheduledDays & curDay) == curDay)
                {
                    return nextDate;
                }
            }

            return DateTime.MaxValue.Date;
        }

        private static ScheduleDays ConvertToScheduleDays(DayOfWeek day)
        {
            return (ScheduleDays)Enum.Parse(typeof(ScheduleDays), day.ToString());
        }

        private static DateTime GetBuildTime(IBuildDetail buildDetail)
        {
            if (buildDetail.Status != BuildStatus.InProgress)
            {
                return buildDetail.FinishTime;
            }
            else
            {
                return buildDetail.StartTime;
            }
        }

        private static IBuildDetail GetLastNonStoppedBuild(IBuildDetail[] buildDetails)
        {
            for (int i = 0; i < buildDetails.Length; i++)
            {
                if (buildDetails[i].Status != BuildStatus.Stopped)
                {
                    return buildDetails[i];
                }
            }

            return buildDetails[0];
        }
    }
}
