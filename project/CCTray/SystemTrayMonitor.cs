using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Windows.Forms;
using Drew.Agents;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTray
{
	/// <summary>
	/// Monitors CruiseControl.NET build activity from a remote machine (normally a development PC)
	/// and reports on the state of the build.  A variety of notification mechanisms are supported,
	/// including system tray icons (the default, and most basic), popup balloon messages, and
	/// Microsoft Agent characters with text-to-speech support.
	/// </summary>
	public class SystemTrayMonitor : Form
	{

		private IContainer components;
		private ContextMenu contextMenu;
		private NotifyIconEx trayIcon;
		private Agent _agent = null;

		private MenuItem mnuExit;
		private MenuItem mnuLaunchWebPage;
		private MenuItem mnuSettings;
		private StatusMonitor statusMonitor;
		private SettingsForm settingsForm;
		private Settings _settings;

		private Exception _agentException = null;

		private MenuItem mnuForceBuild;
		private MenuItem mnuProjects;
		private MenuItem mnuProject1PlaceHolder;
		Exception _audioException = null;
		private string _lastUrl = "";
		private IStatusIconLoader _iconLoader ;


		#region Constructor

		public SystemTrayMonitor()
		{
			InitializeComponent();
			InitialiseSettings();
			InitialiseTrayIcon();
			InitialiseMonitor();
			InitialiseProjectMenu();
			InitialiseSettingsForm();
			
			DisplayStartupBalloon();
		}

		#endregion

		#region Initialisation

		void InitialiseSettings()
		{
		    _settings = SettingsManager.LoadSettings();
		}

		void InitialiseMonitor()
		{
			statusMonitor.Settings = _settings;
			statusMonitor.StartPolling();
		}

		void InitialiseProjectMenu()
		{
			this.mnuProjects.MenuItems.Clear();
			foreach(ProjectStatus project in statusMonitor.GetRemoteProjects())
			{
				MenuItem menuItem = new MenuItem(project.Name);
				menuItem.Click += new EventHandler(this.mnuProjectSelected_Click);
				menuItem.Checked = (project.Name.Equals(_settings.ProjectName));
				this.mnuProjects.MenuItems.Add(menuItem);
			}

			_lastUrl = statusMonitor.Settings.RemoteServerUrl;
		}

		void InitialiseTrayIcon()
		{
			ProjectStatus status = new ProjectStatus();
			status.BuildStatus = IntegrationStatus.Unknown;
			status.Activity = ProjectActivity.Unknown;
		    _iconLoader = CreateIconLoader();

		    trayIcon.Icon = _iconLoader.LoadIcon(status).Icon;                                  
		}

	    private IStatusIconLoader CreateIconLoader()
	    {
	        if(!_settings.Icons.UseDefaultIcons)
	        {
				try
				{
					return new DefaultStatusIconLoader(new FileIconStore(_settings.Icons));    
				}
				catch(IconNotFoundException )
				{
					_settings.Icons.UseDefaultIcons = true;					    
				}
	        }
           return new DefaultStatusIconLoader(new ResourceIconStore());
	    }

	    void DisplayStartupBalloon()
		{
			if (_settings.NotificationBalloon.ShowBalloon)
				trayIcon.ShowBalloon("CruiseControl.NET Monitor", "Monitor started.", NotifyInfoFlags.Info, 1500);
		}

		private void CCTray_Load(object sender, EventArgs e)
		{
			// calling Hide on the window ensures the form's icon doesn't appear
			// while ALT+TABbing between applications, even though it won't appear
			// in the taskbar
			this.Hide();
		}

		void InitialiseSettingsForm()
		{
			settingsForm = new SettingsForm(_settings, statusMonitor);
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
			this.components = new Container();
			this.trayIcon = new NotifyIconEx();
			this.contextMenu = new ContextMenu();
			this.mnuLaunchWebPage = new MenuItem();
			this.mnuProjects = new MenuItem();
			this.mnuProject1PlaceHolder = new MenuItem();
			this.mnuSettings = new MenuItem();
			this.mnuForceBuild = new MenuItem();
			this.mnuExit = new MenuItem();
			this.statusMonitor = new StatusMonitor(new RemoteCruiseProxyLoader());
			// 
			// trayIcon
			// 
			this.trayIcon.ContextMenu = this.contextMenu;
			this.trayIcon.Icon = null;
			this.trayIcon.Text = "No Connection";
			this.trayIcon.Visible = true;
			this.trayIcon.DoubleClick += new EventHandler(this.trayIcon_DoubleClick);
			// 
			// contextMenu
			// 
			this.contextMenu.MenuItems.AddRange(new MenuItem[] {
																						this.mnuLaunchWebPage,
																						this.mnuProjects,
																						this.mnuSettings,
																						this.mnuForceBuild,
																						this.mnuExit});
			this.contextMenu.Popup += new EventHandler(this.contextMenu_Popup);
			// 
			// mnuLaunchWebPage
			// 
			this.mnuLaunchWebPage.Index = 0;
			this.mnuLaunchWebPage.Text = "&Launch web page";
			this.mnuLaunchWebPage.Click += new EventHandler(this.mnuLaunchWebPage_Click);
			// 
			// mnuProjects
			// 
			this.mnuProjects.Index = 1;
			this.mnuProjects.MenuItems.AddRange(new MenuItem[] {
																						this.mnuProject1PlaceHolder});
			this.mnuProjects.Text = "&Project";
			// 
			// mnuProject1PlaceHolder
			// 
			this.mnuProject1PlaceHolder.Index = 0;
			this.mnuProject1PlaceHolder.Text = "Project1";
			// 
			// mnuSettings
			// 
			this.mnuSettings.Index = 2;	//1;
			this.mnuSettings.Text = "&Settings...";
			this.mnuSettings.Click += new EventHandler(this.mnuSettings_Click);
			// 
			// mnuForceBuild
			// 
			this.mnuForceBuild.Index = 3;	//2;
			this.mnuForceBuild.Text = "&Force build";
			this.mnuForceBuild.Click += new EventHandler(this.mnuForceBuild_Click);
			// 
			// mnuExit
			// 
			this.mnuExit.Index = 4;	//3;
			this.mnuExit.Text = "E&xit";
			this.mnuExit.Click += new EventHandler(this.mnuExit_Click);
			// 
			// statusMonitor
			// 
			this.statusMonitor.Settings = null;
			this.statusMonitor.Error += new ErrorEventHandler(this.statusMonitor_Error);
			this.statusMonitor.BuildOccurred += new BuildOccurredEventHandler(this.statusMonitor_BuildOccurred);
			this.statusMonitor.Polled += new PolledEventHandler(this.statusMonitor_Polled);
			// 
			// SystemTrayMonitor
			// 
			this.AutoScaleBaseSize = new Size(5, 13);
			this.ClientSize = new Size(115, 6);	//(104, 19);
			this.ControlBox = false;
			this.Enabled = false;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SystemTrayMonitor";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = SizeGripStyle.Hide;
			this.Text = "CCTray";
			this.WindowState = FormWindowState.Minimized;
			this.Load += new EventHandler(this.CCTray_Load);

		}

		#endregion

		#region Application start

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(String[] args)
		{
			if (args.Length > 0)
			{
				SettingsManager.SettingsFileName = args[0];
			}
			
			Application.Run(new SystemTrayMonitor());
		}


		#endregion

		#region Application exit

		private void mnuExit_Click(object sender, EventArgs e)
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
			trayIcon.Icon = _iconLoader.LoadIcon(e.ProjectStatus).Icon;;
			if(statusMonitor.Settings.RemoteServerUrl != _lastUrl)
				InitialiseProjectMenu();
		}

		private void statusMonitor_BuildOccurred(object sauce, BuildOccurredEventArgs e)
		{
			_exception = null;

			string caption = e.BuildTransitionInfo.Caption;
			string description = _settings.Messages.GetMessageForTransition(e.BuildTransition);
			NotifyInfoFlags icon = GetNotifyInfoFlag(e.BuildTransitionInfo.ErrorLevel);

			HandleBalloonNotification(caption, description, icon);
			HandleAgentNotification(description);

			// play audio, in accordance to settings
			PlayBuildAudio(e.BuildTransition);
		}

		Exception _exception;

		private void statusMonitor_Error(object sender, ErrorEventArgs e)
		{
			if (_exception==null && _settings.ShowExceptions)
			{
				// set the exception before displaying the dialog, because the timer keeps polling and subsequent
				// polls would otherwise cause multiple dialogs, balloons and agents to be displayed
				_exception = e.Exception;

                if (_settings.ShowExceptions)
                {
                    MessageBox.Show(e.Exception.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (ExceptionIsConnectionRefused())
                {
                    string description = _exception.Message;
                    HandleBalloonNotification("No Connection", description, GetNotifyInfoFlag(ErrorLevel.Error));
                    HandleAgentNotification(description);
                }
			}

			_exception = e.Exception;

			trayIcon.Text = GetErrorMessage(e.Exception);
			ProjectStatus status = new ProjectStatus();
			status.BuildStatus = IntegrationStatus.Exception;
			trayIcon.Icon = _iconLoader.LoadIcon(status).Icon;
		}

        private bool ExceptionIsConnectionRefused()
        {
            const int WSAECONNREFUSED = 10061;  // Doesn't seem to be an enum for Socket Error codes?
            return (_exception is SocketException && ((SocketException)_exception).ErrorCode == WSAECONNREFUSED);
        }

		private void mnuProjectSelected_Click(object sender, EventArgs e)
		{
			MenuItem menuItem = (MenuItem) sender;
		    _settings.ProjectName = menuItem.Text;
			foreach(MenuItem item in menuItem.Parent.MenuItems)
			{
				item.Checked = false;
			}
			menuItem.Checked = true;
			SettingsManager.WriteSettings(_settings);
			statusMonitor.Poll();
		}
		
		#endregion

		#region Agent notification

	    void HandleAgentNotification(string description)
		{
			if (_settings.Agents.ShowAgent)
			{
				try 
				{
					EnsureAgentLoaded();
				
					_agent.Speak(description);

					// Hide agent
					if (_settings.Agents.HideAfterMessage)
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
				_agent = _settings.Agents.CreateAgent();
		}


		#endregion

		#region Balloon notification

		void HandleBalloonNotification(string caption, string description, NotifyInfoFlags icon)
		{
			// show a balloon
			if (_settings.NotificationBalloon.ShowBalloon)
				trayIcon.ShowBalloon(caption, description, icon, 5000);
		}


		#endregion

		#region Playing of audio

		void PlayBuildAudio(BuildTransition transition)
		{
			if (_settings.Sounds.ShouldPlaySoundForTransition(transition))
			{
				try
				{
					PlayAudioFile(_settings.Sounds.GetAudioFileLocation(transition));
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


		#region Presentation calculations

		string CalculateTrayText(ProjectStatus projectStatus)
		{
			object activity = (projectStatus.Status==ProjectIntegratorState.Stopped) ? ProjectActivity.Unknown : projectStatus.Activity;

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

		private void trayIcon_DoubleClick(object sender, EventArgs e)
		{
			LaunchWebPage();
		}

		private void mnuLaunchWebPage_Click(object sender, EventArgs e)
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

		private void mnuSettings_Click(object sender, EventArgs e)
		{
			settingsForm.Launch();
		}

		#endregion

		#region Forcing a build

		void mnuForceBuild_Click(object sender, EventArgs e)
		{
			try
			{
				statusMonitor.ForceBuild(_settings.ProjectName);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Unable to force build", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		#endregion

		#region Context menu management

		void contextMenu_Popup(object sender, EventArgs e)
		{
			mnuForceBuild.Enabled = (statusMonitor.ProjectStatus != null && statusMonitor.ProjectStatus.Activity==ProjectActivity.Sleeping);
		}

		#endregion
	}
}
