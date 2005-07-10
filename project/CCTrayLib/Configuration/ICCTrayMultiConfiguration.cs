using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
	public interface ICCTrayMultiConfiguration
	{
		IProjectMonitor[] GetProjectStatusMonitors();
		Project[] Projects { get; set; }
		bool ShouldShowBalloonOnBuildTransition { get; set; }
		
		void Reload();
		ICCTrayMultiConfiguration Clone();
		void Persist();
	}
}