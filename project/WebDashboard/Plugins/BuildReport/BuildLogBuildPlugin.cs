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

		public BuildLogBuildPlugin(IBuildRetriever buildRetriever, IVelocityViewGenerator viewGenerator)
		{
			this.buildRetriever = buildRetriever;
			this.viewGenerator = viewGenerator;
		}

		public IView Execute(ICruiseRequest cruiseRequest)
		{
			Hashtable velocityContext = new Hashtable();
			Build build = buildRetriever.GetBuild(cruiseRequest.BuildSpecifier);
			velocityContext["log"] = build.Log.Replace("<", "&lt;").Replace(">", "&gt;");
			velocityContext["logUrl"] = build.BuildLogLocation;

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
