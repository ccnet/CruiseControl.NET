using System.Collections;
using System.Drawing;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	// ToDo - Test!
	public class VelocityProjectGridAction : IProjectGridAction
	{
		private readonly IFarmService farmService;
		private readonly IUrlBuilder urlBuilder;
		private readonly IVelocityViewGenerator viewGenerator;

		public VelocityProjectGridAction(IFarmService farmService, IUrlBuilder urlBuilder, IVelocityViewGenerator viewGenerator)
		{
			this.farmService = farmService;
			this.urlBuilder = urlBuilder;
			this.viewGenerator = viewGenerator;
		}

		public IView Execute(string[] actionArguments, string actionName)
		{
			Hashtable velocityContext = new Hashtable();
			velocityContext["forceBuildMessage"] = ForceBuildIfNecessary(actionArguments);
			return GenerateView(farmService.GetProjectStatusListAndCaptureExceptions(), velocityContext, actionName);
		}

		public IView Execute(string[] actionArguments, string actionName, IServerSpecifier serverSpecifer)
		{
			Hashtable velocityContext = new Hashtable();
			velocityContext["forceBuildMessage"] = ForceBuildIfNecessary(actionArguments);
			return GenerateView(farmService.GetProjectStatusListAndCaptureExceptions(serverSpecifer), velocityContext, actionName);
		}

		private IView GenerateView(ProjectStatusListAndExceptions projectStatusListAndExceptions, Hashtable velocityContext, string actionName)
		{
			velocityContext["projectGrid"] = CreateProjectGrid(projectStatusListAndExceptions.StatusAndServerList, actionName);
			velocityContext["exceptions"] = projectStatusListAndExceptions.Exceptions;
			velocityContext["refreshButtonName"] = urlBuilder.BuildFormName(new ActionSpecifierWithName(actionName));

			return viewGenerator.GenerateView(@"ProjectGrid.vm", velocityContext);
		}

		private string ForceBuildIfNecessary(string[] actionArguments)
		{
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

		private IList CreateProjectGrid(ProjectStatusOnServer[] statuses, string actionName)
		{
			ArrayList rows = new ArrayList();
			foreach (ProjectStatusOnServer statusOnServer in statuses)
			{
				ProjectStatus status = statusOnServer.ProjectStatus;
				rows.Add(new ProjectGridRow(
					status.Name, status.BuildStatus.ToString(), CalculateHtmlColor(status.BuildStatus), status.LastBuildDate, 
					status.LastBuildLabel, status.Status.ToString(), status.Activity.ToString(), 
					CalculateForceBuildButtonName(statusOnServer.ServerSpecifier, status.Name, actionName), status.WebURL));
			}
			return rows;
		}

		private string CalculateForceBuildButtonName(IServerSpecifier specifier, string projectName, string actionName)
		{
			return urlBuilder.BuildFormName(new ActionSpecifierWithName(actionName), specifier.ServerName, projectName);
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
	}
}
