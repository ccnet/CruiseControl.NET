using System.Text;
using Exortech.NetReflector;
using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Config
{
	public class ConfigurationPersister : IConfigurationPersister
	{
		private readonly IProjectSerializer projectSerializer;
		private const string ROOT_ELEMENT = "cruisecontrol";
		private const string CONFIG_ASSEMBLY_PATTERN = "ccnet.*.plugin.dll";

		internal const string XsdSchemaResourceName = "ThoughtWorks.CruiseControl.Core.configuration.ccnet.xsd";

		private string _configFile;
		private ValidationEventHandler _handler;
		private XmlSchema _schema;

		public ConfigurationPersister(string configFile, IProjectSerializer projectSerializer) : this()
		{
			ConfigFile = configFile;
			this.projectSerializer = projectSerializer;
		}

		internal ConfigurationPersister() 
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

		internal string ConfigFileFullPath
		{
			get
			{
				return Path.GetFullPath(ConfigFile);
			}
		}

		public IConfiguration Load()
		{
			Log.Info(String.Format("Reading configuration file \"{0}\"", ConfigFileFullPath));

			XmlDocument config = LoadConfiguration();
			return PopulateProjectsFromXml(config);
		}

		// ToDo - thread safety?
		public void Save(IConfiguration config)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<cruisecontrol/>");

			StringBuilder concatenatedProjects = new StringBuilder();
			foreach (Project project in config.Projects)
			{
				concatenatedProjects.Append(projectSerializer.Serialize(project));
			}
			doc.DocumentElement.InnerXml = concatenatedProjects.ToString();

			using (StreamWriter fileWriter = new StreamWriter(ConfigFile))
			{
				XmlTextWriter xmlTextWriter = new XmlTextWriter(fileWriter);
				doc.WriteTo(xmlTextWriter);
				xmlTextWriter.Flush();
			}
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
				throw new ConfigurationFileMissingException("Specified configuration file does not exist: " + ConfigFileFullPath);
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
