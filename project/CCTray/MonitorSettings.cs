using System;

namespace tw.ccnet.remote.monitor
{
	/// <summary>
	/// Models preferences for the CruiseControl.NET monitor.
	/// </summary>
	public class MonitorSettings
	{
		internal const int DefaultPollingIntervalSeconds = 15;

		public bool PlaySoundOnAnotherSuccessfulBuild;
		public bool PlaySoundOnAnotherFailedBuild;
		public bool PlaySoundOnBrokenBuild;
		public bool PlaySoundOnFixedBuild;

		public int PollingIntervalSeconds = 15; // default

		public string RemoteServerUrl;

		public MonitorSettings()
		{
		}

		public bool ShouldPlaySoundForTransition(BuildTransition transition)
		{
			switch (transition)
			{
				case BuildTransition.Broken:
					return PlaySoundOnBrokenBuild;

				case BuildTransition.Fixed:
					return PlaySoundOnFixedBuild;
				
				case BuildTransition.StillFailing:
					return PlaySoundOnAnotherFailedBuild;
				
				case BuildTransition.StillSuccessful:
					return PlaySoundOnAnotherSuccessfulBuild;
			}

			throw new Exception("Unsupported build transition.");
		}

		public string GetAudioFileLocation(BuildTransition transition)
		{
			switch (transition)
			{
				case BuildTransition.Broken:
					return "tw.ccnet.remote.monitor.Audio.broken.wav";

				case BuildTransition.Fixed:
					return "tw.ccnet.remote.monitor.Audio.fixed.wav";
				
				case BuildTransition.StillFailing:
					return "tw.ccnet.remote.monitor.Audio.still-failing.wav";
				
				case BuildTransition.StillSuccessful:
					return "tw.ccnet.remote.monitor.Audio.still-successful.wav";
			}

			throw new Exception("Unsupported build transition.");
		}
	}
}
