using System;
using System.Diagnostics;
using System.Collections;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;
using SpeechLib;


namespace ThoughtWorks.CruiseControl.CCTrayLib.Speech
{
	/// <summary>
	/// Say things when build events happen.
	/// </summary>
	public class SpeakingProjectMonitor
	{
		private IProjectMonitor monitor;
		private IBalloonMessageProvider balloonMessageProvider;
        private SpVoice voice = new SpVoice();
		private IDictionary projectStates = new Hashtable();

        public SpeakingProjectMonitor (IProjectMonitor monitor, IBalloonMessageProvider balloonMessageProvider, SpeechConfiguration configuration) {
        	this.monitor = monitor;
        	this.balloonMessageProvider = balloonMessageProvider;

			if (configuration != null && configuration.Enabled)
			{
				if (configuration.SpeakBuildResults) {
					monitor.BuildOccurred += new MonitorBuildOccurredEventHandler(Monitor_BuildOccurred);
				}
				if (configuration.SpeakBuildStarted) {
					monitor.Polled += new MonitorPolledEventHandler(Monitor_Polled);
				}
			}
        }
        
		private void Monitor_BuildOccurred(object sender, MonitorBuildOccurredEventArgs e)
		{
			String projectName = e.ProjectMonitor.Detail.ProjectName;

			CaptionAndMessage captionAndMessage = balloonMessageProvider.GetCaptionAndMessageForBuildTransition(e.BuildTransition);

			String message = String.Format("The {0} project reports {1}", projectName, captionAndMessage.Message);
            voice.Speak(message, SpeechVoiceSpeakFlags.SVSFDefault);
            Trace.WriteLine("speaking: " + message);
		}

		private void Monitor_Polled(object sender, MonitorPolledEventArgs args)
		{
			String projectName = args.ProjectMonitor.Detail.ProjectName;
			ProjectState currentState = (ProjectState)projectStates[projectName];
			ProjectState newState = args.ProjectMonitor.Detail.ProjectState;
			
			if (currentState == null) {
				projectStates.Add(projectName,newState);
			} else if (!currentState.Name.Equals(newState.Name)) {
				projectStates[projectName] = newState;
				if (newState == ProjectState.Building ||
				    newState == ProjectState.BrokenAndBuilding) {
					String message = String.Format("The {0} project has started building", projectName);
		            voice.Speak(message, SpeechVoiceSpeakFlags.SVSFDefault);
		            Trace.WriteLine("speaking: " + message);
				}
			}

		}
	}
}
