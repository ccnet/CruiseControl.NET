using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
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
        private Label label6;
        private RadioButton rdoExtension;
        private ComboBox cmbExtension;
        private Button btnConfigureExtension;
        private Label lblExtensionSettings;
		private ICruiseProjectManagerFactory cruiseProjectManagerFactory;
        private ITransportExtension transportExtension;

		public AddBuildServer(ICruiseProjectManagerFactory cruiseProjectManagerFactory)
		{
			this.cruiseProjectManagerFactory = cruiseProjectManagerFactory;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			rdoRemoting.Checked = true;
			UpdateButtons();
            LoadExtensions();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddBuildServer));
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
            this.label6 = new System.Windows.Forms.Label();
            this.rdoExtension = new System.Windows.Forms.RadioButton();
            this.cmbExtension = new System.Windows.Forms.ComboBox();
            this.btnConfigureExtension = new System.Windows.Forms.Button();
            this.lblExtensionSettings = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(5, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(562, 35);
            this.label1.TabIndex = 0;
            this.label1.Text = "CCTray can monitor build servers in different ways.  Select how you want to monit" +
                "or the server, then enter the required information.";
            // 
            // rdoDashboard
            // 
            this.rdoDashboard.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoDashboard.Location = new System.Drawing.Point(10, 38);
            this.rdoDashboard.Name = "rdoDashboard";
            this.rdoDashboard.Size = new System.Drawing.Size(370, 24);
            this.rdoDashboard.TabIndex = 1;
            this.rdoDashboard.Text = "Via the CruiseControl.NET dashboard";
            this.rdoDashboard.CheckedChanged += new System.EventHandler(this.rdoDashboard_CheckedChanged);
            // 
            // rdoRemoting
            // 
            this.rdoRemoting.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoRemoting.Location = new System.Drawing.Point(10, 120);
            this.rdoRemoting.Name = "rdoRemoting";
            this.rdoRemoting.Size = new System.Drawing.Size(345, 24);
            this.rdoRemoting.TabIndex = 4;
            this.rdoRemoting.TabStop = true;
            this.rdoRemoting.Text = "Connect directly using .NET remoting";
            this.rdoRemoting.CheckedChanged += new System.EventHandler(this.rdoRemoting_CheckedChanged);
            // 
            // rdoHttp
            // 
            this.rdoHttp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoHttp.Location = new System.Drawing.Point(10, 235);
            this.rdoHttp.Name = "rdoHttp";
            this.rdoHttp.Size = new System.Drawing.Size(345, 24);
            this.rdoHttp.TabIndex = 8;
            this.rdoHttp.Text = "Supply a custom HTTP URL";
            this.rdoHttp.CheckedChanged += new System.EventHandler(this.rdoHttp_CheckedChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(22, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(554, 45);
            this.label2.TabIndex = 2;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // txtDashboard
            // 
            this.txtDashboard.Location = new System.Drawing.Point(25, 97);
            this.txtDashboard.Name = "txtDashboard";
            this.txtDashboard.Size = new System.Drawing.Size(532, 20);
            this.txtDashboard.TabIndex = 3;
            this.txtDashboard.Text = "http://localhost/ccnet";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(22, 145);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(554, 45);
            this.label3.TabIndex = 5;
            this.label3.Text = "CCTray will connect directly to the build server using .NET Remoting.  This is ho" +
                "w CruiseControl.NET 1.0 worked, but it often forces you to install a new version" +
                " of CCTray when the server is upgraded.";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(22, 175);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(554, 45);
            this.label4.TabIndex = 6;
            this.label4.Text = resources.GetString("label4.Text");
            // 
            // txtRemoting
            // 
            this.txtRemoting.Location = new System.Drawing.Point(25, 210);
            this.txtRemoting.Name = "txtRemoting";
            this.txtRemoting.Size = new System.Drawing.Size(532, 20);
            this.txtRemoting.TabIndex = 7;
            this.txtRemoting.Text = "localhost";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(22, 262);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(554, 45);
            this.label5.TabIndex = 9;
            this.label5.Text = resources.GetString("label5.Text");
            // 
            // txtHttp
            // 
            this.txtHttp.Location = new System.Drawing.Point(25, 310);
            this.txtHttp.Name = "txtHttp";
            this.txtHttp.Size = new System.Drawing.Size(532, 20);
            this.txtHttp.TabIndex = 10;
            this.txtHttp.Text = "http://localhost/cruisecontrol/xml.jsp";
            // 
            // btnCancel
            // 
            this.btnCancel.CausesValidation = false;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCancel.Location = new System.Drawing.Point(305, 499);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 16;
            this.btnCancel.Text = "Cancel";
            // 
            // btnOK
            // 
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnOK.Location = new System.Drawing.Point(220, 499);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 15;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblFeedback
            // 
            this.lblFeedback.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblFeedback.Location = new System.Drawing.Point(30, 450);
            this.lblFeedback.Name = "lblFeedback";
            this.lblFeedback.Size = new System.Drawing.Size(527, 46);
            this.lblFeedback.TabIndex = 17;
            this.lblFeedback.Text = "lblFeedback";
            this.lblFeedback.Visible = false;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(22, 363);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(554, 31);
            this.label6.TabIndex = 12;
            this.label6.Text = "This will use an extension module to connect to the build server. This requires t" +
                "he extension be installed in the bin folder for CCTray.";
            // 
            // rdoExtension
            // 
            this.rdoExtension.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoExtension.Location = new System.Drawing.Point(12, 336);
            this.rdoExtension.Name = "rdoExtension";
            this.rdoExtension.Size = new System.Drawing.Size(345, 24);
            this.rdoExtension.TabIndex = 11;
            this.rdoExtension.Text = "Using a transport extension";
            this.rdoExtension.CheckedChanged += new System.EventHandler(this.rdoExtension_CheckedChanged);
            // 
            // cmbExtension
            // 
            this.cmbExtension.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmbExtension.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbExtension.Location = new System.Drawing.Point(25, 397);
            this.cmbExtension.Name = "cmbExtension";
            this.cmbExtension.Size = new System.Drawing.Size(532, 21);
            this.cmbExtension.Sorted = true;
            this.cmbExtension.TabIndex = 13;
            this.cmbExtension.SelectedValueChanged += new System.EventHandler(this.OnExtensionChanged);
            this.cmbExtension.TextChanged += new System.EventHandler(this.OnExtensionChanged);
            // 
            // btnConfigureExtension
            // 
            this.btnConfigureExtension.Location = new System.Drawing.Point(379, 424);
            this.btnConfigureExtension.Name = "btnConfigureExtension";
            this.btnConfigureExtension.Size = new System.Drawing.Size(178, 23);
            this.btnConfigureExtension.TabIndex = 14;
            this.btnConfigureExtension.Text = "Configure Extension";
            this.btnConfigureExtension.UseVisualStyleBackColor = true;
            this.btnConfigureExtension.Click += new System.EventHandler(this.btnConfigureExtension_Click);
            // 
            // lblExtensionSettings
            // 
            this.lblExtensionSettings.AutoSize = true;
            this.lblExtensionSettings.Location = new System.Drawing.Point(22, 429);
            this.lblExtensionSettings.Name = "lblExtensionSettings";
            this.lblExtensionSettings.Size = new System.Drawing.Size(153, 13);
            this.lblExtensionSettings.TabIndex = 18;
            this.lblExtensionSettings.Text = "Please configure this extension";
            // 
            // AddBuildServer
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(581, 530);
            this.Controls.Add(this.lblExtensionSettings);
            this.Controls.Add(this.btnConfigureExtension);
            this.Controls.Add(this.cmbExtension);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.rdoExtension);
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
            this.PerformLayout();

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

        private void rdoExtension_CheckedChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

		private void UpdateButtons()
		{
			txtDashboard.Enabled = rdoDashboard.Checked;
			txtHttp.Enabled = rdoHttp.Checked;
			txtRemoting.Enabled = rdoRemoting.Checked;
            cmbExtension.Enabled = rdoExtension.Checked;
            lblExtensionSettings.Enabled = rdoExtension.Checked;
            btnConfigureExtension.Enabled = rdoExtension.Checked && !string.IsNullOrEmpty(cmbExtension.Text);

			if (txtDashboard.Enabled)
				txtDashboard.Focus();

			if (txtHttp.Enabled)
				txtHttp.Focus();

			if (txtRemoting.Enabled)
				txtRemoting.Focus();

            if (cmbExtension.Enabled) cmbExtension.Focus();
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
                return new BuildServer(new WebDashboardUrl(txtDashboard.Text).XmlServerReport);
			}

            if (rdoExtension.Checked)
            {
                return new BuildServer(transportExtension.Configuration.Url, BuildServerTransport.Extension, cmbExtension.Text, transportExtension.Settings);
            }

			return null;
		}

        private void OnExtensionChanged(object sender, EventArgs e)
        {
            btnConfigureExtension.Enabled = !string.IsNullOrEmpty(cmbExtension.Text);
            lblExtensionSettings.Text = "Please configure this extension";
            transportExtension = null;
        }

        private void btnConfigureExtension_Click(object sender, EventArgs e)
        {
            try
            {
                if (transportExtension == null) transportExtension = ExtensionHelpers.RetrieveExtension(cmbExtension.Text);
                if (transportExtension.Configure(this))
                {
                    lblExtensionSettings.Text = transportExtension.DisplayName;
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(this,
                    "Unable to configure extension:" + Environment.NewLine + error.Message,
                    "Extension Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LoadExtensions()
        {
            cmbExtension.Items.Clear();
            string extensionsPath = Path.Combine(Environment.CurrentDirectory, "Extensions");
            cmbExtension.Items.AddRange(ExtensionHelpers.QueryAssembliesForTypes(extensionsPath, "ITransportExtension"));
        }
	}
}