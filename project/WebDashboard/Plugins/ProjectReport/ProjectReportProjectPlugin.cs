using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport
{
	[ReflectorType("projectReportProjectPlugin")]
	public class ProjectReportProjectPlugin : ICruiseAction, IPlugin
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

		public IResponse Execute(ICruiseRequest cruiseRequest)
		{
			Hashtable velocityContext = new Hashtable();
			IProjectSpecifier projectSpecifier = cruiseRequest.ProjectSpecifier;

			IBuildSpecifier[] buildSpecifiers = farmService.GetMostRecentBuildSpecifiers(projectSpecifier, 1);
			if (buildSpecifiers.Length == 1)
			{
				velocityContext["mostRecentBuildUrl"] = linkFactory.CreateProjectLink(projectSpecifier, LatestBuildReportProjectPlugin.ACTION_NAME).Url;
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

		public INamedAction[] NamedActions
		{
			get {  return new INamedAction[] { new ImmutableNamedAction(ACTION_NAME, this) }; }
		}
	}
}
