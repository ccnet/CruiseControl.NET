namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
    partial class BuildParameters
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BuildParameters));
            this.buildButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.parameters = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // buildButton
            // 
            this.buildButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buildButton.Location = new System.Drawing.Point(496, 300);
            this.buildButton.Name = "buildButton";
            this.buildButton.Size = new System.Drawing.Size(75, 23);
            this.buildButton.TabIndex = 1;
            this.buildButton.Text = "&Build";
            this.buildButton.UseVisualStyleBackColor = true;
            this.buildButton.Click += new System.EventHandler(this.buildButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(415, 300);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // parameters
            // 
            this.parameters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.parameters.CommandsVisibleIfAvailable = false;
            this.parameters.Location = new System.Drawing.Point(12, 12);
            this.parameters.Name = "parameters";
            this.parameters.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.parameters.Size = new System.Drawing.Size(559, 282);
            this.parameters.TabIndex = 3;
            this.parameters.ToolbarVisible = false;
            this.parameters.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.parameters_PropertyValueChanged);
            // 
            // BuildParameters
            // 
            this.AcceptButton = this.buildButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(583, 335);
            this.ControlBox = false;
            this.Controls.Add(this.parameters);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.buildButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BuildParameters";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Build Parameters";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buildButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.PropertyGrid parameters;
    }
}