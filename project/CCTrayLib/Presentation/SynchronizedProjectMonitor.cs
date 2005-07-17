using System;
using System.ComponentModel;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	/// <summary>
	/// This is a decorator for an IProjectMonitor that ensures that events are fired
	/// via an ISynchronizeInvoke interface.  The only reason to do this normally is to 
	/// ensure that the events get processed on a WinForms thread.
	/// </summary>
	public class SynchronizedProjectMonitor : IProjectMonitor
	{
		private IProjectMonitor projectMonitor;
		private readonly ISynchronizeInvoke synchronizeInvoke;

		public SynchronizedProjectMonitor(IProjectMonitor projectMonitor, ISynchronizeInvoke synchronizeInvoke)
		{
			this.projectMonitor = projectMonitor;
			this.synchronizeInvoke = synchronizeInvoke;

			projectMonitor.Polled += new MonitorPolledEventHandler(ProjectMonitor_Polled);
			projectMonitor.BuildOccurred += new MonitorBuildOccurredEventHandler(ProjectMonitor_BuildOccurred);
		}

		public string ProjectName
		{
			get { return projectMonitor.ProjectName; }
		}

		public ProjectStatus ProjectStatus
		{
			get { return projectMonitor.ProjectStatus; }
		}

		public ProjectState ProjectState
		{
			get { return projectMonitor.ProjectState; }
		}

		public Exception ConnectException
		{
			get { return projectMonitor.ConnectException; }
		}
		public string SummaryStatusString
		{
			get { return projectMonitor.SummaryStatusString; }
		}

		public void ForceBuild()
		{
			projectMonitor.ForceBuild();
		}

		public void Poll()
		{
			projectMonitor.Poll();
		}


		public event MonitorBuildOccurredEventHandler BuildOccurred;
		public event MonitorPolledEventHandler Polled;

		private void ProjectMonitor_Polled(object sender, MonitorPolledEventArgs args)
		{
			if (Polled != null)
			{
				synchronizeInvoke.BeginInvoke(Polled, new object[] {sender, args} );
			}
		}

		private void ProjectMonitor_BuildOccurred(object sender, MonitorBuildOccurredEventArgs args)
		{
			if (BuildOccurred != null)
			{
				synchronizeInvoke.BeginInvoke(BuildOccurred, new object[] {sender, args } );
			}
		}
	}
}