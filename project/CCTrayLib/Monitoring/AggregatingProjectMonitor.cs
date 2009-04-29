using System;
using System.Text;
using ThoughtWorks.CruiseControl.Remote;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class AggregatingProjectMonitor : IProjectMonitor
	{
		private readonly IProjectMonitor[] monitors;

		public AggregatingProjectMonitor(params IProjectMonitor[] monitors)
		{
			this.monitors = monitors;

			foreach (IProjectMonitor monitor in this.monitors)
			{
				monitor.Polled += new MonitorPolledEventHandler(Monitor_Polled);
				monitor.BuildOccurred += new MonitorBuildOccurredEventHandler(Monitor_BuildOccurred);
				monitor.MessageReceived += new MessageEventHandler(Monitor_MessageReceived);
			}
		}

        public void ForceBuild(Dictionary<string, string> parameters)
		{
			throw new NotImplementedException();
		}
		
		public void AbortBuild()
		{
			throw new NotImplementedException();
		}
		
		public void FixBuild(string fixingUserName)
		{
			throw new NotImplementedException();
		}

		
		public void StopProject()
		{
			throw new NotImplementedException();
		}
		
		public void StartProject()
		{
			throw new NotImplementedException();
		}
		
		public void CancelPending()
		{
			throw new NotImplementedException();
		}

		public string SummaryStatusString
		{
			get
			{
				StringBuilder result = new StringBuilder();
				bool firstOne = true;

				foreach (IProjectMonitor monitor in monitors)
				{
					string statusString = monitor.SummaryStatusString;
					if (statusString.Length == 0)
						continue;

					if (!firstOne)
						result.Append('\n');

					firstOne = false;

					result.Append(statusString);
				}

				if (result.Length == 0)
					return "All builds are good";

				return result.ToString();
			}
		}
		
		public string ProjectIntegratorState
		{
			get { throw new NotImplementedException(); }
		}
		
		public event MonitorBuildOccurredEventHandler BuildOccurred;
		public event MonitorPolledEventHandler Polled;
		public event MessageEventHandler MessageReceived;

		private void Monitor_BuildOccurred(object sender, MonitorBuildOccurredEventArgs e)
		{
			if (BuildOccurred != null) BuildOccurred(this, e);
		}

		private void Monitor_Polled(object sender, MonitorPolledEventArgs args)
		{
			if (Polled != null) Polled(this, args);
		}

		private void Monitor_MessageReceived(Message message)
		{
			if (MessageReceived != null) MessageReceived(message);
		}

		public void Poll()
		{
			foreach (IProjectMonitor monitor in monitors)
			{
				monitor.Poll();
			}
		}

		public void OnPollStarting()
		{
			// No initialisation required.
		}

		public ProjectState ProjectState
		{
			get
			{
				ProjectState worstState = ProjectState.Success;

				foreach (IProjectMonitor monitor in monitors)
				{
					if (monitor.ProjectState.IsMoreImportantThan(worstState))
						worstState = monitor.ProjectState;
				}

				return worstState;
			}
		}

		public IntegrationStatus IntegrationStatus
		{
			get
			{
				IntegrationStatus worstStatus = IntegrationStatus.Success;

				foreach (IProjectMonitor monitor in monitors)
				{
					worstStatus = WorstStatusOf(worstStatus, monitor.IntegrationStatus);
				}

				return worstStatus;
			}
		}

		public bool IsPending
		{
			get { return false; }
		}

		private IntegrationStatus WorstStatusOf(IntegrationStatus status1, IntegrationStatus status2)
		{
			int importanceOfStatus1 = GetIntegrationStatusImportance(status1);
			int importanceOfStatus2 = GetIntegrationStatusImportance(status2);

			if (importanceOfStatus1 > importanceOfStatus2)
				return status1;

			return status2;
		}
		
		public bool IsConnected
		{
			get { throw new NotImplementedException(); }
		}
		
		private int GetIntegrationStatusImportance(IntegrationStatus status)
		{
			switch (status)
			{
				case IntegrationStatus.Success:
					return 1;
				case IntegrationStatus.Unknown:
					return 2;
				case IntegrationStatus.Exception:
					return 3;
				case IntegrationStatus.Failure:
					return 4;
				default:
					return 5;
			}
		}

		public ISingleProjectDetail Detail
		{
			get { throw new InvalidOperationException(); }
		}

        #region RetrieveSnapshot()
        /// <summary>
        /// Retrieves a snapshot of the current build status.
        /// </summary>
        /// <returns>The current build status of the project.</returns>
        public virtual ProjectStatusSnapshot RetrieveSnapshot()
        {
            throw new InvalidOperationException();
        }
        #endregion

        #region RetrievePackageList()
        /// <summary>
        /// Retrieves the current list of available packages.
        /// </summary>
        /// <returns></returns>
        public virtual PackageDetails[] RetrievePackageList()
        {
            throw new InvalidOperationException();
        }
        #endregion

        #region RetrieveFileTransfer()
        /// <summary>
        /// Retrieve a file transfer object.
        /// </summary>
        /// <param name="project">The project to retrieve the file for.</param>
        /// <param name="fileName">The name of the file.</param>
        public virtual IFileTransfer RetrieveFileTransfer(string fileName)
        {
            throw new InvalidOperationException();
        }
        #endregion

        public List<ParameterBase> ListBuildParameters()
        {
            throw new NotImplementedException();
        }
	}
}
