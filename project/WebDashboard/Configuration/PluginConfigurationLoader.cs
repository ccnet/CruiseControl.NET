using System.IO;
using System.Reflection;
using System.Xml;
using Exortech.NetReflector;
using ObjectWizard.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.Configuration
{
	// ToDo - testing, but still to figure out best way to work with NetReflector
	public class PluginConfigurationLoader : IPluginConfiguration
	{
		private readonly ObjectGiverNetReflectorInstantiator instantiator;
		private readonly IPathMapper pathMapper;

		public PluginConfigurationLoader(ObjectGiverNetReflectorInstantiator instantiator, IPathMapper pathMapper)
		{
			this.instantiator = instantiator;
			this.pathMapper = pathMapper;
		}

		IPluginConfiguration loadedConfiguration;
		// This is probably not thread safe
		// ToDo - use XML Snippet
		// ToDo - close file
		private void Load()
		{
			if (loadedConfiguration == null)
			{
				NetReflectorTypeTable typeTable = NetReflectorTypeTable.CreateDefault(instantiator);
				typeTable.Add(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ccnet.*.plugin.dll");
				loadedConfiguration = (IPluginConfiguration) NetReflector.Read(new XmlTextReader(Path.Combine(pathMapper.PhysicalApplicationPath, "dashboard.config")), typeTable);
			}
		}

		public IPlugin[] FarmPlugins
		{
			get
			{
				Load();
				return loadedConfiguration.FarmPlugins;
			}
			set
			{
				Load();
				loadedConfiguration.FarmPlugins = value;
			}
		}

		public IPlugin[] ServerPlugins
		{
			get
			{
				Load();
				return loadedConfiguration.ServerPlugins;
			}
			set
			{
				Load();
				loadedConfiguration.ServerPlugins = value;
			}
		}

		public IPlugin[] ProjectPlugins
		{
			get
			{
				Load();
				return loadedConfiguration.ProjectPlugins;
			}
			set
			{
				Load();
				loadedConfiguration.ProjectPlugins = value;
			}
		}

		public IPlugin[] BuildPlugins
		{
			get
			{
				Load();
				return loadedConfiguration.BuildPlugins;
			}
			set
			{
				Load();
				loadedConfiguration.BuildPlugins = value;
			}
		}
	}
}
