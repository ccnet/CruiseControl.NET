namespace ThoughtWorks.CruiseControl.Core.Config
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Distribution;
    using ThoughtWorks.CruiseControl.Core.Security;
    using ThoughtWorks.CruiseControl.Core.Util;

    /// <summary>
    /// Load a configuration file using NetReflector.
    /// </summary>
	public class NetReflectorConfigurationReader
        : INetReflectorConfigurationReader
	{
		private const string ROOT_ELEMENT = "cruisecontrol";
		private const string CONFIG_ASSEMBLY_PATTERN = "ccnet.*.plugin.dll";
		private readonly NetReflectorTypeTable typeTable;
		private NetReflectorReader reader;

		public NetReflectorConfigurationReader()
		{
			typeTable = new NetReflectorTypeTable();
			typeTable.Add(AppDomain.CurrentDomain);
            string pluginLocation = ConfigurationManager.AppSettings["PluginLocation"];
            if (!string.IsNullOrEmpty(pluginLocation))
            {
                if (Directory.Exists(pluginLocation))
                {
                    typeTable.Add(pluginLocation, CONFIG_ASSEMBLY_PATTERN);
                }
                else
                {
                    throw new CruiseControlException("Unable to find plugin directory: " + pluginLocation);
                }
            }
			typeTable.Add(Directory.GetCurrentDirectory(), CONFIG_ASSEMBLY_PATTERN);
			reader = new NetReflectorReader(typeTable);
		}

        /// <summary>
        /// Reads an XML config document.
        /// </summary>
        /// <param name="document">The document to read.</param>
        /// <param name="errorProcesser">The error processer to use (can be null).</param>
        /// <returns>The loaded configuration.</returns>
        public IConfiguration Read(XmlDocument document, 
            IConfigurationErrorProcesser errorProcesser)
		{
            Configuration configuration = new Configuration();
            string ConflictingXMLNode = string.Empty;
            List<string> projectNames = new List<string>();

            // Validate the document element
            var actualErrorProcesser = errorProcesser ?? new DefaultErrorProcesser();
            VerifyDocumentHasValidRootElement(document, actualErrorProcesser);

            InvalidNodeEventHandler invalidNodeHandler = (args) =>
            {
                if (!actualErrorProcesser.ProcessUnhandledNode(args.Node))
                {
                    actualErrorProcesser.ProcessError(args.Message);
                }
            };
            typeTable.InvalidNode += invalidNodeHandler;
            try
            {
                foreach (XmlNode node in document.DocumentElement)
                {
                    ConflictingXMLNode = string.Empty;                    

                    if (!(node.NodeType == XmlNodeType.Comment || node.NodeType == XmlNodeType.Text))
                    {
                        ConflictingXMLNode = "Conflicting project data : " + node.OuterXml;

                        object loadedItem = ParseElement(node);
                        if (loadedItem is IProject)
                        {
                            this.LoadAndValidateProject(actualErrorProcesser, projectNames, configuration, loadedItem);
                        }
                        else if (loadedItem is IQueueConfiguration)
                        {
                            this.LoadAndValidateQueue(configuration, loadedItem);
                        }
                        else if (loadedItem is IBuildMachine)
                        {
                            this.LoadAndValidateBuildMachine(
                                actualErrorProcesser,
                                configuration,
                                loadedItem);
                        }
                        else if (loadedItem is IBuildAgent)
                        {
                            this.LoadAndValidateBuildAgent(
                                actualErrorProcesser,
                                configuration,
                                loadedItem);
                        }
                        else if (loadedItem is ISecurityManager)
                        {
                            this.LoadAndValidateSecurityManager(configuration, loadedItem);
                        }
                        else
                        {
                            actualErrorProcesser.ProcessError(
                                new ConfigurationException(
                                    "\nUnknown configuration item found\n" + node.OuterXml));
                        }
                    }
                }

                // Do a validation check to ensure internal configuration consistency
                ValidateConfiguration(configuration, actualErrorProcesser);
            }
            catch (NetReflectorException ex)
            {
                actualErrorProcesser.ProcessError(new ConfigurationException("\nUnable to instantiate CruiseControl projects from configuration document." +
                    "\nConfiguration document is likely missing Xml nodes required for properly populating CruiseControl configuration.\n"
                    + ex.Message +
                    "\n " + ConflictingXMLNode, ex));
            }
            finally
            {
                typeTable.InvalidNode -= invalidNodeHandler;
            }

            return configuration;
        }

        /// <summary>
        /// Parses an element.
        /// </summary>
        /// <param name="node">The element to parse.</param>
        /// <returns>The parsed element.</returns>
        public object ParseElement(XmlNode node)
        {
            var loadedItem = reader.Read(node);
            return loadedItem;
        }

        private void LoadAndValidateSecurityManager(Configuration configuration, object loadedItem)
        {
            ISecurityManager securityManager = loadedItem as ISecurityManager;
            configuration.SecurityManager = securityManager as ISecurityManager;
        }

        /// <summary>
        /// Loads and validates a queue.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="loadedItem">The loaded item.</param>
        private void LoadAndValidateQueue(Configuration configuration, object loadedItem)
        {
            var queueConfig = loadedItem as IQueueConfiguration;
            configuration.QueueConfigurations.Add(queueConfig);
            foreach (var project in queueConfig.Projects)
            {
                configuration.AddProject(project);
                project.QueueName = queueConfig.Name;
            }
        }

        /// <summary>
        /// Loads and validates a build machine.
        /// </summary>
        /// <param name="errorProcesser">The error processer.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="loadedItem">The loaded item.</param>
        private void LoadAndValidateBuildMachine(
            IConfigurationErrorProcesser errorProcesser, 
            Configuration configuration, 
            object loadedItem)
        {
            // Check if the machine has already been defined
            var machineConfig = loadedItem as IBuildMachine;
            var existingMachine = (from machine in configuration.BuildMachines
                                   where machine.Name == machineConfig.Name
                                   select machine).SingleOrDefault();

            if (existingMachine != null)
            {
                // Tell the user
                var message = "A duplicate build machine (" + machineConfig.Name +
                    ") has been found - build machines must be unique per server";
                errorProcesser.ProcessError(
                    new CruiseControlException(message));
            }
            else
            {
                // Add the machine
                configuration.BuildMachines.Add(machineConfig);
            }
        }

        /// <summary>
        /// Loads and validates a build agent.
        /// </summary>
        /// <param name="errorProcesser">The error processer.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="loadedItem">The loaded item.</param>
        private void LoadAndValidateBuildAgent(
            IConfigurationErrorProcesser errorProcesser,
            Configuration configuration,
            object loadedItem)
        {
            // Add the agent
            var agentConfig = loadedItem as IBuildAgent;
            agentConfig.ConfigurationReader = this;
            configuration.BuildAgents.Add(agentConfig);
        }

        private void LoadAndValidateProject(IConfigurationErrorProcesser errorProcesser, 
            List<string> projectNames, Configuration configuration, object loadedItem)
        {
            IProject project = loadedItem as IProject;

            // Validate that the project name is unique
            string projectName = project.Name.ToLowerInvariant();
            if (projectNames.Contains(projectName))
            {
                errorProcesser.ProcessError(
                    new CruiseControlException(
                        string.Format(
                            "A duplicate project name ({0})has been found - projects must be unique per server",
                            projectName)));
            }
            else
            {
                projectNames.Add(projectName);
            }

            configuration.AddProject(project);
        }

        private void VerifyDocumentHasValidRootElement(XmlDocument configXml, IConfigurationErrorProcesser errorProcesser)
		{
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            if (configXml.DocumentElement == null || configXml.DocumentElement.Name != ROOT_ELEMENT)
			{
				throw new ConfigurationException("The configuration document has an invalid root element.  Expected <cruisecontrol>.");
            }
            else if (string.IsNullOrEmpty(configXml.DocumentElement.NamespaceURI))
            {
                // Tell the user there is no version information
                errorProcesser.ProcessWarning("Configuration does not have any version information - assuming the configuration is for version " + version.ToString(2));
            }
            else
            {
                // The last two items are the version number
                var parts = configXml.DocumentElement.NamespaceURI.Split('/');
                var versionNumber = parts[parts.Length - 2] + "." + parts[parts.Length - 1];
                if (version.ToString(2) != versionNumber)
                {
                    // Tell the user the version does not match
                    errorProcesser.ProcessWarning(
                        "Version mismatch - CruiseControl.NET is version " + version.ToString(2) +
                        ", the configuration is for version " + versionNumber);
                }
            }
		}

        /// <summary>
        /// Validate the internal consistency of the configuration.
        /// </summary>
        /// <param name="value">The configuration to check.</param>
        /// <param name="errorProcesser">The error processer to use.</param>
        /// <remarks>
        /// <para>
        /// This will add the following internal consistency checks:
        /// </para>
        /// <list type="bullet">
        /// <item>
        /// <description>Each queue definitition is used by at least one project.</description>
        /// </item>
        /// </list>
        /// </remarks>
        private void ValidateConfiguration(Configuration value, IConfigurationErrorProcesser errorProcesser)
        {
            var rootTrace = ConfigurationTrace.Start(value);

            // Validate the security manager - need to do this first
            if (value.SecurityManager is IConfigurationValidation)
            {
                (value.SecurityManager as IConfigurationValidation).Validate(value, rootTrace, errorProcesser);
            }

            // Validate all the projects
            foreach (IProject project in value.Projects)
            {
                if (project is IConfigurationValidation)
                {
                    (project as IConfigurationValidation).Validate(value, rootTrace, errorProcesser);
                }
            }

            // Validate all the queues
            foreach (IQueueConfiguration queue in value.QueueConfigurations)
            {
                if (queue is IConfigurationValidation)
                {
                    (queue as IConfigurationValidation).Validate(value, rootTrace, errorProcesser);
                }
            }

            // Validate all the build machines
            foreach (var buildMachine in value.BuildMachines)
            {
                var validator = buildMachine as IConfigurationValidation;
                if (validator != null)
                {
                    validator.Validate(value, rootTrace, errorProcesser);
                }
            }
        }

        private class DefaultErrorProcesser
            : IConfigurationErrorProcesser
        {
            public void ProcessError(string message)
            {
                throw new ConfigurationException(message);
            }

            public void ProcessError(Exception error)
            {
                throw error;
            }

            public void ProcessWarning(string message)
            {
                Log.Warning(message);
            }

            public bool ProcessUnhandledNode(XmlNode node)
            {
                return false;
            }

            public void ProcessError(string message, params object[] args)
            {
                throw new ConfigurationException(string.Format(message,args)) ;
            }

            public void ProcessWarning(string message, params object[] args)
            {
                Log.Warning(message, args) ;
            }
        }
	}
}
