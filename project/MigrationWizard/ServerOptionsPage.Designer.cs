namespace ThoughtWorks.CruiseControl.MigrationWizard
{
    partial class ServerOptionsPage
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerOptionsPage));
            this.migrateServer = new System.Windows.Forms.CheckBox();
            this.settingsPanel = new System.Windows.Forms.GroupBox();
            this.selectLocationButton = new System.Windows.Forms.Button();
            this.settingsLocation = new System.Windows.Forms.TextBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.backupFile = new System.Windows.Forms.CheckBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            this.settingsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(4, 22);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(88, 13);
            label1.TabIndex = 0;
            label1.Text = "Current Location:";
            // 
            // label2
            // 
            label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            label2.Location = new System.Drawing.Point(6, 65);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(526, 156);
            label2.TabIndex = 3;
            label2.Text = resources.GetString("label2.Text");
            // 
            // migrateServer
            // 
            this.migrateServer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.migrateServer.AutoSize = true;
            this.migrateServer.Checked = true;
            this.migrateServer.CheckState = System.Windows.Forms.CheckState.Checked;
            this.migrateServer.Location = new System.Drawing.Point(10, 54);
            this.migrateServer.Name = "migrateServer";
            this.migrateServer.Size = new System.Drawing.Size(132, 17);
            this.migrateServer.TabIndex = 0;
            this.migrateServer.Text = "Migrate server settings";
            this.migrateServer.UseVisualStyleBackColor = true;
            this.migrateServer.CheckedChanged += new System.EventHandler(this.migrateServer_CheckedChanged);
            // 
            // settingsPanel
            // 
            this.settingsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsPanel.Controls.Add(this.backupFile);
            this.settingsPanel.Controls.Add(label2);
            this.settingsPanel.Controls.Add(this.selectLocationButton);
            this.settingsPanel.Controls.Add(this.settingsLocation);
            this.settingsPanel.Controls.Add(label1);
            this.settingsPanel.Location = new System.Drawing.Point(3, 55);
            this.settingsPanel.Name = "settingsPanel";
            this.settingsPanel.Size = new System.Drawing.Size(538, 224);
            this.settingsPanel.TabIndex = 1;
            this.settingsPanel.TabStop = false;
            // 
            // selectLocationButton
            // 
            this.selectLocationButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.selectLocationButton.Location = new System.Drawing.Point(486, 19);
            this.selectLocationButton.Name = "selectLocationButton";
            this.selectLocationButton.Size = new System.Drawing.Size(23, 23);
            this.selectLocationButton.TabIndex = 2;
            this.selectLocationButton.Text = "…";
            this.selectLocationButton.UseVisualStyleBackColor = true;
            this.selectLocationButton.Click += new System.EventHandler(this.selectLocationButton_Click);
            // 
            // settingsLocation
            // 
            this.settingsLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsLocation.Location = new System.Drawing.Point(98, 19);
            this.settingsLocation.Name = "settingsLocation";
            this.settingsLocation.Size = new System.Drawing.Size(382, 20);
            this.settingsLocation.TabIndex = 1;
            this.settingsLocation.TextChanged += new System.EventHandler(this.settingsLocation_TextChanged);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // backupFile
            // 
            this.backupFile.AutoSize = true;
            this.backupFile.Checked = true;
            this.backupFile.CheckState = System.Windows.Forms.CheckState.Checked;
            this.backupFile.Location = new System.Drawing.Point(98, 45);
            this.backupFile.Name = "backupFile";
            this.backupFile.Size = new System.Drawing.Size(244, 17);
            this.backupFile.TabIndex = 5;
            this.backupFile.Text = "Back up the configuration file before modifying";
            this.backupFile.UseVisualStyleBackColor = true;
            // 
            // ServerOptionsPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.migrateServer);
            this.Controls.Add(this.settingsPanel);
            this.HeaderText = "What do you want to migrate for the server?";
            this.HeaderTitle = "Server Migration";
            this.Name = "ServerOptionsPage";
            this.Size = new System.Drawing.Size(544, 282);
            this.settingsPanel.ResumeLayout(false);
            this.settingsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox migrateServer;
        private System.Windows.Forms.GroupBox settingsPanel;
        private System.Windows.Forms.Button selectLocationButton;
        private System.Windows.Forms.TextBox settingsLocation;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.CheckBox backupFile;
    }
}
