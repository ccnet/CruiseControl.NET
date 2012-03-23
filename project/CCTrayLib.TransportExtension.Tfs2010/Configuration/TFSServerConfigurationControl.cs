/*
 * TFSServerConfigurationControl.cs
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
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Server;
using System.Linq;

namespace CCTray.TransportExtension.Tfs2010.Configuration
{
    internal partial class TFSServerConfigurationControl : UserControl
    {
        private TFSServerManagerSettings _settings;
        public TFSServerManagerSettings Settings {
            get
            {
                EnsureSettings();
                return _settings;
            }
            set
            {
                _settings = value;
            }
        }

        private void EnsureSettings()
        {
            if (_settings == null)
            {
                _settings = new TFSServerManagerSettings();
                if (cboServerName.SelectedIndex >= 0)
                {
                    _settings.ServerUrl = cboServerName.SelectedItem.ToString();
                }
                if (cboTeamProject.SelectedIndex >= 0)
                {
                    _settings.TeamProject = cboTeamProject.SelectedItem.ToString();
                }
            }
        }

        public TFSServerConfigurationControl()
        {
            InitializeComponent();
        }

        private void cboServerName_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor tmpCursor = Cursor;
            try
            {
                Cursor = Cursors.WaitCursor;
                if (cboServerName.SelectedIndex >= 0)
                {
                    SelectTeamServer(cboServerName.SelectedItem.ToString());
                }
                else
                {
                    Settings.ServerUrl = String.Empty;
                    Settings.TeamProject = String.Empty;

                    cboTeamProject.Enabled = false;
                    cboTeamProject.Items.Clear();
                }
            }
            finally
            {
                Cursor = tmpCursor;
            }
        }

        private void cboTeamProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboTeamProject.SelectedIndex >= 0)
            {
                Settings.TeamProject = cboTeamProject.SelectedItem.ToString();
            }
            else
            {
                Settings.TeamProject = String.Empty;
            }
        }

        private void TFSServerConfigurationControl_Load(object sender, EventArgs e)
        {
            LoadTeamServers();

            if (!String.IsNullOrEmpty(Settings.ServerUrl))
            {
                cboServerName.SelectedIndex = cboServerName.FindString(Settings.ServerUrl);
            }

            if (!String.IsNullOrEmpty(Settings.TeamProject))
            {
                cboTeamProject.SelectedIndex = cboTeamProject.FindString(Settings.TeamProject);
                cboTeamProject.Enabled = true;
            }
        }

        private void LoadTeamServers()
        {
            Cursor tmpCursor = Cursor;
            try
            {
                Cursor = Cursors.WaitCursor;

                Settings.ServerVersion = TfsServerVersion.NotSet;

                cboServerName.Items.Clear();
                cboServerName.Text = String.Empty;

                cboTeamProject.Items.Clear();
                cboTeamProject.Text = String.Empty;
                cboTeamProject.Enabled = false;

                Settings.ServerVersion = TfsServerVersion.Tfs2010;
                cboServerName.Enabled = true;

                RegisteredConfigurationServer[] servers = RegisteredTfsConnections.GetConfigurationServers();
                
                foreach (RegisteredConfigurationServer server in servers)
                {
                    cboServerName.Items.Add(server.Uri);
                }

                if (cboServerName.Items.Count == 1)
                {
                    cboServerName.SelectedIndex = 0;
                    SelectTeamServer(cboServerName.SelectedItem.ToString());
                }
            }
            finally
            {
                Cursor = tmpCursor;
            }
        }

        private void LoadTeamProjects(string serverUrl)
        {
            cboTeamProject.Items.Clear();
            cboTeamProject.Text = String.Empty;

            List<string> projectNames = GetTeamProjectNames().ToList();

            projectNames.ForEach(p => cboTeamProject.Items.Add(p));
        }

        private IEnumerable<string> GetTeamProjectNames()
        {
            var structService = new TfsTeamProjectCollection(new Uri(Settings.ServerUrl)).GetService<ICommonStructureService>();

            foreach (var p in structService.ListAllProjects())
                yield return p.Name;
        }

        private void SelectTeamServer(string serverUrl)
        {
            Settings.ServerUrl = serverUrl;

            LoadTeamProjects(Settings.ServerUrl);

            cboTeamProject.Enabled = true;
        }
    }
}
