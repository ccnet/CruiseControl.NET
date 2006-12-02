using System.Drawing;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class ProjectGridRow
	{
		private readonly ProjectStatus status;
		private readonly IServerSpecifier serverSpecifier;
		private readonly string url;

		public ProjectGridRow(ProjectStatus status, IServerSpecifier serverSpecifier, string url)
		{
			this.status = status;
			this.serverSpecifier = serverSpecifier;
			this.url = url;
		}

		public string Name
		{
			get { return status.Name; }
		}

		public string ServerName
		{
			get { return serverSpecifier.ServerName; }
		}

		public string Category
		{
			get { return status.Category; }
		}

		public string BuildStatus
		{
			get { return status.BuildStatus.ToString(); }
		}

		public string BuildStatusHtmlColor
		{
			get { return CalculateHtmlColor(status.BuildStatus); }
		}

		public string LastBuildDate
		{
			get { return DateUtil.FormatDate(status.LastBuildDate); }
		}

		public string LastBuildLabel
		{
			get { return (status.LastBuildLabel != null ? status.LastBuildLabel : "no build available"); }
		}

		public string Status
		{
			get { return status.Status.ToString(); }
		}

		public string Activity
		{
			get { return status.Activity.ToString(); }
		}

		public string CurrentMessage
		{
			get { return status.CurrentMessage; }
		}

		public string Url
		{
			get { return url; }
		}

		public string StartStopButtonName
		{
			get { return (status.Status == ProjectIntegratorState.Running) ? "StopBuild" : "StartBuild"; }
		}

		public string StartStopButtonValue
		{
			get { return (status.Status == ProjectIntegratorState.Running) ? "Stop" : "Start"; }
		}

		public bool AllowForceBuild
		{
			get { return serverSpecifier.AllowForceBuild; }
		}

		public bool AllowStartStopBuild
		{
			get { return serverSpecifier.AllowStartStopBuild; }
		}

		private string CalculateHtmlColor(IntegrationStatus integrationStatus)
		{
			if (integrationStatus == IntegrationStatus.Success)
			{
				return Color.Green.Name;
			}
			else if (integrationStatus == IntegrationStatus.Unknown)
			{
				return Color.Yellow.Name;
			}
			else
			{
				return Color.Red.Name;
			}
		}
	}
}