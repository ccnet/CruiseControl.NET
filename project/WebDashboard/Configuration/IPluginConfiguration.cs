using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Configuration
{
	public interface IPluginConfiguration
	{
        string TemplateLocation { get; }
		IPlugin[] FarmPlugins { get; set; }
		IPlugin[] ServerPlugins { get; set; }
		IPlugin[] ProjectPlugins { get; set; }
		IBuildPlugin[] BuildPlugins { get; set; }
	}
}
