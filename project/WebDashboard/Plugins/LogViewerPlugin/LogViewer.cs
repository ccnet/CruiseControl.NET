using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.LogViewerPlugin
{
	public class LogViewer
	{
		private readonly IBuildRetriever buildRetriever;

		public LogViewer(IBuildRetriever buildRetriever)
		{
			this.buildRetriever = buildRetriever;
		}

		public LogViewerResults Do()
		{
			return new LogViewerResults(buildRetriever.GetBuild().Url);
		}
	}
}
