using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public partial class AddProjects
	{
		public ListBox lbProject;
		public ListView lbServer;
		private GroupBox groupBox1;
		private Button btnAdd;
		private GroupBox groupBox2;

		private Button btnCancel;
		private Button btnOK;
		private Label label1;
		private Label label4;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddProjects));
            this.lbProject = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbServer = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.imlSmall = new System.Windows.Forms.ImageList(this.components);
            this.butConfigure = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbProject
            // 
            this.lbProject.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbProject.Location = new System.Drawing.Point(10, 25);
            this.lbProject.Name = "lbProject";
            this.lbProject.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbProject.Size = new System.Drawing.Size(255, 160);
            this.lbProject.Sorted = true;
            this.lbProject.TabIndex = 0;
            this.lbProject.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbProject_KeyDown);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.lbServer);
            this.groupBox1.Controls.Add(this.butConfigure);
            this.groupBox1.Controls.Add(this.btnAdd);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox1.Location = new System.Drawing.Point(10, 80);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(280, 220);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Build Server";
            // 
            // lbServer
            // 
            this.lbServer.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lbServer.FullRowSelect = true;
            this.lbServer.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lbServer.LargeImageList = this.imlSmall;
            this.lbServer.Location = new System.Drawing.Point(6, 19);
            this.lbServer.MultiSelect = false;
            this.lbServer.Name = "lbServer";
            this.lbServer.Size = new System.Drawing.Size(268, 163);
            this.lbServer.SmallImageList = this.imlSmall;
            this.lbServer.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lbServer.TabIndex = 4;
            this.lbServer.UseCompatibleStateImageBehavior = false;
            this.lbServer.View = System.Windows.Forms.View.Details;
            this.lbServer.SelectedIndexChanged += new System.EventHandler(this.lbServer_SelectedIndexChanged);
            this.lbServer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbServer_KeyDown);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Server";
            this.columnHeader1.Width = 240;
            // 
            // imlSmall
            // 
            this.imlSmall.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlSmall.ImageStream")));
            this.imlSmall.TransparentColor = System.Drawing.Color.Transparent;
            this.imlSmall.Images.SetKeyName(0, "Secure");
            this.imlSmall.Images.SetKeyName(1, "NonSecure");
            // 
            // butConfigure
            // 
            this.butConfigure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.butConfigure.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.butConfigure.Location = new System.Drawing.Point(138, 188);
            this.butConfigure.Name = "butConfigure";
            this.butConfigure.Size = new System.Drawing.Size(75, 23);
            this.butConfigure.TabIndex = 3;
            this.butConfigure.Text = "Configure";
            this.butConfigure.Click += new System.EventHandler(this.butConfigure_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnAdd.Location = new System.Drawing.Point(57, 188);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Add Server";
            this.btnAdd.Click += new System.EventHandler(this.btnAddServer_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.lbProject);
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox2.Location = new System.Drawing.Point(300, 80);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(280, 220);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Available Projects";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.CausesValidation = false;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCancel.Location = new System.Drawing.Point(300, 310);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnOK.Location = new System.Drawing.Point(215, 310);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(10, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(570, 35);
            this.label1.TabIndex = 0;
            this.label1.Text = "The list on the left shows the build servers that CCTray currently knows about.  " +
                "Select a build server, then select one or more projects to add.";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(10, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(570, 20);
            this.label4.TabIndex = 1;
            this.label4.Text = "If you want to add a new build server, click Add Server.";
            // 
            // AddProjects
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(589, 346);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AddProjects";
            this.Padding = new System.Windows.Forms.Padding(4);
            this.Text = "Project";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AddProjects_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion
	}
}
