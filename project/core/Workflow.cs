using System;
using System.Collections;
using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Security;
using ThoughtWorks.CruiseControl.Remote;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.Core
{
	/// <summary>
	/// A generic project contains a collection of tasks.  It will execute them in the specified order.  It is possible to have multiple tasks of the same type.
	/// <code>
	/// <![CDATA[
	/// <workflow name="foo">
	///		<tasks>
	///			<sourcecontrol type="cvs"></sourcecontrol>
	///			<build type="nant"></build>
	///		</tasks>
	///		<state type="state"></state>
	/// </workflow>
	/// ]]>
	/// </code>
	/// </summary>
	[ReflectorType("workflow")]
	public class Workflow : ProjectBase, IProject
	{
		private IList _tasks = new ArrayList();
		private WorkflowResult _currentIntegrationResult;
        private ProjectInitialState initialState = ProjectInitialState.Started;
        private ProjectStartupMode startupMode = ProjectStartupMode.UseLastState;

        [ReflectorProperty("tasks", InstanceType = typeof(ArrayList))]
		public IList Tasks
		{
			get { return _tasks; }
			set { _tasks = value; }
		}

		public IntegrationResult CurrentIntegration
		{
			get { return _currentIntegrationResult; }
		}

		public IIntegrationResult Integrate(IntegrationRequest request)
		{
			_currentIntegrationResult = new WorkflowResult();

			foreach (ITask task in Tasks)
			{
				try 
				{ 
					RunTask(task); 
				}
				catch (CruiseControlException ex) 
				{
					_currentIntegrationResult.ExceptionResult = ex;
				}
			}
			return _currentIntegrationResult;
		}

		public void NotifyPendingState()
		{
			throw new NotImplementedException();
		}

		public void NotifySleepingState()
		{
			throw new NotImplementedException();
		}

		private void RunTask(ITask task)
		{
			task.Run(_currentIntegrationResult);
		}
		
		public IntegrationStatus LatestBuildStatus
		{
			get { return _currentIntegrationResult.Status; }
		}
		
		public void AbortRunningBuild()
		{
			throw new NotImplementedException();
		}
		
		public void Purge(bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment)
		{
			return;
		}

		public string Statistics
		{
			get { throw new NotImplementedException(); }
		}

        public string ModificationHistory
        {
            get { throw new NotImplementedException(); }
        }

        public string RSSFeed
        {
            get { throw new NotImplementedException(); }
        }


		public IIntegrationRepository IntegrationRepository
		{
			get { throw new NotImplementedException(); }
		}

		public string QueueName
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public int QueuePriority
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public void Initialize()
		{
			throw new NotImplementedException();
		}

		public ProjectStatus CreateProjectStatus(IProjectIntegrator integrator)
		{
			throw new NotImplementedException();
		}

        public ProjectActivity CurrentActivity
        {
            get { throw new NotImplementedException(); }
        }

		public void AddMessage(Message message)
		{
			throw new NotImplementedException();
		}

		public int MinimumSleepTimeMillis 
		{ 
			get { return 0; }
		}

		public string WebURL 
		{ 
			get { return string.Empty; }
		}
        public IProjectAuthorisation Security
        {
            get { return null; }
        }

        public int MaxSourceControlRetries
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// The initial start-up state to set.
        /// </summary>
        [ReflectorProperty("initialState", Required = false)]
        public ProjectInitialState InitialState
        {
            get { return initialState; }
            set { initialState = value; }
        }

        #region Links
        /// <summary>
        /// Link this project to other sites.
        /// </summary>
        [ReflectorProperty("linkedSites", Required = false)]
        public NameValuePair[] LinkedSites { get; set; }
        #endregion

        /// <summary>
        /// The start-up mode for this project.
        /// </summary>
        [ReflectorProperty("startupMode", Required = false)]
        public ProjectStartupMode StartupMode
        {
            get { return startupMode; }
            set { startupMode = value; }
        }

        public bool stopProjectOnReachingMaxSourceControlRetries
        {
            get { throw new NotImplementedException(); }
        }

        public ThoughtWorks.CruiseControl.Core.Sourcecontrol.Common.SourceControlErrorHandlingPolicy SourceControlErrorHandling
        {
            get { throw new NotImplementedException(); }
        }

        #region RetrievePackageList()
        /// <summary>
        /// Retrieves the latest list of packages.
        /// </summary>
        /// <returns></returns>
        public virtual List<PackageDetails> RetrievePackageList()
        {
            List<PackageDetails> packages = new List<PackageDetails>();
            return packages;
        }

        /// <summary>
        /// Retrieves the list of packages for a build.
        /// </summary>
        /// <param name="buildLabel"></param>
        /// <returns></returns>
        public virtual List<PackageDetails> RetrievePackageList(string buildLabel)
        {
            List<PackageDetails> packages = new List<PackageDetails>();
            return packages;
        }
        #endregion
    }
}
