using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib
{
	/// <summary>
	/// The tooltip text on the CCTray 
	/// </summary>
	public class TrayTooltip
	{
		private readonly ProjectStatus status;
		private const string FORMAT_TRAY_TOOLTIP = "Server: {0}\nProject: {1}\nLast Build: {2} ({3})";

		public TrayTooltip(ProjectStatus status)
		{
			this.status = status;
		}

		public string Text
		{
			get { return CalculateTrayText(); }
		}

		private string CalculateTrayText()
		{
			object activity = (status.Status == ProjectIntegratorState.Stopped) ? ProjectActivity.Sleeping : status.Activity;

			return string.Format(FORMAT_TRAY_TOOLTIP,
				activity,
				status.Name,
				status.BuildStatus,
				status.LastBuildLabel);
		}

	}

}