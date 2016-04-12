using System;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.Resources;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
    public class ProjectGridParameters
    {
        private readonly ProjectStatusOnServer[] statusList;
        private readonly ProjectGridSortColumn sortColumn;
        private readonly bool sortIsAscending;
        private readonly string categoryFilter;
        private readonly ICruiseUrlBuilder urlBuilder;
        private readonly IFarmService farmService;
        private readonly Translations translation;

        public ProjectGridParameters(ProjectStatusOnServer[] statusList, ProjectGridSortColumn sortColumn, bool sortIsAscending, string categoryFilter,
                                                        ICruiseUrlBuilder urlBuilder, IFarmService farmService, Translations translation)
        {
            this.statusList = statusList;
            this.sortColumn = sortColumn;
            this.sortIsAscending = sortIsAscending;
            this.categoryFilter = categoryFilter;
            this.urlBuilder = urlBuilder;
            this.farmService = farmService;
            this.translation = translation;
        }

        public ProjectStatusOnServer[] StatusList { get { return statusList; } }

        public ProjectGridSortColumn SortColumn { get { return sortColumn; } }

        public bool SortIsAscending { get { return sortIsAscending; } }

        public string CategoryFilter { get { return categoryFilter; } }

        public ICruiseUrlBuilder UrlBuilder { get { return urlBuilder; } }

        public IFarmService FarmService { get { return farmService; } }

        public Translations Translation { get { return translation; } }
    }
}
