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
		private Button btnCancel;
		private Button btnOK;
		private RadioButton rdoDashboard;
		private RadioButton rdoRemoting;
        private RadioButton rdoHttp;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

        private BuildServer buildServer;
        private RadioButton rdoExtension;
		private ICruiseProjectManagerFactory cruiseProjectManagerFactory;
        private TextBox txtFeedback;
        private Panel dashboardPanel;
        private TextBox txtDashboard;
        private Label label2;
        private Panel remotingPanel;
        private CheckBox connectToOldServer;
        private TextBox txtRemoting;
        private Label label4;
        private Label label3;
        private Panel nonCcnetPanel;
        private TextBox txtHttp;
        private Label label5;
        private Panel extensionPanel;
        private Button btnConfigureExtension;
        private ComboBox cmbExtension;
        private Label label6;
        private Label lblExtensionSettings;
        private CheckBox connectToOldDashboard;
        private Label label7;
        private CheckBox remotingEncryption;
        private Label label8;
        private CheckBox httpEncryption;
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.rdoExtension = new System.Windows.Forms.RadioButton();
            this.txtFeedback = new System.Windows.Forms.TextBox();
            this.dashboardPanel = new System.Windows.Forms.Panel();
            this.connectToOldDashboard = new System.Windows.Forms.CheckBox();
            this.txtDashboard = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.remotingPanel = new System.Windows.Forms.Panel();
            this.connectToOldServer = new System.Windows.Forms.CheckBox();
            this.txtRemoting = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.nonCcnetPanel = new System.Windows.Forms.Panel();
            this.txtHttp = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.extensionPanel = new System.Windows.Forms.Panel();
            this.lblExtensionSettings = new System.Windows.Forms.Label();
            this.btnConfigureExtension = new System.Windows.Forms.Button();
            this.cmbExtension = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.remotingEncryption = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.httpEncryption = new System.Windows.Forms.CheckBox();
            this.dashboardPanel.SuspendLayout();
            this.remotingPanel.SuspendLayout();
            this.nonCcnetPanel.SuspendLayout();
            this.extensionPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(17, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(623, 30);
            this.label1.TabIndex = 0;
            this.label1.Text = "CCTray can monitor build servers in different ways.  Select how you want to monit" +
                "or the server, then enter the required information.";
            // 
            // rdoDashboard
            // 
            this.rdoDashboard.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoDashboard.Location = new System.Drawing.Point(17, 43);
            this.rdoDashboard.Name = "rdoDashboard";
            this.rdoDashboard.Size = new System.Drawing.Size(623, 24);
            this.rdoDashboard.TabIndex = 1;
            this.rdoDashboard.Text = "Via the CruiseControl.NET dashboard";
            this.rdoDashboard.CheckedChanged += new System.EventHandler(this.rdoDashboard_CheckedChanged);
            // 
            // rdoRemoting
            // 
            this.rdoRemoting.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoRemoting.Location = new System.Drawing.Point(17, 70);
            this.rdoRemoting.Name = "rdoRemoting";
            this.rdoRemoting.Size = new System.Drawing.Size(623, 24);
            this.rdoRemoting.TabIndex = 4;
            this.rdoRemoting.TabStop = true;
            this.rdoRemoting.Text = "Connect directly using .NET remoting";
            this.rdoRemoting.CheckedChanged += new System.EventHandler(this.rdoRemoting_CheckedChanged);
            // 
            // rdoHttp
            // 
            this.rdoHttp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoHttp.Location = new System.Drawing.Point(17, 97);
            this.rdoHttp.Name = "rdoHttp";
            this.rdoHttp.Size = new System.Drawing.Size(623, 24);
            this.rdoHttp.TabIndex = 9;
            this.rdoHttp.Text = "Supply a custom HTTP URL";
            this.rdoHttp.CheckedChanged += new System.EventHandler(this.rdoHttp_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.CausesValidation = false;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCancel.Location = new System.Drawing.Point(334, 384);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 19;
            this.btnCancel.Text = "Cancel";
            // 
            // btnOK
            // 
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnOK.Location = new System.Drawing.Point(253, 384);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 18;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // rdoExtension
            // 
            this.rdoExtension.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoExtension.Location = new System.Drawing.Point(17, 124);
            this.rdoExtension.Name = "rdoExtension";
            this.rdoExtension.Size = new System.Drawing.Size(623, 24);
            this.rdoExtension.TabIndex = 12;
            this.rdoExtension.Text = "Using a transport extension";
            this.rdoExtension.CheckedChanged += new System.EventHandler(this.rdoExtension_CheckedChanged);
            // 
            // txtFeedback
            // 
            this.txtFeedback.Location = new System.Drawing.Point(17, 322);
            this.txtFeedback.Multiline = true;
            this.txtFeedback.Name = "txtFeedback";
            this.txtFeedback.ReadOnly = true;
            this.txtFeedback.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtFeedback.Size = new System.Drawing.Size(623, 56);
            this.txtFeedback.TabIndex = 17;
            this.txtFeedback.TabStop = false;
            // 
            // dashboardPanel
            // 
            this.dashboardPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dashboardPanel.Controls.Add(this.label8);
            this.dashboardPanel.Controls.Add(this.httpEncryption);
            this.dashboardPanel.Controls.Add(this.connectToOldDashboard);
            this.dashboardPanel.Controls.Add(this.txtDashboard);
            this.dashboardPanel.Controls.Add(this.label2);
            this.dashboardPanel.Location = new System.Drawing.Point(17, 151);
            this.dashboardPanel.Name = "dashboardPanel";
            this.dashboardPanel.Size = new System.Drawing.Size(623, 165);
            this.dashboardPanel.TabIndex = 20;
            this.dashboardPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // connectToOldDashboard
            // 
            this.connectToOldDashboard.AutoSize = true;
            this.connectToOldDashboard.Location = new System.Drawing.Point(10, 65);
            this.connectToOldDashboard.Name = "connectToOldDashboard";
            this.connectToOldDashboard.Size = new System.Drawing.Size(155, 17);
            this.connectToOldDashboard.TabIndex = 13;
            this.connectToOldDashboard.Text = "Connect to pre-1.5.0 server";
            this.connectToOldDashboard.UseVisualStyleBackColor = true;
            this.connectToOldDashboard.CheckedChanged += new System.EventHandler(this.connectToOldDashboard_CheckedChanged);
            // 
            // txtDashboard
            // 
            this.txtDashboard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDashboard.Location = new System.Drawing.Point(9, 39);
            this.txtDashboard.Name = "txtDashboard";
            this.txtDashboard.Size = new System.Drawing.Size(607, 20);
            this.txtDashboard.TabIndex = 5;
            this.txtDashboard.Text = "http://localhost/ccnet";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(7, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(609, 30);
            this.label2.TabIndex = 4;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // remotingPanel
            // 
            this.remotingPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.remotingPanel.Controls.Add(this.label7);
            this.remotingPanel.Controls.Add(this.remotingEncryption);
            this.remotingPanel.Controls.Add(this.connectToOldServer);
            this.remotingPanel.Controls.Add(this.txtRemoting);
            this.remotingPanel.Controls.Add(this.label4);
            this.remotingPanel.Controls.Add(this.label3);
            this.remotingPanel.Location = new System.Drawing.Point(17, 151);
            this.remotingPanel.Name = "remotingPanel";
            this.remotingPanel.Size = new System.Drawing.Size(623, 165);
            this.remotingPanel.TabIndex = 21;
            this.remotingPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // connectToOldServer
            // 
            this.connectToOldServer.AutoSize = true;
            this.connectToOldServer.Location = new System.Drawing.Point(6, 107);
            this.connectToOldServer.Name = "connectToOldServer";
            this.connectToOldServer.Size = new System.Drawing.Size(155, 17);
            this.connectToOldServer.TabIndex = 12;
            this.connectToOldServer.Text = "Connect to pre-1.5.0 server";
            this.connectToOldServer.UseVisualStyleBackColor = true;
            this.connectToOldServer.CheckedChanged += new System.EventHandler(this.connectToOldServer_CheckedChanged);
            // 
            // txtRemoting
            // 
            this.txtRemoting.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRemoting.Location = new System.Drawing.Point(6, 77);
            this.txtRemoting.Name = "txtRemoting";
            this.txtRemoting.Size = new System.Drawing.Size(610, 20);
            this.txtRemoting.TabIndex = 11;
            this.txtRemoting.Text = "localhost";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.Location = new System.Drawing.Point(6, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(610, 30);
            this.label4.TabIndex = 10;
            this.label4.Text = resources.GetString("label4.Text");
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(6, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(610, 30);
            this.label3.TabIndex = 9;
            this.label3.Text = "CCTray will connect directly to the build server using .NET Remoting.  This is ho" +
                "w CruiseControl.NET 1.0 worked, but it often forces you to install a new version" +
                " of CCTray when the server is upgraded.";
            // 
            // nonCcnetPanel
            // 
            this.nonCcnetPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.nonCcnetPanel.Controls.Add(this.txtHttp);
            this.nonCcnetPanel.Controls.Add(this.label5);
            this.nonCcnetPanel.Location = new System.Drawing.Point(17, 151);
            this.nonCcnetPanel.Name = "nonCcnetPanel";
            this.nonCcnetPanel.Size = new System.Drawing.Size(623, 165);
            this.nonCcnetPanel.TabIndex = 21;
            this.nonCcnetPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.panel3_Paint);
            // 
            // txtHttp
            // 
            this.txtHttp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHttp.Location = new System.Drawing.Point(9, 65);
            this.txtHttp.Name = "txtHttp";
            this.txtHttp.Size = new System.Drawing.Size(607, 20);
            this.txtHttp.TabIndex = 13;
            this.txtHttp.Text = "http://localhost/cruisecontrol/xml.jsp";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.Location = new System.Drawing.Point(7, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(609, 52);
            this.label5.TabIndex = 12;
            this.label5.Text = resources.GetString("label5.Text");
            // 
            // extensionPanel
            // 
            this.extensionPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.extensionPanel.Controls.Add(this.lblExtensionSettings);
            this.extensionPanel.Controls.Add(this.btnConfigureExtension);
            this.extensionPanel.Controls.Add(this.cmbExtension);
            this.extensionPanel.Controls.Add(this.label6);
            this.extensionPanel.Location = new System.Drawing.Point(17, 151);
            this.extensionPanel.Name = "extensionPanel";
            this.extensionPanel.Size = new System.Drawing.Size(623, 165);
            this.extensionPanel.TabIndex = 22;
            this.extensionPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.panel4_Paint);
            // 
            // lblExtensionSettings
            // 
            this.lblExtensionSettings.AutoSize = true;
            this.lblExtensionSettings.Location = new System.Drawing.Point(7, 75);
            this.lblExtensionSettings.Name = "lblExtensionSettings";
            this.lblExtensionSettings.Size = new System.Drawing.Size(153, 13);
            this.lblExtensionSettings.TabIndex = 21;
            this.lblExtensionSettings.Text = "Please configure this extension";
            // 
            // btnConfigureExtension
            // 
            this.btnConfigureExtension.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfigureExtension.Location = new System.Drawing.Point(438, 70);
            this.btnConfigureExtension.Name = "btnConfigureExtension";
            this.btnConfigureExtension.Size = new System.Drawing.Size(178, 23);
            this.btnConfigureExtension.TabIndex = 20;
            this.btnConfigureExtension.Text = "Configure Extension";
            this.btnConfigureExtension.UseVisualStyleBackColor = true;
            // 
            // cmbExtension
            // 
            this.cmbExtension.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbExtension.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmbExtension.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbExtension.Location = new System.Drawing.Point(7, 47);
            this.cmbExtension.Name = "cmbExtension";
            this.cmbExtension.Size = new System.Drawing.Size(609, 21);
            this.cmbExtension.Sorted = true;
            this.cmbExtension.TabIndex = 18;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.Location = new System.Drawing.Point(7, 7);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(609, 37);
            this.label6.TabIndex = 17;
            this.label6.Text = "This will use an extension module to connect to the build server. This requires t" +
                "he extension be installed in the bin folder for CCTray.";
            // 
            // remotingEncryption
            // 
            this.remotingEncryption.AutoSize = true;
            this.remotingEncryption.Location = new System.Drawing.Point(6, 130);
            this.remotingEncryption.Name = "remotingEncryption";
            this.remotingEncryption.Size = new System.Drawing.Size(141, 17);
            this.remotingEncryption.TabIndex = 13;
            this.remotingEncryption.Text = "Encrypt communications";
            this.remotingEncryption.UseVisualStyleBackColor = true;
            this.remotingEncryption.CheckedChanged += new System.EventHandler(this.remotingEncryption_CheckedChanged);
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(167, 108);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(142, 39);
            this.label7.TabIndex = 14;
            this.label7.Text = "Encryption is not available for pre-1.5.0 servers.";
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(166, 65);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(142, 39);
            this.label8.TabIndex = 16;
            this.label8.Text = "Encryption is not available for pre-1.5.0 servers.";
            // 
            // httpEncryption
            // 
            this.httpEncryption.AutoSize = true;
            this.httpEncryption.Location = new System.Drawing.Point(10, 84);
            this.httpEncryption.Name = "httpEncryption";
            this.httpEncryption.Size = new System.Drawing.Size(141, 17);
            this.httpEncryption.TabIndex = 15;
            this.httpEncryption.Text = "Encrypt communications";
            this.httpEncryption.UseVisualStyleBackColor = true;
            this.httpEncryption.CheckedChanged += new System.EventHandler(this.httpEncryption_CheckedChanged);
            // 
            // AddBuildServer
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(652, 414);
            this.Controls.Add(this.txtFeedback);
            this.Controls.Add(this.rdoExtension);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.rdoHttp);
            this.Controls.Add(this.rdoRemoting);
            this.Controls.Add(this.rdoDashboard);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dashboardPanel);
            this.Controls.Add(this.extensionPanel);
            this.Controls.Add(this.nonCcnetPanel);
            this.Controls.Add(this.remotingPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AddBuildServer";
            this.Text = "Build Server";
            this.dashboardPanel.ResumeLayout(false);
            this.dashboardPanel.PerformLayout();
            this.remotingPanel.ResumeLayout(false);
            this.remotingPanel.PerformLayout();
            this.nonCcnetPanel.ResumeLayout(false);
            this.nonCcnetPanel.PerformLayout();
            this.extensionPanel.ResumeLayout(false);
            this.extensionPanel.PerformLayout();
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
            dashboardPanel.Visible = rdoDashboard.Checked;
            remotingPanel.Visible = rdoRemoting.Checked;
            nonCcnetPanel.Visible = rdoHttp.Checked;
            extensionPanel.Visible = rdoExtension.Checked;

			if (txtDashboard.Enabled) txtDashboard.Focus();

			if (txtHttp.Enabled) txtHttp.Focus();

			if (txtRemoting.Enabled) txtRemoting.Focus();

            if (cmbExtension.Enabled) cmbExtension.Focus();
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			buildServer = null;

			txtFeedback.Visible = true;
			txtFeedback.Text = "Validating...";
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
				txtFeedback.Text = "Failed to connect to server: " + ex.Message;
			}
		}

		private void ValidateConnection(BuildServer server)
		{
			cruiseProjectManagerFactory.GetProjectList(server, true);
		}

		private BuildServer ConstructBuildServerFromSelectedOptions()
		{
			if (rdoRemoting.Checked)
			{
				var server = BuildServer.BuildFromRemotingDisplayName(txtRemoting.Text);
                server.ExtensionSettings = ClientStartUpSettingsExtensions.GenerateExtensionSettings(connectToOldServer.Checked,
                    remotingEncryption.Checked);
                return server;
			}

			if (rdoHttp.Checked)
			{
				var server = new BuildServer(txtHttp.Text);
                server.ExtensionSettings = ClientStartUpSettingsExtensions.GenerateExtensionSettings(true, false);
                return server;
			}

            if (rdoDashboard.Checked)
			{
                var baseUri = txtDashboard.Text;
                if (baseUri.EndsWith("XmlStatusReport.aspx", StringComparison.CurrentCultureIgnoreCase) ||
                    baseUri.EndsWith("XmlServerReport.aspx", StringComparison.CurrentCultureIgnoreCase))
                {
                    baseUri = baseUri.Substring(0, baseUri.Length - 20);
                }
                var server = new BuildServer(baseUri);
                server.ExtensionSettings = ClientStartUpSettingsExtensions.GenerateExtensionSettings(connectToOldDashboard.Checked,
                    httpEncryption.Checked);
                return server;
			}

            if (rdoExtension.Checked)
            {
                if (transportExtension == null) throw new ApplicationException("No extension selected");
                var server = new BuildServer(transportExtension.Configuration.Url, 
                    BuildServerTransport.Extension, 
                    cmbExtension.Text, 
                    transportExtension.Settings);
                return server;
            }

			return null;
		}



        private void LoadExtensions()
        {
            cmbExtension.Items.Clear();
            string extensionsPath = Path.Combine(Environment.CurrentDirectory, "Extensions");
            cmbExtension.Items.AddRange(ExtensionHelpers.QueryAssembliesForTypes(extensionsPath, "ITransportExtension"));
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void connectToOldServer_CheckedChanged(object sender, EventArgs e)
        {
            if (connectToOldServer.Checked) remotingEncryption.Checked = false;
        }

        private void remotingEncryption_CheckedChanged(object sender, EventArgs e)
        {
            if (remotingEncryption.Checked) connectToOldServer.Checked = false;
        }

        private void connectToOldDashboard_CheckedChanged(object sender, EventArgs e)
        {
            if (connectToOldDashboard.Checked) httpEncryption.Checked = false;
        }

        private void httpEncryption_CheckedChanged(object sender, EventArgs e)
        {
            if (httpEncryption.Checked) connectToOldDashboard.Checked = false;
        }
	}
}