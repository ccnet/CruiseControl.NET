using System;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Configuration;
using System.Reflection;
using System.Runtime.Remoting;

namespace tw.ccnet.remote.monitor
{
	public class CCTray : Form
	{
		const int DefaultPollingIntervalMillis = 15000;

		IContainer components;
		ContextMenu contextMenu;
		NotifyIconEx trayIcon;
		Hashtable _icons = null;

		MenuItem mnuExit;
		private System.Windows.Forms.MenuItem mnuLaunchWebPage;
		StatusMonitor statusMonitor;
		
		public CCTray()
		{
			InitializeComponent();
			InitialiseTrayIcon();
			DisplayStartupBalloon();

			this.Visible = false;

			statusMonitor.PollingIntervalSeconds = GetPollingIntervalFromConfiguration();
			statusMonitor.RemoteServerUrl = GetRemoteServerUrlFromConfiguration();
			statusMonitor.StartPolling();
		}


		#region Initialisation

		void InitialiseTrayIcon()
		{
			trayIcon.Icon = GetStatusIcon(IntegrationStatus.Unknown);
		}

		void DisplayStartupBalloon()
		{
			trayIcon.ShowBalloon("CruiseControl.NET Monitor", "Monitor started.", NotifyInfoFlags.Info, 1500);
		}


		string GetRemoteServerUrlFromConfiguration()
		{
			string remoteServerUrl = ConfigurationSettings.AppSettings["cc.net.url"];

			if (remoteServerUrl == null || remoteServerUrl.Length == 0)
			{
				DisplayConfigurationProblemAndExit("No server Url is specified");
			}

			return remoteServerUrl;
		}

		int GetPollingIntervalFromConfiguration()
		{
			int pollingInterval;

			string pollingIntervalString = ConfigurationSettings.AppSettings["check.interval"];
			if (pollingIntervalString == null || pollingIntervalString.Length == 0 || Convert.ToInt32(pollingIntervalString) == 0)
			{
				pollingInterval = DefaultPollingIntervalMillis;
			}
			else
			{
				pollingInterval = Convert.ToInt32(pollingIntervalString);
			}

			return pollingInterval;
		}

		void DisplayConfigurationProblemAndExit(string message)
		{
			trayIcon.ShowBalloon("Configuration Error", message, NotifyInfoFlags.Error, 3000);
			System.Threading.Thread.Sleep(2500);
			Exit();
		}

		private void CCTray_Load(object sender, System.EventArgs e)
		{
			// calling Hide on the window ensures the form's icon doesn't appear
			// while ALT+TABbing between applications, even though it won't appear
			// in the taskbar
			this.Hide();
		}


		#endregion

		#region Windows Form Designer generated code

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components!=null) 
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.trayIcon = new tw.ccnet.remote.monitor.NotifyIconEx();
			this.contextMenu = new System.Windows.Forms.ContextMenu();
			this.mnuExit = new System.Windows.Forms.MenuItem();
			this.statusMonitor = new tw.ccnet.remote.monitor.StatusMonitor(this.components);
			this.mnuLaunchWebPage = new System.Windows.Forms.MenuItem();
			// 
			// trayIcon
			// 
			this.trayIcon.ContextMenu = this.contextMenu;
			this.trayIcon.Icon = null;
			this.trayIcon.Text = "No Connection";
			this.trayIcon.Visible = true;
			this.trayIcon.DoubleClick += new System.EventHandler(this.trayIcon_DoubleClick);
			// 
			// contextMenu
			// 
			this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						this.mnuLaunchWebPage,
																						this.mnuExit});
			// 
			// mnuExit
			// 
			this.mnuExit.Index = 1;
			this.mnuExit.Text = "E&xit";
			this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
			// 
			// statusMonitor
			// 
			this.statusMonitor.PollingIntervalSeconds = 15;
			this.statusMonitor.RemoteServerUrl = null;
			this.statusMonitor.Error += new tw.ccnet.remote.monitor.ErrorEventHandler(this.statusMonitor_Error);
			this.statusMonitor.BuildOccurred += new tw.ccnet.remote.monitor.BuildOccurredEventHandler(this.statusMonitor_BuildOccurred);
			this.statusMonitor.Polled += new tw.ccnet.remote.monitor.PolledEventHandler(this.statusMonitor_Polled);
			// 
			// mnuLaunchWebPage
			// 
			this.mnuLaunchWebPage.Index = 0;
			this.mnuLaunchWebPage.Text = "&Launch web page";
			this.mnuLaunchWebPage.Click += new System.EventHandler(this.mnuLaunchWebPage_Click);
			// 
			// CCTray
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(104, 42);
			this.ControlBox = false;
			this.Enabled = false;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CCTray";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "CCTray";
			this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
			this.Load += new System.EventHandler(this.CCTray_Load);

		}

		#endregion

		#region Application start

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.Run(new CCTray());
		}


		#endregion

		#region Application exit

		private void mnuExit_Click(object sender, System.EventArgs e)
		{
			Exit();
		}

		void Exit()
		{
			statusMonitor.StopPolling();
			this.Close();
			Application.Exit();
		}


		#endregion

		#region Monitor event handlers

		private void statusMonitor_Polled(object sauce, PolledEventArgs e)
		{
			// update tray icon and tooltip
			trayIcon.Text = CalculateTrayText(e.ProjectStatus);
			trayIcon.Icon = GetStatusIcon(e.ProjectStatus);
		}

		private void statusMonitor_BuildOccurred(object sauce, BuildOccurredEventArgs e)
		{
			string caption = e.BuildTransitionInfo.Caption;
			string description = e.BuildTransitionInfo.Description;
			NotifyInfoFlags icon = GetNotifyInfoFlag(e.BuildTransitionInfo.ErrorLevel);

			// show a balloon
			trayIcon.ShowBalloon(caption, description, icon, 5000);
		}

		private void statusMonitor_Error(object sender, ErrorEventArgs e)
		{
			trayIcon.Text = GetErrorMessage(e.Exception);
			trayIcon.Icon = GetStatusIcon(IntegrationStatus.Unknown);
		}


		#endregion

		#region Icons

		Icon GetStatusIcon(ProjectStatus status)
		{
			return GetStatusIcon(status.BuildStatus);
		}

		Icon GetStatusIcon(IntegrationStatus status)
		{
			if (_icons==null)
				LoadIcons();

			return (Icon)_icons[status];
		}

		void LoadIcons()
		{
			_icons = new Hashtable(3);
			_icons[IntegrationStatus.Failure] = LoadIcon("tw.ccnet.remote.monitor.Red.ico");
			_icons[IntegrationStatus.Success] = LoadIcon("tw.ccnet.remote.monitor.Green.ico");
			_icons[IntegrationStatus.Unknown] = LoadIcon("tw.ccnet.remote.monitor.Gray.ico");
		}

		Icon LoadIcon(string name) 
		{
			return Icon.FromHandle(((Bitmap)Image.FromStream(GetIconStream(name))).GetHicon());
		}

		Stream GetIconStream(string name)
		{
			return Assembly.GetCallingAssembly().GetManifestResourceStream(name);
		}


		#endregion

		#region Presentation calculations

		string CalculateTrayText(ProjectStatus projectStatus)
		{
			object activity = (projectStatus.Status==CruiseControlStatus.Stopped) ? ProjectActivity.Unknown : projectStatus.Activity;

			return String.Format("Server: {0}\nProject: {1}\nLast Build: {2} ({3})", 
				activity,
				projectStatus.Name,
				projectStatus.BuildStatus,
				projectStatus.LastBuildLabel);
		}

		NotifyInfoFlags GetNotifyInfoFlag(ErrorLevel errorLevel)
		{
			if (errorLevel==ErrorLevel.Error)
				return NotifyInfoFlags.Error;
			else if (errorLevel==ErrorLevel.Info)
				return NotifyInfoFlags.Info;
			else if (errorLevel==ErrorLevel.Warning)
				return NotifyInfoFlags.Warning;
			else
				return NotifyInfoFlags.None;
		}

		string GetErrorMessage(Exception ex)
		{
			if (ex is RemotingException)
				return "No Connection";
			else
				return ex.Message;	
		}


		#endregion

		#region Launching web page

		private void trayIcon_DoubleClick(object sender, System.EventArgs e)
		{
			LaunchWebPage();
		}

		private void mnuLaunchWebPage_Click(object sender, System.EventArgs e)
		{
			LaunchWebPage();
		}

		// TODO keep tabs on browser process -- if it's still running (and still
		// on the same server) bring it to the foreground.

		void LaunchWebPage()
		{
			if (statusMonitor.WebUrl==null || statusMonitor.WebUrl.Trim().Length==0)
				UnableToLaunchWebPage();
			else
				Process.Start(statusMonitor.WebUrl);
		}

		void UnableToLaunchWebPage()
		{
			// TODO this messagebox appears in the background... bring it to the foreground somehow
			MessageBox.Show(this, "The web page url isn't specified.", "Unable to launch web page", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		#endregion
	}
}
