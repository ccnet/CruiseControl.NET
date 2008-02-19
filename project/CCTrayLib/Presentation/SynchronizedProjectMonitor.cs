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
		private readonly IProjectMonitor projectMonitor;
		private readonly ISynchronizeInvoke synchronizeInvoke;

		public SynchronizedProjectMonitor(IProjectMonitor projectMonitor, ISynchronizeInvoke synchronizeInvoke)
		{
			this.projectMonitor = projectMonitor;
			this.synchronizeInvoke = synchronizeInvoke;

			projectMonitor.Polled += new MonitorPolledEventHandler(ProjectMonitor_Polled);
			projectMonitor.BuildOccurred += new MonitorBuildOccurredEventHandler(ProjectMonitor_BuildOccurred);
			projectMonitor.MessageReceived += new MessageEventHandler(ProjectMonitor_MessageReceived);
		}


		public ProjectState ProjectState
		{
			get { return projectMonitor.ProjectState; }
		}


		public ISingleProjectDetail Detail
		{
			get { return projectMonitor.Detail; }
		}

		public string SummaryStatusString
		{
			get { return projectMonitor.SummaryStatusString; }
		}
		
		public string ProjectIntegratorState
		{
			get { return projectMonitor.ProjectIntegratorState; }
		}
		
		public void ForceBuild()
		{
			projectMonitor.ForceBuild();
		}
		
		public void AbortBuild()
		{
			projectMonitor.AbortBuild();
		}
		
		public void FixBuild(string fixingUserName)
		{
			projectMonitor.FixBuild(fixingUserName);
		}

		
		public void StopProject()
		{
			projectMonitor.StopProject();
		}
		
		public void StartProject()
		{
			projectMonitor.StartProject();
		}
		
		public void CancelPending()
		{
			projectMonitor.CancelPending();
		}

		public void Poll()
		{
			projectMonitor.Poll();
		}

		public void OnPollStarting()
		{
			projectMonitor.OnPollStarting();
		}

		public event MonitorBuildOccurredEventHandler BuildOccurred;
		public event MonitorPolledEventHandler Polled;
		public event MessageEventHandler MessageReceived;

		private void ProjectMonitor_Polled(object sender, MonitorPolledEventArgs args)
		{
			if (Polled != null) synchronizeInvoke.BeginInvoke(Polled, new object[] {sender, args});
		}

		private void ProjectMonitor_BuildOccurred(object sender, MonitorBuildOccurredEventArgs args)
		{
			if (BuildOccurred != null) synchronizeInvoke.BeginInvoke(BuildOccurred, new object[] {sender, args});
		}

		private void ProjectMonitor_MessageReceived(Message message)
		{
			if (MessageReceived != null) synchronizeInvoke.BeginInvoke(MessageReceived, new object[] {message});
		}

		public IntegrationStatus IntegrationStatus
		{
			get { return projectMonitor.IntegrationStatus; }
		}

		public bool IsPending
		{
			get { return projectMonitor.IsPending; }
		}
		
		public bool IsConnected
		{
			get { return projectMonitor.IsConnected; }
		}
	}
}
