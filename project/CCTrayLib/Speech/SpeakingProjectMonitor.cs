#if !DISABLE_COM

using System;
using System.Diagnostics;
using System.Collections;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;
using System.Speech.Synthesis;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Speech
{
	/// <summary>
	/// Say things when build events happen.
	/// </summary>
	public class SpeakingProjectMonitor
	{
		private IProjectMonitor monitor;
		private IBalloonMessageProvider balloonMessageProvider;
        //private SpVoice voice = new SpVoice();

        private IDictionary projectStates = new Hashtable();
		private bool speakBuildSucceded;
		private bool speakBuildFailed;

        public SpeakingProjectMonitor (IProjectMonitor monitor, IBalloonMessageProvider balloonMessageProvider, SpeechConfiguration configuration) {
        	this.monitor = monitor;
        	this.balloonMessageProvider = balloonMessageProvider;

			if (configuration != null && configuration.Enabled)
			{
				speakBuildSucceded = configuration.SpeakBuildSucceded;
				speakBuildFailed = configuration.SpeakBuildFailed;
				
				if (speakBuildSucceded || speakBuildFailed) {
					monitor.BuildOccurred += new MonitorBuildOccurredEventHandler(Monitor_BuildOccurred);
				}
				if (configuration.SpeakBuildStarted) {
					monitor.Polled += new MonitorPolledEventHandler(Monitor_Polled);
				}
			}
        }
        
		private void Monitor_BuildOccurred(object sender, MonitorBuildOccurredEventArgs e)
		{
			if (SpeechUtil.shouldSpeak(e.BuildTransition,speakBuildSucceded,speakBuildFailed)) {
				String projectName = e.ProjectMonitor.Detail.ProjectName;
				projectName = SpeechUtil.makeProjectNameMoreSpeechFriendly(projectName);
	
				CaptionAndMessage captionAndMessage = balloonMessageProvider.GetCaptionAndMessageForBuildTransition(e.BuildTransition);
				String message = String.Format("The {0} project reports {1}", projectName, captionAndMessage.Message);
                SpeechSynthesizer speaker = new SpeechSynthesizer();
                speaker.Speak(message);	            
                Trace.WriteLine("speaking: " + message);
			}
		}

		private void Monitor_Polled(object sender, MonitorPolledEventArgs args)
		{
			String projectName = args.ProjectMonitor.Detail.ProjectName;
			projectName = SpeechUtil.makeProjectNameMoreSpeechFriendly(projectName);

			ProjectState currentState = (ProjectState)projectStates[projectName];
			ProjectState newState = args.ProjectMonitor.Detail.ProjectState;
			
			if (currentState == null) {
				projectStates.Add(projectName,newState);
			} else if (!currentState.Name.Equals(newState.Name)) {
				projectStates[projectName] = newState;
				if (newState == ProjectState.Building ||
				    newState == ProjectState.BrokenAndBuilding) {
					String message = String.Format("The {0} project has started building", projectName);
                    SpeechSynthesizer speaker = new SpeechSynthesizer();
                    speaker.Speak(message);
                    Trace.WriteLine("speaking: " + message);
				}
			}
		}
		
		
	}
}
#endif
