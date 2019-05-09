using System;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.Resources;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
    public class DataGridRow
    {
        private readonly string buildStatus;
        private readonly string date;
        private readonly string runningTime;
        private readonly string link;

        public DataGridRow(string buildStatus, string date, string runningTime, string link)
        {
            this.buildStatus = buildStatus;
            this.date = date;
            this.runningTime = runningTime;
            this.link = link;
        }

        public string BuildStatus
        {
            get { return buildStatus; }
        }

        public string Date
        {
            get { return date; }
        }

        public string RunningTime
        {
            get { return runningTime; }
        }

        public string Link
        {
            get { return link; }
        }
    }
}
