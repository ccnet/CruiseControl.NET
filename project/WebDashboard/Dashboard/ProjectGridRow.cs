using System;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class ProjectGridRow
	{
		private readonly string name;
		private readonly string serverName;
		private readonly string buildStatus;
		private readonly string buildStatusHtmlColor;
		private readonly DateTime lastBuildDate;
		private readonly string lastBuildLabel;
		private readonly string status;
		private readonly string activity;
		private readonly string url;

		public ProjectGridRow(string name, string serverName, string buildStatus, string buildStatusHtmlColor, DateTime lastBuildDate, string lastBuildLabel, string status, string activity, string url)
		{
			this.name = name;
			this.serverName = serverName;
			this.buildStatus = buildStatus;
			this.buildStatusHtmlColor = buildStatusHtmlColor;
			this.lastBuildDate = lastBuildDate;
			this.lastBuildLabel = lastBuildLabel;
			this.status = status;
			this.activity = activity;
			this.url = url;
		}

		public string Name
		{
			get { return name; }
		}

		public string ServerName
		{
			get { return serverName; }
		}

		public string BuildStatus
		{
			get { return buildStatus; }
		}

		public string BuildStatusHtmlColor
		{
			get { return buildStatusHtmlColor; }
		}

		public string LastBuildDate
		{
			get { return DateUtil.FormatDate(lastBuildDate); }
		}

		public string LastBuildLabel
		{
			get { return lastBuildLabel; }
		}

		public string Status
		{
			get { return status; }
		}

		public string Activity
		{
			get { return activity; }
		}

		public string Url
		{
			get { return url; }
		}
	}
}
