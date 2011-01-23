using System;
using NMock;
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
		private IMock mockSourceControl;
		private IMock mockDateTimeProvider;
		private QuietPeriod quietPeriod;

		[SetUp]
		protected void SetUpFixtureData()
		{
			lastBuild = IntegrationResultMother.CreateSuccessful(CreateDateTime(12, 01, 00));
			thisBuild = IntegrationResultMother.CreateSuccessful(CreateDateTime(12, 02, 00));

			mods = new Modification[] { CreateModificationAtTime(12, 01, 30) };

			mockSourceControl = new DynamicMock(typeof (ISourceControl));
			mockSourceControl.Strict = true;
			mockDateTimeProvider = new DynamicMock(typeof (DateTimeProvider));
			mockDateTimeProvider.Strict = true;
			quietPeriod = new QuietPeriod((DateTimeProvider) mockDateTimeProvider.MockInstance);
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

			mockSourceControl.ExpectAndReturn("GetModifications", mods, lastBuild, thisBuild);
			mockDateTimeProvider.ExpectNoCall("Sleep", typeof (TimeSpan));

			Modification[] actualMods = quietPeriod.GetModifications((ISourceControl) mockSourceControl.MockInstance, lastBuild, thisBuild);

			Assert.AreEqual(mods, actualMods);
		}

		[Test]
		public void WhenThereIsAModificationWithinTheDelayPeriodSleepsUntilTheEndOfThePeriodAndTriesAgain()
		{
			quietPeriod.ModificationDelaySeconds = 45;

			// last build was at 12:01:00, current build is at 12:02:00, modification at 12:01:30
			mockSourceControl.ExpectAndReturn("GetModifications", mods, lastBuild, thisBuild);
			mockDateTimeProvider.Expect("Sleep", TimeSpan.FromSeconds(15));

			mockDateTimeProvider.ExpectAndReturn("Now", CreateDateTime(12, 02, 15));
			mockSourceControl.ExpectAndReturn("GetModifications", mods, lastBuild, IntegrationResultMother.CreateSuccessful(CreateDateTime(12, 02, 15)));

			Modification[] actualMods = quietPeriod.GetModifications((ISourceControl) mockSourceControl.MockInstance, lastBuild, thisBuild);

			Assert.AreEqual(mods, actualMods);
		}

		
		[Test]
		public void ShouldCheckModificationsUntilThereAreNoModsInModificationDelay()
		{
			quietPeriod.ModificationDelaySeconds = 45;		

			// last build was at 12:01:00, current build is at 12:02:00
			
			// with a modification just at 12:01:30...
			Modification[] newMods = new Modification[] { CreateModificationAtTime(12, 01, 30) };
			
			// ...should wait for 15 seconds so that the 45 second delay completes (at 12:02:15)
			mockSourceControl.ExpectAndReturn("GetModifications", newMods, lastBuild, thisBuild);
			mockDateTimeProvider.Expect("Sleep", TimeSpan.FromSeconds(15));
			mockDateTimeProvider.ExpectAndReturn("Now", CreateDateTime(12, 02, 15));

			// After this time it should ask for modifications again.  Introduce a new modification at 12:02:10, meaning that the ealiest a build can start is at 12:02:55
			//  i.e. 40 seconds from the time of the current build (12:02:15)
			newMods = new Modification[] {newMods[0], CreateModificationAtTime(12, 02, 10)};		
			mockSourceControl.ExpectAndReturn("GetModifications", newMods, lastBuild, IntegrationResultMother.CreateSuccessful(CreateDateTime(12, 02, 15)));
			mockDateTimeProvider.Expect("Sleep", TimeSpan.FromSeconds(40));
			mockDateTimeProvider.ExpectAndReturn("Now", CreateDateTime(12, 02, 55));
			mockSourceControl.ExpectAndReturn("GetModifications", newMods, lastBuild, IntegrationResultMother.CreateSuccessful(CreateDateTime(12, 02, 55)));

			Modification[] actualMods = quietPeriod.GetModifications((ISourceControl) mockSourceControl.MockInstance, lastBuild, thisBuild);

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
			mockSourceControl.ExpectAndReturn("GetModifications", null, lastBuild, thisBuild);
			mockDateTimeProvider.ExpectNoCall("Sleep", typeof (TimeSpan));

			quietPeriod.ModificationDelaySeconds = 60;
			Modification[] actualMods = quietPeriod.GetModifications((ISourceControl) mockSourceControl.MockInstance, lastBuild, thisBuild);

			Assert.AreEqual(new Modification[0], actualMods);
		}

		[Test]
		public void ShouldHandleTimeDifferencesThatAreLessThanOneMillisecondFromModificationDelay()
		{
			thisBuild = IntegrationResultMother.CreateSuccessful(mods[0].ModifiedTime.AddSeconds(60).AddTicks(-1));

			mockSourceControl.ExpectAndReturn("GetModifications", mods, lastBuild, thisBuild);
			mockDateTimeProvider.ExpectNoCall("Sleep", typeof (TimeSpan));

			quietPeriod.ModificationDelaySeconds = 60;
			Modification[] actualMods = quietPeriod.GetModifications((ISourceControl) mockSourceControl.MockInstance, lastBuild, thisBuild);

			Assert.AreEqual(mods, actualMods);
		}
		
		[Test]
		public void WhenTheTimeDifferenceIsLessThanATenthOfASecondIgnoreTheQuietPeriodAndDoTheBuildAnyway()
		{
			quietPeriod.ModificationDelaySeconds = 60;
			thisBuild = IntegrationResultMother.CreateSuccessful(mods[0].ModifiedTime.AddSeconds(quietPeriod.ModificationDelaySeconds).AddMilliseconds(-99));

			mockSourceControl.ExpectAndReturn("GetModifications", mods, lastBuild, thisBuild);
			mockDateTimeProvider.ExpectNoCall("Sleep", typeof (TimeSpan));

			Modification[] actualMods = quietPeriod.GetModifications((ISourceControl) mockSourceControl.MockInstance, lastBuild, thisBuild);

			Assert.AreEqual(mods, actualMods);
		}

		[Test]
		public void WhenTheTimeDifferenceIsATenthOfASecondDoNormalQuietPeriodProcessing()
		{
			quietPeriod.ModificationDelaySeconds = 60;
			thisBuild = IntegrationResultMother.CreateSuccessful(mods[0].ModifiedTime.AddSeconds(quietPeriod.ModificationDelaySeconds).AddMilliseconds(-100));

			mockSourceControl.ExpectAndReturn("GetModifications", mods, lastBuild, thisBuild);
		
			mockDateTimeProvider.Expect("Sleep", TimeSpan.FromMilliseconds(100));

			DateTime nextBuildTime = thisBuild.StartTime.AddSeconds(1);
			mockDateTimeProvider.ExpectAndReturn("Now", nextBuildTime);
			mockSourceControl.ExpectAndReturn("GetModifications", mods, lastBuild, IntegrationResultMother.CreateSuccessful(nextBuildTime));

			Modification[] actualMods = quietPeriod.GetModifications((ISourceControl) mockSourceControl.MockInstance, lastBuild, thisBuild);

			Assert.AreSame(mods, actualMods);
		}

		[Test]
		public void WhenAModifcationIsBetween10And60SecondsInTheFutureTheQuietPeriodIsObeyed()
		{
			quietPeriod.ModificationDelaySeconds = 60;
			mods[0].ModifiedTime = thisBuild.StartTime.AddSeconds(55);
			
			// last build was at 12:01:00, current build is at 12:02:00, modification at 12:02:55
			// so should wait until 12:03:55
			mockSourceControl.ExpectAndReturn("GetModifications", mods, lastBuild, thisBuild);
			mockDateTimeProvider.Expect("Sleep", TimeSpan.FromSeconds(60 + 55));

			mockDateTimeProvider.ExpectAndReturn("Now", CreateDateTime(12, 03, 55));
			mockSourceControl.ExpectAndReturn("GetModifications", mods, lastBuild, IntegrationResultMother.CreateSuccessful(CreateDateTime(12, 03, 55)));

			Modification[] actualMods = quietPeriod.GetModifications((ISourceControl) mockSourceControl.MockInstance, lastBuild, thisBuild);

			Assert.AreEqual(mods, actualMods);
			
		}

		[Test]
		public void WhenAModifcationIsMoreThan60SecondsInTheFutureTheQuietPeriodIsSkipped()
		{
			quietPeriod.ModificationDelaySeconds = 60;
			mods[0].ModifiedTime = thisBuild.StartTime.AddSeconds(65);
			
			// last build was at 12:01:00, current build is at 12:02:00, modification at 12:02:55
			// so should wait until 12:03:55
			mockSourceControl.ExpectAndReturn("GetModifications", mods, lastBuild, thisBuild);
			mockDateTimeProvider.ExpectNoCall("Sleep", typeof (TimeSpan));

			Modification[] actualMods = quietPeriod.GetModifications((ISourceControl) mockSourceControl.MockInstance, lastBuild, thisBuild);

			Assert.AreSame(mods, actualMods);
			
		}

	}
}