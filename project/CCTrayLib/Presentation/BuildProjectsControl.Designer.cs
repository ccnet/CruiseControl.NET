using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	partial class BuildProjectsControl
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
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.chkCheckAllProjects = new System.Windows.Forms.CheckBox();
			this.lvProjects = new System.Windows.Forms.ListView();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnMoveDown = new System.Windows.Forms.Button();
			this.btnMoveUp = new System.Windows.Forms.Button();
			this.btnRemove = new System.Windows.Forms.Button();
			this.lblProjectsToMonitor = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Build Server";
			this.columnHeader1.Width = 135;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Transport";
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Project";
			this.columnHeader2.Width = 287;
			// 
			// chkCheckAllProjects
			// 
			this.chkCheckAllProjects.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.chkCheckAllProjects.AutoSize = true;
			this.chkCheckAllProjects.Checked = true;
			this.chkCheckAllProjects.CheckState = System.Windows.Forms.CheckState.Indeterminate;
			this.chkCheckAllProjects.Location = new System.Drawing.Point(16, 267);
			this.chkCheckAllProjects.Name = "chkCheckAllProjects";
			this.chkCheckAllProjects.Size = new System.Drawing.Size(122, 17);
			this.chkCheckAllProjects.TabIndex = 15;
			this.chkCheckAllProjects.Text = "check / uncheck all";
			this.chkCheckAllProjects.UseVisualStyleBackColor = true;
			this.chkCheckAllProjects.CheckedChanged += new System.EventHandler(this.chkCheck_CheckedChanged);
			// 
			// lvProjects
			// 
			this.lvProjects.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lvProjects.CheckBoxes = true;
			this.lvProjects.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader3,
            this.columnHeader2});
			this.lvProjects.FullRowSelect = true;
			this.lvProjects.HideSelection = false;
			this.lvProjects.Location = new System.Drawing.Point(8, 40);
			this.lvProjects.Name = "lvProjects";
			this.lvProjects.Size = new System.Drawing.Size(540, 220);
			this.lvProjects.TabIndex = 8;
			this.lvProjects.UseCompatibleStateImageBehavior = false;
			this.lvProjects.View = System.Windows.Forms.View.Details;
			this.lvProjects.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvProjects_ItemChecked);
			this.lvProjects.SelectedIndexChanged += new System.EventHandler(this.lvProjects_SelectedIndexChanged);
			this.lvProjects.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvProjects_KeyDown);
			// 
			// btnAdd
			// 
			this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnAdd.Location = new System.Drawing.Point(560, 48);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(75, 23);
			this.btnAdd.TabIndex = 9;
			this.btnAdd.Text = "&Add...";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnMoveDown
			// 
			this.btnMoveDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnMoveDown.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnMoveDown.Location = new System.Drawing.Point(560, 184);
			this.btnMoveDown.Name = "btnMoveDown";
			this.btnMoveDown.Size = new System.Drawing.Size(75, 23);
			this.btnMoveDown.TabIndex = 12;
			this.btnMoveDown.Text = "Move &Down";
			this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
			// 
			// btnMoveUp
			// 
			this.btnMoveUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnMoveUp.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnMoveUp.Location = new System.Drawing.Point(560, 152);
			this.btnMoveUp.Name = "btnMoveUp";
			this.btnMoveUp.Size = new System.Drawing.Size(75, 23);
			this.btnMoveUp.TabIndex = 11;
			this.btnMoveUp.Text = "Move &Up";
			this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
			// 
			// btnRemove
			// 
			this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnRemove.Location = new System.Drawing.Point(560, 80);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(75, 23);
			this.btnRemove.TabIndex = 10;
			this.btnRemove.Text = "&Remove";
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// lblProjectsToMonitor
			// 
			this.lblProjectsToMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lblProjectsToMonitor.Location = new System.Drawing.Point(8, 8);
			this.lblProjectsToMonitor.Name = "lblProjectsToMonitor";
			this.lblProjectsToMonitor.Size = new System.Drawing.Size(625, 20);
			this.lblProjectsToMonitor.TabIndex = 7;
			this.lblProjectsToMonitor.Text = "Use this section to define the projects to monitor. ";
			// 
			// BuildProjectsControl
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Controls.Add(this.chkCheckAllProjects);
			this.Controls.Add(this.lvProjects);
			this.Controls.Add(this.lblProjectsToMonitor);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.btnMoveDown);
			this.Controls.Add(this.btnMoveUp);
			this.Controls.Add(this.btnRemove);
			this.Name = "BuildProjectsControl";
			this.Size = new System.Drawing.Size(667, 289);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ColumnHeader columnHeader1;
		private ColumnHeader columnHeader2;
		private ColumnHeader columnHeader3;
		private CheckBox chkCheckAllProjects;
		private ListView lvProjects;
		private Button btnAdd;
		private Button btnMoveDown;
		private Button btnMoveUp;
		private Button btnRemove;
		private Label lblProjectsToMonitor;
	}
}
