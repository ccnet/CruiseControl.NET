using System.Collections;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport
{
	public class ProjectReportProjectPlugin : ICruiseAction, IPluginLinkRenderer, IPlugin
	{
		private readonly IFarmService farmService;
		private readonly IVelocityViewGenerator viewGenerator;
		public static readonly string ACTION_NAME = "ViewProjectReport";

		public ProjectReportProjectPlugin(IFarmService farmService, IVelocityViewGenerator viewGenerator)
		{
			this.farmService = farmService;
			this.viewGenerator = viewGenerator;
		}

		public IView Execute(ICruiseRequest cruiseRequest)
		{
			Hashtable velocityContext = new Hashtable();
			velocityContext["projectName"] = cruiseRequest.ProjectSpecifier.ProjectName;
			velocityContext["externalLinks"] = farmService.GetExternalLinks(cruiseRequest.ProjectSpecifier);

			return viewGenerator.GenerateView(@"ProjectReport.vm", velocityContext);
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
