namespace ThoughtWorks.CruiseControl.MigrationWizard
{
    partial class CompletedPage
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
            this.finishedBox = new System.Windows.Forms.TextBox();
            this.viewLogButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // finishedBox
            // 
            this.finishedBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.finishedBox.BackColor = System.Drawing.SystemColors.Window;
            this.finishedBox.Location = new System.Drawing.Point(3, 52);
            this.finishedBox.Multiline = true;
            this.finishedBox.Name = "finishedBox";
            this.finishedBox.ReadOnly = true;
            this.finishedBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.finishedBox.Size = new System.Drawing.Size(527, 235);
            this.finishedBox.TabIndex = 1;
            this.finishedBox.WordWrap = false;
            // 
            // viewLogButton
            // 
            this.viewLogButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.viewLogButton.Location = new System.Drawing.Point(455, 293);
            this.viewLogButton.Name = "viewLogButton";
            this.viewLogButton.Size = new System.Drawing.Size(75, 23);
            this.viewLogButton.TabIndex = 2;
            this.viewLogButton.Text = "&View Log";
            this.viewLogButton.UseVisualStyleBackColor = true;
            this.viewLogButton.Click += new System.EventHandler(this.viewLogButton_Click);
            // 
            // CompletedPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CanCancel = false;
            this.CanFinish = true;
            this.Controls.Add(this.viewLogButton);
            this.Controls.Add(this.finishedBox);
            this.HeaderText = "The following are the results of the migration:";
            this.HeaderTitle = "Migration Completed";
            this.Name = "CompletedPage";
            this.Size = new System.Drawing.Size(533, 319);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox finishedBox;
        private System.Windows.Forms.Button viewLogButton;
    }
}
