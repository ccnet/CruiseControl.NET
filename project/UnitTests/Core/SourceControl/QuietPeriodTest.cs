using System;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class QuietPeriodTest
	{
		private Modification[] mods;
		private IntegrationResult lastBuild;
		private IntegrationResult thisBuild;
		private Mock<ISourceControl> mockSourceControl;
		private Mock<DateTimeProvider> mockDateTimeProvider;
		private QuietPeriod quietPeriod;

		[SetUp]
		protected void SetUpFixtureData()
		{
			lastBuild = IntegrationResultMother.CreateSuccessful(CreateDateTime(12, 01, 00));
			thisBuild = IntegrationResultMother.CreateSuccessful(CreateDateTime(12, 02, 00));

			mods = new Modification[] { CreateModificationAtTime(12, 01, 30) };

			mockSourceControl = new Mock<ISourceControl>(MockBehavior.Strict);
			mockDateTimeProvider = new Mock<DateTimeProvider>(MockBehavior.Strict);
			quietPeriod = new QuietPeriod((DateTimeProvider) mockDateTimeProvider.Object);
		}

		[TearDown]
		protected void VerifyMock()
		{
			mockDateTimeProvider.Verify();
			mockSourceControl.Verify();
		}

		[Test]
		public void ShouldCheckModificationsAndReturnIfDelayIsZero()
		{
			quietPeriod.ModificationDelaySeconds = 0;

			mockSourceControl.Setup(sc => sc.GetModifications(lastBuild, thisBuild)).Returns(mods).Verifiable();

			Modification[] actualMods = quietPeriod.GetModifications((ISourceControl) mockSourceControl.Object, lastBuild, thisBuild);

			Assert.AreEqual(mods, actualMods);

			mockDateTimeProvider.VerifyNoOtherCalls();
		}

		[Test]
		public void WhenThereIsAModificationWithinTheDelayPeriodSleepsUntilTheEndOfThePeriodAndTriesAgain()
		{
			quietPeriod.ModificationDelaySeconds = 45;

			// last build was at 12:01:00, current build is at 12:02:00, modification at 12:01:30
			mockSourceControl.Setup(sc => sc.GetModifications(lastBuild, thisBuild)).Returns(mods).Verifiable();
			mockDateTimeProvider.Setup(provider => provider.Sleep(TimeSpan.FromSeconds(15))).Verifiable();

			mockDateTimeProvider.SetupGet(provider => provider.Now).Returns(CreateDateTime(12, 02, 15)).Verifiable();
			mockSourceControl.Setup(sc => sc.GetModifications(lastBuild, IntegrationResultMother.CreateSuccessful(CreateDateTime(12, 02, 15)))).Returns(mods).Verifiable();

			Modification[] actualMods = quietPeriod.GetModifications((ISourceControl) mockSourceControl.Object, lastBuild, thisBuild);

			Assert.AreEqual(mods, actualMods);
		}

		
		[Test]
		public void ShouldCheckModificationsUntilThereAreNoModsInModificationDelay()
		{
			MockSequence sequence = new MockSequence();
			quietPeriod.ModificationDelaySeconds = 45;		

			// last build was at 12:01:00, current build is at 12:02:00
			
			// with a modification just at 12:01:30...
			Modification[] newMods = new Modification[] { CreateModificationAtTime(12, 01, 30) };
			
			// ...should wait for 15 seconds so that the 45 second delay completes (at 12:02:15)
			mockSourceControl.InSequence(sequence).Setup(sc => sc.GetModifications(lastBuild, thisBuild)).Returns(newMods).Verifiable();
			mockDateTimeProvider.InSequence(sequence).Setup(provider => provider.Sleep(TimeSpan.FromSeconds(15))).Verifiable();
			mockDateTimeProvider.InSequence(sequence).SetupGet(provider => provider.Now).Returns(CreateDateTime(12, 02, 15)).Verifiable();

			// After this time it should ask for modifications again.  Introduce a new modification at 12:02:10, meaning that the ealiest a build can start is at 12:02:55
			//  i.e. 40 seconds from the time of the current build (12:02:15)
			newMods = new Modification[] {newMods[0], CreateModificationAtTime(12, 02, 10)};		
			mockSourceControl.InSequence(sequence).Setup(sc => sc.GetModifications(lastBuild, IntegrationResultMother.CreateSuccessful(CreateDateTime(12, 02, 15)))).Returns(newMods).Verifiable();
			mockDateTimeProvider.InSequence(sequence).Setup(provider => provider.Sleep(TimeSpan.FromSeconds(40))).Verifiable();
			mockDateTimeProvider.InSequence(sequence).SetupGet(provider => provider.Now).Returns(CreateDateTime(12, 02, 55)).Verifiable();
			mockSourceControl.InSequence(sequence).Setup(sc => sc.GetModifications(lastBuild, IntegrationResultMother.CreateSuccessful(CreateDateTime(12, 02, 55)))).Returns(newMods).Verifiable();

			Modification[] actualMods = quietPeriod.GetModifications((ISourceControl) mockSourceControl.Object, lastBuild, thisBuild);

			Assert.AreEqual(newMods, actualMods);
		}

		private Modification CreateModificationAtTime(int hour, int minute, int seconds)
		{
			Modification modification = new Modification();
			modification.ModifiedTime = CreateDateTime(hour, minute, seconds);
			return modification;
		}

		private DateTime CreateDateTime(int hour, int minute, int seconds)
		{
			return new DateTime(2004, 12, 1, hour, minute, seconds);
		}


		[Test]
		public void ShouldHandleIfNoModificationsAreReturned()
		{
			mockSourceControl.Setup(sc => sc.GetModifications(lastBuild, thisBuild)).Returns(() => null).Verifiable();

			quietPeriod.ModificationDelaySeconds = 60;
			Modification[] actualMods = quietPeriod.GetModifications((ISourceControl) mockSourceControl.Object, lastBuild, thisBuild);

			Assert.AreEqual(new Modification[0], actualMods);

			mockDateTimeProvider.VerifyNoOtherCalls();
		}

		[Test]
		public void ShouldHandleTimeDifferencesThatAreLessThanOneMillisecondFromModificationDelay()
		{
			thisBuild = IntegrationResultMother.CreateSuccessful(mods[0].ModifiedTime.AddSeconds(60).AddTicks(-1));

			mockSourceControl.Setup(sc => sc.GetModifications(lastBuild, thisBuild)).Returns(mods).Verifiable();

			quietPeriod.ModificationDelaySeconds = 60;
			Modification[] actualMods = quietPeriod.GetModifications((ISourceControl) mockSourceControl.Object, lastBuild, thisBuild);

			Assert.AreEqual(mods, actualMods);

			mockDateTimeProvider.VerifyNoOtherCalls();
		}

		[Test]
		public void WhenTheTimeDifferenceIsLessThanATenthOfASecondIgnoreTheQuietPeriodAndDoTheBuildAnyway()
		{
			quietPeriod.ModificationDelaySeconds = 60;
			thisBuild = IntegrationResultMother.CreateSuccessful(mods[0].ModifiedTime.AddSeconds(quietPeriod.ModificationDelaySeconds).AddMilliseconds(-99));

			mockSourceControl.Setup(sc => sc.GetModifications(lastBuild, thisBuild)).Returns(mods).Verifiable();

			Modification[] actualMods = quietPeriod.GetModifications((ISourceControl) mockSourceControl.Object, lastBuild, thisBuild);

			Assert.AreEqual(mods, actualMods);

			mockDateTimeProvider.VerifyNoOtherCalls();
		}

		[Test]
		public void WhenTheTimeDifferenceIsATenthOfASecondDoNormalQuietPeriodProcessing()
		{
			quietPeriod.ModificationDelaySeconds = 60;
			thisBuild = IntegrationResultMother.CreateSuccessful(mods[0].ModifiedTime.AddSeconds(quietPeriod.ModificationDelaySeconds).AddMilliseconds(-100));

			mockSourceControl.Setup(sc => sc.GetModifications(lastBuild, thisBuild)).Returns(mods).Verifiable();
		
			mockDateTimeProvider.Setup(provider => provider.Sleep(TimeSpan.FromMilliseconds(100))).Verifiable();

			DateTime nextBuildTime = thisBuild.StartTime.AddSeconds(1);
			mockDateTimeProvider.SetupGet(provider => provider.Now).Returns(nextBuildTime).Verifiable();
			mockSourceControl.Setup(sc => sc.GetModifications(lastBuild, IntegrationResultMother.CreateSuccessful(nextBuildTime))).Returns(mods).Verifiable();

			Modification[] actualMods = quietPeriod.GetModifications((ISourceControl) mockSourceControl.Object, lastBuild, thisBuild);

			Assert.AreSame(mods, actualMods);
		}

		[Test]
		public void WhenAModifcationIsBetween10And60SecondsInTheFutureTheQuietPeriodIsObeyed()
		{
			quietPeriod.ModificationDelaySeconds = 60;
			mods[0].ModifiedTime = thisBuild.StartTime.AddSeconds(55);
			
			// last build was at 12:01:00, current build is at 12:02:00, modification at 12:02:55
			// so should wait until 12:03:55
			mockSourceControl.Setup(sc => sc.GetModifications(lastBuild, thisBuild)).Returns(mods).Verifiable();
			mockDateTimeProvider.Setup(provider => provider.Sleep(TimeSpan.FromSeconds(60 + 55))).Verifiable();

			mockDateTimeProvider.SetupGet(provider => provider.Now).Returns(CreateDateTime(12, 03, 55)).Verifiable();
			mockSourceControl.Setup(sc => sc.GetModifications(lastBuild, IntegrationResultMother.CreateSuccessful(CreateDateTime(12, 03, 55)))).Returns(mods).Verifiable();

			Modification[] actualMods = quietPeriod.GetModifications((ISourceControl) mockSourceControl.Object, lastBuild, thisBuild);

			Assert.AreEqual(mods, actualMods);
			
		}

		[Test]
		public void WhenAModifcationIsMoreThan60SecondsInTheFutureTheQuietPeriodIsSkipped()
		{
			quietPeriod.ModificationDelaySeconds = 60;
			mods[0].ModifiedTime = thisBuild.StartTime.AddSeconds(65);
			
			// last build was at 12:01:00, current build is at 12:02:00, modification at 12:02:55
			// so should wait until 12:03:55
			mockSourceControl.Setup(sc => sc.GetModifications(lastBuild, thisBuild)).Returns(mods).Verifiable();

			Modification[] actualMods = quietPeriod.GetModifications((ISourceControl) mockSourceControl.Object, lastBuild, thisBuild);

			Assert.AreSame(mods, actualMods);

			mockDateTimeProvider.VerifyNoOtherCalls();
		}

	}
}