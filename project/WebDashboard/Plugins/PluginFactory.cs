using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.LogViewerPlugin;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReporterPlugin;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.SiteTemplatePlugin;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins
{
	// ToDo - tests? Its 'just' instantiation. I guess that's a 'yes'...
	// ToDo - cache instances?
	public class PluginFactory
	{
		private readonly DashboardComponentFactory dcFactory;

		public PluginFactory(DashboardComponentFactory dcFactory)
		{
			this.dcFactory = dcFactory;
		}

		public ProjectReporter ProjectReporter
		{
			get { return new ProjectReporter(dcFactory.QueryStringRequestWrapper, dcFactory.DefaultBuildRetrieverForRequest, dcFactory.HttpPathMapper); }
		}

		public SiteTemplate SiteTemplate
		{
			get { return new SiteTemplate(dcFactory.QueryStringRequestWrapper, dcFactory.ConfigurationSettingsConfigGetter, 
					  DefaultBuildLister, dcFactory.DefaultBuildRetrieverForRequest, dcFactory.CruiseManagerBuildNameRetriever);}
		}

		public LogViewer LogViewer
		{
			get { return new LogViewer(dcFactory.QueryStringRequestWrapper, dcFactory.DefaultBuildRetrieverForRequest, dcFactory.LocalFileCacheManager);}
		}

		public DefaultBuildLister DefaultBuildLister
		{
			get { return new DefaultBuildLister(dcFactory.ServerAggregatingCruiseManagerWrapper); }
		}
	}
}
