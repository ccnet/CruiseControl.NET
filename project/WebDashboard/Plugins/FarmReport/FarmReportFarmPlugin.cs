namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.FarmReport
{
    using System;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
    using ThoughtWorks.CruiseControl.WebDashboard.IO;
    using ThoughtWorks.CruiseControl.WebDashboard.MVC;
    using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

    /// <summary>
    /// The main page of CCNet. This gives an overview of all projects of all build servers.
    /// <para>
    /// LinkDescription : Farm Report.
    /// </para>
    /// </summary>
    /// <title>Farm Report Farm Plugin</title>
    /// <version>1.0.0</version>
    [ReflectorType("farmReportFarmPlugin")]
    public class FarmReportFarmPlugin : ICruiseAction, IPlugin
    {
        public static readonly string ACTION_NAME = "ViewFarmReport";

        private readonly IProjectGridAction projectGridAction;
        private readonly ProjectParametersAction parametersAction;
        private ProjectGridSortColumn? sortColumn;
        private readonly CategorizedFarmReportFarmPlugin categorizedView;

        #region Public properties
        #region DefaultSortColumn
        /// <summary>
        /// The default column to sort by.
        /// </summary>
        /// <default>N/A</default>
        /// <version>1.4.4</version>
        [ReflectorProperty("defaultSort", Required = false)]
        public string DefaultSortColumn
        {
            get { return sortColumn.GetValueOrDefault(ProjectGridSortColumn.Name).ToString(); }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    sortColumn = null;
                }
                else
                {
                    sortColumn = (ProjectGridSortColumn)Enum.Parse(typeof(ProjectGridSortColumn), value);
                }
            }
        }
        #endregion

        #region SuccessIndicatorBarLocation
        /// <summary>
        /// The location of the success indicator bar.
        /// </summary>
        /// <value>The success indicator bar location.</value>
        /// <default>Bottom</default>
        [ReflectorProperty("successBar", Required = false)]
        public IndicatorBarLocation SuccessIndicatorBarLocation { get; set; }
        #endregion

        #region UseCategories
        /// <summary>
        /// Displays the categoried farm view or not.
        /// </summary>
        [ReflectorProperty("categories", Required = false)]
        public bool UseCategories { get; set; }
        #endregion


        /// <summary>
        /// Amount in seconds to autorefresh
        /// </summary>
        /// <default>0 - no refresh</default>
        /// <version>1.7</version>
        [ReflectorProperty("refreshInterval", Required = false)]
        public Int32 RefreshInterval { get; set; }

        #endregion

        public FarmReportFarmPlugin(
            IProjectGridAction projectGridAction,
            ProjectParametersAction parametersAction,
            CategorizedFarmReportFarmPlugin categorizedView)
        {
            this.projectGridAction = projectGridAction;
            this.parametersAction = parametersAction;
            this.SuccessIndicatorBarLocation = IndicatorBarLocation.Bottom;
            this.categorizedView = categorizedView;
        }

        public IResponse Execute(ICruiseRequest request)
        {
            request.Request.RefreshInterval = RefreshInterval;

            if (sortColumn.HasValue) projectGridAction.DefaultSortColumn = sortColumn.Value;
            this.projectGridAction.SuccessIndicatorBarLocation = this.SuccessIndicatorBarLocation;
            return projectGridAction.Execute(ACTION_NAME, request);
        }

        public string LinkDescription
        {
            get { return "Farm Report"; }
        }

        public INamedAction[] NamedActions
        {
            get
            {
                var action = this.UseCategories ?
                    (ICruiseAction)categorizedView :
                    (ICruiseAction)this;
                return new INamedAction[] { new ImmutableNamedAction(ACTION_NAME, action),
                        new ImmutableNamedActionWithoutSiteTemplate(ProjectParametersAction.ActionName, parametersAction)
                    };
            }
        }
    }
}
