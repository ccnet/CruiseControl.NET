using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport
{
	public class ProjectReportProjectPlugin : ICruiseAction, IPluginLinkRenderer, IPlugin
	{
		public static readonly string ACTION_NAME = "ViewProjectReport";

		public ProjectReportProjectPlugin()
		{
		}

		public IView Execute(ICruiseRequest cruiseRequest)
		{
			return new DefaultView("Project report in development. For now, click one of the builds in the side bar to see a build report");
		}

		public string LinkDescription
		{
			get { return "Project Report"; }
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
