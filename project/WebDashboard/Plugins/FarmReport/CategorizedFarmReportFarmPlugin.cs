namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.FarmReport
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
    using ThoughtWorks.CruiseControl.WebDashboard.IO;
    using ThoughtWorks.CruiseControl.WebDashboard.MVC;
    using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
    using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
    using ThoughtWorks.CruiseControl.WebDashboard.Resources;
    using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

    using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
    using System;
    using ThoughtWorks.CruiseControl.Core;
    using System.Web;
    using System.Linq;


    [ReflectorType("categorizedFarmReportFarmPlugin")]
    public class CategorizedFarmReportFarmPlugin : IPlugin, ICruiseAction
    {
        private const string BaseActionName = "ViewCategorizedFarmReport";

        private readonly IFarmService farmService;
        private readonly IProjectGrid projectGrid;
        private readonly IVelocityViewGenerator viewGenerator;
        private readonly ImmutableNamedAction baseAction;

        private Translations translations;

        public CategorizedFarmReportFarmPlugin(IFarmService farmService,
                                               IProjectGrid projectGrid,
                                               IVelocityViewGenerator viewGenerator)
        {
            this.farmService = farmService;
            this.projectGrid = projectGrid;
            this.viewGenerator = viewGenerator;
            this.LinkDescription = "Categorized Farm Report";
            this.baseAction = new ImmutableNamedAction(BaseActionName, this);
        }

        /// <summary>
        /// Gest instances of all the actions in the plugin.
        /// </summary>
        public INamedAction[] NamedActions
        {
            get
            {
                return new INamedAction[] { this.baseAction };
            }
        }

        /// <summary>
        /// Gets the text that appears in the Dashboard UI to link to this 
        /// plugin.
        /// </summary>
        [ReflectorProperty("description", Required = false)]
        public string LinkDescription { get; set; }

        public IResponse Execute(ICruiseRequest request)
        {
            var velocityContext = new Hashtable();
            this.translations = Translations.RetrieveCurrent();

            var projectStatus = farmService.GetProjectStatusListAndCaptureExceptions(request.RetrieveSessionToken());
            var urlBuilder = request.UrlBuilder;
            var category = request.Request.GetText("Category");

            var sessionToken = request.RetrieveSessionToken();
            velocityContext["forceBuildMessage"] = ForceBuildIfNecessary(request.Request, sessionToken);

            var gridRows = this.projectGrid.GenerateProjectGridRows(projectStatus.StatusAndServerList, BaseActionName,
                                                                    ProjectGridSortColumn.Category, true,
                                                                    category, urlBuilder, this.translations);

            var categories = new SortedDictionary<string, CategoryInformation>();

            foreach (var row in gridRows.OrderBy( s=> s.ServerName).ThenBy( p => p.Name))
            {
                var rowCategory = row.Category;
                CategoryInformation categoryRows;
                if (!categories.TryGetValue(rowCategory, out categoryRows))
                {
                    categoryRows = new CategoryInformation(rowCategory);
                    categories.Add(rowCategory, categoryRows);
                }

                categoryRows.AddRow(row);
            }

            // there is a category specified via a link, so expand that category by default
            // it's annoying to specify a category and still have to press the show link
            if (!string.IsNullOrEmpty(category))
            {
                    categories[category].Display = true;
            }



            velocityContext["categories"] = categories.Values;

            return viewGenerator.GenerateView("CategorizedFarmReport.vm", velocityContext);
        }

        public class CategoryInformation
        {
            public string Name { get; set; }
            public IList<ProjectGridRow> Rows { get; set; }
            public string CategoryColor { get; set; }
            public bool Display { get; set; }

            public CategoryInformation(string name)
            {
                this.Name = name;
                this.Rows = new List<ProjectGridRow>();
                this.CategoryColor = Color.Green.Name;
                this.Display = false;
            }

            public void AddRow(ProjectGridRow row)
            {
                this.Rows.Add(row);
                if (row.BuildStatusHtmlColor == Color.Red.Name)
                {
                    this.CategoryColor = Color.Red.Name;
                    this.Display = true;
                }
                else if (row.BuildStatusHtmlColor == Color.Blue.Name
                         && this.CategoryColor != Color.Red.Name)
                {
                    this.CategoryColor = Color.Blue.Name;
                }
            }
        }

        private string ForceBuildIfNecessary(IRequest request, string sessionToken)
        {
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
