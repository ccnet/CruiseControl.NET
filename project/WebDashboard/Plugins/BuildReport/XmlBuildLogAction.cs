using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport
{
	public class XmlBuildLogAction : ICruiseAction
	{
		public static readonly string ACTION_NAME = "XmlBuildLog";

		private readonly IBuildRetriever buildRetriever;

		public XmlBuildLogAction(IBuildRetriever buildRetriever)
		{
			this.buildRetriever = buildRetriever;
		}

		public IResponse Execute(ICruiseRequest cruiseRequest)
		{
			return new XmlFragmentResponse(buildRetriever.GetBuild(cruiseRequest.BuildSpecifier, cruiseRequest.RetrieveSessionToken()).Log);
		}
	}
}