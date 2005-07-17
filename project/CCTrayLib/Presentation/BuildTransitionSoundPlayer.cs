using System;
using System.Collections;
using System.Collections.Specialized;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class BuildTransitionSoundPlayer
	{
		private readonly IAudioPlayer audioPlayer;
		private readonly IDictionary soundFileLookup = new HybridDictionary();

		public BuildTransitionSoundPlayer(IProjectMonitor monitor, IAudioPlayer audioPlayer, AudioFiles configuration)
		{
			this.audioPlayer = audioPlayer;

			if (configuration != null)
			{
				soundFileLookup[BuildTransition.Broken] = configuration.BrokenBuildSound;
				soundFileLookup[BuildTransition.Fixed] = configuration.FixedBuildSound;
				soundFileLookup[BuildTransition.StillFailing] = configuration.StillFailingBuildSound;
				soundFileLookup[BuildTransition.StillSuccessful] = configuration.StillSuccessfulBuildSound;
			}

			monitor.BuildOccurred += new MonitorBuildOccurredEventHandler(Monitor_BuildOccurred);
		}

		private void Monitor_BuildOccurred(object sender, MonitorBuildOccurredEventArgs e)
		{
			string soundFileToPlay = ChooseSoundFile(e.BuildTransition);

			if (soundFileToPlay == null || soundFileToPlay.Length == 0)
				return;
				
			audioPlayer.Play(soundFileToPlay);
		}

		private string ChooseSoundFile(BuildTransition transition)
		{
			return (string) soundFileLookup[transition];
		}
	}
}
