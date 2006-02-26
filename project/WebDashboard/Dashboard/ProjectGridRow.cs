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
			get { return this.serverSpecifier.AllowForceBuild; }
		}

		public bool AllowStartStopBuild
		{
			get { return this.serverSpecifier.AllowStartStopBuild; }
		}

		private string CalculateHtmlColor(IntegrationStatus status)
		{
			if (status == IntegrationStatus.Success)
			{
				return Color.Green.Name;
			}
			else if (status == IntegrationStatus.Unknown)
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
