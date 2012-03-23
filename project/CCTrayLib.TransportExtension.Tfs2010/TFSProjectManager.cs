/*
 * TFSProjectManager.cs
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
using System.Windows.Forms;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Build.Controls;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace CCTray.TransportExtension.Tfs2010
{
    public class TFSProjectManager : ICruiseProjectManager
    {
        private TFSServerManager _serverManager;
        private string _projectName;

        public TFSProjectManager(string projectName, ICruiseServerManager serverManager)
        {
            _projectName = projectName;
            _serverManager = serverManager as TFSServerManager;
        }

        public void ForceBuild(string sessionToken, Dictionary<string, string> parameters, string userName)
        {
            System.Reflection.Assembly dlgAsm = typeof(BuildPolicy).Assembly;
            Type dlgType = dlgAsm.GetType("Microsoft.TeamFoundation.Build.Controls.DialogQueueBuild", false);
            if (dlgType != null)
            {
                Form buildDlg = default(Form);
                try
                {
                    string teamProject = _serverManager.Settings.TeamProject;
                    IBuildDefinition buildDefinition = _serverManager.TfsBuildServer.GetBuildDefinition(teamProject, _projectName);
                    
                    List<IBuildAgent> agents = new List<IBuildAgent>();

                    IBuildController[] controllers = _serverManager.GetBuildControllers();
                    foreach (IBuildController controller in controllers)
                    {
                        foreach (IBuildAgent agent in controller.Agents)
                        {
                            if (agent.TeamProject == teamProject)
                            {
                                agents.Add(agent);
                            }
                        }
                    }

                    object[] args = new object[] { teamProject, new IBuildDefinition[] { buildDefinition }, 0, controllers, _serverManager.TfsTeamProjectCollection };

                    buildDlg = dlgAsm.CreateInstance(dlgType.FullName, true, System.Reflection.BindingFlags.CreateInstance, null, args, null, null) as Form;
                    
                    if (buildDlg.ShowDialog() == DialogResult.OK)
                    {
                        IBuildDetailSpec querySpec = _serverManager.TfsBuildServer.CreateBuildDetailSpec(
                                                        teamProject,
                                                        _projectName);
                        querySpec.MaxBuildsPerDefinition = 1;
                        querySpec.Status = BuildStatus.InProgress;

                        IBuildQueryResult queryResult = _serverManager.TfsBuildServer.QueryBuilds(querySpec);
                        if (queryResult != null && queryResult.Builds.Length > 0)
                        {
                            _serverManager.UpdateProjectStatus(queryResult.Builds[0]);
                        }
                    }
                }
                finally
                {
                    if (buildDlg != null)
                    {
                        buildDlg.Dispose();
                    }
                }
            }
            else
            {
                MessageBox.Show("Unable to locate build dialog");
            }
        }

        public void FixBuild(string sessionToken, string fixingUserName)
        {
            MessageBox.Show("Fix Build not supported by TFS managed projects");
        }

        public void AbortBuild(string sessionToken, string userName)
        {
            MessageBox.Show("Abort Build not supported by TFS managed projects");
        }

        public void AbortBuild(string sessionToken)
        {
            // retrieve the current build
            IBuildDetailSpec querySpec = _serverManager.TfsBuildServer.CreateBuildDetailSpec(
                                            _serverManager.Settings.TeamProject,
                                            _projectName);
            querySpec.MaxBuildsPerDefinition = 1;
            querySpec.Status = BuildStatus.InProgress;

            IBuildQueryResult queryResult = _serverManager.TfsBuildServer.QueryBuilds(querySpec);
            if (queryResult != null && queryResult.Builds.Length > 0)
            {
                queryResult.Builds[0].Stop();
                queryResult.Builds[0].RefreshAllDetails();
                _serverManager.UpdateProjectStatus(queryResult.Builds[0]);
            }
        }

        public void StopProject(string sessionToken)
        {
            AbortBuild(sessionToken);
        }

        public void StartProject(string sessionToken)
        {
            ForceBuild(sessionToken, null, null);
        }

        public void CancelPendingRequest(string sessionToken)
        {
            //no-op
        }

        public string ProjectName
        {
            get { return _projectName; }
        }

        public ProjectStatusSnapshot RetrieveSnapshot()
        {
            return _serverManager.GetProjectStatusSnapshot(ProjectName);
        }

        public PackageDetails[] RetrievePackageList()
        {
            throw new NotImplementedException("No Package List Available for TFS Projects");
        }

        public IFileTransfer RetrieveFileTransfer(string fileName)
        {
            throw new NotImplementedException("No File Transfers Available for TFS Projects");
        }

        public List<ParameterBase> ListBuildParameters()
        {
            return new List<ParameterBase>();
        }
    }
}
