/*
 * TFSTransportExtension.cs
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
using CCTray.TransportExtension.Tfs2010.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace CCTray.TransportExtension.Tfs2010
{
    public class TFSTransportExtension : ITransportExtension
    {
        internal TFSServerManagerSettings TFSSettings
        {
            get;
            set;
        }

        private TFSServerManager _serverManager;
        private object _serverManagerLock = new object();
        internal TFSServerManager ServerManager
        {
            get
            {
                EnsureServerManager();
                return _serverManager;
            }
        }

        private void EnsureServerManager()
        {
            if (_serverManager == null)
            {
                lock (_serverManagerLock)
                {
                    if (_serverManager == null)
                    {
                        _serverManager = new TFSServerManager(Configuration);
                    }
                }
            }
        }

        private Dictionary<string, TFSProjectManager> _projectManagers = new Dictionary<string, TFSProjectManager>();
        private object _projectManagersLock = new object();
        internal Dictionary<string, TFSProjectManager> ProjectManagers
        {
            get { return _projectManagers; }
        }
        private void EnsureProjectManager(string projectName)
        {
            if (!_projectManagers.ContainsKey(projectName))
            {
                lock (_projectManagersLock)
                {
                    if (!_projectManagers.ContainsKey(projectName))
                    {
                        _projectManagers.Add(projectName, new TFSProjectManager(projectName, ServerManager));
                    }
                }
            }
        }

        public string DisplayName
        {
            get { return "Team Foundation Server Transport Extension"; }
        }

        private string _settingsData = String.Empty;
        public string Settings
        {
            get
            {
                return GetTFSSettings();
            }
            set
            {
                if (_settingsData != value)
                {
                    _settingsData = value;
                    TFSSettings = TFSServerManagerSettings.GetSettings(value);
                }
            }
        }

        private BuildServer _buildServer;
        private object _buildServerLock = new object();
        public BuildServer Configuration
        {
            get
            {
                EnsureBuildServer();
                return _buildServer;
            }
            set
            {
                lock (_buildServerLock)
                {
                    _buildServer = value;
                }
            }
        }

        private void EnsureBuildServer()
        {
            if (_buildServer == null)
            {
                lock (_buildServerLock)
                {
                    if (_buildServer == null)
                    {
                        _buildServer = new BuildServer(TFSTransportExtension.GetBuildServerUrl(TFSSettings), BuildServerTransport.Extension, DisplayName, Settings);
                    }
                }
            }
        }

        public CCTrayProject[] GetProjectList(BuildServer server)
        {
            TFSServerManager mgr = new TFSServerManager(server);
            return mgr.GetProjectList();
        }

        public ICruiseProjectManager RetrieveProjectManager(string projectName)
        {
            EnsureProjectManager(projectName);
            return _projectManagers[projectName];
        }

        public ICruiseServerManager RetrieveServerManager()
        {
            return ServerManager;
        }

        public bool Configure(System.Windows.Forms.IWin32Window owner)
        {
            if (TFSSettings == null)
            {
                TFSSettings = new TFSServerManagerSettings();
            }

            TFSServerConfigurationDialog dlg = new TFSServerConfigurationDialog(TFSSettings);

            if (dlg.ShowDialog(owner) == System.Windows.Forms.DialogResult.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private string GetTFSSettings()
        {
            if (TFSSettings == null)
            {
                return String.Empty;
            }
            else
            {
                return TFSSettings.ToString();
            }
        }

        private static string GetBuildServerUrl(TFSServerManagerSettings settings)
        {
            Uri serverUri = new Uri(settings.ServerUrl);
            return String.Format("tfs://{0}/{1}", serverUri.Host, settings.TeamProject);
        }
    }
}
