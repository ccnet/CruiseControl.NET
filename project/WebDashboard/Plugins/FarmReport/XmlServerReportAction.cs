using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.FarmReport
{
    public class XmlServerReportAction : IAction
    {
        public const string ACTION_NAME = "XmlServerReport";

        private readonly IFarmService farmService;

        public XmlServerReportAction(IFarmService farmService)
        {
            this.farmService = farmService;
        }

        public IResponse Execute(IRequest request)
        {
            CruiseServerSnapshotListAndExceptions allCruiseServerSnapshots = farmService.GetCruiseServerSnapshotListAndExceptions(null);
            CruiseServerSnapshot[] cruiseServerSnapshots = allCruiseServerSnapshots.Snapshots;

            return new XmlFragmentResponse(new CruiseXmlWriter().Write(cruiseServerSnapshots));
        }
    }
}