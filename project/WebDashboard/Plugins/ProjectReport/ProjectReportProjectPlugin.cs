using System.Collections;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport
{
	public class ProjectReportProjectPlugin : ICruiseAction, IPluginLinkRenderer, IPlugin
	{
		private readonly IFarmService farmService;
		private readonly IVelocityViewGenerator viewGenerator;
		private readonly ILinkFactory linkFactory;
		public static readonly string ACTION_NAME = "ViewProjectReport";

		public ProjectReportProjectPlugin(IFarmService farmService, IVelocityViewGenerator viewGenerator, ILinkFactory linkFactory)
		{
			this.farmService = farmService;
			this.viewGenerator = viewGenerator;
			this.linkFactory = linkFactory;
		}

		public IView Execute(ICruiseRequest cruiseRequest)
		{
			Hashtable velocityContext = new Hashtable();
			IProjectSpecifier projectSpecifier = cruiseRequest.ProjectSpecifier;

			IBuildSpecifier[] buildSpecifiers = farmService.GetMostRecentBuildSpecifiers(projectSpecifier, 1);
			if (buildSpecifiers.Length == 1)
			{
				velocityContext["mostRecentBuildUrl"] = linkFactory.CreateBuildLink(buildSpecifiers[0], new ActionSpecifierWithName(BuildReportBuildPlugin.ACTION_NAME)).Url;
			}

			velocityContext["projectName"] = projectSpecifier.ProjectName;
			velocityContext["externalLinks"] = farmService.GetExternalLinks(projectSpecifier);
			velocityContext["noLogsAvailable"] = (buildSpecifiers.Length == 0);

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
