
using System;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Core.Security;


namespace ThoughtWorks.CruiseControl.Core.Config
{
	public class Configuration : IConfiguration
	{
		private ProjectList projects = new ProjectList();
        private List<IQueueConfiguration> queueConfigurations = new List<IQueueConfiguration>();
        private ISecurityManager securityManager = new NullSecurityManager();

        /// <summary>
        /// Store the security manager that is being used.
        /// </summary>
        public ISecurityManager SecurityManager
        {
            get { return securityManager; }
            set { securityManager = value; }
        }

        /// <summary>
        /// Store any custom queue configurations.
        /// </summary>
        public virtual List<IQueueConfiguration> QueueConfigurations
        {
            get { return queueConfigurations; }
        }

		public void AddProject(IProject project)
		{
			projects.Add(project);
		}

        /// <summary>
        /// Finds the queue configuration by name.
        /// </summary>
        /// <param name="name">The name of the configuration to find.</param>
        /// <returns>The queue configuration if found, or a default instance of the queue configuration.</returns>
        public virtual IQueueConfiguration FindQueueConfiguration(string name)
        {
            IQueueConfiguration actualConfig = null;

            // Attempt to find the configuration
            foreach (IQueueConfiguration config in queueConfigurations)
            {
                if (string.Equals(config.Name, name, StringComparison.InvariantCultureIgnoreCase))
                {
                    actualConfig = config;
                    break;
                }
            }

            // If we haven't found one then generate a default instance
            if (actualConfig == null) actualConfig = new DefaultQueueConfiguration(name);

            return actualConfig;
        }

		public void DeleteProject(string name)
		{
			projects.Delete(name);
		}

		public IProjectList Projects
		{
			get { return projects; }
		}
	}
}
