using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReporterPlugin;

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
			get { return new BuildReporterPageRenderer(dcFactory.RequestWrappingCruiseRequest, dcFactory.CachingBuildRetriever, dcFactory.HttpPathMapper); }
		}
	}
}
