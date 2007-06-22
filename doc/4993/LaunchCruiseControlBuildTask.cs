using System;
using System.Runtime.Remoting;
using System.Threading;
using NAnt.Core;
using NAnt.Core.Attributes;
using ThoughtWorks.CruiseControl.Remote;

namespace LoGeek.BuildServer.CustomTasks
{
	/// <summary>
	/// A NAnt task to launch a build on a CCNet server.
	/// </summary>
	[TaskName("launchccnetbuild")]
	public class LaunchCruiseControlBuildTask : Task
	{
		#region Private Instance Fields

		private string _serverUrl;
		private string _projectName;
		private int _timeOutInSeconds = 30*60;
		private int _pollingIntervalInSeconds = 5;
		private ICruiseManager _cruiseManager;

		#endregion Private Instance Fields

		#region Public Instance Properties

		[TaskAttribute("serverurl", Required=true)]
		public string ServerUrl
		{
			get { return _serverUrl; }
			set { _serverUrl = value; }
		}

		[TaskAttribute("projectname", Required=true)]
		public string ProjectName
		{
			get { return _projectName; }
			set { _projectName = value; }
		}

		/// <summary>
		/// Time out in seconds. Default to 1800 seconds (30 minutes)
		/// </summary>
		[TaskAttribute("timeoutinseconds", Required=false)]
		public int TimeOut
		{
			get { return _timeOutInSeconds; }
			set { _timeOutInSeconds = value; }
		}

		/// <summary>
		/// Polling interval in seconds. Default to 5 seconds.
		/// </summary>
		[TaskAttribute("pollinginterval", Required=false)]
		public int PollingInterval
		{
			get { return _pollingIntervalInSeconds; }
			set { _pollingIntervalInSeconds = value; }
		}

		#endregion Public Instance Properties

		#region implementation of Task

		protected override void ExecuteTask()
		{
			Log(Level.Info, "Connecting to CCNet server " + ServerUrl);
			_cruiseManager = (ICruiseManager) RemotingServices.Connect(typeof (ICruiseManager), ServerUrl);

			IntegrationStatus status = LaunchBuild(_cruiseManager, ProjectName, PollingInterval, TimeOut);
			if (status != IntegrationStatus.Success)
				throw new BuildException(string.Format("Project '{0}' failed : {1}", ProjectName, status));
		}

		#endregion 

		/// <summary>
		/// Return the current status of the project. Return null if the project does not exist.
		/// </summary>
		private ProjectStatus GetCurrentProjectStatus(ICruiseManager cruiseManager, string name)
		{
			ProjectStatus[] allStatus = cruiseManager.GetProjectStatus();
			foreach (ProjectStatus status in allStatus)
			{
				if (status.Name == name)
					return status;
			}
			return null;
		}

		public IntegrationStatus LaunchBuild(ICruiseManager cruiseManager, string projectName, int pollingIntervalInSeconds, int timeOutInSeconds)
		{
			ProjectStatus status = GetCurrentProjectStatus(cruiseManager, projectName);
			if (status == null)
				throw new BuildException(string.Format("Project '{0}' not found on the build server.", projectName));

			if (status.Activity != ProjectActivity.Sleeping)
				throw new BuildException(string.Format("Project '{0}' activity is '{1}' instead of expected '{2}'", projectName, status.Activity, ProjectActivity.Sleeping));

			Log(Level.Info, "Forcing build for project '{0}'", projectName);
			cruiseManager.ForceBuild(projectName);

			DateTime startTime = DateTime.Now;
			TimeSpan timeout = new TimeSpan(0, 0, timeOutInSeconds);
			while (true)
			{
				TimeSpan elapsed = DateTime.Now - startTime;
				if (elapsed >= timeout)
					throw new BuildException(string.Format("Project '{0}' build timed-out (lasted more than {1} seconds)", projectName, timeOutInSeconds));
				Thread.Sleep(pollingIntervalInSeconds*1000);
				// check current integration status to decide what to do
				status = GetCurrentProjectStatus(cruiseManager, projectName);
				switch (status.Activity)
				{
					case ProjectActivity.Building:
						// fine, keep it rolling
						break;
					case ProjectActivity.CheckingModifications:
						// fine
						break;
					case ProjectActivity.Sleeping:
						// the build is finished (may be successful or have failed)
						return status.BuildStatus;
					default:
						throw new Exception(string.Format("Unknown ProjectActivity '{0}'",status.Activity));
				}
			}
		}
	}
}

