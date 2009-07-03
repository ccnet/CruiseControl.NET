using System.ComponentModel;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;
using System.Windows.Forms;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote.Parameters;

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
		
        public void ForceBuild(Dictionary<string, string> parameters)
		{
			projectMonitor.ForceBuild(parameters);
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
            if (Polled != null)
            {
                var canInvoke = true;
                if (synchronizeInvoke is Control) canInvoke = !(synchronizeInvoke as Control).IsDisposed;

                if (canInvoke) synchronizeInvoke.BeginInvoke(Polled, new object[] { sender, args });
            }
		}

		private void ProjectMonitor_BuildOccurred(object sender, MonitorBuildOccurredEventArgs args)
		{
            if (BuildOccurred != null)
            {
                var canInvoke = true;
                if (synchronizeInvoke is Control) canInvoke = !(synchronizeInvoke as Control).IsDisposed;

                if (canInvoke) synchronizeInvoke.BeginInvoke(BuildOccurred, new object[] { sender, args });
            }
		}

		private void ProjectMonitor_MessageReceived(string projectName, ThoughtWorks.CruiseControl.Remote.Message message)
		{
            if (MessageReceived != null)
            {
                var canInvoke = true;
                if (synchronizeInvoke is Control) canInvoke = !(synchronizeInvoke as Control).IsDisposed;

                string caption = string.Concat("Project Name : ", projectName);

                if (canInvoke) synchronizeInvoke.BeginInvoke(MessageReceived, new object[] { caption, message });
            }
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

        #region RetrieveSnapshot()
        /// <summary>
        /// Retrieves a snapshot of the current build status.
        /// </summary>
        /// <returns>The current build status of the project.</returns>
        public virtual ProjectStatusSnapshot RetrieveSnapshot()
        {
            return projectMonitor.RetrieveSnapshot();
        }
        #endregion

        #region RetrievePackageList()
        /// <summary>
        /// Retrieves the current list of available packages.
        /// </summary>
        /// <returns></returns>
        public virtual PackageDetails[] RetrievePackageList()
        {
            return projectMonitor.RetrievePackageList();
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
            return projectMonitor.RetrieveFileTransfer(fileName);
        }
        #endregion

        public List<ParameterBase> ListBuildParameters()
        {
            return projectMonitor.ListBuildParameters();
        }
    }
}
