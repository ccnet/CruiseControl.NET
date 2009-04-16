using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Configuration
{
	public interface IPluginConfiguration
	{
        StylesheetConfiguration[] Stylesheets { get; set; }
        string TemplateLocation { get; }
		IPlugin[] FarmPlugins { get; set; }
		IPlugin[] ServerPlugins { get; set; }
		IPlugin[] ProjectPlugins { get; set; }
		IBuildPlugin[] BuildPlugins { get; set; }
        ISecurityPlugin[] SecurityPlugins { get; set; }
        ISessionStore SessionStore { get; set; }
	}
}
