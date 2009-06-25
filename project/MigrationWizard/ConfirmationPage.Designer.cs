namespace ThoughtWorks.CruiseControl.MigrationWizard
{
    partial class ConfirmationPage
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
            this.confirmationBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // confirmationBox
            // 
            this.confirmationBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.confirmationBox.BackColor = System.Drawing.SystemColors.Window;
            this.confirmationBox.Location = new System.Drawing.Point(3, 55);
            this.confirmationBox.Multiline = true;
            this.confirmationBox.Name = "confirmationBox";
            this.confirmationBox.ReadOnly = true;
            this.confirmationBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.confirmationBox.Size = new System.Drawing.Size(414, 180);
            this.confirmationBox.TabIndex = 0;
            this.confirmationBox.WordWrap = false;
            // 
            // ConfirmationPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.confirmationBox);
            this.HeaderText = "Please confirm these are the settings you want to use for data migration:";
            this.HeaderTitle = "Confirm Settings";
            this.Name = "ConfirmationPage";
            this.Size = new System.Drawing.Size(417, 238);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox confirmationBox;
    }
}
