using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerLogViewerPlugin
{
	public class ServerLogViewerPageRenderer
	{
		private readonly IRequestWrapper requestWrapper;
		private readonly ICruiseManagerWrapper cruiseManagerWrapper;

		public ServerLogViewerPageRenderer(IRequestWrapper requestWrapper, ICruiseManagerWrapper cruiseManagerWrapper)
		{
			this.cruiseManagerWrapper = cruiseManagerWrapper;
			this.requestWrapper = requestWrapper;
		}

		public ServerLogViewerResults Do ()
		{
			return new ServerLogViewerResults(GenerateLogHtml());
		}

		private string GenerateLogHtml()
		{
			return string.Format("<pre>{0}</pre>", cruiseManagerWrapper.GetServerLog(requestWrapper.GetServerName()));
		}
	}
}
