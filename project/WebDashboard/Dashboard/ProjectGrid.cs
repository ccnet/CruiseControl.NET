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
                //string dir = @"c:\heartbeat\";
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

                List<DataGridRow> lastFiveDataGridRows = getLastFiveDataGridRows(serverSpecifier, projectSpecifier, dir, farmService, status);
                int queuePosition = getQueuePosition(status, projectSpecifier);

                rows.Add(
                    new ProjectGridRow(status,
                                       serverSpecifier,
                                       parameters.UrlBuilder.BuildProjectUrl(ProjectReportProjectPlugin.ACTION_NAME, projectSpecifier),
                                       parameters.UrlBuilder.BuildProjectUrl(ProjectParametersAction.ActionName, projectSpecifier),
                                       statistics,
                                       lastFiveDataGridRows,
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

        private List<DataGridRow> getLastFiveDataGridRows(IServerSpecifier serverSpecifier, DefaultProjectSpecifier projectSpecifier, string dir, IFarmService farmService, ProjectStatus status)
        {
            var lastFiveDataList = new List<DataGridRow>();
            IBuildSpecifier[] mostRecentBuildSpecifiers = farmService.GetMostRecentBuildSpecifiers(projectSpecifier, 5, BuildReportBuildPlugin.ACTION_NAME);
            if (Directory.Exists(dir))
            {
                foreach (IBuildSpecifier buildSpecifier in mostRecentBuildSpecifiers)
                {
                    lastFiveDataList.Add(getBuildData(serverSpecifier, projectSpecifier, dir, status, buildSpecifier)); 
                }
            }
            return lastFiveDataList;
        }

        private DataGridRow getBuildData(IServerSpecifier serverSpecifier, DefaultProjectSpecifier projectSpecifier, string dir, ProjectStatus status, IBuildSpecifier buildSpecifier)
        {
            DataGridRow dataToReturn;
            string localserver = new AppSettingsReader().GetValue("servername", typeof(System.String)).ToString();

            var file = String.Format(dir + @"{0}\ccnet\{1}\Artifacts\buildlogs\{2}", localserver, projectSpecifier.ProjectName,buildSpecifier.BuildName);
            try
            {
                var doc = XDocument.Load(file);
                string lastStatus = "";
                string lastDate = "";
                string lastRunningTime = "";
                string lastLink = "";
                var tests = doc.Descendants("CCNetIntegrationStatus");
                foreach (var test in tests)
                {
                    lastStatus = test.Value;
                }

                var elemList = doc.Descendants("build");
                foreach (var node in elemList)
                {
                    lastDate = (string)node.Attribute("date");

                    lastRunningTime = (string)node.Attribute("buildtime");
                    if (status.BuildStatus.ToString() == "Success")
                    {
                        lastLink = String.Format("http://{0}/ccnet/server/{0}/project/{1}/build/{2}/ViewBuildReport.aspx", serverSpecifier.ServerName, projectSpecifier.ProjectName, buildSpecifier.BuildName);
                    }
                    else
                    {
                        lastLink = "";
                    }
                }
                DataGridRow aux = new DataGridRow(lastStatus, lastDate, lastRunningTime, lastLink);
                dataToReturn = aux;
            }
            catch (Exception e)
            {
                throw new System.ArgumentException("File not found or corrupted. Error: " + e, "Argument");
            }
            return dataToReturn;
        }

        private int getQueuePosition(ProjectStatus status, DefaultProjectSpecifier projectSpecifier)
        {
            if(status.Activity.ToString().Equals("Pending"))
            {
                return getPositionInQueueList(status, projectSpecifier);  
            }
            else
            {
                return -1;
            }
        }

        private int getPositionInQueueList(ProjectStatus status, DefaultProjectSpecifier projectSpecifier)
        {
            int positionInQueue;
            QueueSnapshot queueSnapshot = new QueueSnapshot(status.Queue);
            var queueList = queueSnapshot.Requests;
            int count = 1; //Position 0 would be the building project
            foreach (QueuedRequestSnapshot queueRequestSnapshot in queueList)
            {
                if (projectSpecifier.ProjectName == queueRequestSnapshot.ProjectName)
                {
                    positionInQueue = count;
                    return positionInQueue;
                }
                count++;
            }
            return -1;
        }
    }
}
