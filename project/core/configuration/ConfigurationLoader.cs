using Exortech.NetReflector;
using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Reflection;

namespace ThoughtWorks.CruiseControl.Core.Configuration
{
	public class ConfigurationLoader : IConfigurationLoader, IDisposable
	{
		private const string ROOT_ELEMENT = "cruisecontrol";
		private const string CONFIG_ASSEMBLY_PATTERN = "ccnet.*.plugin.dll";
		private static readonly ReflectorHashAttribute PROJECTS_ATTRIBUTE = new ReflectorHashAttribute("cruisecontrol", "name");

		private string _configFile;
		private FileSystemWatcher _watcher = new FileSystemWatcher();
		private ConfigurationChangedHandler _configurationChangedHandler;
		private System.Timers.Timer timer;
		private ValidationEventHandler _handler;
		private XmlSchema _schema;

		public ConfigurationLoader(string configFile) : this()
		{
			ConfigFile = configFile;
		}

		internal ConfigurationLoader() 
		{
			_handler = new ValidationEventHandler(handleSchemaEvent);
			Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("tw.ccnet.core.configuration.ccnet.xsd");
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
				InitialiseFileWatcher();
			}
		}

		public IConfiguration Load()
		{
			XmlDocument config = LoadConfiguration();
			return PopulateProjectsFromXml(config);
		}

		private void InitialiseFileWatcher()
		{
			VerifyConfigFileExists();
			_watcher.Path = (new FileInfo(ConfigFile)).DirectoryName;
			_watcher.Filter = Path.GetFileName(ConfigFile);
			_watcher.NotifyFilter = NotifyFilters.LastWrite;
			_watcher.Changed += new FileSystemEventHandler(OnConfigurationChanged);
			timer = new System.Timers.Timer( 500 );
			timer.AutoReset=false;
			timer.Enabled=false;
			timer.Elapsed+=new System.Timers.ElapsedEventHandler(OnTimer);
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
				throw new ConfigurationException("Specified configuration file does not exist: " + ConfigFile);
			}
		}

		internal IConfiguration PopulateProjectsFromXml(XmlDocument configXml)
		{
			VerifyConfigRoot(configXml);
			try
			{
				NetReflectorTypeTable typeTable = new NetReflectorTypeTable();
				typeTable.Add(Assembly.GetExecutingAssembly());
				typeTable.Add(Directory.GetCurrentDirectory(), CONFIG_ASSEMBLY_PATTERN);
				IConfiguration configuration = new Configuration();
				foreach (XmlNode node in configXml.DocumentElement)
				{
					IProject project = NetReflector.Read(node, typeTable) as IProject;
					configuration.AddProject(project);
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

		public void AddConfigurationChangedHandler(ConfigurationChangedHandler handler)
		{
			_configurationChangedHandler += handler;
			_watcher.EnableRaisingEvents = true;
		}

		private void ConfigChanged() 
		{
			_configurationChangedHandler();
		}

		protected void OnTimer(Object source, System.Timers.ElapsedEventArgs e)
		{
			lock(this)
			{
				ConfigChanged();
				timer.Enabled=false;
			}
		}

		private void OnConfigurationChanged(object sender, FileSystemEventArgs e)
		{
			// don't need null check because should be guaranteed of handlers
			lock(this)
			{
				if(!timer.Enabled)
					timer.Enabled=true;
				timer.Start();
			}
		}

		public void Dispose()
		{
			_watcher.EnableRaisingEvents = false;
			_watcher.Dispose();
		}

		private void handleSchemaEvent(object sender, ValidationEventArgs args) 
		{
			System.Diagnostics.Trace.WriteLine(args.Message);
		}
	}
}
