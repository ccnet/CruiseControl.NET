using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;
using Exortech.NetReflector;
using Objection.NetReflectorPlugin;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.Configuration
{
	// ToDo - testing
	public class DashboardConfigurationLoader : IDashboardConfiguration
	{
        private const string CONFIG_ASSEMBLY_PATTERN = "ccnet.*.plugin.dll";

		private readonly ObjectionNetReflectorInstantiator instantiator;
		private static readonly string DashboardConfigAppSettingKey = "DashboardConfigLocation";
		private static readonly string DefaultDashboardConfigLocation = "dashboard.config";
		private IRemoteServicesConfiguration remoteServicesConfiguration;
		private IPluginConfiguration pluginsConfiguration;
		private NetReflectorTypeTable typeTable;

		public DashboardConfigurationLoader(ObjectionNetReflectorInstantiator instantiator)
		{
			this.instantiator = instantiator;
			typeTable = GetTypeTable();
		}

		private void LoadRemoteServicesConfiguration()
		{
			if (remoteServicesConfiguration == null)
			{
				remoteServicesConfiguration = (IRemoteServicesConfiguration) Load("/dashboard/remoteServices");
			}
		}

		private void LoadPluginsConfiguration()
		{
			if (pluginsConfiguration == null)
			{
				pluginsConfiguration = (IPluginConfiguration) Load("/dashboard/plugins");
			}
		}

		private object Load(string xpath)
		{
			string dashboardConfig;
			using (StreamReader sr = new StreamReader(CalculateDashboardConfigPath()))
			{
				dashboardConfig = sr.ReadToEnd();
			}

			XmlNode node = XmlUtil.SelectNode(dashboardConfig, xpath);
			return NetReflector.Read(node, typeTable);
		}

		private NetReflectorTypeTable GetTypeTable()
		{
			NetReflectorTypeTable newTypeTable = NetReflectorTypeTable.CreateDefault(instantiator);

			// split the relative search path only by ';', thats also valid with Mono on Unix
            foreach (string searchPathDir in AppDomain.CurrentDomain.RelativeSearchPath.Split(';'))
            {
                newTypeTable.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, searchPathDir), CONFIG_ASSEMBLY_PATTERN);
            }
			newTypeTable.Add(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), CONFIG_ASSEMBLY_PATTERN);
			return newTypeTable;
		}

		public static string CalculateDashboardConfigPath()
		{
			string path = ConfigurationManager.AppSettings[DashboardConfigAppSettingKey];
			if (path == null || path == string.Empty)
			{
				path = DefaultDashboardConfigLocation;
			}
			if (! Path.IsPathRooted(path))
			{
				path = ProgramDataFolder.MapPath(path);
			}
			return path;
		}

		public IRemoteServicesConfiguration RemoteServices
		{
			get
			{
				LoadRemoteServicesConfiguration();
				return remoteServicesConfiguration;
			}
		}

		public IPluginConfiguration PluginConfiguration
		{
			get
			{
				LoadPluginsConfiguration();
				return pluginsConfiguration;
			}
		}
	}
}