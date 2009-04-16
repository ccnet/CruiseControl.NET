namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
    partial class ConfigureServer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigureServer));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.chkIsSecure = new System.Windows.Forms.CheckBox();
            this.lblAuthMode = new System.Windows.Forms.Label();
            this.cmbAuthMode = new System.Windows.Forms.ComboBox();
            this.butConfigureAuth = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.CausesValidation = false;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCancel.Location = new System.Drawing.Point(212, 95);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnOK.Location = new System.Drawing.Point(127, 95);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // chkIsSecure
            // 
            this.chkIsSecure.AutoSize = true;
            this.chkIsSecure.Location = new System.Drawing.Point(12, 12);
            this.chkIsSecure.Name = "chkIsSecure";
            this.chkIsSecure.Size = new System.Drawing.Size(102, 17);
            this.chkIsSecure.TabIndex = 8;
            this.chkIsSecure.Text = "Server is secure";
            this.chkIsSecure.UseVisualStyleBackColor = true;
            this.chkIsSecure.CheckedChanged += new System.EventHandler(this.chkIsSecure_CheckedChanged);
            // 
            // lblAuthMode
            // 
            this.lblAuthMode.AutoSize = true;
            this.lblAuthMode.Enabled = false;
            this.lblAuthMode.Location = new System.Drawing.Point(31, 35);
            this.lblAuthMode.Name = "lblAuthMode";
            this.lblAuthMode.Size = new System.Drawing.Size(101, 13);
            this.lblAuthMode.TabIndex = 9;
            this.lblAuthMode.Text = "Authorisation Mode:";
            // 
            // cmbAuthMode
            // 
            this.cmbAuthMode.Enabled = false;
            this.cmbAuthMode.FormattingEnabled = true;
            this.cmbAuthMode.Location = new System.Drawing.Point(138, 32);
            this.cmbAuthMode.Name = "cmbAuthMode";
            this.cmbAuthMode.Size = new System.Drawing.Size(269, 21);
            this.cmbAuthMode.TabIndex = 10;
            // 
            // butConfigureAuth
            // 
            this.butConfigureAuth.Enabled = false;
            this.butConfigureAuth.Location = new System.Drawing.Point(332, 59);
            this.butConfigureAuth.Name = "butConfigureAuth";
            this.butConfigureAuth.Size = new System.Drawing.Size(75, 23);
            this.butConfigureAuth.TabIndex = 11;
            this.butConfigureAuth.Text = "Configure";
            this.butConfigureAuth.UseVisualStyleBackColor = true;
            this.butConfigureAuth.Click += new System.EventHandler(this.butConfigureAuth_Click);
            // 
            // ConfigureServer
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(419, 130);
            this.Controls.Add(this.butConfigureAuth);
            this.Controls.Add(this.cmbAuthMode);
            this.Controls.Add(this.lblAuthMode);
            this.Controls.Add(this.chkIsSecure);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigureServer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure Server";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox chkIsSecure;
        private System.Windows.Forms.Label lblAuthMode;
        private System.Windows.Forms.ComboBox cmbAuthMode;
        private System.Windows.Forms.Button butConfigureAuth;
    }
}
