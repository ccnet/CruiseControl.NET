using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport;
using ThoughtWorks.CruiseControl.WebDashboard.Resources;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
    public class ProjectGrid : IProjectGrid
    {
        public ProjectGridRow[] GenerateProjectGridRows(ProjectGridParameters parameters)
        {
            var rows = new List<ProjectGridRow>();
            var farmService = parametes.FarmService;
            var appSettings = ConfigurationManager.AppSettings;
            string disk = appSettings["disk"];
            string server = appSettings["server"]; 
            foreach (ProjectStatusOnServer statusOnServer in parameters.StatusList)
            {
                ProjectStatus status = statusOnServer.ProjectStatus;
                IServerSpecifier serverSpecifier = statusOnServer.ServerSpecifier;
                DefaultProjectSpecifier projectSpecifier = new DefaultProjectSpecifier(serverSpecifier, status.Name);

                if ((parametrs.CategoryFilter != string.Empty) && (parametrs.CategoryFilter != status.Category))
                    continue;

                string dir = disk + server;
                string statistics = "<!-- UNABLE TO FIND HEARTBEAT FOLDER -->";

                if (Directory.Exists(dir))
                {
                    var file = String.Format(dir + @"{0}\ccnet\{1}\TestResults.html", serverSpecifier.ServerName, projectSpecifier.ProjectName);
                    try
                    {
                        statistics = File.ReadAllText(file);
                    }
                    catch (Exception e)
                    {
                        statistics = String.Format("<!-- UNABLE TO GET STATISTICS: {0} -->", e.Message.Replace("-->", "- - >"));
                    }
                }

                List<DataGridrow> lastFiveDataGridRows = getLastFiveDataGridRows(serverSpecifier, projectSpecifier, dir, farmService);
                if(lastFiveDataGirRows.count == 0)
                {
                    MessageBox.Show("Data of the last 5 builds is missing!", "Error finding data", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                int queuePosition = getQueuePosition(status);

                rows.Add(
                    new ProjectGridRow(status,
                                       serverSpecifier,
                                       parameters.UrlBuilder.BuildProjectUrl(ProjectReportProjectPlugin.ACTION_NAME, projectSpecifier),
                                       parameters.UrlBuilder.BuildProjectUrl(ProjectParametersAction.ActionName, projectSpecifier),
                                       statistics,
                                       lastFiveDataGridRows.ToArray,
                                       queuePosition,
                                       parameters.Translation));
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

        private List<DataGridRow> getLastFiveDataGridRows(IServerSpecifier serverSpecifier, DefaultProjectSpecifier projectSpecifier, string dir, IFarmService farmService)
        {
            var lastFiveDataList = new List<DataGridRow>();
            IBuildSpecifier[] mostRecentBuildSpecifiers = farmService.GetMostRecentBuildSpecifiers(projectSpecifier, 5, BuildReportBuildPlugin.ACTION_NAME);
            if (Directory.Exists(dir))
            {
                foreach (IBuildSpecifier buildSpecifier in mostRecentBuildSpecifiers)
                {
                    lastFiveDataList = getBuildData(serverSpecifier, projectSpecifier, dir); 
                }
            }
            return lastFiveDataList;
        }

        private List<string> getBuildData(IServerSpecifier serverSpecifier, DefaultProjectSpecifier projectSpecifier, string dir)
        {
            var dataToReturn = new List<string>();
            var file = String.Format(dir + @"{0}\ccnet\{1}\Artifacts\buildlogs\{2}", serverSpecifier.ServerName, projectSpecifier.ProjectName, buildSpecifier.BuildName);
            try
            {
                var doc = XDocument.Load(file);
                string lastStatus;
                string lastDate;
                string lastRunningTime;
                string lastLink;

                lastStatus = doc.Root.Elements().Select(x => x.Element("CCNetIntegrationStatus"));
                XmlNodeList elemList = doc.GetElementsByTagName("build");

                foreach (XmlNode node in elemList)
                {
                    lastDate = node.Attributes["date"].Value;
                    lastRunningTime = node.Attributes["buildtime"].Value;
                    if (state.Equals("Unknown"))
                    {
                        lastLink = "";
                    }
                    else
                    {
                        var lastlink = String.Format("http://{0}/ccnet/server/{0}/project/{1}/build/{2}/ViewBuildReport.aspx", serverSpecifier.ServerName, projectSpecifier.ProjectName, buildSpecifier.BuildName);
                    }
                }

                dataToReturn.Add(new DataGridRow(lastStatus, lastDate, lastRunningTime, lastLink));
                return dataToReturn;
            }
            catch (Exception e)
            {
                throw new System.ArgumentException("File not found or corrupted. Error: " + e, "Argument");
            }
        }

        private int getQueuePosition(ProjectStatus status)
        {
            if(status.Activity.ToString().equals("Pending"))
            {
                return getPositionInQueueList();  
            }
            else
            {
                return -1;
            }
        }

        private int getPositionInQueueList()
        {
            int positionInQueue;
            QueueSnapshot queueSnapshot = new QueueSnapshot(status.Queue);
            var queueList = queueSnapshot.Request;
            int count = 1; //Position 0 would be the building project
            foreach (QueuedRequestSnapshot queueRequestSnapshot in queueList)
            {
                if (projectSpecifier.ProjectName == queueRequestSnapshot.ProjectName)
                {
                    positionInQueue = count;
                }
                count++;
            }
            return positionInQueue;
        }
    }
}
