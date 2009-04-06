namespace Validator
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.label1 = new System.Windows.Forms.Label();
            this.versionLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.officalLink = new System.Windows.Forms.LinkLabel();
            this.blogLink = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(105, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(345, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "CruiseControl.Net: Configuration Validation Tool";
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.Location = new System.Drawing.Point(106, 40);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(35, 13);
            this.versionLabel.TabIndex = 1;
            this.versionLabel.Text = "label2";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(106, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(381, 145);
            this.label2.TabIndex = 2;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // officalLink
            // 
            this.officalLink.AutoSize = true;
            this.officalLink.Location = new System.Drawing.Point(106, 210);
            this.officalLink.Name = "officalLink";
            this.officalLink.Size = new System.Drawing.Size(196, 13);
            this.officalLink.TabIndex = 3;
            this.officalLink.TabStop = true;
            this.officalLink.Text = "Visit the official site for CruiseControl.Net";
            this.officalLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.officalLink_LinkClicked);
            // 
            // blogLink
            // 
            this.blogLink.AutoSize = true;
            this.blogLink.Location = new System.Drawing.Point(106, 234);
            this.blogLink.Name = "blogLink";
            this.blogLink.Size = new System.Drawing.Size(218, 13);
            this.blogLink.TabIndex = 4;
            this.blogLink.TabStop = true;
            this.blogLink.Text = "Visit my blog (technical resources on CC.Net)";
            this.blogLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.blogLink_LinkClicked);
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::Validator.Properties.Resources.dialog;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(499, 320);
            this.Controls.Add(this.blogLink);
            this.Controls.Add(this.officalLink);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About...";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel officalLink;
        private System.Windows.Forms.LinkLabel blogLink;

    }
}