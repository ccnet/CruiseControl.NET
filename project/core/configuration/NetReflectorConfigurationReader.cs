using System;
using System.IO;
using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Security;
using System.Collections.Generic;
using System.Configuration;

namespace ThoughtWorks.CruiseControl.Core.Config
{
	public class NetReflectorConfigurationReader
        : INetReflectorConfigurationReader
	{
		private const string ROOT_ELEMENT = "cruisecontrol";
		private const string CONFIG_ASSEMBLY_PATTERN = "ccnet.*.plugin.dll";
		private readonly NetReflectorTypeTable typeTable;
		private NetReflectorReader reader;

		public event InvalidNodeEventHandler InvalidNodeEventHandler;

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
			typeTable.InvalidNode += new InvalidNodeEventHandler(HandleUnusedNode);
			reader = new NetReflectorReader(typeTable);
		}

		public IConfiguration Read(XmlDocument document)
		{
            string ConflictingXMLNode = string.Empty;
            List<string> projectNames = new List<string>();

			VerifyDocumentHasValidRootElement(document);
			try
			{
				Configuration configuration = new Configuration();
				foreach (XmlNode node in document.DocumentElement)
				{
                    ConflictingXMLNode = string.Empty;

					if (!(node is XmlComment))
					{
                        ConflictingXMLNode = "Conflicting project data : " + node.OuterXml;

                        object loadedItem = reader.Read(node);
                        if (loadedItem is IProject)
                        {
                            IProject project = loadedItem as IProject;

                            // Validate that the project name is unique
                            string projectName = project.Name.ToLowerInvariant();
                            if (projectNames.Contains(projectName))
                            {
                                throw new CruiseControlException(
                                    string.Format(
                                        "A duplicate project name ({0})has been found - projects must be unique per server",
                                        projectName));
                            }
                            else
                            {
                                projectNames.Add(projectName);
                            }

                            configuration.AddProject(project);
                        }
                        else if (loadedItem is IQueueConfiguration)
                        {
                            IQueueConfiguration queueConfig = loadedItem as IQueueConfiguration;
                            configuration.QueueConfigurations.Add(queueConfig);
                        }
                        else if (loadedItem is ISecurityManager)
                        {
                            ISecurityManager securityManager = loadedItem as ISecurityManager;
                            configuration.SecurityManager = securityManager as ISecurityManager;
                        }
                        else
                        {
                            throw new ConfigurationException("\nUnknown configuration item found\n" + node.OuterXml);
                        }
					}
				}

                // Do a validation check to ensure internal configuration consistency
                ValidateConfiguration(configuration);

				return configuration;
			}
			catch (NetReflectorException ex)
			{
				throw new ConfigurationException("\nUnable to instantiate CruiseControl projects from configuration document." +
                    "\nConfiguration document is likely missing Xml nodes required for properly populating CruiseControl configuration.\n" 
                    + ex.Message + 
                    "\n " + ConflictingXMLNode  , ex);
			}
		}

		private static void VerifyDocumentHasValidRootElement(XmlDocument configXml)
		{
			if (configXml.DocumentElement == null || configXml.DocumentElement.Name != ROOT_ELEMENT)
			{
				throw new ConfigurationException("The configuration document has an invalid root element.  Expected <cruisecontrol>.");
			}
		}

        /// <summary>
        /// Validate the internal consistency of the configuration.
        /// </summary>
        /// <param name="value">The configuration to check.</param>
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
        private void ValidateConfiguration(Configuration value)
        {
            // Validate the security manager - need to do this first
            if (value.SecurityManager is IConfigurationValidation)
            {
                (value.SecurityManager as IConfigurationValidation).Validate(value, null);
            }

            // Validate all the projects
            foreach (IProject project in value.Projects)
            {
                if (project is IConfigurationValidation)
                {
                    (project as IConfigurationValidation).Validate(value, null);
                }
            }

            // Validate all the queues
            foreach (IQueueConfiguration queue in value.QueueConfigurations)
            {
                if (queue is IConfigurationValidation)
                {
                    (queue as IConfigurationValidation).Validate(value, null);
                }
            }
        }

		private void HandleUnusedNode(InvalidNodeEventArgs args)
		{
			if (InvalidNodeEventHandler != null)
				InvalidNodeEventHandler(args);
		}
	}
}
