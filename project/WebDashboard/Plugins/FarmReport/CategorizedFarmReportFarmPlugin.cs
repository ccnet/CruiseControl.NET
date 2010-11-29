using System.Collections;
using System.Collections.Generic;
using System.Drawing;

using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Resources;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.FarmReport
{
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
        public string LinkDescription
        {
            get { return "Categorized Farm Report"; }
        }

        public IResponse Execute(ICruiseRequest request)
        {
            var velocityContext = new Hashtable();
            this.translations = Translations.RetrieveCurrent();

            var projectStatus = farmService.GetProjectStatusListAndCaptureExceptions(request.RetrieveSessionToken());
            var urlBuilder = request.UrlBuilder;
            var category = request.Request.GetText("Category");

            var gridRows = this.projectGrid.GenerateProjectGridRows(projectStatus.StatusAndServerList, BaseActionName, 
                                                                    ProjectGridSortColumn.Category, true, 
                                                                    category, urlBuilder,this.translations);

            var categories = new SortedDictionary<string, CategoryInformation>();
           
            foreach (var row in gridRows)
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

            velocityContext["categories"] = categories.Values;          

            return viewGenerator.GenerateView("CategorizedFarmReport.vm", velocityContext);
        }

        private class CategoryInformation
        {
            private string Name { get; set; }
            private IList<ProjectGridRow> Rows { get; set; }
            private string CategoryColor { get; set; }
            private bool Display { get; set; }

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
    }
}
