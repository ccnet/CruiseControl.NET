using System.IO;
using System.Reflection;
using System.Xml;
using Exortech.NetReflector;
using ObjectWizard.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.Configuration
{
	// ToDo - testing
	public class DashboardConfigurationLoader : IDashboardConfiguration
	{
		private readonly ObjectGiverNetReflectorInstantiator instantiator;
		private readonly IPathMapper pathMapper;

		public DashboardConfigurationLoader(ObjectGiverNetReflectorInstantiator instantiator, IPathMapper pathMapper)
		{
			this.instantiator = instantiator;
			this.pathMapper = pathMapper;
		}

		IRemoteServicesConfiguration remoteServicesConfiguration;
		IPluginConfiguration pluginsConfiguration;

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
			using (StreamReader sr = new StreamReader(Path.Combine(pathMapper.PhysicalApplicationPath, "dashboard.config"))) 
			{
				dashboardConfig = sr.ReadToEnd();
			}

			XmlNode node = XmlUtil.SelectNode(dashboardConfig, xpath);

			NetReflectorTypeTable typeTable = NetReflectorTypeTable.CreateDefault(instantiator);
			typeTable.Add(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ccnet.*.plugin.dll");
			return NetReflector.Read(node, typeTable);
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
