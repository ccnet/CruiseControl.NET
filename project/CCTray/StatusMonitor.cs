using System;
using System.ComponentModel;
using System.Runtime.Remoting;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.Remote.monitor
{
	#region Public delegates

	public delegate void BuildOccurredEventHandler(object sauce, BuildOccurredEventArgs e);
	public delegate void PolledEventHandler(object sauce, PolledEventArgs e);
	public delegate void ErrorEventHandler(object sauce, ErrorEventArgs e);

	#endregion

	/// <summary>
	/// Monitors a remote CruiseControl.NET instance, and raises events in
	/// responses to changes in the server's state.
	/// </summary>
	public class StatusMonitor : Component
	{
		#region Field declarations

		public event BuildOccurredEventHandler BuildOccurred;
		public event PolledEventHandler Polled;
		public event ErrorEventHandler Error;

		Timer pollTimer;
		IContainer components;

		ProjectStatus _currentProjectStatus;
		Settings _settings;

		#endregion

		#region Constructors

		public StatusMonitor(System.ComponentModel.IContainer container)
		{
			container.Add(this);
			InitializeComponent();
		}

		public StatusMonitor()
		{
			InitializeComponent();
		}


		#endregion

		#region Properties

		/// <summary>
		/// Gets the Url of the build results web page for the current project.
		/// </summary>
		public string WebUrl
		{
			get
			{
				return _currentProjectStatus.WebURL;
			}
		}

		public Settings Settings
		{
			get
			{
				return _settings;
			}
			set
			{
				_settings = value;
			}
		}

		public ProjectStatus ProjectStatus
		{
			get
			{
				return _currentProjectStatus;
			}
		}


		#endregion

		#region Component Designer generated code
		
		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
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
			this.pollTimer = new System.Windows.Forms.Timer(this.components);
			// 
			// pollTimer
			// 
			this.pollTimer.Interval = 15000;
			this.pollTimer.Tick += new System.EventHandler(this.pollTimer_Tick);

		}

		#endregion

		#region Polling

		public void StartPolling()
		{
			// poll immediately
			Poll();

			// use timer to ensure periodic polling
			pollTimer.Enabled = true;
			pollTimer.Start();
		}

		public void StopPolling()
		{
			pollTimer.Enabled = false;
			pollTimer.Stop();
		}

		void pollTimer_Tick(object sender, EventArgs e)
		{
			Poll();

			// update interval, in case it has changed
			pollTimer.Interval = Settings.PollingIntervalSeconds * 1000;
		}

		void Poll()
		{
			// check for any change in status, and raise events accordingly
			try
			{
				ProjectStatus latestProjectStatus = GetRemoteProjectStatus();

				OnPolled(new PolledEventArgs(latestProjectStatus));

				if (HasBuildOccurred(latestProjectStatus))
				{
					BuildTransition transition = GetBuildTransition(latestProjectStatus);
					OnBuildOccurred(new BuildOccurredEventArgs(latestProjectStatus, transition));
				}

				_currentProjectStatus = latestProjectStatus;
			}
			catch (Exception ex)
			{
				OnError(new ErrorEventArgs(ex));
			}
		}


		#endregion

		#region Build transition detection

		bool HasBuildOccurred(ProjectStatus newProjectStatus)
		{
			// If last build date is DateTime.MinValue (struct's default value),
			// then the remote status has not yet been recorded.
			if (_currentProjectStatus.LastBuildDate==DateTime.MinValue)
				return false;

			// compare dates
			return (newProjectStatus.LastBuildDate!=_currentProjectStatus.LastBuildDate);
		}


		#endregion

		#region Protected virtual event raisers

		protected virtual void OnPolled(PolledEventArgs e)
		{
			if (Polled!=null)
				Polled(this, e);
		}

		protected virtual void OnBuildOccurred(BuildOccurredEventArgs e)
		{
			if (BuildOccurred!=null)
				BuildOccurred(this, e);
		}

		protected virtual void OnError(ErrorEventArgs e)
		{
			if (Error!=null)
				Error(this, e);
		}


		#endregion

		#region Remoting communication

		ProjectStatus GetRemoteProjectStatus()
		{
			ICruiseManager remoteCC = GetRemoteCruiseControlProxy();
			return remoteCC.GetProjectStatus();
		}

		ICruiseManager GetRemoteCruiseControlProxy()
		{
			ICruiseManager remoteCC
				= (ICruiseManager)RemotingServices.Connect(typeof(ICruiseManager), Settings.RemoteServerUrl);
			return remoteCC;
		}


		#endregion

		#region Forcing a build

		public void ForceBuild(string projectName)
		{
			ICruiseManager remoteCC = GetRemoteCruiseControlProxy();
			remoteCC.ForceBuild(projectName);
		}


		#endregion

		#region Build transitions

		BuildTransition GetBuildTransition(ProjectStatus projectStatus)
		{
			bool wasOk = _currentProjectStatus.BuildStatus==IntegrationStatus.Success;
			bool isOk = projectStatus.BuildStatus==IntegrationStatus.Success;

			if (wasOk && isOk)
				return BuildTransition.StillSuccessful;
			else if (!wasOk && !isOk)
				return BuildTransition.StillFailing;
			else if (wasOk && !isOk)
				return BuildTransition.Broken;
			else if (!wasOk && isOk)
				return BuildTransition.Fixed;

			throw new Exception("The universe has gone crazy.");
		}


		#endregion
	}

	#region Event argument classes

	public class BuildOccurredEventArgs : EventArgs
	{
		ProjectStatus _projectStatus;
		BuildTransition _transition;

		public BuildOccurredEventArgs(ProjectStatus newProjectStatus, BuildTransition transition)
		{
			_projectStatus = newProjectStatus;
			_transition = transition;
		}

		public ProjectStatus ProjectStatus
		{
			get
			{
				return _projectStatus;
			}
		}

		public BuildTransition BuildTransition
		{
			get
			{
				return _transition;
			}
		}

		public BuildTransitionAttribute BuildTransitionInfo
		{
			get
			{
				return BuildTransitionUtil.GetBuildTransitionAttribute(_transition);
			}
		}
	}

	public class PolledEventArgs : EventArgs
	{
		ProjectStatus _projectStatus;

		public PolledEventArgs(ProjectStatus projectStatus)
		{
			_projectStatus = projectStatus;
		}

		public ProjectStatus ProjectStatus
		{
			get
			{
				return _projectStatus;
			}
		}
	}

	public class ErrorEventArgs : EventArgs
	{
		Exception _exception;

		public ErrorEventArgs(Exception exception)
		{
			_exception = exception;
		}

		public Exception Exception
		{
			get
			{
				return _exception;
			}
		}
	}


	#endregion
}
