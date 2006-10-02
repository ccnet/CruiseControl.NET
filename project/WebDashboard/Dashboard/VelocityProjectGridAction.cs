using System;
using System.Collections;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
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
		private readonly ICruiseUrlBuilder cruiseUrlBuilder;
		private readonly IVelocityViewGenerator viewGenerator;
		private readonly IProjectGrid projectGrid;

		public VelocityProjectGridAction(IFarmService farmService, IUrlBuilder urlBuilder, ICruiseUrlBuilder cruiseUrlBuilder, IVelocityViewGenerator viewGenerator, IProjectGrid projectGrid)
		{
			this.farmService = farmService;
			this.urlBuilder = urlBuilder;
			this.cruiseUrlBuilder = cruiseUrlBuilder;
			this.viewGenerator = viewGenerator;
			this.projectGrid = projectGrid;
		}

		public IResponse Execute(string actionName, IRequest request)
		{
			return GenerateView(farmService.GetProjectStatusListAndCaptureExceptions(), actionName, request, null);
		}

		public IResponse Execute(string actionName, IServerSpecifier serverSpecifier, IRequest request)
		{
			//Added code so since defaultServerSpecifier only sets the name of the server - not the actual config
			serverSpecifier = farmService.GetServerConfiguration(serverSpecifier.ServerName);
			return GenerateView(farmService.GetProjectStatusListAndCaptureExceptions(serverSpecifier), actionName, request, serverSpecifier);
		}

		private HtmlFragmentResponse GenerateView(ProjectStatusListAndExceptions projectStatusListAndExceptions, string actionName, IRequest request, IServerSpecifier serverSpecifier)
		{
			Hashtable velocityContext = new Hashtable();
			velocityContext["forceBuildMessage"] = ForceBuildIfNecessary(request);

			velocityContext["wholeFarm"] = serverSpecifier == null ?  true : false;

			ProjectGridSortColumn sortColumn = GetSortColumn(request);
			bool sortReverse = SortAscending(request);

			velocityContext["projectNameSortLink"] = GenerateSortLink(serverSpecifier, actionName, ProjectGridSortColumn.Name, sortColumn, sortReverse);
			velocityContext["buildStatusSortLink"] = GenerateSortLink(serverSpecifier, actionName, ProjectGridSortColumn.BuildStatus, sortColumn, sortReverse);
			velocityContext["lastBuildDateSortLink"] = GenerateSortLink(serverSpecifier, actionName, ProjectGridSortColumn.LastBuildDate, sortColumn, sortReverse);
			velocityContext["serverNameSortLink"] = GenerateSortLink(serverSpecifier, actionName, ProjectGridSortColumn.ServerName, sortColumn, sortReverse);
			velocityContext["projectGrid"] = projectGrid.GenerateProjectGridRows(
				projectStatusListAndExceptions.StatusAndServerList, actionName, sortColumn, sortReverse);
			velocityContext["exceptions"] = projectStatusListAndExceptions.Exceptions;

			return viewGenerator.GenerateView(@"ProjectGrid.vm", velocityContext);
		}

		private bool SortAscending(IRequest request)
		{
			return request.FindParameterStartingWith("ReverseSort") == string.Empty;
		}

		private ProjectGridSortColumn GetSortColumn(IRequest request)
		{
			string columnName = request.GetText("SortColumn");
			if (columnName == string.Empty)
			{
				columnName = "Name";
			}
			try
			{
				return (ProjectGridSortColumn) Enum.Parse(typeof(ProjectGridSortColumn), columnName);	
			}
			catch (Exception)
			{
				throw new CruiseControlException(string.Format("Error attempting to calculate column to sort. Specified column name was [{0}]", columnName));
			}
		}

		private object GenerateSortLink(IServerSpecifier serverSpecifier, string action, ProjectGridSortColumn column, ProjectGridSortColumn currentColumn, bool currentReverse)
		{
			string queryString = "SortColumn=" + column.ToString();
			if (column == currentColumn && !currentReverse)
			{
				queryString += "&ReverseSort=ReverseSort";
			}
			if (serverSpecifier == null)
			{
				return urlBuilder.BuildUrl(action, queryString);
			}
			else
			{
				return cruiseUrlBuilder.BuildServerUrl(action, serverSpecifier, queryString);
			}
		}

		private string ForceBuildIfNecessary(IRequest request)
		{
			if (request.FindParameterStartingWith("StopBuild") != string.Empty)
			{
				farmService.Stop(ProjectSpecifier(request));
				return string.Format("Stopping project {0}", SelectedProject(request));
			}
			else if (request.FindParameterStartingWith("StartBuild") != string.Empty)
			{
				farmService.Start(ProjectSpecifier(request));
				return string.Format("Starting project {0}", SelectedProject(request));				
			}
			else if (request.FindParameterStartingWith("forcebuild") != string.Empty)
			{
				farmService.ForceBuild(ProjectSpecifier(request));
				return string.Format("Build successfully forced for {0}", SelectedProject(request));
			}
			else
			{
				return "";
			}
		}

		private DefaultProjectSpecifier ProjectSpecifier(IRequest request)
		{
			return new DefaultProjectSpecifier(
				farmService.GetServerConfiguration(request.GetText("forceBuildServer")), SelectedProject(request));
		}

		private static string SelectedProject(IRequest request)
		{
			return request.GetText("forceBuildProject");
		}
	}
}
