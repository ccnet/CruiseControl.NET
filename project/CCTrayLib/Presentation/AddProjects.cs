using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class AddProjects : Form
	{
		public ListBox lbProject;
		public ListBox lbServer;
		private GroupBox groupBox1;
		private Button btnAdd;
		private GroupBox groupBox2;

		private BuildServer selectedServer;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		private readonly ICruiseProjectManagerFactory cruiseProjectManagerFactory;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label4;
		private readonly CCTrayProject[] currentProjectList;

		public AddProjects(ICruiseProjectManagerFactory cruiseProjectManagerFactory, CCTrayProject[] currentProjectList)
		{
			this.cruiseProjectManagerFactory = cruiseProjectManagerFactory;
			this.currentProjectList = currentProjectList;

			InitializeComponent();

			foreach (CCTrayProject project in currentProjectList)
			{
				if (!lbServer.Items.Contains(project.BuildServer))
					lbServer.Items.Add(project.BuildServer);
			}
		}

		public CCTrayProject[] GetListOfNewProjects(IWin32Window owner)
		{
			if (ShowDialog(owner) == DialogResult.OK)
			{
				ArrayList newProjects = new ArrayList();
				foreach (string projectName in lbProject.SelectedItems)
				{
					newProjects.Add(new CCTrayProject(selectedServer, projectName));
				}
				return (CCTrayProject[]) newProjects.ToArray(typeof (CCTrayProject));
			}
			return null;
		}

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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AddProjects));
			this.lbProject = new System.Windows.Forms.ListBox();
			this.lbServer = new System.Windows.Forms.ListBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
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
			this.lbProject.Size = new System.Drawing.Size(255, 186);
			this.lbProject.Sorted = true;
			this.lbProject.TabIndex = 0;
			// 
			// lbServer
			// 
			this.lbServer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left)));
			this.lbServer.Location = new System.Drawing.Point(10, 25);
			this.lbServer.Name = "lbServer";
			this.lbServer.Size = new System.Drawing.Size(255, 147);
			this.lbServer.Sorted = true;
			this.lbServer.TabIndex = 0;
			this.lbServer.SelectedIndexChanged += new System.EventHandler(this.lbServer_SelectedIndexChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox1.Controls.Add(this.btnAdd);
			this.groupBox1.Controls.Add(this.lbServer);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox1.Location = new System.Drawing.Point(10, 80);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(280, 220);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Build Server";
			// 
			// btnAdd
			// 
			this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnAdd.Location = new System.Drawing.Point(100, 185);
			this.btnAdd.Name = "btnAdd";
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
			this.DockPadding.All = 4;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "AddProjects";
			this.Text = "Project";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private void btnAddServer_Click(object sender, EventArgs e)
		{
			AddBuildServer addBuildServer = new AddBuildServer(cruiseProjectManagerFactory);
			
			BuildServer buildServer = addBuildServer.ChooseNewBuildServer(this);
			if (buildServer != null)
			{
				lbServer.SelectedIndex = lbServer.Items.Add(buildServer);
			}
		}

		private void lbServer_SelectedIndexChanged(object sender, EventArgs e)
		{
			selectedServer = (BuildServer) lbServer.SelectedItem;
			if (selectedServer != null)
				RetrieveListOfProjects(selectedServer);
		}

		private void RetrieveListOfProjects(BuildServer server)
		{
			Cursor.Current = Cursors.WaitCursor;
			try
			{
				lbProject.Items.Clear();

				CCTrayProject[] projectList = cruiseProjectManagerFactory.GetProjectList(server);

				foreach (CCTrayProject project in projectList)
				{
					if (! IsProjectAlreadyAdded(project))
						lbProject.Items.Add(project.ProjectName);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, "Unable to connect to server " + server.DisplayName + ": " + ex.Message, "Error");
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}

		private bool IsProjectAlreadyAdded(CCTrayProject project)
		{
			foreach (CCTrayProject currentProject in currentProjectList)
			{
				if (project.ServerUrl == currentProject.ServerUrl && project.ProjectName == currentProject.ProjectName)
					return true;
			}
			return false;
		}
	}
}