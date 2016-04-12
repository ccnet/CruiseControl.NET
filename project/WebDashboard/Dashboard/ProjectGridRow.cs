using System;
using System.Collections.Generic;
using System.Drawing;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.Resources;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
    public class ProjectGridRow
    {
        private readonly ProjectStatus status;
        private readonly IServerSpecifier serverSpecifier;
        private readonly string url;
        private readonly string parametersUrl;
        private readonly string statistics;
        private readonly List<DataGridRow> lastFiveData;
        private readonly int queuePosition;
        private Translations translations;

        public ProjectGridRow(ProjectStatus status, IServerSpecifier serverSpecifier,
            string url, string parametersUrl, Translations translations)
        {
            this.status = status;
            this.serverSpecifier = serverSpecifier;
            this.url = url;
            this.parametersUrl = parametersUrl;
        }

        public ProjectGridRow(ProjectStatus status, IServerSpecifier serverSpecifier,
            string url, string parametersUrl, string statistics, List<DataGridRow> lastFiveData, int queuePosition, Translations translations)
            : this(status, serverSpecifier, url, parametersUrl, translations)
        {
            this.statistics = statistics;
            this.lastFiveData = lastFiveData;
            this.queuePosition = queuePosition;
        }

        public string Name { get { return status.Name; } private set; }

        public string ServerName { get { return serverSpecifier.ServerName; } private set; }

        public string Category { get { return status.Category; } private set; }

        public string BuildStatus { get { return status.BuildStatus.ToString(); } private set; }

        public List<DataGridRow> LastFiveData { get { return lastFiveData; } private set; }

        public string BuildStatusHtmlColor { get { return CalculateHtmlColor(status.BuildStatus); } private set; }

        public string LastBuildDate { get { return DateUtil.FormatDate(status.LastBuildDate); } private set; }

        public string LastBuildLabel { get { return (status.LastBuildLabel != null ? status.LastBuildLabel : "no build available"); } private set; }

        public string Status { get { return status.Status.ToString(); } private set; }

        public string Activity { get { return status.Activity.ToString(); } private set; }

        public string CurrentMessage { get { return status.CurrentMessage; } private set; }

        public string Breakers { get { return GetMessageText(Message.MessageKind.Breakers); } private set; }

        public string FailingTasks { get { return GetMessageText(Message.MessageKind.FailingTasks); } private set; }

        public string Fixer { get { return GetMessageText(Message.MessageKind.Fixer); } private set; }

        public string Url { get { return url; } private set; }

        public string Queue { get { return status.Queue; } private set; }

        public int QueuePriority { get { return status.QueuePriority; } private set; }

        public int QueuePosition { get { return queuePosition; } private set; }

        public string StartStopButtonName { get { return (status.Status == ProjectIntegratorState.Running) ? "StopBuild" : "StartBuild"; } private set; }

        public string StartStopButtonValue { get { return (status.Status == ProjectIntegratorState.Running) ? "Stop" : "Start"; } private set; }

        public string ForceAbortBuildButtonName { get { return (status.Activity != ProjectActivity.Building) ? "ForceBuild" : "AbortBuild"; } private set; }

        public string ForceAbortBuildButtonValue { get { return (status.Activity != ProjectActivity.Building) ? "Force" : "Abort"; } private set; }

        public bool AllowForceBuild { get { return serverSpecifier.AllowForceBuild && status.ShowForceBuildButton; } private set; }

        public bool AllowStartStopBuild { get { return serverSpecifier.AllowStartStopBuild && status.ShowStartStopButton; } private set; }

        public string Statistics { get { return this.statistics; } private set; }

        public string ParametersUrl { get { return parametersUrl; } private set; }

        public string Description
        {
            get
            {
                if (String.IsNullOrEmpty(status.Description)) return "";

                return status.Description;
            }

            private set;
        }

        public string NextBuildTime
        {
            get
            {
                if (status.NextBuildTime == System.DateTime.MaxValue)
                {
                    return "Force Build Only";
                }
                else
                {
                    return DateUtil.FormatDate(status.NextBuildTime);
                }
            }

            private set;
        }

        public string BuildStage
        {
            get
            {
                string CurrentBuildStage = status.BuildStage;

                if (CurrentBuildStage.Length == 0)
                {
                    return string.Empty;
                }
                else
                {
                    return CurrentBuildStage;
                }
            }
            private set;
        }

        private string CalculateHtmlColor(IntegrationStatus integrationStatus)
        {
            if (integrationStatus == IntegrationStatus.Success)
            {
                return Color.Green.Name;
            }
            else if (integrationStatus == IntegrationStatus.Unknown)
            {
                return Color.Blue.Name;
            }
            else
            {
                return Color.Red.Name;
            }
        }

        private string GetMessageText(Message.MessageKind messageType)
        {
            foreach (Message m in status.Messages)
            {
                if (m.Kind == messageType)
                {
                    return m.Text;
                }
            }
            return string.Empty;
        }
    }
}
