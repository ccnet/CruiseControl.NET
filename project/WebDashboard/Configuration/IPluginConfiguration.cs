using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Configuration
{
	public interface IPluginConfiguration
	{
		IPlugin[] FarmPlugins { get; set; }
		IPlugin[] ServerPlugins { get; set; }
		IPlugin[] ProjectPlugins { get; set; }
		IPlugin[] BuildPlugins { get; set; }
	}
}
