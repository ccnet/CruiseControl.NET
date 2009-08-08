using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
	public interface ICCTrayMultiConfiguration
	{
		ISingleServerMonitor[] GetServerMonitors();
		IProjectMonitor[] GetProjectStatusMonitors(ISingleServerMonitor[] serverMonitors);
		ICruiseServerManagerFactory CruiseServerManagerFactory { get; }
		ICruiseProjectManagerFactory CruiseProjectManagerFactory { get; }
		BuildServer[] GetUniqueBuildServerList();

		CCTrayProject[] Projects { get; set; }
		bool ShouldShowBalloonOnBuildTransition { get; set; }
		int PollPeriodSeconds { get; set; }
		AudioFiles Audio { get; }
		TrayIconDoubleClickAction TrayIconDoubleClickAction { get; set; }
		BalloonMessages BalloonMessages { get; }
        NotifyInfoFlags MinimumNotificationLevel { get; set; }
		Icons Icons { get; }
		X10Configuration X10 { get; }
		SpeechConfiguration Speech { get; }
		GrowlConfiguration Growl { get; }

        bool AlwaysOnTop { get; set; }
		bool ShowInTaskbar { get; set; }
        /// <summary>
        /// Report any changes to the projects.
        /// </summary>
        bool ReportProjectChanges { get; set; }
        string FixUserName { get; set; }

        void Reload();
        void Persist();
        /// <summary>
        /// Load the settings from a different location.
        /// </summary>
        /// <param name="settingsFile"></param>
        void Load(string settingsFile);
        /// <summary>
        /// Save the settings to a different location.
        /// </summary>
        /// <param name="settingsFile"></param>
        void Save(string settingsFile);

		ICCTrayMultiConfiguration Clone();
	}
}
