using System.Xml;
using ThoughtWorks.CruiseControl.Core.Security;
using ThoughtWorks.CruiseControl.Remote;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.Core
{
	/// <summary>
	/// Interface to which all projects must adhere, and via which all application
	/// code should interact with projects.
	/// </summary>
	public interface IProject : IIntegratable
	{
		/// <summary>
		/// The name of this project.
		/// </summary>
		string Name
		{
			get;
		}

        #region Links
        /// <summary>
        /// Link this project to other sites.
        /// </summary>
        NameValuePair[] LinkedSites { get; set; }
        #endregion

		/// <summary>
		/// An optional category that groups the project
		/// </summary>
		string Category
		{
			get;
		}

        /// <summary>
        /// An optional description for the project.
        /// </summary>
        string Description { get; }

		/// <summary>
		/// A component to trigger integrations for this project.
		/// TODO: remove
		/// </summary>
		ITrigger Triggers 
		{
			get;
		}

		/// <summary>
		/// Where the results web page for this project can be found
		/// </summary>
		string WebURL 
		{ 
			get;
		}

		/// <summary>
		/// Gets the project's working directory, where the primary build and checkout happens
		/// </summary>
		string WorkingDirectory
		{
			get;
		}

		/// <summary>
		/// Gets the project's artifact directory, where build logs and distributables can be placed
		/// </summary>
		string ArtifactDirectory
		{
			get;
		}

		/// <summary>
		/// This method is called when the project is being deleted from the server. It allows resources to be cleaned up, SCM clients to be unregistered, etc.
		/// </summary>
		void Purge(bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment);

		ExternalLink[] ExternalLinks { get; }

		string Statistics { get; }

        string ModificationHistory { get; }

        string RSSFeed { get; }

		IIntegrationRepository IntegrationRepository { get; }

		/// <summary>
		/// Gets or sets the build queue this project will be added to when a start of the build is triggered.
		/// If no queue name specified, uses the project name.
		/// </summary>
		string QueueName { get; set; }

		/// <summary>
		/// Gets or sets the optional queue priority for when multiple projects share a queue. 
		/// A priority of zero (default) indicates a FIFO queue.
		/// An item with priority 1 will be inserted before an item of priority 2.
		/// </summary>
		int QueuePriority { get; }
		
		void Initialize();
		
		ProjectStatus CreateProjectStatus(IProjectIntegrator integrator);
        ProjectActivity CurrentActivity { get; }

		void AbortRunningBuild();
		
        /// <summary>
        /// adds a message 
        /// </summary>
        /// <param name="message"></param>
		void AddMessage(Message message);

    	
		/// <summary>
		/// Notification that project should enter a pending state due to being queued.
		/// </summary>
		void NotifyPendingState();

		/// <summary>
		/// Notification of last project exiting the integration queue and hence can return to sleeping state.
		/// </summary>
		void NotifySleepingState();

        /// <summary>
        /// The associated security configuration.
        /// </summary>
        IProjectAuthorisation Security { get; }

        /// <summary>
        /// Maximum amount of sourcecontrol exceptions allowed, before stopping the project (if specified to do so).
        /// This equals to the amount of errors in GetModifications. 
        /// </summary>
        int MaxSourceControlRetries { get; }

        #region AskForForceBuildReason
        /// <summary>
        /// Should a comment be requested when a force build is triggered.
        /// </summary>
        DisplayLevel AskForForceBuildReason { get; }
        #endregion

        /// <summary>
        /// Stop the project when the MaxSourceControlRetries limit has been reached
        /// </summary>
        bool stopProjectOnReachingMaxSourceControlRetries { get; }



        /// <summary>
        /// What do do when an error occurs in the getmodifications stage of the source control 
        /// </summary>
        Sourcecontrol.Common.SourceControlErrorHandlingPolicy SourceControlErrorHandling { get; }

        /// <summary>
        /// The initial start-up state to set.
        /// </summary>
        ProjectInitialState InitialState { get; }

        /// <summary>
        /// The start-up mode for this project.
        /// </summary>
        ProjectStartupMode StartupMode { get; }

        #region RetrievePackageList()
        /// <summary>
        /// Retrieves the latest list of packages.
        /// </summary>
        /// <returns></returns>
        List<PackageDetails> RetrievePackageList();

        /// <summary>
        /// Retrieves the list of packages for a build.
        /// </summary>
        /// <param name="buildName"></param>
        /// <returns></returns>
        List<PackageDetails> RetrievePackageList(string buildName);
        #endregion

    }
}
