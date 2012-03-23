/*
 * TFSServerConfigurationControl.Designer.cs
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
 namespace CCTray.TransportExtension.Tfs2010.Configuration
{
    partial class TFSServerConfigurationControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cboServerName = new System.Windows.Forms.ComboBox();
            this.lblSelectServer = new System.Windows.Forms.Label();
            this.lblSelectProject = new System.Windows.Forms.Label();
            this.cboTeamProject = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // cboServerName
            // 
            this.cboServerName.FormattingEnabled = true;
            this.cboServerName.Location = new System.Drawing.Point(145, 13);
            this.cboServerName.Name = "cboServerName";
            this.cboServerName.Size = new System.Drawing.Size(316, 21);
            this.cboServerName.TabIndex = 0;
            this.cboServerName.SelectedIndexChanged += new System.EventHandler(this.cboServerName_SelectedIndexChanged);
            // 
            // lblSelectServer
            // 
            this.lblSelectServer.AutoSize = true;
            this.lblSelectServer.Location = new System.Drawing.Point(15, 16);
            this.lblSelectServer.Name = "lblSelectServer";
            this.lblSelectServer.Size = new System.Drawing.Size(124, 13);
            this.lblSelectServer.TabIndex = 1;
            this.lblSelectServer.Text = "Select TFS 2010 Server:";
            // 
            // lblSelectProject
            // 
            this.lblSelectProject.AutoSize = true;
            this.lblSelectProject.Location = new System.Drawing.Point(33, 50);
            this.lblSelectProject.Name = "lblSelectProject";
            this.lblSelectProject.Size = new System.Drawing.Size(106, 13);
            this.lblSelectProject.TabIndex = 2;
            this.lblSelectProject.Text = "Select Team Project:";
            // 
            // cboTeamProject
            // 
            this.cboTeamProject.FormattingEnabled = true;
            this.cboTeamProject.Location = new System.Drawing.Point(145, 47);
            this.cboTeamProject.Name = "cboTeamProject";
            this.cboTeamProject.Size = new System.Drawing.Size(316, 21);
            this.cboTeamProject.TabIndex = 3;
            this.cboTeamProject.SelectedIndexChanged += new System.EventHandler(this.cboTeamProject_SelectedIndexChanged);
            // 
            // TFSServerConfigurationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cboTeamProject);
            this.Controls.Add(this.lblSelectProject);
            this.Controls.Add(this.lblSelectServer);
            this.Controls.Add(this.cboServerName);
            this.Name = "TFSServerConfigurationControl";
            this.Size = new System.Drawing.Size(471, 81);
            this.Load += new System.EventHandler(this.TFSServerConfigurationControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboServerName;
        private System.Windows.Forms.Label lblSelectServer;
        private System.Windows.Forms.Label lblSelectProject;
        private System.Windows.Forms.ComboBox cboTeamProject;
    }
}
