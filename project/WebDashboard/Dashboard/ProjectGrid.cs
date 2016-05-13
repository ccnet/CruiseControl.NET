using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport;
using ThoughtWorks.CruiseControl.WebDashboard.Resources;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using ThoughtWorks.CruiseControl.Core.Queues;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
    public class ProjectGrid : IProjectGrid
    {
        public ProjectGridRow[] GenerateProjectGridRows(ProjectGridParameters parameters)
        {
            var rows = new List<ProjectGridRow>();
            var farmService = parameters.FarmService;
            string disk = new AppSettingsReader().GetValue("disk", typeof(System.String)).ToString();
            string server = new AppSettingsReader().GetValue("framework", typeof(System.String)).ToString();
           

            foreach (ProjectStatusOnServer statusOnServer in parameters.StatusList)
            {
                ProjectStatus status = statusOnServer.ProjectStatus;
                IServerSpecifier serverSpecifier = statusOnServer.ServerSpecifier;
                DefaultProjectSpecifier projectSpecifier = new DefaultProjectSpecifier(serverSpecifier, status.Name);

                if ((parameters.CategoryFilter != string.Empty) && (parameters.CategoryFilter != status.Category))
                    continue;

                string dir = disk + server + @"\";
                string statistics = "<!-- UNABLE TO FIND HEARTBEAT FOLDER -->";
                string runningTime = String.Empty;

                if (Directory.Exists(dir))
                {
                    var file = String.Format(dir + @"{0}\ccnet\{1}\TestResults.html", serverSpecifier.ServerName, projectSpecifier.ProjectName);
                    var fileRunningTime = String.Format(dir + @"{0}\ccnet\{1}\RunningTime.html", serverSpecifier.ServerName, projectSpecifier.ProjectName);
                    try
                    {
                        if(File.Exists(file))
                            statistics = File.ReadAllText(file);

                        if (File.Exists(fileRunningTime))
                            runningTime = File.ReadAllText(fileRunningTime);

                        if (!File.Exists(fileRunningTime) || runningTime.Length < 1)
                            runningTime = readRunningTimeIfFileEmpty(farmService, projectSpecifier, serverSpecifier, dir); 
                    }
                    catch (Exception e)
                    {
                        statistics = String.Format("<!-- UNABLE TO GET STATISTICS: {0} -->", e.Message.Replace("-->", "- - >"));
                        runningTime = String.Format("<!-- UNABLE TO GET RUNNING TIME: {0} -->", e.Message.Replace("-->", "- - >"));
                    }
                }

                List<DataGridRow> lastFiveDataGridRows = getLastFiveDataGridRows(serverSpecifier, projectSpecifier, dir, farmService, status);
                int queuePosition = getQueuePosition(status, serverSpecifier, projectSpecifier, farmService);
                ProjectGridRow test = new ProjectGridRow(status,
                                       serverSpecifier,
                                       parameters.UrlBuilder.BuildProjectUrl(ProjectReportProjectPlugin.ACTION_NAME, projectSpecifier),
                                       parameters.UrlBuilder.BuildProjectUrl(ProjectParametersAction.ActionName, projectSpecifier),
                                       statistics,
                                       runningTime,
                                       lastFiveDataGridRows,
                                       queuePosition,
                                       parameters.Translation);
                rows.Add(test);
            }

            rows.Sort(GetComparer(parameters.SortColumn, parameters.SortIsAscending));
            return rows.ToArray();
        }

        private IComparer<ProjectGridRow> GetComparer(ProjectGridSortColumn column, bool ascending)
        {
            return new ProjectGridRowComparer(column, ascending);
        }

        private class ProjectGridRowComparer
            : IComparer<ProjectGridRow>
        {
            private readonly ProjectGridSortColumn column;
            private readonly bool ascending;

            public ProjectGridRowComparer(ProjectGridSortColumn column, bool ascending)
            {
                this.column = column;
                this.ascending = ascending;
            }

            public int Compare(ProjectGridRow x, ProjectGridRow y)
            {
                if (column == ProjectGridSortColumn.Name)
                {
                    return x.Name.CompareTo(y.Name) * (ascending ? 1 : -1);
                }
                else if (column == ProjectGridSortColumn.LastBuildDate)
                {
                    return x.LastBuildDate.CompareTo(y.LastBuildDate) * (ascending ? 1 : -1);
                }
                else if (column == ProjectGridSortColumn.BuildStatus)
                {
                    return x.BuildStatus.CompareTo(y.BuildStatus) * (ascending ? 1 : -1);
                }
                else if (column == ProjectGridSortColumn.ServerName)
                {
                    return x.ServerName.CompareTo(y.ServerName) * (ascending ? 1 : -1);
                }
                else if (column == ProjectGridSortColumn.Category)
                {
                    return x.Category.CompareTo(y.Category) * (ascending ? 1 : -1);
                }
                else
                {
                    return 0;
                }
            }
        }

        private string readRunningTimeIfFileEmpty(IFarmService farmService, DefaultProjectSpecifier projectSpecifier, IServerSpecifier serverSpecifier, string dir)
        {
            IBuildSpecifier[] mostRecentBuildSpecifiers = farmService.GetMostRecentBuildSpecifiers(projectSpecifier, 1, BuildReportBuildPlugin.ACTION_NAME);
            var buildFile = String.Format(dir + @"{0}\ccnet\{1}\Artifacts\buildlogs\{2}", serverSpecifier.ServerName, projectSpecifier.ProjectName, mostRecentBuildSpecifiers[0].BuildName);
            var doc = XDocument.Load(buildFile);
            IEnumerable<XElement> elemList = doc.Descendants("build");
            if (Directory.Exists(buildFile))
            {
                foreach (var node in elemList)
                {
                    return (string)node.Attribute("buildtime");
                }
            }
            return String.Empty;
        }

        private List<DataGridRow> getLastFiveDataGridRows(IServerSpecifier serverSpecifier, DefaultProjectSpecifier projectSpecifier, string dir, IFarmService farmService, ProjectStatus status)
        {
            var lastFiveDataList = new List<DataGridRow>();
            int cont = 0;
            if (Directory.Exists(String.Format(dir + @"{0}\ccnet\{1}\Artifacts\buildlogs", serverSpecifier.ServerName, projectSpecifier.ProjectName)))
            {
                IBuildSpecifier[] mostRecentBuildSpecifiers = farmService.GetMostRecentBuildSpecifiers(projectSpecifier, 5, BuildReportBuildPlugin.ACTION_NAME);
                if (Directory.Exists(dir))
                {
                    foreach (IBuildSpecifier buildSpecifier in mostRecentBuildSpecifiers)
                    {
                        lastFiveDataList.Add(getBuildData(serverSpecifier, projectSpecifier, dir, status, buildSpecifier, cont));
                        cont = 1;
                    }
                }
                return lastFiveDataList;
            }
            lastFiveDataList.Add(new DataGridRow("Unknown", string.Empty, string.Empty));
            return lastFiveDataList;
        }

        private DataGridRow getBuildData(IServerSpecifier serverSpecifier, DefaultProjectSpecifier projectSpecifier, string dir, ProjectStatus status, IBuildSpecifier buildSpecifier, int cont)
        {
            DataGridRow dataToReturn;
            string buildName = buildSpecifier.BuildName;
            string lastStatus = "";
            string localserver = new AppSettingsReader().GetValue("servername", typeof(System.String)).ToString();
            if (cont == 0)
            {
                lastStatus = status.BuildStatus.ToString();
            }
            else
            {
                if (buildName.Contains("Lbuild"))
                { lastStatus = "Success"; }
                else
                { lastStatus = "Failure"; }
            }
            string lastDate = String.Format("{0}-{1}-{2} {3}:{4}:{5}", buildName.Substring(3, 4), buildName.Substring(7, 2), buildName.Substring(9, 2),
                                                                        buildName.Substring(11, 2), buildName.Substring(13, 2), buildName.Substring(15, 2));
            string lastLink = String.Format("http://{0}/ccnet/server/{0}/project/{1}/build/{2}/ViewBuildReport.aspx", 
                                                    serverSpecifier.ServerName, projectSpecifier.ProjectName, buildName);
            dataToReturn = new DataGridRow(lastStatus, lastDate, lastLink);
            return dataToReturn;
        }

        private int getQueuePosition(ProjectStatus status, IServerSpecifier serverSpecifier, DefaultProjectSpecifier projectSpecifier, IFarmService farmService)
        {
            if(status.Activity.ToString().Equals("Pending"))
            {
                return getPositionInQueueList(status, serverSpecifier, projectSpecifier, farmService);  
            }
            return -1; 
        }

        private int getPositionInQueueList(ProjectStatus status, IServerSpecifier serverSpecifier, DefaultProjectSpecifier projectSpecifier, IFarmService farmService)
        {
            CruiseServerSnapshotListAndExceptions snapshot = farmService.GetCruiseServerSnapshotListAndExceptions(serverSpecifier, string.Empty);
            List<QueueSnapshot> queues = new List<QueueSnapshot>();

            foreach (CruiseServerSnapshot cruiseServerSnapshot in snapshot.Snapshots)
            {
                QueueSetSnapshot queueSnapshot = cruiseServerSnapshot.QueueSetSnapshot;
                foreach (QueueSnapshot queueSnapshotItem in queueSnapshot.Queues) {
                    var index = checkPositionQueue(queueSnapshotItem, projectSpecifier);
                    if (index > -1) { return index; }
                }
            }
            return -1;
        }

        private int checkPositionQueue(QueueSnapshot queueSnapshotItem, DefaultProjectSpecifier projectSpecifier)
        {
            for (int i = 0; i<queueSnapshotItem.Requests.Count; i++)
            {
                if (queueSnapshotItem.Requests[i].ProjectName == projectSpecifier.ProjectName)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
