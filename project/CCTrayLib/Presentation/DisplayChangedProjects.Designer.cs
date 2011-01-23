namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
    partial class DisplayChangedProjects
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
            System.Windows.Forms.TabControl tabControl1;
            System.Windows.Forms.ColumnHeader columnHeader1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DisplayChangedProjects));
            System.Windows.Forms.ColumnHeader columnHeader2;
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.addedProjectsList = new System.Windows.Forms.ListView();
            this.images = new System.Windows.Forms.ImageList(this.components);
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.deletedProjectsList = new System.Windows.Forms.ListView();
            this.updateButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            tabControl1 = new System.Windows.Forms.TabControl();
            columnHeader1 = new System.Windows.Forms.ColumnHeader();
            columnHeader2 = new System.Windows.Forms.ColumnHeader();
            tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            tabControl1.Controls.Add(this.tabPage1);
            tabControl1.Controls.Add(this.tabPage2);
            tabControl1.ImageList = this.images;
            tabControl1.Location = new System.Drawing.Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(431, 290);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.addedProjectsList);
            this.tabPage1.ImageKey = "AddedProject";
            this.tabPage1.Location = new System.Drawing.Point(4, 23);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(423, 263);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Added Projects";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // addedProjectsList
            // 
            this.addedProjectsList.CheckBoxes = true;
            this.addedProjectsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnHeader1});
            this.addedProjectsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.addedProjectsList.Location = new System.Drawing.Point(3, 3);
            this.addedProjectsList.Name = "addedProjectsList";
            this.addedProjectsList.Size = new System.Drawing.Size(417, 257);
            this.addedProjectsList.SmallImageList = this.images;
            this.addedProjectsList.TabIndex = 0;
            this.addedProjectsList.UseCompatibleStateImageBehavior = false;
            this.addedProjectsList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Project Name";
            columnHeader1.Width = 384;
            // 
            // images
            // 
            this.images.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("images.ImageStream")));
            this.images.TransparentColor = System.Drawing.Color.Transparent;
            this.images.Images.SetKeyName(0, "AddedProject");
            this.images.Images.SetKeyName(1, "DeletedProject");
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.deletedProjectsList);
            this.tabPage2.ImageKey = "DeletedProject";
            this.tabPage2.Location = new System.Drawing.Point(4, 23);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(423, 263);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Deleted Projects";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // deletedProjectsList
            // 
            this.deletedProjectsList.CheckBoxes = true;
            this.deletedProjectsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnHeader2});
            this.deletedProjectsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deletedProjectsList.Location = new System.Drawing.Point(3, 3);
            this.deletedProjectsList.Name = "deletedProjectsList";
            this.deletedProjectsList.Size = new System.Drawing.Size(417, 257);
            this.deletedProjectsList.SmallImageList = this.images;
            this.deletedProjectsList.TabIndex = 0;
            this.deletedProjectsList.UseCompatibleStateImageBehavior = false;
            this.deletedProjectsList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Project Name";
            columnHeader2.Width = 380;
            // 
            // updateButton
            // 
            this.updateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.updateButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.updateButton.Location = new System.Drawing.Point(368, 308);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(75, 23);
            this.updateButton.TabIndex = 1;
            this.updateButton.Text = "&Update";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(287, 308);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // DisplayChangedProjects
            // 
            this.AcceptButton = this.updateButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(455, 343);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.updateButton);
            this.Controls.Add(tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DisplayChangedProjects";
            this.ShowInTaskbar = false;
            this.Text = "Changed Projects";
            tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ListView addedProjectsList;
        private System.Windows.Forms.ListView deletedProjectsList;
        private System.Windows.Forms.ImageList images;
    }
}