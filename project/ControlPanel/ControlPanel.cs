using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.Remoting;

using tw.ccnet.remote;

namespace tw.ccnet.controlpanel
{
    public class ControlPanel : System.Windows.Forms.Form
    {
        internal System.Windows.Forms.NotifyIcon notifyIcon;
        internal System.Windows.Forms.TabControl tabControl;
        internal CruiseControlStatus status = CruiseControlStatus.Unknown;
        internal System.Windows.Forms.StatusBar statusBar;
        internal System.Windows.Forms.Timer timer;
        private System.ComponentModel.IContainer components;
        private ICruiseManager manager;

        // service manager page
        private System.Windows.Forms.TabPage serviceManagerPage;
        internal System.Windows.Forms.Button startButton;
        internal System.Windows.Forms.Button stopButton;
        internal System.Windows.Forms.Button stopNowButton;
        internal System.Windows.Forms.Button projectStatusButton;
        internal System.Windows.Forms.TextBox statusTextBox;

        // configuration page
        internal System.Windows.Forms.TabPage configurationPage;
        internal System.Windows.Forms.TextBox configurationTextBox;
        internal System.Windows.Forms.Button loadConfigurationButton;
        internal System.Windows.Forms.Button saveConfigurationButton;

        // console output page
        private System.Windows.Forms.TabPage consoleOutputPage;
        internal System.Windows.Forms.TextBox consoleOutputTextBox;

        public ControlPanel(ICruiseManager manager)
        {
            this.manager = manager; 

            InitializeComponent();
            
            notifyIcon.ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("Open", new EventHandler(Open)),
                    new MenuItem("Exit", new EventHandler(Exit))});

//            ConsoleOutputSink sink = new ConsoleOutputSink(this);
//            manager.AttachConsoleOutputSink(sink);
        }

        protected override void Dispose( bool disposing )
        {
            if (disposing && components != null) 
                components.Dispose();
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ControlPanel));
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.tabControl = new System.Windows.Forms.TabControl();
            this.serviceManagerPage = new System.Windows.Forms.TabPage();
            this.statusTextBox = new System.Windows.Forms.TextBox();
            this.statusBar = new System.Windows.Forms.StatusBar();
            this.startButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.stopNowButton = new System.Windows.Forms.Button();
            this.projectStatusButton = new System.Windows.Forms.Button();
            this.configurationPage = new System.Windows.Forms.TabPage();
            this.loadConfigurationButton = new System.Windows.Forms.Button();
            this.configurationTextBox = new System.Windows.Forms.TextBox();
            this.saveConfigurationButton = new System.Windows.Forms.Button();
            this.consoleOutputPage = new System.Windows.Forms.TabPage();
            this.consoleOutputTextBox = new System.Windows.Forms.TextBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.tabControl.SuspendLayout();
            this.serviceManagerPage.SuspendLayout();
            this.configurationPage.SuspendLayout();
            this.consoleOutputPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "CC.Net Control Panel";
            this.notifyIcon.Visible = true;
            this.notifyIcon.DoubleClick += new System.EventHandler(this.Open);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.serviceManagerPage);
            this.tabControl.Controls.Add(this.configurationPage);
            this.tabControl.Controls.Add(this.consoleOutputPage);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(552, 430);
            this.tabControl.TabIndex = 0;
            // 
            // serviceManagerPage
            // 
            this.serviceManagerPage.Controls.Add(this.statusTextBox);
            this.serviceManagerPage.Controls.Add(this.statusBar);
            this.serviceManagerPage.Controls.Add(this.startButton);
            this.serviceManagerPage.Controls.Add(this.stopButton);
            this.serviceManagerPage.Controls.Add(this.stopNowButton);
            this.serviceManagerPage.Controls.Add(this.projectStatusButton);
            this.serviceManagerPage.Location = new System.Drawing.Point(4, 22);
            this.serviceManagerPage.Name = "serviceManagerPage";
            this.serviceManagerPage.Size = new System.Drawing.Size(544, 404);
            this.serviceManagerPage.TabIndex = 0;
            this.serviceManagerPage.Text = "Service Manager";
            // 
            // statusTextBox
            // 
            this.statusTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.statusTextBox.Location = new System.Drawing.Point(104, 0);
            this.statusTextBox.Multiline = true;
            this.statusTextBox.Name = "statusTextBox";
            this.statusTextBox.Size = new System.Drawing.Size(440, 384);
            this.statusTextBox.TabIndex = 2;
            this.statusTextBox.Text = "";
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point(0, 382);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(544, 22);
            this.statusBar.TabIndex = 1;
            this.statusBar.Text = "Service is running";
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(16, 16);
            this.startButton.Name = "startButton";
            this.startButton.TabIndex = 0;
            this.startButton.Text = "Start";
            this.startButton.Click += new System.EventHandler(this.OnStart);
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(16, 56);
            this.stopButton.Name = "stopButton";
            this.stopButton.TabIndex = 0;
            this.stopButton.Text = "Stop";
            this.stopButton.Click += new System.EventHandler(this.OnStop);
            // 
            // stopNowButton
            // 
            this.stopNowButton.Location = new System.Drawing.Point(16, 96);
            this.stopNowButton.Name = "stopNowButton";
            this.stopNowButton.TabIndex = 0;
            this.stopNowButton.Text = "Stop Now";
            this.stopNowButton.Click += new System.EventHandler(this.OnStopNow);
            // 
            // projectStatusButton
            // 
            this.projectStatusButton.Location = new System.Drawing.Point(16, 136);
            this.projectStatusButton.Name = "projectStatusButton";
            this.projectStatusButton.Size = new System.Drawing.Size(75, 40);
            this.projectStatusButton.TabIndex = 0;
            this.projectStatusButton.Text = "Get Project Status";
            this.projectStatusButton.Click += new System.EventHandler(this.OnGetProjectStatus);
            // 
            // configurationPage
            // 
            this.configurationPage.Controls.Add(this.loadConfigurationButton);
            this.configurationPage.Controls.Add(this.configurationTextBox);
            this.configurationPage.Controls.Add(this.saveConfigurationButton);
            this.configurationPage.Location = new System.Drawing.Point(4, 22);
            this.configurationPage.Name = "configurationPage";
            this.configurationPage.Size = new System.Drawing.Size(544, 404);
            this.configurationPage.TabIndex = 1;
            this.configurationPage.Text = "Configuration";
            // 
            // loadConfigurationButton
            // 
            this.loadConfigurationButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.loadConfigurationButton.Location = new System.Drawing.Point(376, 376);
            this.loadConfigurationButton.Name = "loadConfigurationButton";
            this.loadConfigurationButton.TabIndex = 1;
            this.loadConfigurationButton.Text = "Load";
            this.loadConfigurationButton.Click += new System.EventHandler(this.OnLoadConfiguration);
            // 
            // configurationTextBox
            // 
            this.configurationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.configurationTextBox.Location = new System.Drawing.Point(0, 0);
            this.configurationTextBox.Multiline = true;
            this.configurationTextBox.Name = "configurationTextBox";
            this.configurationTextBox.Size = new System.Drawing.Size(544, 368);
            this.configurationTextBox.TabIndex = 0;
            this.configurationTextBox.Text = "";
            // 
            // saveConfigurationButton
            // 
            this.saveConfigurationButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveConfigurationButton.Location = new System.Drawing.Point(464, 376);
            this.saveConfigurationButton.Name = "saveConfigurationButton";
            this.saveConfigurationButton.TabIndex = 1;
            this.saveConfigurationButton.Text = "Save";
            this.saveConfigurationButton.Click += new System.EventHandler(this.OnSaveConfiguration);
            // 
            // consoleOutputPage
            // 
            this.consoleOutputPage.Controls.Add(this.consoleOutputTextBox);
            this.consoleOutputPage.Location = new System.Drawing.Point(4, 22);
            this.consoleOutputPage.Name = "consoleOutputPage";
            this.consoleOutputPage.Size = new System.Drawing.Size(544, 404);
            this.consoleOutputPage.TabIndex = 2;
            this.consoleOutputPage.Text = "Console Output";
            // 
            // consoleOutputTextBox
            // 
            this.consoleOutputTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.consoleOutputTextBox.Location = new System.Drawing.Point(0, 0);
            this.consoleOutputTextBox.Multiline = true;
            this.consoleOutputTextBox.Name = "consoleOutputTextBox";
            this.consoleOutputTextBox.ReadOnly = true;
            this.consoleOutputTextBox.Size = new System.Drawing.Size(544, 404);
            this.consoleOutputTextBox.TabIndex = 0;
            this.consoleOutputTextBox.Text = "";
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.OnTimerTick);
            // 
            // ControlPanel
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(552, 430);
            this.Controls.Add(this.tabControl);
            this.Name = "ControlPanel";
            this.Text = "CC.Net Control Panel";
            this.Resize += new System.EventHandler(this.OnResize);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.OnClosing);
            this.tabControl.ResumeLayout(false);
            this.serviceManagerPage.ResumeLayout(false);
            this.configurationPage.ResumeLayout(false);
            this.consoleOutputPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private void Open(object sender, System.EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void Exit(object sender, EventArgs e) 
        {
            Application.Exit();
        }

        private void OnResize(object sender, System.EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized) 
                Hide();
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            WindowState = FormWindowState.Minimized;
        }

        internal void OnTimerTick(object sender, System.EventArgs e)
        {
            CruiseControlStatus newStatus = manager.GetStatus();
            if (status != newStatus) 
            {
                status = newStatus;
                startButton.Enabled = false;
                stopButton.Enabled = false;
                stopNowButton.Enabled = false;

                switch(status) 
                {
                    case CruiseControlStatus.Running:
                        stopButton.Enabled = true;
                        stopNowButton.Enabled = true;
                        statusBar.Text = "Service is running.";
                        break;
                    case CruiseControlStatus.Stopped:
                        startButton.Enabled = true;
                        statusBar.Text = "Service is stopped.";
                        break;
                    case CruiseControlStatus.WillBeStopped:
                        statusBar.Text = "Service is stopping...";
                        break;
                }
            }
        }

        private void OnStart(object sender, System.EventArgs e)
        {
            manager.StartCruiseControl();
        }

        private void OnStop(object sender, System.EventArgs e)
        {
            manager.StopCruiseControl();
        }

        private void OnStopNow(object sender, System.EventArgs e)
        {
            manager.StopCruiseControlNow();
        }

        private void OnGetProjectStatus(object sender, System.EventArgs e)
        {
        }

        [STAThread]
        static void Main() 
        {
            string url = "tcp://localhost:1234/CruiseManager.rem";
            ICruiseManager manager = (ICruiseManager) RemotingServices.Connect(typeof(ICruiseManager), url);
            try 
            {
                manager.GetStatus();
            } 
            catch (System.Net.Sockets.SocketException e)
            {
                MessageBox.Show("Can't connect to cruise control, make sure it's running with remoting turned on\n" + e);
                return;
            }

            ControlPanel panel = new ControlPanel(manager);
            Application.Run(panel);
        }

        private void OnLoadConfiguration(object sender, System.EventArgs e)
        {
            configurationTextBox.Text = manager.Configuration;
        }

        private void OnSaveConfiguration(object sender, System.EventArgs e)
        {
            manager.Configuration = configurationTextBox.Text;        
        }
    }
}
