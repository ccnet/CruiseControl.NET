using Exortech.NetReflector;
using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Config
{
	public class DefaultConfigurationFileLoader : IConfigurationFileLoader
	{
		private const string ROOT_ELEMENT = "cruisecontrol";
		private const string CONFIG_ASSEMBLY_PATTERN = "ccnet.*.plugin.dll";

		internal const string XsdSchemaResourceName = "ThoughtWorks.CruiseControl.Core.configuration.ccnet.xsd";

		private ValidationEventHandler _handler;
		private XmlSchema _schema;

		public DefaultConfigurationFileLoader() 
		{
			_handler = new ValidationEventHandler(handleSchemaEvent);
			Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(XsdSchemaResourceName);
			
			if (s==null)
				throw new CruiseControlException("Unable to load ccnet.xsd resource from assembly.");

			_schema = XmlSchema.Read(s, _handler);
			_schema.Compile(_handler);
		}

		public IConfiguration Load(FileInfo configFile)
		{
			Log.Info(String.Format("Reading configuration file \"{0}\"", configFile.FullName));

			XmlDocument config = LoadConfiguration(configFile);
			return PopulateProjectsFromXml(config);
		}

		// TODO - this should be private - update tests and make it so
		public XmlDocument LoadConfiguration(FileInfo configFile)
		{
			VerifyConfigFileExists(configFile);

			XmlDocument config = AttemptLoadConfiguration(configFile);
			return config;
		}

		private XmlDocument AttemptLoadConfiguration(FileInfo configFile)
		{
			XmlValidatingLoader loader = null;
			try
			{
				loader = initializeLoader(configFile);
				return loader.Load();
			}
			catch (XmlException ex)
			{
				throw new ConfigurationException("The configuration file contains invalid Xml: " + configFile.FullName, ex);
			}
			finally 
			{
				if (loader != null)
					loader.Dispose();
			}
		}

		private XmlValidatingLoader initializeLoader(FileInfo configFile) 
		{
			XmlValidatingLoader loader = new XmlValidatingLoader(new XmlTextReader(configFile.FullName));
			loader.ValidationEventHandler += _handler;
			//loader.Schemas.Add(_schema);
			return loader;
		}

		private void VerifyConfigFileExists(FileInfo configFile)
		{
			if (! configFile.Exists)
			{
				throw new ConfigurationFileMissingException("Specified configuration file does not exist: " + configFile.FullName);
			}
		}

		// TODO - this should be private - update tests and make it so
		public IConfiguration PopulateProjectsFromXml(XmlDocument configXml)
		{
			VerifyConfigRoot(configXml);
			try
			{
				NetReflectorTypeTable typeTable = new NetReflectorTypeTable();
				typeTable.Add(AppDomain.CurrentDomain);
				typeTable.Add(Directory.GetCurrentDirectory(), CONFIG_ASSEMBLY_PATTERN);
				Configuration configuration = new Configuration();
				foreach (XmlNode node in configXml.DocumentElement)
				{
                    if (!(node is XmlComment))
                    {
                        IProject project = NetReflector.Read(node, typeTable) as IProject;
                        configuration.AddProject(project);
                    }
				}
				return configuration;
			}
			catch (NetReflectorException ex)
			{
				throw new ConfigurationException("Unable to instantiate CruiseControl projects from configuration document. " +  
					"Configuration document is likely missing Xml nodes required for properly populating CruiseControl configuration." + ex.Message, ex);
			}
		}

		private void VerifyConfigRoot(XmlDocument configXml)
		{
			if (configXml.DocumentElement == null || configXml.DocumentElement.Name != ROOT_ELEMENT)
			{
				throw new ConfigurationException("The configuration document has an invalid root element.  Expected <cruisecontrol>.");
			}
		}

		private void handleSchemaEvent(object sender, ValidationEventArgs args) 
		{
			Log.Info("Loading config schema: " + args.Message);
		}
	}
}
