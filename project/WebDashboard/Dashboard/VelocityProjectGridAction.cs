using System;
using System.Collections;
using ThoughtWorks.CruiseControl.Core;
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
		private readonly IProjectGrid projectGrid;

		public VelocityProjectGridAction(IFarmService farmService, IUrlBuilder urlBuilder, IVelocityViewGenerator viewGenerator, IProjectGrid projectGrid)
		{
			this.farmService = farmService;
			this.urlBuilder = urlBuilder;
			this.viewGenerator = viewGenerator;
			this.projectGrid = projectGrid;
		}

		public IView Execute(string[] actionArguments, string actionName, IRequest request)
		{
			Hashtable velocityContext = new Hashtable();
			velocityContext["forceBuildMessage"] = ForceBuildIfNecessary(actionArguments);
			return GenerateView(farmService.GetProjectStatusListAndCaptureExceptions(), velocityContext, actionName, request, null);
		}

		public IView Execute(string[] actionArguments, string actionName, IServerSpecifier serverSpecifier, IRequest request)
		{
			Hashtable velocityContext = new Hashtable();
			velocityContext["forceBuildMessage"] = ForceBuildIfNecessary(actionArguments);
			return GenerateView(farmService.GetProjectStatusListAndCaptureExceptions(serverSpecifier), velocityContext, actionName, request, serverSpecifier);
		}

		private IView GenerateView(ProjectStatusListAndExceptions projectStatusListAndExceptions, Hashtable velocityContext, string actionName, IRequest request, IServerSpecifier serverSpecifier)
		{
			ActionSpecifierWithName actionSpecifier = new ActionSpecifierWithName(actionName);
			ProjectGridSortColumn sortColumn = GetSortColumn(request);
			bool sortReverse = GetSortReverse(request);

			velocityContext["projectNameSortLink"] = GenerateSortLink(serverSpecifier, actionSpecifier, ProjectGridSortColumn.Name, sortColumn, sortReverse);
			velocityContext["projectGrid"] = projectGrid.GenerateProjectGridRows(
				projectStatusListAndExceptions.StatusAndServerList, actionName, sortColumn, sortReverse);
			velocityContext["exceptions"] = projectStatusListAndExceptions.Exceptions;
			velocityContext["refreshButtonName"] = urlBuilder.BuildFormName(actionSpecifier);

			return viewGenerator.GenerateView(@"ProjectGrid.vm", velocityContext);
		}

		private bool GetSortReverse(IRequest request)
		{
			return request.FindParameterStartingWith("ReverseSort") != string.Empty;
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

		private object GenerateSortLink(IServerSpecifier serverSpecifier, IActionSpecifier actionSpecifier, ProjectGridSortColumn column, ProjectGridSortColumn currentColumn, bool currentReverse)
		{
			string queryString = "SortColumn=" + column.ToString();
			if (column == currentColumn && !currentReverse)
			{
				queryString += "&ReverseSort=ReverseSort";
			}
			if (serverSpecifier == null)
			{
				return urlBuilder.BuildUrl(actionSpecifier, queryString);
			}
			else
			{
				return urlBuilder.BuildServerUrl(actionSpecifier, serverSpecifier, queryString);
			}
		}

		private string ForceBuildIfNecessary(string[] actionArguments)
		{
			if (actionArguments.Length == 2)
			{
				return ForceBuild(actionArguments[0], actionArguments[1]);
			}
			else
			{
				return "";
			}
		}

		private string ForceBuild(string serverName, string projectName)
		{
			farmService.ForceBuild(new DefaultProjectSpecifier(new DefaultServerSpecifier(serverName), projectName));
			return string.Format("Build successfully forced for {0}", projectName);
		}
	}
}
