using Exortech.NetReflector;
using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Config
{
	public class ConfigurationLoader : IConfigurationLoader
	{
		private const string ROOT_ELEMENT = "cruisecontrol";
		private const string CONFIG_ASSEMBLY_PATTERN = "ccnet.*.plugin.dll";
		private static readonly ReflectorHashAttribute PROJECTS_ATTRIBUTE = new ReflectorHashAttribute("cruisecontrol", "name");

		internal const string XsdSchemaResourceName = "ThoughtWorks.CruiseControl.Core.configuration.ccnet.xsd";

		private string _configFile;
		private ValidationEventHandler _handler;
		private XmlSchema _schema;

		public ConfigurationLoader(string configFile) : this()
		{
			ConfigFile = configFile;
		}

		internal ConfigurationLoader() 
		{
			_handler = new ValidationEventHandler(handleSchemaEvent);
			Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(XsdSchemaResourceName);
			
			if (s==null)
				throw new CruiseControlException("Unable to load ccnet.xsd resource from assembly.");

			_schema = XmlSchema.Read(s, _handler);
			_schema.Compile(_handler);
		}

		private XmlValidatingLoader initializeLoader() 
		{
			XmlValidatingLoader loader = new XmlValidatingLoader(new XmlTextReader(ConfigFile));
			loader.ValidationEventHandler += _handler;
			//loader.Schemas.Add(_schema);
			return loader;
		}

		internal string ConfigFile
		{
			get { return _configFile; }
			set 
			{ 
				_configFile = value; 
			}
		}

		public IConfiguration Load()
		{
			XmlDocument config = LoadConfiguration();
			return PopulateProjectsFromXml(config);
		}

		internal XmlDocument LoadConfiguration()
		{
			VerifyConfigFileExists();

			XmlDocument config = AttemptLoadConfiguration();
			return config;
		}

		private XmlDocument AttemptLoadConfiguration()
		{
			XmlValidatingLoader loader = null;
			try
			{
				loader = initializeLoader();
				return loader.Load();
			}
			catch (XmlException ex)
			{
				throw new ConfigurationException("The configuration file contains invalid Xml: " + ConfigFile, ex);
			}
			finally 
			{
				if (loader != null)
					loader.Dispose();
			}
		}

		private void VerifyConfigFileExists()
		{
			if (! File.Exists(ConfigFile))
			{
				throw new ConfigurationException("Specified configuration file does not exist: " + Path.GetFullPath(ConfigFile));
			}
		}

		internal IConfiguration PopulateProjectsFromXml(XmlDocument configXml)
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
