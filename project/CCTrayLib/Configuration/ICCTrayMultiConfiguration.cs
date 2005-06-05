using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
	public interface ICCTrayMultiConfiguration
	{
		IProjectMonitor[] GetProjectStatusMonitors();
		Project[] Projects { get; }
		bool ShouldShowBalloonOnBuildTransition { get; }
	}
}