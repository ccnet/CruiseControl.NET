using System;
using System.ComponentModel;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class AddBuildServer : Form
	{
		private Label label1;
		private Label label2;
		private Label label3;
		private Label label4;
		private Label label5;
		private Button btnCancel;
		private Button btnOK;
		private RadioButton rdoDashboard;
		private RadioButton rdoRemoting;
		private RadioButton rdoHttp;
		private TextBox txtDashboard;
		private TextBox txtRemoting;
		private TextBox txtHttp;
		private Label lblFeedback;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		private BuildServer buildServer;
		private ICruiseProjectManagerFactory cruiseProjectManagerFactory;

		public AddBuildServer(ICruiseProjectManagerFactory cruiseProjectManagerFactory)
		{
			this.cruiseProjectManagerFactory = cruiseProjectManagerFactory;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			rdoDashboard.Checked = true;
			UpdateButtons();
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AddBuildServer));
			this.label1 = new System.Windows.Forms.Label();
			this.rdoDashboard = new System.Windows.Forms.RadioButton();
			this.rdoRemoting = new System.Windows.Forms.RadioButton();
			this.rdoHttp = new System.Windows.Forms.RadioButton();
			this.label2 = new System.Windows.Forms.Label();
			this.txtDashboard = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.txtRemoting = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.txtHttp = new System.Windows.Forms.TextBox();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.lblFeedback = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(5, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(490, 35);
			this.label1.TabIndex = 0;
			this.label1.Text = "CCTray can monitor build servers in different ways.  Select how you want to monit" +
				"or the server, then enter the required information.";
			// 
			// rdoDashboard
			// 
			this.rdoDashboard.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.rdoDashboard.Location = new System.Drawing.Point(10, 45);
			this.rdoDashboard.Name = "rdoDashboard";
			this.rdoDashboard.Size = new System.Drawing.Size(370, 24);
			this.rdoDashboard.TabIndex = 1;
			this.rdoDashboard.Text = "Via the CruiseControl.NET dashboard";
			this.rdoDashboard.CheckedChanged += new System.EventHandler(this.rdoDashboard_CheckedChanged);
			// 
			// rdoRemoting
			// 
			this.rdoRemoting.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.rdoRemoting.Location = new System.Drawing.Point(10, 150);
			this.rdoRemoting.Name = "rdoRemoting";
			this.rdoRemoting.Size = new System.Drawing.Size(345, 24);
			this.rdoRemoting.TabIndex = 2;
			this.rdoRemoting.Text = "Connect directly using .NET remoting";
			this.rdoRemoting.CheckedChanged += new System.EventHandler(this.rdoRemoting_CheckedChanged);
			// 
			// rdoHttp
			// 
			this.rdoHttp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.rdoHttp.Location = new System.Drawing.Point(10, 295);
			this.rdoHttp.Name = "rdoHttp";
			this.rdoHttp.Size = new System.Drawing.Size(345, 24);
			this.rdoHttp.TabIndex = 3;
			this.rdoHttp.Text = "Supply a custom HTTP URL";
			this.rdoHttp.CheckedChanged += new System.EventHandler(this.rdoHttp_CheckedChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(30, 70);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(465, 45);
			this.label2.TabIndex = 4;
			this.label2.Text = "This is the preferred way of monitoring build status.  In the box below, enter th" +
				"e http address of the dashboard home directory in the box below, e.g. http://bui" +
				"ld-server/ccnet.  The dashboard must be version 1.1 or later.";
			// 
			// txtDashboard
			// 
			this.txtDashboard.Location = new System.Drawing.Point(30, 116);
			this.txtDashboard.Name = "txtDashboard";
			this.txtDashboard.Size = new System.Drawing.Size(350, 20);
			this.txtDashboard.TabIndex = 5;
			this.txtDashboard.Text = "http://localhost/ccnet";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(30, 175);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(470, 45);
			this.label3.TabIndex = 6;
			this.label3.Text = "CCTray will connect directly to the build server using .NET Remoting.  This is ho" +
				"w CruiseControl.NET 1.0 worked, but it often forces you to install a new version" +
				" of CCTray when the server is upgraded.";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(30, 220);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(470, 45);
			this.label4.TabIndex = 7;
			this.label4.Text = "In the box below, enter the name of the build server to monitor.  If you have con" +
				"figured your build server to run on a port other than the default of 21234, you " +
				"can follow the name of the build server with :port.";
			// 
			// txtRemoting
			// 
			this.txtRemoting.Location = new System.Drawing.Point(30, 265);
			this.txtRemoting.Name = "txtRemoting";
			this.txtRemoting.Size = new System.Drawing.Size(350, 20);
			this.txtRemoting.TabIndex = 8;
			this.txtRemoting.Text = "localhost";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(30, 325);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(465, 45);
			this.label5.TabIndex = 9;
			this.label5.Text = "Use this option to monitor a non-CruiseControl.NET build server (e.g. CruiseContr" +
				"ol for Java).  CCTray will perform an HTTP GET on this URL, and expect to retrie" +
				"ve an XML file as described at http://confluence.public.thoughtworks.org/display" +
				"/CI/.";
			// 
			// txtHttp
			// 
			this.txtHttp.Location = new System.Drawing.Point(30, 370);
			this.txtHttp.Name = "txtHttp";
			this.txtHttp.Size = new System.Drawing.Size(350, 20);
			this.txtHttp.TabIndex = 10;
			this.txtHttp.Text = "http://localhost/cruisecontrol/xml.jsp";
			// 
			// btnCancel
			// 
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnCancel.Location = new System.Drawing.Point(260, 450);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 12;
			this.btnCancel.Text = "Cancel";
			// 
			// btnOK
			// 
			this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnOK.Location = new System.Drawing.Point(175, 450);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 11;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// lblFeedback
			// 
			this.lblFeedback.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblFeedback.Location = new System.Drawing.Point(30, 400);
			this.lblFeedback.Name = "lblFeedback";
			this.lblFeedback.Size = new System.Drawing.Size(455, 40);
			this.lblFeedback.TabIndex = 13;
			this.lblFeedback.Text = "lblFeedback";
			this.lblFeedback.Visible = false;
			// 
			// AddBuildServer
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(502, 483);
			this.Controls.Add(this.lblFeedback);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.txtHttp);
			this.Controls.Add(this.txtRemoting);
			this.Controls.Add(this.txtDashboard);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.rdoHttp);
			this.Controls.Add(this.rdoRemoting);
			this.Controls.Add(this.rdoDashboard);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "AddBuildServer";
			this.Text = "Build Server";
			this.ResumeLayout(false);

		}

		#endregion

		public BuildServer ChooseNewBuildServer(IWin32Window parent)
		{
			if (ShowDialog(parent) == DialogResult.OK)
			{
				return buildServer;
			}

			return null;
		}

		private void rdoDashboard_CheckedChanged(object sender, EventArgs e)
		{
			UpdateButtons();
		}

		private void rdoRemoting_CheckedChanged(object sender, EventArgs e)
		{
			UpdateButtons();
		}

		private void rdoHttp_CheckedChanged(object sender, EventArgs e)
		{
			UpdateButtons();
		}

		private void UpdateButtons()
		{
			txtDashboard.Enabled = rdoDashboard.Checked;
			txtHttp.Enabled = rdoHttp.Checked;
			txtRemoting.Enabled = rdoRemoting.Checked;

			if (txtDashboard.Enabled)
				txtDashboard.Focus();

			if (txtHttp.Enabled)
				txtHttp.Focus();

			if (txtRemoting.Enabled)
				txtRemoting.Focus();
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			buildServer = null;

			lblFeedback.Visible = true;
			lblFeedback.Text = "Validating...";
			Refresh();

			try
			{
				buildServer = ConstructBuildServerFromSelectedOptions();
				ValidateConnection(buildServer);
				DialogResult = DialogResult.OK;
				Hide();
			}
			catch (Exception ex)
			{
				lblFeedback.Text = "Failed to connect to server: " + ex.Message;
			}
		}

		private void ValidateConnection(BuildServer server)
		{
			cruiseProjectManagerFactory.GetProjectList(server);
		}

		private BuildServer ConstructBuildServerFromSelectedOptions()
		{
			if (txtRemoting.Enabled)
			{
				return BuildServer.BuildFromRemotingDisplayName(txtRemoting.Text);
			}

			if (txtHttp.Enabled)
			{
				return new BuildServer(txtHttp.Text);
			}

			if (txtDashboard.Enabled)
			{
				return new BuildServer(txtDashboard.Text + "/XmlStatusReport.aspx");
			}

			return null;
		}
	}
}