using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerLogViewerPlugin
{
	public class ServerLogViewerPageRenderer
	{
		private readonly ICruiseRequest request;
		private readonly ICruiseManagerWrapper cruiseManagerWrapper;

		public ServerLogViewerPageRenderer(ICruiseRequest request, ICruiseManagerWrapper cruiseManagerWrapper)
		{
			this.cruiseManagerWrapper = cruiseManagerWrapper;
			this.request = request;
		}

		public ServerLogViewerResults Do ()
		{
			return new ServerLogViewerResults(GenerateLogHtml());
		}

		private string GenerateLogHtml()
		{
			return string.Format("<pre>{0}</pre>", cruiseManagerWrapper.GetServerLog(request.ServerName));
		}
	}
}
