using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.LogViewerPlugin;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReporterPlugin;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerLogViewerPlugin;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.SiteTemplatePlugin;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins
{
	// ToDo - tests? Its 'just' instantiation. I guess that's a 'yes'...
	// ToDo - cache instances?
	public class PluginPageRendererFactory
	{
		private readonly DashboardComponentFactory dcFactory;

		public PluginPageRendererFactory(DashboardComponentFactory dcFactory)
		{
			this.dcFactory = dcFactory;
		}

		public BuildReporterPageRenderer BuildReporterPageRenderer
		{
			get { return new BuildReporterPageRenderer(dcFactory.QueryStringRequestWrapper, dcFactory.DefaultBuildRetrieverForRequest, dcFactory.HttpPathMapper); }
		}

		public SiteTemplate SiteTemplate
		{
			get { return new SiteTemplate(dcFactory.QueryStringRequestWrapper, dcFactory.ConfigurationSettingsConfigGetter, 
					  DefaultBuildLister, dcFactory.DefaultBuildRetrieverForRequest, dcFactory.CruiseManagerBuildNameRetriever);}
		}

		public LogViewerPageRenderer LogViewerPageRenderer
		{
			get { return new LogViewerPageRenderer(dcFactory.QueryStringRequestWrapper, dcFactory.DefaultBuildRetrieverForRequest, dcFactory.LocalFileCacheManager);}
		}

		public DefaultBuildLister DefaultBuildLister
		{
			get { return new DefaultBuildLister(dcFactory.ServerAggregatingCruiseManagerWrapper); }
		}

		public ServerLogViewerPageRenderer ServerLogViewerPageRenderer
		{
			get { return new ServerLogViewerPageRenderer(dcFactory.QueryStringRequestWrapper, dcFactory.ServerAggregatingCruiseManagerWrapper); }
		}
	}
}
