using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using System.Windows.Forms;

using Drew.Agents;

namespace ThoughtWorks.CruiseControl.Remote.Monitor
{
	/// <summary>
	/// Monitors CruiseControl.NET build activity from a remote machine (normally a development PC)
	/// and reports on the state of the build.  A variety of notification mechanisms are supported,
	/// including system tray icons (the default, and most basic), popup balloon messages, and
	/// Microsoft Agent characters with text-to-speech support.
	/// </summary>
	public class SystemTrayMonitor : Form
	{
		#region Field declarations

		IContainer components;
		ContextMenu contextMenu;
		NotifyIconEx trayIcon;
		Hashtable _icons = null;
		Agent _agent = null;

		MenuItem mnuExit;
		MenuItem mnuLaunchWebPage;
		MenuItem mnuSettings;
		StatusMonitor statusMonitor;
		SettingsForm settingsForm;
		Settings settings;

		Exception _agentException = null;
		private System.Windows.Forms.MenuItem mnuForceBuild;
		Exception _audioException = null;

		#endregion

		#region Constructor

		public SystemTrayMonitor()
		{
			InitializeComponent();
			InitialiseTrayIcon();
			InitialiseSettings();
			InitialiseMonitor();
			InitialiseSettingsForm();

			DisplayStartupBalloon();
		}

		#endregion

		#region Initialisation

		void InitialiseSettings()
		{
			settings = SettingsManager.LoadSettings();
		}

		void InitialiseMonitor()
		{
			statusMonitor.Settings = settings;
			statusMonitor.StartPolling();
		}

		void InitialiseTrayIcon()
		{
			trayIcon.Icon = GetStatusIcon(IntegrationStatus.Unknown);
		}

		void DisplayStartupBalloon()
		{
			if (settings.NotificationBalloon.ShowBalloon)
				trayIcon.ShowBalloon("CruiseControl.NET Monitor", "Monitor started.", NotifyInfoFlags.Info, 1500);
		}

		private void CCTray_Load(object sender, System.EventArgs e)
		{
			// calling Hide on the window ensures the form's icon doesn't appear
			// while ALT+TABbing between applications, even though it won't appear
			// in the taskbar
			this.Hide();
		}

		void InitialiseSettingsForm()
		{
			settingsForm = new SettingsForm(settings, statusMonitor);
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
			this.trayIcon = new ThoughtWorks.CruiseControl.Remote.Monitor.NotifyIconEx();
			this.contextMenu = new System.Windows.Forms.ContextMenu();
			this.mnuLaunchWebPage = new System.Windows.Forms.MenuItem();
			this.mnuSettings = new System.Windows.Forms.MenuItem();
			this.mnuForceBuild = new System.Windows.Forms.MenuItem();
			this.mnuExit = new System.Windows.Forms.MenuItem();
			this.statusMonitor = new ThoughtWorks.CruiseControl.Remote.Monitor.StatusMonitor(this.components);
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
																						this.mnuSettings,
																						this.mnuForceBuild,
																						this.mnuExit});
			this.contextMenu.Popup += new System.EventHandler(this.contextMenu_Popup);
			// 
			// mnuLaunchWebPage
			// 
			this.mnuLaunchWebPage.Index = 0;
			this.mnuLaunchWebPage.Text = "&Launch web page";
			this.mnuLaunchWebPage.Click += new System.EventHandler(this.mnuLaunchWebPage_Click);
			// 
			// mnuSettings
			// 
			this.mnuSettings.Index = 1;
			this.mnuSettings.Text = "&Settings...";
			this.mnuSettings.Click += new System.EventHandler(this.mnuSettings_Click);
			// 
			// mnuForceBuild
			// 
			this.mnuForceBuild.Index = 2;
			this.mnuForceBuild.Text = "&Force build";
			this.mnuForceBuild.Click += new System.EventHandler(this.mnuForceBuild_Click);
			// 
			// mnuExit
			// 
			this.mnuExit.Index = 3;
			this.mnuExit.Text = "E&xit";
			this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
			// 
			// statusMonitor
			// 
			this.statusMonitor.Settings = null;
			this.statusMonitor.Error += new ThoughtWorks.CruiseControl.Remote.Monitor.ErrorEventHandler(this.statusMonitor_Error);
			this.statusMonitor.BuildOccurred += new ThoughtWorks.CruiseControl.Remote.Monitor.BuildOccurredEventHandler(this.statusMonitor_BuildOccurred);
			this.statusMonitor.Polled += new ThoughtWorks.CruiseControl.Remote.Monitor.PolledEventHandler(this.statusMonitor_Polled);
			// 
			// SystemTrayMonitor
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(104, 19);
			this.ControlBox = false;
			this.Enabled = false;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SystemTrayMonitor";
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
			Application.Run(new SystemTrayMonitor());
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
			_exception = null;

			// update tray icon and tooltip
			trayIcon.Text = CalculateTrayText(e.ProjectStatus);
			trayIcon.Icon = GetStatusIcon(e.ProjectStatus);
		}

		private void statusMonitor_BuildOccurred(object sauce, BuildOccurredEventArgs e)
		{
			_exception = null;

			string caption = e.BuildTransitionInfo.Caption;
			string description = settings.Messages.GetMessageForTransition(e.BuildTransition);
			NotifyInfoFlags icon = GetNotifyInfoFlag(e.BuildTransitionInfo.ErrorLevel);

			HandleBalloonNotification(caption, description, icon);
			HandleAgentNotification(description, caption);

			// play audio, in accordance to settings
			PlayBuildAudio(e.BuildTransition);
		}

		Exception _exception;

		private void statusMonitor_Error(object sender, ErrorEventArgs e)
		{
			if (_exception==null && settings.ShowExceptions)
			{
				// set the exception before displaying the dialog, because the timer keeps polling and subsequent
				// polls would otherwise cause multiple dialogs to be displayed
				_exception = e.Exception;

				MessageBox.Show(e.Exception.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			_exception = e.Exception;

			trayIcon.Text = GetErrorMessage(e.Exception);
			trayIcon.Icon = GetStatusIcon(IntegrationStatus.Exception);
		}

		#endregion

		#region Agent notification

		void HandleAgentNotification(string description, string caption)
		{
			if (settings.Agents.ShowAgent)
			{
				try 
				{
					EnsureAgentLoaded();
				
					_agent.Speak(description);

					// Hide agent
					if (settings.Agents.HideAfterMessage)
					{
						_agent.Hide();
					}
				}
				catch (Exception ex)
				{
					// only display the first exception with agents
					if (_agentException==null)
					{
						MessageBox.Show(ex.Message, "Unable to initialise agent", MessageBoxButtons.OK, MessageBoxIcon.Error);
						_agentException = ex;
					}
				}
			}
		}

		void EnsureAgentLoaded()
		{
			if (_agent==null)
				_agent = settings.Agents.CreateAgent();
		}


		#endregion

		#region Balloon notification

		void HandleBalloonNotification(string caption, string description, NotifyInfoFlags icon)
		{
			// show a balloon
			if (settings.NotificationBalloon.ShowBalloon)
				trayIcon.ShowBalloon(caption, description, icon, 5000);
		}


		#endregion

		#region Playing of audio

		void PlayBuildAudio(BuildTransition transition)
		{
			if (settings.Sounds.ShouldPlaySoundForTransition(transition))
			{
				try
				{
					PlayAudioFile(settings.Sounds.GetAudioFileLocation(transition));
				}
				catch (Exception ex)
				{
					// only display the first exception with audio
					if (_audioException==null)
					{
						MessageBox.Show(ex.Message, "Unable to initialise audio", MessageBoxButtons.OK, MessageBoxIcon.Error);
						_audioException = ex;
					}
				}
			}
		}

		void PlayAudioFile(string fileName)
		{
			Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			byte[] bytes = new byte[stream.Length];
			stream.Read(bytes, 0, bytes.Length);
			Audio.PlaySound(bytes, true, true);
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

			if (!_icons.ContainsKey(status))
				throw new Exception("Unsupported IntegrationStatus: " + status);

			return (Icon)_icons[status];
		}

		void LoadIcons()
		{
			_icons = new Hashtable(3);
			_icons[IntegrationStatus.Failure] = LoadIcon("ThoughtWorks.CruiseControl.Remote.Monitor.Red.ico");
			_icons[IntegrationStatus.Success] = LoadIcon("ThoughtWorks.CruiseControl.Remote.Monitor.Green.ico");
			_icons[IntegrationStatus.Unknown] = LoadIcon("ThoughtWorks.CruiseControl.Remote.Monitor.Gray.ico");
			_icons[IntegrationStatus.Exception] = LoadIcon("ThoughtWorks.CruiseControl.Remote.Monitor.Gray.ico");
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

			return string.Format("Server: {0}\nProject: {1}\nLast Build: {2} ({3})", 
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

		#region Settings

		private void mnuSettings_Click(object sender, System.EventArgs e)
		{
			settingsForm.Launch();
		}

		#endregion

		#region Forcing a build

		void mnuForceBuild_Click(object sender, System.EventArgs e)
		{
			try
			{
				statusMonitor.ForceBuild(settings.ProjectName);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Unable to force build", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		#endregion

		#region Context menu management

		void contextMenu_Popup(object sender, System.EventArgs e)
		{
			mnuForceBuild.Enabled = (statusMonitor.ProjectStatus.Activity==ProjectActivity.Sleeping);
		}

		#endregion
	}
}
