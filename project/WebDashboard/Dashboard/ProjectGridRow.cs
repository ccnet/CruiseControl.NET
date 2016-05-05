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
        private readonly string runningTime;
        private readonly List<DataGridRow> lastFiveData;
        private readonly int queuePosition;

        public ProjectGridRow(ProjectStatus status, IServerSpecifier serverSpecifier,
            string url, string parametersUrl, Translations translations)
        {
            this.status = status;
            this.serverSpecifier = serverSpecifier;
            this.url = url;
            this.parametersUrl = parametersUrl;
        }

        public ProjectGridRow(ProjectStatus status, IServerSpecifier serverSpecifier,
            string url, string parametersUrl, string statistics, string runningTime, List<DataGridRow> lastFiveData, int queuePosition, Translations translations)
            : this(status, serverSpecifier, url, parametersUrl, translations)
        {
            this.statistics = statistics;
            this.runningTime = runningTime;
            this.lastFiveData = lastFiveData;
            this.queuePosition = queuePosition;
        }

        public string Name { get { return status.Name; } }

        public string ServerName { get { return serverSpecifier.ServerName; } }

        public string Category { get { return status.Category; } }

        public string BuildStatus { get { return status.BuildStatus.ToString(); } }

        public List<DataGridRow> LastFiveData { get { return lastFiveData; } }

        public string BuildStatusHtmlColor { get { return CalculateHtmlColor(status.BuildStatus); } }

        public string LastBuildDate { get { return DateUtil.FormatDate(status.LastBuildDate); } }

        public string LastBuildLabel { get { return (status.LastBuildLabel != null ? status.LastBuildLabel : "no build available"); } }

        public string Status { get { return status.Status.ToString(); } }

        public string Activity { get { return status.Activity.ToString(); } }

        public string CurrentMessage { get { return status.CurrentMessage; } }

        public string Breakers { get { return GetMessageText(Message.MessageKind.Breakers); } }

        public string FailingTasks { get { return GetMessageText(Message.MessageKind.FailingTasks); } }

        public string Fixer { get { return GetMessageText(Message.MessageKind.Fixer); } }

        public string Url { get { return url; } }

        public string Queue { get { return status.Queue; } }

        public int QueuePriority { get { return status.QueuePriority; } }

        public int QueuePosition { get { return queuePosition; } }

        public string StartStopButtonName { get { return (status.Status == ProjectIntegratorState.Running) ? "StopBuild" : "StartBuild"; } }

        public string StartStopButtonValue { get { return (status.Status == ProjectIntegratorState.Running) ? "Stop" : "Start"; } }

        public string ForceAbortBuildButtonName { get { return (status.Activity != ProjectActivity.Building) ? "ForceBuild" : "AbortBuild"; } }

        public string ForceAbortBuildButtonValue { get { return (status.Activity != ProjectActivity.Building) ? "Force" : "Abort"; } }

        public string CancelPendingButtonName { get { return (status.Activity == ProjectActivity.Pending) ? "CancelPending" : string.Empty; } }

        public bool AllowForceBuild { get { return serverSpecifier.AllowForceBuild && status.ShowForceBuildButton; } }

        public bool AllowStartStopBuild { get { return serverSpecifier.AllowStartStopBuild && status.ShowStartStopButton; } }

        public string Statistics { get { return this.statistics; } }

        public string RunningTime { get { return this.runningTime; } }

        public string ParametersUrl { get { return parametersUrl; } }

        public string[] BreakersNames
        {
            get
            {
                string text = GetMessageText(Message.MessageKind.Breakers);
                text = System.Text.RegularExpressions.Regex.Replace(text, ",.", "/", System.Text.RegularExpressions.RegexOptions.None);
                string[] users = text.Split('/');
                return users;
            }
        }

        public string Description
        {
            get
            {
                if (String.IsNullOrEmpty(status.Description)) return "";

                return status.Description;
            }
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
