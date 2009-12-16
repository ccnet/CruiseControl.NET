using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;
using ThoughtWorks.CruiseControl.WebDashboard.Resources;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	// ToDo - Test!
	public class VelocityProjectGridAction : IProjectGridAction
	{
		private readonly IFarmService farmService;
		private  IUrlBuilder urlBuilder;
		private  ICruiseUrlBuilder cruiseUrlBuilder;
		private readonly IVelocityViewGenerator viewGenerator;
		private readonly IProjectGrid projectGrid;
        private readonly ISessionRetriever sessionRetriever;
        private ProjectGridSortColumn sortColumn = ProjectGridSortColumn.Name;
        private Translations translations;

		public VelocityProjectGridAction(IFarmService farmService, IVelocityViewGenerator viewGenerator, 
            IProjectGrid projectGrid,
            ISessionRetriever sessionRetriever)
		{
			this.farmService = farmService;
			this.viewGenerator = viewGenerator;
			this.projectGrid = projectGrid;
            this.sessionRetriever = sessionRetriever;
		}

        #region Properties
        #region DefaultSortColumn
        /// <summary>
        /// The default column to sort by.
        /// </summary>
        public ProjectGridSortColumn DefaultSortColumn
        {
            get { return sortColumn; }
            set { sortColumn = value; }
        }
        #endregion
        #endregion


        public IResponse Execute(string actionName, ICruiseRequest request)
		{
            return GenerateView(farmService.GetProjectStatusListAndCaptureExceptions(request.RetrieveSessionToken()), actionName, request, null);
		}

        public IResponse Execute(string actionName, IServerSpecifier serverSpecifier, ICruiseRequest request)
		{
			//Added code so since defaultServerSpecifier only sets the name of the server - not the actual config
			serverSpecifier = farmService.GetServerConfiguration(serverSpecifier.ServerName);
            this.translations = new Translations();
            return GenerateView(farmService.GetProjectStatusListAndCaptureExceptions(serverSpecifier, request.RetrieveSessionToken()), actionName, request, serverSpecifier);
		}

		private HtmlFragmentResponse GenerateView(ProjectStatusListAndExceptions projectStatusListAndExceptions,
            string actionName, ICruiseRequest request, IServerSpecifier serverSpecifier)
		{
            cruiseUrlBuilder = request.UrlBuilder;
            urlBuilder = request.UrlBuilder.InnerBuilder;
			Hashtable velocityContext = new Hashtable();
            velocityContext["forceBuildMessage"] = ForceBuildIfNecessary(request.Request);
            velocityContext["parametersCall"] = new ServerLink(cruiseUrlBuilder, new DefaultServerSpecifier("null"), string.Empty, ProjectParametersAction.ActionName).Url;

			velocityContext["wholeFarm"] = serverSpecifier == null ?  true : false;

			string category = request.Request.GetText("Category");
			velocityContext["showCategoryColumn"] = string.IsNullOrEmpty(category) ? true : false;

			ProjectGridSortColumn sortColumn = GetSortColumn(request.Request);
			bool sortReverse = SortAscending(request.Request);

			velocityContext["projectNameSortLink"] = GenerateSortLink(serverSpecifier, actionName, ProjectGridSortColumn.Name, sortColumn, sortReverse);
			velocityContext["buildStatusSortLink"] = GenerateSortLink(serverSpecifier, actionName, ProjectGridSortColumn.BuildStatus, sortColumn, sortReverse);
			velocityContext["lastBuildDateSortLink"] = GenerateSortLink(serverSpecifier, actionName, ProjectGridSortColumn.LastBuildDate, sortColumn, sortReverse);
			velocityContext["serverNameSortLink"] = GenerateSortLink(serverSpecifier, actionName, ProjectGridSortColumn.ServerName, sortColumn, sortReverse);
			velocityContext["projectCategorySortLink"] = GenerateSortLink(serverSpecifier, actionName, ProjectGridSortColumn.Category, sortColumn, sortReverse);

            ProjectGridRow[] projectGridRows = projectGrid.GenerateProjectGridRows(projectStatusListAndExceptions.StatusAndServerList, actionName, sortColumn, sortReverse, category, cruiseUrlBuilder, this.translations);

            velocityContext["projectGrid"] = projectGridRows;
			velocityContext["exceptions"] = projectStatusListAndExceptions.Exceptions;

            Array categoryList = this.GenerateCategoryList(projectGridRows);
            velocityContext["categoryList"] = categoryList;

			return viewGenerator.GenerateView(@"ProjectGrid.vm", velocityContext);
		}

        private Array GenerateCategoryList(ProjectGridRow[] projectGridRows)
        {
            if (projectGridRows == null) return null;

            List<string> categories = new List<string>();

            foreach (ProjectGridRow projectGridRow in projectGridRows)
            {
                string category = projectGridRow.Category;
                System.Diagnostics.Debug.WriteLine(category);

                if (!string.IsNullOrEmpty(category) && !categories.Contains(category))
                    categories.Add(category);
            }

            // sort list if at least one element exists
            if (categories.Count == 0) return null;

            categories.Sort();

            return categories.ToArray();
        }

		private bool SortAscending(IRequest request)
		{
			return request.FindParameterStartingWith("ReverseSort") == string.Empty;
		}

		private ProjectGridSortColumn GetSortColumn(IRequest request)
		{
			string columnName = request.GetText("SortColumn");
            if (string.IsNullOrEmpty(columnName))
            {
                return sortColumn;
            }
            else
            {
                try
                {
                    return (ProjectGridSortColumn)Enum.Parse(typeof(ProjectGridSortColumn), columnName);
                }
                catch (Exception)
                {
                    throw new CruiseControlException(string.Format("Error attempting to calculate column to sort. Specified column name was [{0}]", columnName));
                }
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
            // Attempt to find a session token
            string sessionToken = request.GetText("sessionToken");
            if (string.IsNullOrEmpty(sessionToken) && (sessionRetriever != null))
            {
                sessionToken = sessionRetriever.RetrieveSessionToken(request);
            }

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            foreach (string parameterName in HttpContext.Current.Request.Form.AllKeys)
            {
                if (parameterName.StartsWith("param_"))
                {
                    parameters.Add(parameterName.Substring(6), HttpContext.Current.Request.Form[parameterName]);
                }
            }

            // Make the actual call
			if (request.FindParameterStartingWith("StopBuild") != string.Empty)
			{
				farmService.Stop(ProjectSpecifier(request), sessionToken);
                return this.translations.Translate("Stopping project {0}", SelectedProject(request));
			}
			else if (request.FindParameterStartingWith("StartBuild") != string.Empty)
			{
                farmService.Start(ProjectSpecifier(request), sessionToken);
                return this.translations.Translate("Starting project {0}", SelectedProject(request));				
			}
			else if (request.FindParameterStartingWith("ForceBuild") != string.Empty)
			{
				farmService.ForceBuild(ProjectSpecifier(request), sessionToken, parameters);
                return this.translations.Translate("Build successfully forced for {0}", SelectedProject(request));
			}
			else if (request.FindParameterStartingWith("AbortBuild") != string.Empty)
			{
				farmService.AbortBuild(ProjectSpecifier(request), sessionToken);
                return this.translations.Translate("Abort successfully forced for {0}", SelectedProject(request));
			}
			else
			{
				return string.Empty;
			}
		}

		private DefaultProjectSpecifier ProjectSpecifier(IRequest request)
		{
			return new DefaultProjectSpecifier(
				farmService.GetServerConfiguration(request.GetText("serverName")), SelectedProject(request));
		}

		private static string SelectedProject(IRequest request)
		{
			return request.GetText("projectName");
		}
	}
}
