using ThoughtWorks.CruiseControl.Remote;

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
            get { return snapshotAndServerList; }
        }

        public CruiseServerException[] Exceptions
        {
            get { return exceptions; }
        }

        public CruiseServerSnapshot[] Snapshots
        {
            get
            {
                CruiseServerSnapshot[] snapshots = new CruiseServerSnapshot[snapshotAndServerList.Length];
                for (int i = 0; i < snapshots.Length; i++)
                {
                    snapshots[i] = snapshotAndServerList[i].CruiseServerSnapshot;
                }
                return snapshots;
            }
        }
    }
}