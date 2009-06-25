namespace ThoughtWorks.CruiseControl.MigrationWizard
{
    partial class WebDashboardOptionsPage
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
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label1;
            this.migrateDashboard = new System.Windows.Forms.CheckBox();
            this.settingsPanel = new System.Windows.Forms.GroupBox();
            this.selectLocationButton = new System.Windows.Forms.Button();
            this.settingsLocation = new System.Windows.Forms.TextBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            this.settingsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            label2.Location = new System.Drawing.Point(8, 51);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(638, 209);
            label2.TabIndex = 7;
            label2.Text = "This will migrate the following files:\r\n* Configuration files\r\n* Packages and pac" +
                "kage configuration files";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(6, 26);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(88, 13);
            label1.TabIndex = 4;
            label1.Text = "Current Location:";
            // 
            // migrateDashboard
            // 
            this.migrateDashboard.AutoSize = true;
            this.migrateDashboard.Checked = true;
            this.migrateDashboard.CheckState = System.Windows.Forms.CheckState.Checked;
            this.migrateDashboard.Location = new System.Drawing.Point(10, 55);
            this.migrateDashboard.Name = "migrateDashboard";
            this.migrateDashboard.Size = new System.Drawing.Size(181, 17);
            this.migrateDashboard.TabIndex = 2;
            this.migrateDashboard.Text = "Migrate Web Dashboard settings";
            this.migrateDashboard.UseVisualStyleBackColor = true;
            this.migrateDashboard.CheckedChanged += new System.EventHandler(this.migrateDashboard_CheckedChanged);
            // 
            // settingsPanel
            // 
            this.settingsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsPanel.Controls.Add(label2);
            this.settingsPanel.Controls.Add(this.selectLocationButton);
            this.settingsPanel.Controls.Add(this.settingsLocation);
            this.settingsPanel.Controls.Add(label1);
            this.settingsPanel.Location = new System.Drawing.Point(3, 55);
            this.settingsPanel.Name = "settingsPanel";
            this.settingsPanel.Size = new System.Drawing.Size(652, 263);
            this.settingsPanel.TabIndex = 3;
            this.settingsPanel.TabStop = false;
            // 
            // selectLocationButton
            // 
            this.selectLocationButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.selectLocationButton.Location = new System.Drawing.Point(598, 21);
            this.selectLocationButton.Name = "selectLocationButton";
            this.selectLocationButton.Size = new System.Drawing.Size(23, 23);
            this.selectLocationButton.TabIndex = 6;
            this.selectLocationButton.Text = "…";
            this.selectLocationButton.UseVisualStyleBackColor = true;
            this.selectLocationButton.Click += new System.EventHandler(this.selectLocationButton_Click);
            // 
            // settingsLocation
            // 
            this.settingsLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsLocation.Location = new System.Drawing.Point(100, 23);
            this.settingsLocation.Name = "settingsLocation";
            this.settingsLocation.Size = new System.Drawing.Size(492, 20);
            this.settingsLocation.TabIndex = 5;
            this.settingsLocation.TextChanged += new System.EventHandler(this.settingsLocation_TextChanged);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // WebDashboardOptionsPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.migrateDashboard);
            this.Controls.Add(this.settingsPanel);
            this.HeaderText = "What do you want to migrate for the Web Dashboard?";
            this.HeaderTitle = "Web Dashboard Migration";
            this.Name = "WebDashboardOptionsPage";
            this.Size = new System.Drawing.Size(658, 321);
            this.settingsPanel.ResumeLayout(false);
            this.settingsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox migrateDashboard;
        private System.Windows.Forms.GroupBox settingsPanel;
        private System.Windows.Forms.Button selectLocationButton;
        private System.Windows.Forms.TextBox settingsLocation;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}
