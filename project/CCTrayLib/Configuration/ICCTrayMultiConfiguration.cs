using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
	public interface ICCTrayMultiConfiguration
	{
		IProjectMonitor[] GetProjectStatusMonitors();

		Project[] Projects { get; set; }
		bool ShouldShowBalloonOnBuildTransition { get; set; }
		int PollPeriodSeconds { get; set; }
		AudioFiles Audio { get; }
		TrayIconDoubleClickAction TrayIconDoubleClickAction { get; set; }
		BalloonMessages BalloonMessages { get; }
		Icons Icons { get; }

		void Reload();
		void Persist();

		ICCTrayMultiConfiguration Clone();
	}
}