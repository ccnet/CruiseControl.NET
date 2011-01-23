namespace ThoughtWorks.CruiseControl.MigrationWizard
{
    partial class ProgressPage
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
            this.progressDisplay = new System.Windows.Forms.ListBox();
            this.dataMigrationWorker = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // progressDisplay
            // 
            this.progressDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressDisplay.FormattingEnabled = true;
            this.progressDisplay.IntegralHeight = false;
            this.progressDisplay.Location = new System.Drawing.Point(3, 53);
            this.progressDisplay.Name = "progressDisplay";
            this.progressDisplay.Size = new System.Drawing.Size(428, 173);
            this.progressDisplay.TabIndex = 0;
            // 
            // dataMigrationWorker
            // 
            this.dataMigrationWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.dataMigrationWorker_DoWork);
            this.dataMigrationWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.dataMigrationWorker_RunWorkerCompleted);
            // 
            // ProgressPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CanCancel = false;
            this.Controls.Add(this.progressDisplay);
            this.HeaderText = "Please wait...";
            this.HeaderTitle = "Migrating Data";
            this.Name = "ProgressPage";
            this.Size = new System.Drawing.Size(434, 229);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox progressDisplay;
        private System.ComponentModel.BackgroundWorker dataMigrationWorker;
    }
}
