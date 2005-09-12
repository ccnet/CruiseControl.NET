using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;
using Exortech.NetReflector;
using ObjectWizard.NetReflector;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.WebDashboard.Configuration
{
	// ToDo - testing
	public class DashboardConfigurationLoader : IDashboardConfiguration
	{
		private readonly ObjectGiverNetReflectorInstantiator instantiator;
		private readonly IPathMapper pathMapper;
		private static readonly string DashboardConfigAppSettingKey = "DashboardConfigLocation";
		private static readonly string DefaultDashboardConfigLocation = "dashboard.config";
		
		public DashboardConfigurationLoader(ObjectGiverNetReflectorInstantiator instantiator, IPathMapper pathMapper)
		{
			this.instantiator = instantiator;
			this.pathMapper = pathMapper;
		}

		private IRemoteServicesConfiguration remoteServicesConfiguration;
		private IPluginConfiguration pluginsConfiguration;

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
			string dashboardConfig = "";
			using (StreamReader sr = new StreamReader(CalculateDashboardConfigPath()))
			{
				dashboardConfig = sr.ReadToEnd();
			}

			XmlNode node = XmlUtil.SelectNode(dashboardConfig, xpath);

			NetReflectorTypeTable typeTable = NetReflectorTypeTable.CreateDefault(instantiator);
			typeTable.Add(Path.GetDirectoryName(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.RelativeSearchPath)), "ccnet.*.plugin.dll");
			typeTable.Add(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ccnet.*.plugin.dll");

			return NetReflector.Read(node, typeTable);
		}

		private string CalculateDashboardConfigPath()
		{
			string path = ConfigurationSettings.AppSettings[DashboardConfigAppSettingKey];
			if (path == null || path == string.Empty)
			{
				path = DefaultDashboardConfigLocation;
			}
			if (! Path.IsPathRooted(path))
			{
				path = Path.Combine(pathMapper.PhysicalApplicationPath, path);
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