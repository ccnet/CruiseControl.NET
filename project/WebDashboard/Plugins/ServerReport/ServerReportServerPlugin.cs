using System.Collections;
using System.Drawing;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerReport
{
	public class ServerReportServerPlugin : ICruiseAction, IPluginLinkRenderer, IPlugin
	{
		private readonly IFarmService farmService;
		private readonly IUrlBuilder urlBuilder;
		private readonly IVelocityViewGenerator viewGenerator;

		public ServerReportServerPlugin(IFarmService farmService, IUrlBuilder urlBuilder, IVelocityViewGenerator viewGenerator)
		{
			this.farmService = farmService;
			this.urlBuilder = urlBuilder;
			this.viewGenerator = viewGenerator;
		}

		public IView Execute(ICruiseRequest request)
		{
			Hashtable velocityContext = new Hashtable();
			velocityContext["forceBuildMessage"] = ForceBuildIfNecessary(request);
			velocityContext["projectGrid"] = CreateProjectGrid(farmService.GetProjectStatusList(request.ServerSpecifier), request.ServerSpecifier);
			velocityContext["refreshButtonName"] = urlBuilder.BuildFormName(new ActionSpecifierWithName(LinkActionName));

			return viewGenerator.GenerateView(@"ProjectGrid.vm", velocityContext);
		}

		private string ForceBuildIfNecessary(ICruiseRequest request)
		{
			string[] actionArguments = request.Request.ActionArguments;
			if (actionArguments.Length == 0)
			{
				return "";
			}
			else if (actionArguments.Length == 2)
			{
				return ForceBuild(actionArguments[0], actionArguments[1]);
			}
			else
			{
				return "Unexpected action arguments - not forcing build";
			}
		}

		private string ForceBuild(string serverName, string projectName)
		{
			farmService.ForceBuild(new DefaultProjectSpecifier(new DefaultServerSpecifier(serverName), projectName));
			return string.Format("Build successfully forced for {0}", projectName);
		}

		private IList CreateProjectGrid(ProjectStatus[] statuses, IServerSpecifier serverSpecifier)
		{
			ArrayList rows = new ArrayList();
			foreach (ProjectStatus status in statuses)
			{
				rows.Add(new ProjectGridRow(status.Name, status.BuildStatus.ToString(), CalculateHtmlColor(status.BuildStatus), status.LastBuildDate, status.LastBuildLabel, status.Status.ToString(), status.Activity.ToString(), CalculateForceBuildButtonName(serverSpecifier, status.Name)));
			}
			return rows;
		}

		private string CalculateForceBuildButtonName(IServerSpecifier specifier, string projectName)
		{
			return urlBuilder.BuildFormName(new ActionSpecifierWithName(LinkActionName), specifier.ServerName, projectName);
		}

		private string CalculateHtmlColor(IntegrationStatus status)
		{
			if (status == IntegrationStatus.Success)
			{
				return Color.Green.Name;
			}
			else if (status == IntegrationStatus.Unknown)
			{
				return Color.Yellow.Name;
			}
			else
			{
				return Color.Red.Name;
			}
		}

		public string LinkDescription
		{
			get { return "New Server Report (in development!)"; }
		}

		public string LinkActionName
		{
			get { return "ViewServerReport"; }
		}

		public TypedAction[] Actions
		{
			get {  return new TypedAction[] { new TypedAction(LinkActionName, this.GetType()) }; }
		}
	}
}
