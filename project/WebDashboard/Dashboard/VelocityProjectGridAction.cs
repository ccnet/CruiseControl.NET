using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
//using System.Web.Http;
using System.Diagnostics;
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
		private readonly IFarmService FarmService;
		private  IUrlBuilder UrlBuilder;
		private  ICruiseUrlBuilder CruiseUrlBuilder;
		private readonly IVelocityViewGenerator ViewGenerator;
		private readonly IProjectGrid ProjectGrid;
        private readonly ISessionRetriever SessionRetriever;
        private ProjectGridSortColumn SortColumn = ProjectGridSortColumn.Name;
        private Translations translation;

		public VelocityProjectGridAction(IFarmService farmServices, IVelocityViewGenerator viewGenerator, 
            IProjectGrid projectGrid,
            ISessionRetriever sessionRetriever)
		{
			this.FarmService = farmServices;
			this.ViewGenerator = viewGenerator;
			this.ProjectGrid = projectGrid;
            this.SessionRetriever = sessionRetriever;
		}

        #region Properties
        #region DefaultSortColumn
        /// <summary>
        /// The default column to sort by.
        /// </summary>
        public ProjectGridSortColumn DefaultSortColumn
        {
            get { return SortColumn; }
            set { SortColumn = value; }
        }
        #endregion

        #region SuccessIndicatorBarLocation
        /// <summary>
        /// Gets or sets the success indicator bar location.
        /// </summary>
        /// <value>The success indicator bar location.</value>
        public IndicatorBarLocation SuccessIndicatorBarLocation { get; set; }
        #endregion
        #endregion

        public IResponse Execute(string actionName, ICruiseRequest request)
		{
            return GenerateView(FarmService.GetProjectStatusListAndCaptureExceptions(request.RetrieveSessionToken()), actionName, request, null);
		}

        public IResponse Execute(string actionName, IServerSpecifier serverSpecifier, ICruiseRequest request)
		{
			//Added code so since defaultServerSpecifier only sets the name of the server - not the actual config
            var serverName = serverSpecifier.ServerName;
            serverSpecifier = FarmService.GetServerConfiguration(serverName);
            if (serverSpecifier == null)
            {
                throw new UnknownServerException(serverName);
            }
            else
            {
                return GenerateView(FarmService.GetProjectStatusListAndCaptureExceptions(serverSpecifier, request.RetrieveSessionToken()), actionName, request, serverSpecifier);
            }
		}

		private HtmlFragmentResponse GenerateView(ProjectStatusListAndExceptions projectStatusListAndExceptions,
            string actionName, ICruiseRequest request, IServerSpecifier serverSpecifier)
		{
            this.translation = Translations.RetrieveCurrent();
            bool sortReverse = SortAscending(request.Request);
            string category = request.Request.GetText("Category");
            CruiseUrlBuilder = request.UrlBuilder;
            UrlBuilder = request.UrlBuilder.InnerBuilder;
            ProjectGridSortColumn sortColumn = GetSortColumn(request.Request);
			Hashtable velocityContext = new Hashtable();
            velocityContext["forceBuildMessage"] = ForceBuildIfNecessary(request.Request);
            velocityContext["parametersCall"] = new ServerLink(CruiseUrlBuilder, new DefaultServerSpecifier("null"), string.Empty, ProjectParametersAction.ActionName).Url;
			velocityContext["wholeFarm"] = serverSpecifier == null ?  true : false;
			velocityContext["showCategoryColumn"] = string.IsNullOrEmpty(category) ? true : false;
			velocityContext["projectNameSortLink"] = GenerateSortLink(serverSpecifier, actionName, ProjectGridSortColumn.Name, sortColumn, sortReverse);
			velocityContext["buildStatusSortLink"] = GenerateSortLink(serverSpecifier, actionName, ProjectGridSortColumn.BuildStatus, sortColumn, sortReverse);
			velocityContext["lastBuildDateSortLink"] = GenerateSortLink(serverSpecifier, actionName, ProjectGridSortColumn.LastBuildDate, sortColumn, sortReverse);
			velocityContext["serverNameSortLink"] = GenerateSortLink(serverSpecifier, actionName, ProjectGridSortColumn.ServerName, sortColumn, sortReverse);
			velocityContext["projectCategorySortLink"] = GenerateSortLink(serverSpecifier, actionName, ProjectGridSortColumn.Category, sortColumn, sortReverse);
            velocityContext["exceptions"] = projectStatusListAndExceptions.Exceptions;

            ProjectGridParameters parameters = new ProjectGridParameters(projectStatusListAndExceptions.StatusAndServerList, sortColumn, sortReverse, category, CruiseUrlBuilder, FarmService, this.translation);
            ProjectGridRow[] projectGridRows = ProjectGrid.GenerateProjectGridRows(parameters);
            velocityContext["projectGrid"] = projectGridRows;
			
            Array categoryList = this.GenerateCategoryList(projectGridRows);
            velocityContext["categoryList"] = categoryList;

            velocityContext["barAtTop"] = (this.SuccessIndicatorBarLocation == IndicatorBarLocation.Top) ||
                (this.SuccessIndicatorBarLocation == IndicatorBarLocation.TopAndBottom);
            velocityContext["barAtBottom"] = (this.SuccessIndicatorBarLocation == IndicatorBarLocation.Bottom) ||
                (this.SuccessIndicatorBarLocation == IndicatorBarLocation.TopAndBottom);

			return ViewGenerator.GenerateView(@"ProjectGrid.vm", velocityContext);
		}

        private Array GenerateCategoryList(ProjectGridRow[] projectGridRows)
        {
            if (projectGridRows == null) return null;

            List<string> categories = new List<string>();

            foreach (ProjectGridRow projectGridRow in projectGridRows)
            {
                string category = projectGridRow.Category;
                //debug.WriteLine(category);

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
                return SortColumn;
            }
            else
            {
                try
                {
                    return (ProjectGridSortColumn)Enum.Parse(typeof(ProjectGridSortColumn), columnName);
                }
                catch (Exception)
                {
                    throw new CruiseControlException(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Error attempting to calculate column to sort. Specified column name was [{0}]", columnName));
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
				return UrlBuilder.BuildUrl(action, queryString);
			}
			else
			{
				return CruiseUrlBuilder.BuildServerUrl(action, serverSpecifier, queryString);
			}
		}

		private string ForceBuildIfNecessary(IRequest request)
		{
            // Attempt to find a session token
            string sessionToken = request.GetText("sessionToken");
            if (string.IsNullOrEmpty(sessionToken) && (SessionRetriever != null))
            {
                sessionToken = SessionRetriever.RetrieveSessionToken(request);
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
                FarmService.Stop(ProjectSpecifier(request), sessionToken);
                return this.translation.Translate("Stopping project {0}", SelectedProject(request));
			}
			else if (request.FindParameterStartingWith("StartBuild") != string.Empty)
			{
                FarmService.Start(ProjectSpecifier(request), sessionToken);
                return this.translation.Translate("Starting project {0}", SelectedProject(request));				
			}
			else if (request.FindParameterStartingWith("ForceBuild") != string.Empty)
			{
                FarmService.ForceBuild(ProjectSpecifier(request), sessionToken, parameters);
                return this.translation.Translate("Build successfully forced for {0}", SelectedProject(request));
			}
			else if (request.FindParameterStartingWith("AbortBuild") != string.Empty)
			{
                FarmService.AbortBuild(ProjectSpecifier(request), sessionToken);
                return this.translation.Translate("Abort successfully forced for {0}", SelectedProject(request));
			}
            else if (request.FindParameterStartingWith("CancelPending") != string.Empty)
            {
                FarmService.CancelPendingRequest(ProjectSpecifier(request), sessionToken);
                return this.translation.Translate("Cancel pending successfully forced for {0}", SelectedProject(request));
            }
			else
			{
				return string.Empty;
			}
		}

		private DefaultProjectSpecifier ProjectSpecifier(IRequest request)
		{
			return new DefaultProjectSpecifier(
                FarmService.GetServerConfiguration(request.GetText("serverName")), SelectedProject(request));
		}

		private static string SelectedProject(IRequest request)
		{
			return request.GetText("projectName");
		}
	}
}
