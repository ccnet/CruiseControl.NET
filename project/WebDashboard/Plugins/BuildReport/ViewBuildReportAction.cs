using System.Collections;
using System.Configuration;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport
{
	public class BuildReportBuildPlugin : ICruiseAction, IPluginLinkRenderer, IPlugin
	{
		public static readonly string ACTION_NAME = "ViewBuildReport";

		private readonly IBuildLogTransformer buildLogTransformer;

		public BuildReportBuildPlugin(IBuildLogTransformer buildLogTransformer)
		{
			this.buildLogTransformer = buildLogTransformer;
		}

		// ToDo - this shouldn't access Configuration Settings directly ... but maybe we want a new plugin impl anyway using configurable plugins
		public IView Execute (ICruiseRequest cruiseRequest)
		{
			return new HtmlView(buildLogTransformer.Transform(cruiseRequest.BuildSpecifier, (string[]) ((ArrayList) ConfigurationSettings.GetConfig("CCNet/xslFiles")).ToArray(typeof (string))));
		}

		public string LinkDescription
		{
			get { return "Build Report"; }
		}

		public string LinkActionName
		{
			get { return ACTION_NAME; }
		}

		public TypedAction[] Actions
		{
			get {  return new TypedAction[] { new TypedAction(LinkActionName, this.GetType()) }; }
		}
	}
}
