using System;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Presentation
{
	public class StubProjectMonitor : IProjectMonitor, ISingleProjectDetail
	{
		private ProjectStatus projectStatus;
		private ProjectState projectState = ProjectState.NotConnected;
		private IntegrationStatus integrationStatus = IntegrationStatus.Unknown;
		private string projectName;
		private Exception connectException;

		public StubProjectMonitor( string projectName )
		{
			this.projectName = projectName;
		}

		public string ProjectName
		{
			get { return projectName; }
		}

		public ProjectStatus ProjectStatus
		{
			get { return projectStatus; }
			set { projectStatus = value; }
		}


		public ISingleProjectDetail Detail
		{
			get { return this; }
		}

		public ProjectState ProjectState
		{
			get { return projectState; }
			set { projectState = value; }
		}

		public IntegrationStatus IntegrationStatus
		{
			get { return integrationStatus; }
			set { integrationStatus = value; }
		}

		public bool IsConnected
		{
			get { return ProjectStatus != null; }
		}

		public ProjectActivity Activity
		{
			get { return ProjectStatus.Activity; }
		}

		public string LastBuildLabel
		{
			get { return ProjectStatus.LastBuildLabel; }
		}

		public DateTime LastBuildTime
		{
			get { return ProjectStatus.LastBuildDate; }
		}

		public DateTime NextBuildTime
		{
			get { return ProjectStatus.NextBuildTime; }
		}

		public string WebURL
		{
			get { return ProjectStatus.WebURL; }
		}

		public TimeSpan EstimatedTimeRemainingOnCurrentBuild
		{
			get { return TimeSpan.Zero; }
		}

		public void OnBuildOccurred( MonitorBuildOccurredEventArgs args )
		{
			if (BuildOccurred != null)
				BuildOccurred( this, args );
		}

		public void OnPolled( MonitorPolledEventArgs args )
		{
			if (Polled != null)
				Polled( this, args );
		}

		public event MonitorBuildOccurredEventHandler BuildOccurred;
		public event MonitorPolledEventHandler Polled;

		public void Poll()
		{
			OnPolled(new MonitorPolledEventArgs(this));
		}

		public void ForceBuild()
		{
			throw new NotImplementedException();
		}

		public void SetUpAsIfExceptionOccurredOnConnect( Exception exception )
		{
			ProjectState = ProjectState.NotConnected;
			ProjectStatus = null;
			ConnectException = exception;
		}

		public Exception ConnectException
		{
			get { return connectException; }
			set { connectException = value; }
		}
		public string SummaryStatusString
		{
			get { throw new NotImplementedException(); }
		}
	}
}