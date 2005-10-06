using System.Collections;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport
{
	public class HtmlBuildLogAction : ICruiseAction
	{
		public static readonly string ACTION_NAME = "ViewBuildLog";

		private readonly IBuildRetriever buildRetriever;
		private readonly IVelocityViewGenerator viewGenerator;
		private readonly ICruiseUrlBuilder urlBuilder;

		public HtmlBuildLogAction(IBuildRetriever buildRetriever, IVelocityViewGenerator viewGenerator, ICruiseUrlBuilder urlBuilder)
		{
			this.buildRetriever = buildRetriever;
			this.viewGenerator = viewGenerator;
			this.urlBuilder = urlBuilder;
		}

		public IResponse Execute(ICruiseRequest cruiseRequest)
		{
			Hashtable velocityContext = new Hashtable();
			IBuildSpecifier buildSpecifier = cruiseRequest.BuildSpecifier;
			Build build = buildRetriever.GetBuild(buildSpecifier);
			velocityContext["log"] = build.Log.Replace("<", "&lt;").Replace(">", "&gt;");
			urlBuilder.Extension = "xml";
			velocityContext["logUrl"] = urlBuilder.BuildBuildUrl(XmlBuildLogAction.ACTION_NAME, buildSpecifier);

			return viewGenerator.GenerateView(@"BuildLog.vm", velocityContext);
		}
	}
}
