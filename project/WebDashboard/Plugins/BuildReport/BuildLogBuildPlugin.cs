using System.Collections;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport
{
	public class BuildLogBuildPlugin : ICruiseAction, IPluginLinkRenderer, IPlugin
	{
		private readonly IBuildRetriever buildRetriever;
		private readonly IVelocityViewGenerator viewGenerator;
		private readonly IUrlBuilder urlBuilder;

		public BuildLogBuildPlugin(IBuildRetriever buildRetriever, IVelocityViewGenerator viewGenerator, IUrlBuilder urlBuilder)
		{
			this.buildRetriever = buildRetriever;
			this.viewGenerator = viewGenerator;
			this.urlBuilder = urlBuilder;
		}

		public IView Execute(ICruiseRequest cruiseRequest)
		{
			Hashtable velocityContext = new Hashtable();
			IBuildSpecifier buildSpecifier = cruiseRequest.BuildSpecifier;
			Build build = buildRetriever.GetBuild(buildSpecifier);
			velocityContext["log"] = build.Log.Replace("<", "&lt;").Replace(">", "&gt;");

			// This hack is only here until we can have actions that can do File Responses
			velocityContext["logUrl"] = urlBuilder.BuildUrl(string.Format("{5}?{0}={1}&{2}={3}&{4}={5}",
				RequestWrappingCruiseRequest.ServerQueryStringParameter, buildSpecifier.ProjectSpecifier.ServerSpecifier.ServerName,
				RequestWrappingCruiseRequest.ProjectQueryStringParameter, buildSpecifier.ProjectSpecifier.ProjectName,
				RequestWrappingCruiseRequest.BuildQueryStringParameter, buildSpecifier.BuildName));

			return viewGenerator.GenerateView(@"BuildLog.vm", velocityContext);
		}

		public string LinkDescription
		{
			get { return "View Build Log"; }
		}

		public string LinkActionName
		{
			get { return "ViewBuildLog"; }
		}

		public TypedAction[] Actions
		{
			get {  return new TypedAction[] { new TypedAction(LinkActionName, this.GetType()) }; }
		}
	}
}
