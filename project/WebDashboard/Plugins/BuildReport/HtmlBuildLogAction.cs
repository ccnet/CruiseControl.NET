using System.Collections;
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
		private readonly ILinkFactory linkFactory;

		public HtmlBuildLogAction(IBuildRetriever buildRetriever, IVelocityViewGenerator viewGenerator, ILinkFactory linkFactory)
		{
			this.buildRetriever = buildRetriever;
			this.viewGenerator = viewGenerator;
			this.linkFactory = linkFactory;
		}

		public IView Execute(ICruiseRequest cruiseRequest)
		{
			Hashtable velocityContext = new Hashtable();
			IBuildSpecifier buildSpecifier = cruiseRequest.BuildSpecifier;
			Build build = buildRetriever.GetBuild(buildSpecifier);
			velocityContext["log"] = build.Log.Replace("<", "&lt;").Replace(">", "&gt;");
			velocityContext["logUrl"] = linkFactory.CreateBuildLinkWithFileName(buildSpecifier, new ActionSpecifierWithName(XmlBuildLogAction.ACTION_NAME), buildSpecifier.BuildName).Url;

			return viewGenerator.GenerateView(@"BuildLog.vm", velocityContext);
		}
	}
}
