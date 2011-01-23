using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Security;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// The configuration for a server.
    /// </summary>
    public interface IConfiguration
	{
        /// <summary>
        /// Gets the projects.
        /// </summary>
        /// <value>The projects.</value>
		IProjectList Projects { get; }

        /// <summary>
        /// Store any custom queue configurations.
        /// </summary>
        List<IQueueConfiguration> QueueConfigurations { get; }

        /// <summary>
        /// Finds the queue configuration by name.
        /// </summary>
        /// <param name="name">The name of the configuration to find.</param>
        /// <returns>The queue configuration if found, or a default instance of the queue configuration.</returns>
        IQueueConfiguration FindQueueConfiguration(string name);

        /// <summary>
        /// Store the security manager that is being used.
        /// </summary>
        ISecurityManager SecurityManager { get; }

        /// <summary>
        /// Adds a project.
        /// </summary>
        /// <param name="project">The project.</param>
        void AddProject(IProject project);
        
        /// <summary>
        /// Deletes a project.
        /// </summary>
        /// <param name="name">The name.</param>
		void DeleteProject(string name);
	}
}
