using System;
using Moq;
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
		private Mock<IAudioPlayer> mockAudioPlayer;

		[SetUp]
		public void SetUp()
		{
			stubProjectMonitor = new StubProjectMonitor("project");

			mockAudioPlayer = new Mock<IAudioPlayer>(MockBehavior.Strict);
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
				(IAudioPlayer) mockAudioPlayer.Object,
				files);

			mockAudioPlayer.Setup(player => player.Play(files.BrokenBuildSound)).Verifiable();
			stubProjectMonitor.OnBuildOccurred(new MonitorBuildOccurredEventArgs(stubProjectMonitor, BuildTransition.Broken));

			mockAudioPlayer.Setup(player => player.Play(files.FixedBuildSound)).Verifiable();
			stubProjectMonitor.OnBuildOccurred(new MonitorBuildOccurredEventArgs(stubProjectMonitor, BuildTransition.Fixed));

			mockAudioPlayer.Setup(player => player.Play(files.StillFailingBuildSound)).Verifiable();
			stubProjectMonitor.OnBuildOccurred(new MonitorBuildOccurredEventArgs(stubProjectMonitor, BuildTransition.StillFailing));

			mockAudioPlayer.Setup(player => player.Play(files.StillSuccessfulBuildSound)).Verifiable();
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
				(IAudioPlayer) mockAudioPlayer.Object,
				files);

			stubProjectMonitor.OnBuildOccurred(new MonitorBuildOccurredEventArgs(stubProjectMonitor, BuildTransition.StillSuccessful));

			stubProjectMonitor.OnBuildOccurred(new MonitorBuildOccurredEventArgs(stubProjectMonitor, BuildTransition.StillFailing));

			mockAudioPlayer.VerifyNoOtherCalls();
		}

		[Test]
		public void WhenNullIsPassedForTheConfigurationNoSoundsPlay()
		{
			new BuildTransitionSoundPlayer(
				stubProjectMonitor, 
				(IAudioPlayer) mockAudioPlayer.Object,
				null);

			stubProjectMonitor.OnBuildOccurred(new MonitorBuildOccurredEventArgs(stubProjectMonitor, BuildTransition.StillSuccessful));

			stubProjectMonitor.OnBuildOccurred(new MonitorBuildOccurredEventArgs(stubProjectMonitor, BuildTransition.StillFailing));

			mockAudioPlayer.VerifyNoOtherCalls();
		}
	}
}
