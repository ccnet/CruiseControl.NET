namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
    partial class CurrentStatusWindow
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ToolStrip commands;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CurrentStatusWindow));
            System.Windows.Forms.StatusStrip statusBar;
            System.Windows.Forms.ImageList explorerImages;
            this.refreshCommand = new System.Windows.Forms.ToolStripButton();
            this.currentStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.statusExplorer = new System.Windows.Forms.TreeView();
            this.statusDetails = new System.Windows.Forms.PropertyGrid();
            this.statusProgress = new System.Windows.Forms.ProgressBar();
            this.displayWorker = new System.ComponentModel.BackgroundWorker();
            this.refreshTimer = new System.Windows.Forms.Timer(this.components);
            commands = new System.Windows.Forms.ToolStrip();
            statusBar = new System.Windows.Forms.StatusStrip();
            explorerImages = new System.Windows.Forms.ImageList(this.components);
            commands.SuspendLayout();
            statusBar.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // commands
            // 
            commands.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            commands.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshCommand});
            commands.Location = new System.Drawing.Point(0, 0);
            commands.Name = "commands";
            commands.Size = new System.Drawing.Size(533, 25);
            commands.TabIndex = 0;
            commands.Text = "toolStrip1";
            // 
            // refreshCommand
            // 
            this.refreshCommand.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.refreshCommand.Image = ((System.Drawing.Image)(resources.GetObject("refreshCommand.Image")));
            this.refreshCommand.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.refreshCommand.Name = "refreshCommand";
            this.refreshCommand.Size = new System.Drawing.Size(23, 22);
            this.refreshCommand.Text = "Refresh";
            this.refreshCommand.Click += new System.EventHandler(this.refreshCommand_Click);
            // 
            // statusBar
            // 
            statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.currentStatus});
            statusBar.Location = new System.Drawing.Point(0, 366);
            statusBar.Name = "statusBar";
            statusBar.Size = new System.Drawing.Size(533, 22);
            statusBar.TabIndex = 1;
            statusBar.Text = "statusStrip1";
            // 
            // currentStatus
            // 
            this.currentStatus.Name = "currentStatus";
            this.currentStatus.Size = new System.Drawing.Size(518, 17);
            this.currentStatus.Spring = true;
            this.currentStatus.Text = "Loading status...";
            this.currentStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // explorerImages
            // 
            explorerImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("explorerImages.ImageStream")));
            explorerImages.TransparentColor = System.Drawing.Color.Transparent;
            explorerImages.Images.SetKeyName(0, "Unknown");
            explorerImages.Images.SetKeyName(1, "CompletedSuccess");
            explorerImages.Images.SetKeyName(2, "CompletedFailed");
            explorerImages.Images.SetKeyName(3, "Pending");
            explorerImages.Images.SetKeyName(4, "Cancelled");
            explorerImages.Images.SetKeyName(5, "Running");
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.statusExplorer);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.statusDetails);
            this.splitContainer1.Panel2.Controls.Add(this.statusProgress);
            this.splitContainer1.Size = new System.Drawing.Size(533, 341);
            this.splitContainer1.SplitterDistance = 177;
            this.splitContainer1.TabIndex = 2;
            // 
            // statusExplorer
            // 
            this.statusExplorer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusExplorer.ImageKey = "Unknown";
            this.statusExplorer.ImageList = explorerImages;
            this.statusExplorer.Location = new System.Drawing.Point(0, 0);
            this.statusExplorer.Name = "statusExplorer";
            this.statusExplorer.SelectedImageIndex = 0;
            this.statusExplorer.Size = new System.Drawing.Size(177, 341);
            this.statusExplorer.TabIndex = 0;
            this.statusExplorer.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.statusExplorer_AfterSelect);
            // 
            // statusDetails
            // 
            this.statusDetails.CommandsVisibleIfAvailable = false;
            this.statusDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusDetails.Location = new System.Drawing.Point(0, 23);
            this.statusDetails.Name = "statusDetails";
            this.statusDetails.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.statusDetails.Size = new System.Drawing.Size(352, 318);
            this.statusDetails.TabIndex = 0;
            this.statusDetails.ToolbarVisible = false;
            // 
            // statusProgress
            // 
            this.statusProgress.Dock = System.Windows.Forms.DockStyle.Top;
            this.statusProgress.Location = new System.Drawing.Point(0, 0);
            this.statusProgress.Name = "statusProgress";
            this.statusProgress.Size = new System.Drawing.Size(352, 23);
            this.statusProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.statusProgress.TabIndex = 1;
            // 
            // displayWorker
            // 
            this.displayWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.displayWorker_DoWork);
            this.displayWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.displayWorker_RunWorkerCompleted);
            // 
            // refreshTimer
            // 
            this.refreshTimer.Interval = 5000;
            this.refreshTimer.Tick += new System.EventHandler(this.refreshTimer_Tick);
            // 
            // CurrentStatusWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(533, 388);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(statusBar);
            this.Controls.Add(commands);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CurrentStatusWindow";
            this.Text = "Current Status for ...";
            commands.ResumeLayout(false);
            commands.PerformLayout();
            statusBar.ResumeLayout(false);
            statusBar.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripButton refreshCommand;
        private System.Windows.Forms.ToolStripStatusLabel currentStatus;
        private System.Windows.Forms.TreeView statusExplorer;
        private System.Windows.Forms.PropertyGrid statusDetails;
        private System.ComponentModel.BackgroundWorker displayWorker;
        private System.Windows.Forms.Timer refreshTimer;
        private System.Windows.Forms.ProgressBar statusProgress;
    }
}