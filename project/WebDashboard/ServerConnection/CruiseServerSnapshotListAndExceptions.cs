
namespace ThoughtWorks.CruiseControl.WebDashboard.ServerConnection
{
    public class CruiseServerSnapshotListAndExceptions
    {
        private readonly CruiseServerSnapshotOnServer[] snapshotAndServerList;
        private readonly CruiseServerException[] exceptions;

        public CruiseServerSnapshotListAndExceptions(CruiseServerSnapshotOnServer[] snapshotAndServerList, CruiseServerException[] exceptions)
        {
            this.snapshotAndServerList = snapshotAndServerList;
            this.exceptions = exceptions;
        }

        public CruiseServerSnapshotOnServer[] SnapshotAndServerList
        {
            get { return this.snapshotAndServerList; }
        }

        public CruiseServerException[] Exceptions
        {
            get { return this.exceptions; }
        }
    }
}
