using System;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib
{
	/// <summary>
	/// The tooltip text on the CCTray 
	/// </summary>
	public class TrayTooltip
	{
		private const string FORMAT_TRAY_TOOLTIP = "Server: {0}\nProject: {1}\nLast Build: {2} ({3}) \nNext Build in {4}";

		private readonly ProjectStatus status;
		private DateTimeProvider dtProvider;

		public TrayTooltip(ProjectStatus status) : this(status, new DateTimeProvider())
		{
		}

		public TrayTooltip(ProjectStatus status, DateTimeProvider dateTimeProvider)
		{
			this.status = status;
			dtProvider = dateTimeProvider;
		}

		public string Text
		{
			get { return CalculateTrayText(); }
		}

		private string CalculateTrayText()
		{
			object activity = (status.Status == ProjectIntegratorState.Stopped) ? ProjectActivity.Sleeping : status.Activity;
			TimeSpan timeRemaining = status.NextBuildTime.Subtract(dtProvider.Now);
			string formattedNextBuildTime = String.Format("{0} Day(s), {1} Hour(s), {2} Minute(s)", 
				timeRemaining.Days, timeRemaining.Hours, timeRemaining.Minutes);
			return string.Format(FORMAT_TRAY_TOOLTIP,
			                     activity,
			                     status.Name,
			                     status.BuildStatus,
			                     status.LastBuildLabel,
			                     formattedNextBuildTime);
		}
	}
}