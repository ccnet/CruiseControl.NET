using System;
using System.ComponentModel;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.ServerConnection;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class AddEditProject : Form
	{
		private readonly Project project;
		private Label label1;
		private Label label2;
		private Label label3;
		private Button btnOK;
		private Button btnCancel;
		private Label label4;
		private Label label5;
		private TextBox txtServer;
		private ComboBox txtProject;
		private Button btnFetch;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public AddEditProject(Project project)
		{
			this.project = project;
			InitializeComponent();
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.txtServer = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtProject = new System.Windows.Forms.ComboBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.btnFetch = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(580, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Enter the details of the CruiseControl.NET project to monitor.";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(10, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(165, 15);
			this.label2.TabIndex = 1;
			this.label2.Text = "Build Server Connection String:";
			// 
			// txtServer
			// 
			this.txtServer.Location = new System.Drawing.Point(180, 35);
			this.txtServer.Name = "txtServer";
			this.txtServer.Size = new System.Drawing.Size(415, 20);
			this.txtServer.TabIndex = 2;
			this.txtServer.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(10, 105);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(165, 15);
			this.label3.TabIndex = 3;
			this.label3.Text = "Project name:";
			// 
			// txtProject
			// 
			this.txtProject.Location = new System.Drawing.Point(180, 100);
			this.txtProject.Name = "txtProject";
			this.txtProject.Size = new System.Drawing.Size(335, 21);
			this.txtProject.TabIndex = 4;
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnOK.Location = new System.Drawing.Point(223, 175);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 5;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnCancel.Location = new System.Drawing.Point(308, 175);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "Cancel";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(180, 60);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(415, 35);
			this.label4.TabIndex = 7;
			this.label4.Text = "An example connection string is tcp://server:21234/CruiseManager.rem. Your build " +
				"server administrator can tell you what to enter here.";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(180, 130);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(415, 35);
			this.label5.TabIndex = 8;
			this.label5.Text = "Build servers can host multiple projects.  Enter the project name to monitor here" +
				".  Click Fetch to retrieve a list of projects hosted on the server.";
			// 
			// btnFetch
			// 
			this.btnFetch.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnFetch.Location = new System.Drawing.Point(520, 100);
			this.btnFetch.Name = "btnFetch";
			this.btnFetch.TabIndex = 9;
			this.btnFetch.Text = "Fetch";
			this.btnFetch.Click += new System.EventHandler(this.btnFetch_Click);
			// 
			// AddEditProject
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(607, 208);
			this.Controls.Add(this.btnFetch);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.txtProject);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtServer);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "AddEditProject";
			this.Text = "Project";
			this.Load += new System.EventHandler(this.AddEditProject_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private void AddEditProject_Load(object sender, EventArgs e)
		{
			txtServer.Text = project.ServerUrl;
			txtProject.Text = project.ProjectName;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			project.ServerUrl = txtServer.Text;
			project.ProjectName = txtProject.Text;
		}

		private void btnFetch_Click(object sender, EventArgs e)
		{
			Cursor.Current = Cursors.WaitCursor;
			try
			{
				RemoteCruiseManagerFactory factory = new RemoteCruiseManagerFactory();
				ICruiseManager manager = factory.GetCruiseManager(txtServer.Text);
				ProjectStatus[] projectStatuses = manager.GetProjectStatus();

				txtProject.Items.Clear();
				foreach (ProjectStatus status in projectStatuses)
				{
					txtProject.Items.Add(status.Name);
				}
				txtProject.DroppedDown = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, "Unable to connect to server: " + ex.Message, "Error");
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}
	}
}