using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
	public interface ICCTrayMultiConfiguration
	{
		IProjectMonitor[] GetProjectStatusMonitors();
		ICruiseProjectManagerFactory CruiseProjectManagerFactory { get; }

		CCTrayProject[] Projects { get; set; }
		bool ShouldShowBalloonOnBuildTransition { get; set; }
		int PollPeriodSeconds { get; set; }
		AudioFiles Audio { get; }
		TrayIconDoubleClickAction TrayIconDoubleClickAction { get; set; }
		BalloonMessages BalloonMessages { get; }
		Icons Icons { get; }
		X10Configuration X10 { get; }

		void Reload();
		void Persist();

		ICCTrayMultiConfiguration Clone();
	}
}