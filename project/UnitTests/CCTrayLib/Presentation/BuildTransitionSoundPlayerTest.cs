using System;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Presentation
{
	[TestFixture]
	public class BuildTransitionSoundPlayerTest
	{
		private StubProjectMonitor stubProjectMonitor;
		private DynamicMock mockAudioPlayer;

		[SetUp]
		public void SetUp()
		{
			stubProjectMonitor = new StubProjectMonitor("project");

			mockAudioPlayer = new DynamicMock(typeof(IAudioPlayer));
			mockAudioPlayer.Strict = true;
		}

		[Test]
		public void PlaysTheCorrectSoundFileWhenBuildTransitionsOccur()
		{
			AudioFiles files = new AudioFiles();
			files.StillFailingBuildSound = "anotherFailed.wav";
			files.StillSuccessfulBuildSound = "anotherSuccess.wav";
			files.BrokenBuildSound = "broken.wav";
			files.FixedBuildSound = "fixed.wav";

			new BuildTransitionSoundPlayer(
				stubProjectMonitor, 
				(IAudioPlayer) mockAudioPlayer.MockInstance,
				files);

			mockAudioPlayer.Expect("Play", files.BrokenBuildSound);
			stubProjectMonitor.OnBuildOccurred(new MonitorBuildOccurredEventArgs(stubProjectMonitor, BuildTransition.Broken));

			mockAudioPlayer.Expect("Play", files.FixedBuildSound);
			stubProjectMonitor.OnBuildOccurred(new MonitorBuildOccurredEventArgs(stubProjectMonitor, BuildTransition.Fixed));

			mockAudioPlayer.Expect("Play", files.StillFailingBuildSound);
			stubProjectMonitor.OnBuildOccurred(new MonitorBuildOccurredEventArgs(stubProjectMonitor, BuildTransition.StillFailing));
			
			mockAudioPlayer.Expect("Play", files.StillSuccessfulBuildSound);
			stubProjectMonitor.OnBuildOccurred(new MonitorBuildOccurredEventArgs(stubProjectMonitor, BuildTransition.StillSuccessful));

			mockAudioPlayer.Verify();
		}

		[Test]
		public void WhenATransitionIsNullOrEmptyStringNoAudioIsPlayed()
		{
			AudioFiles files = new AudioFiles();
			files.StillSuccessfulBuildSound =string.Empty;
			files.StillFailingBuildSound = null;

			new BuildTransitionSoundPlayer(
				stubProjectMonitor, 
				(IAudioPlayer) mockAudioPlayer.MockInstance,
				files);

			mockAudioPlayer.ExpectNoCall("Play", typeof(string));
			stubProjectMonitor.OnBuildOccurred(new MonitorBuildOccurredEventArgs(stubProjectMonitor, BuildTransition.StillSuccessful));

			
			mockAudioPlayer.ExpectNoCall("Play", typeof(string));
			stubProjectMonitor.OnBuildOccurred(new MonitorBuildOccurredEventArgs(stubProjectMonitor, BuildTransition.StillFailing));
		}

		[Test]
		public void WhenNullIsPassedForTheConfigurationNoSoundsPlay()
		{
			new BuildTransitionSoundPlayer(
				stubProjectMonitor, 
				(IAudioPlayer) mockAudioPlayer.MockInstance,
				null);

			mockAudioPlayer.ExpectNoCall("Play", typeof(string));
			stubProjectMonitor.OnBuildOccurred(new MonitorBuildOccurredEventArgs(stubProjectMonitor, BuildTransition.StillSuccessful));

			
			mockAudioPlayer.ExpectNoCall("Play", typeof(string));
			stubProjectMonitor.OnBuildOccurred(new MonitorBuildOccurredEventArgs(stubProjectMonitor, BuildTransition.StillFailing));
			
		}
	}
}
