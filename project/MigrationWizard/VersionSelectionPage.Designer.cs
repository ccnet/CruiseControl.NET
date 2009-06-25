namespace ThoughtWorks.CruiseControl.MigrationWizard
{
    partial class VersionSelectionPage
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
            System.Windows.Forms.Label label1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VersionSelectionPage));
            System.Windows.Forms.Label label2;
            this.currentVersion = new System.Windows.Forms.ComboBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            label1.Location = new System.Drawing.Point(3, 81);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(530, 295);
            label1.TabIndex = 0;
            label1.Text = resources.GetString("label1.Text");
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(3, 60);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(45, 13);
            label2.TabIndex = 1;
            label2.Text = "Version:";
            // 
            // currentVersion
            // 
            this.currentVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.currentVersion.FormattingEnabled = true;
            this.currentVersion.Location = new System.Drawing.Point(54, 57);
            this.currentVersion.Name = "currentVersion";
            this.currentVersion.Size = new System.Drawing.Size(131, 21);
            this.currentVersion.TabIndex = 2;
            // 
            // VersionSelectionPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.currentVersion);
            this.Controls.Add(label2);
            this.Controls.Add(label1);
            this.HeaderText = "What version of CruiseControl.NET are you currently using?";
            this.HeaderTitle = "Current Version of CruiseControl.NET";
            this.Name = "VersionSelectionPage";
            this.Size = new System.Drawing.Size(536, 376);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox currentVersion;
    }
}
