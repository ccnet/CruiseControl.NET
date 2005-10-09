using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class AddProjects : Form
	{
		private Button btnOK;
		private Button btnCancel;
		private Label label4;
		private ListBox lbProject;
		private Label label1;
		public ListBox lbServer;
		private GroupBox groupBox1;
		private Button btnAdd;
		private GroupBox groupBox2;
		private TextBox txtServer;

		private Project selectedServer;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public AddProjects(Project[] projects)
		{
			InitializeComponent();

			foreach (Project project in projects)
			{
				if (!lbServer.Items.Contains(project.ServerDisplayName))
					lbServer.Items.Add(project.ServerDisplayName);
			}
		}

		public Project[] GetListOfNewProjects(IWin32Window owner)
		{
			if (ShowDialog(owner) == DialogResult.OK)
			{
				ArrayList newProjects = new ArrayList();
				foreach (string projectName in lbProject.SelectedItems)
				{
					newProjects.Add(new Project(selectedServer.ServerUrl, projectName));
				}
				return (Project[]) newProjects.ToArray(typeof (Project));
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof (AddProjects));
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.lbProject = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.lbServer = new System.Windows.Forms.ListBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnAdd = new System.Windows.Forms.Button();
			this.txtServer = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnOK.Location = new System.Drawing.Point(215, 310);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 4;
			this.btnOK.Text = "OK";
			// 
			// btnCancel
			// 
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnCancel.Location = new System.Drawing.Point(300, 310);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 5;
			this.btnCancel.Text = "Cancel";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(10, 35);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(570, 35);
			this.label4.TabIndex = 1;
			this.label4.Text = "If you have configured your build server to run on a port other than the default " +
				"of 21234, you can follow the name of the build server with :port.";
			// 
			// lbProject
			// 
			this.lbProject.Location = new System.Drawing.Point(10, 25);
			this.lbProject.Name = "lbProject";
			this.lbProject.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lbProject.Size = new System.Drawing.Size(255, 186);
			this.lbProject.Sorted = true;
			this.lbProject.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(570, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Select the build server, then select one or more projects to add.";
			// 
			// lbServer
			// 
			this.lbServer.Location = new System.Drawing.Point(10, 25);
			this.lbServer.Name = "lbServer";
			this.lbServer.Size = new System.Drawing.Size(255, 147);
			this.lbServer.Sorted = true;
			this.lbServer.TabIndex = 0;
			this.lbServer.SelectedIndexChanged += new System.EventHandler(this.lbServer_SelectedIndexChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.btnAdd);
			this.groupBox1.Controls.Add(this.txtServer);
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
			this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnAdd.Location = new System.Drawing.Point(190, 185);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.TabIndex = 2;
			this.btnAdd.Text = "Add Server";
			this.btnAdd.Click += new System.EventHandler(this.btnAddServer_Click);
			// 
			// txtServer
			// 
			this.txtServer.AcceptsReturn = true;
			this.txtServer.Location = new System.Drawing.Point(10, 185);
			this.txtServer.Name = "txtServer";
			this.txtServer.Size = new System.Drawing.Size(170, 20);
			this.txtServer.TabIndex = 1;
			this.txtServer.Text = "";
			this.txtServer.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtServer_KeyPress);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.lbProject);
			this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox2.Location = new System.Drawing.Point(300, 80);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(280, 220);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Project";
			// 
			// AddProjects
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(589, 346);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
			this.Name = "AddProjects";
			this.Text = "Project";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private void btnAddServer_Click(object sender, EventArgs e)
		{
			if (txtServer.Text.Trim().Length == 0)
				return;

			try
			{
				// validate
				new Project().SetServerUrlFromDisplayName(txtServer.Text);

				lbServer.SelectedIndex = lbServer.Items.Add(txtServer.Text);
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, "Invalid Server Name");
			}

		}

		private void lbServer_SelectedIndexChanged(object sender, EventArgs e)
		{
			string serverName = lbServer.Text;
			selectedServer = new Project();
			selectedServer.SetServerUrlFromDisplayName(serverName);

			RetrieveListOfProjects(selectedServer);
		}

		private void RetrieveListOfProjects(Project server)
		{
			Cursor.Current = Cursors.WaitCursor;
			try
			{
				lbProject.Items.Clear();

				RemoteCruiseManagerFactory factory = new RemoteCruiseManagerFactory();
				ICruiseManager manager = factory.GetCruiseManager(server.ServerUrl);
				ProjectStatus[] projectStatuses = manager.GetProjectStatus();

				foreach (ProjectStatus status in projectStatuses)
				{
					lbProject.Items.Add(status.Name);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, "Unable to connect to server " + server.ServerDisplayName + ": " + ex.Message, "Error");
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}

		private void txtServer_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == 13)
			{
				e.Handled = true;
				btnAddServer_Click(this, EventArgs.Empty);
			}
		}
	}
}