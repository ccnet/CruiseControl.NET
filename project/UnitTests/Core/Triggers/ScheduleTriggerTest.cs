using System;
using Exortech.NetReflector;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Triggers;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Config;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Triggers
{
	[TestFixture]
	public class ScheduleTriggerTest : IntegrationFixture
	{
		private IMock mockDateTime;
		private ScheduleTrigger trigger;

		[SetUp]
		public void Setup()
		{
			Source = "ScheduleTrigger";
			mockDateTime = new DynamicMock(typeof (DateTimeProvider));
			trigger = new ScheduleTrigger((DateTimeProvider) mockDateTime.MockInstance);
		}

		[TearDown]
		public void VerifyAll()
		{
			mockDateTime.Verify();
		}

		[Test]
		public void ShouldRunIntegrationIfCalendarTimeIsAfterIntegrationTime()
		{
			mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 23, 25, 0, 0));
			trigger.Time = "23:30";
			trigger.BuildCondition = BuildCondition.IfModificationExists;
			Assert.IsNull(trigger.Fire());

			mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 23, 31, 0, 0));
			Assert.AreEqual(ModificationExistRequest(), trigger.Fire());
		}

		[Test]
		public void ShouldRunIntegrationOnTheNextDay()
		{
			mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 23, 25, 0, 0));
			trigger.Time = "23:30";
			trigger.BuildCondition = BuildCondition.IfModificationExists;
			Assert.IsNull(trigger.Fire());

			mockDateTime.SetupResult("Now", new DateTime(2004, 1, 2, 1, 1, 0, 0));
			Assert.AreEqual(ModificationExistRequest(), trigger.Fire());
		}

		[Test]
		public void ShouldIncrementTheIntegrationTimeToTheNextDayAfterIntegrationIsCompleted()
		{
			mockDateTime.SetupResult("Now", new DateTime(2004, 6, 27, 13, 00, 0, 0));
			trigger.Time = "14:30";
			trigger.BuildCondition = BuildCondition.IfModificationExists;
			Assert.IsNull(trigger.Fire());

			mockDateTime.SetupResult("Now", new DateTime(2004, 6, 27, 15, 00, 0, 0));
			Assert.AreEqual(ModificationExistRequest(), trigger.Fire());
			trigger.IntegrationCompleted();
			Assert.IsNull(trigger.Fire());

			mockDateTime.SetupResult("Now", new DateTime(2004, 6, 28, 15, 00, 0, 0));
			Assert.AreEqual(ModificationExistRequest(), trigger.Fire());
		}

		[Test]
		public void ShouldReturnSpecifiedBuildConditionWhenShouldRunIntegration()
		{
			foreach (BuildCondition expectedCondition in Enum.GetValues(typeof (BuildCondition)))
			{
				mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 23, 25, 0, 0));
				trigger.Time = "23:30";
				trigger.BuildCondition = expectedCondition;
				Assert.IsNull(trigger.Fire());

				mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 23, 31, 0, 0));
				Assert.AreEqual(Request(expectedCondition), trigger.Fire());
			}
		}

		[Test]
		public void ShouldOnlyRunOnSpecifiedDays()
		{
			trigger.WeekDays = new DayOfWeek[] {DayOfWeek.Monday, DayOfWeek.Wednesday};
			trigger.BuildCondition = BuildCondition.ForceBuild;
			mockDateTime.SetupResult("Now", new DateTime(2004, 11, 30));
			Assert.AreEqual(new DateTime(2004, 12, 1), trigger.NextBuild);
			mockDateTime.SetupResult("Now", new DateTime(2004, 12, 1, 0, 0, 1));
			Assert.AreEqual(ForceBuildRequest(), trigger.Fire());

			mockDateTime.SetupResult("Now", new DateTime(2004, 12, 2));
			Assert.IsNull(trigger.Fire());
		}

		[Test]
		public void ShouldFullyPopulateFromReflector()
		{
			string xml = string.Format(@"<scheduleTrigger name=""nightly"" time=""12:00:00"" buildCondition=""ForceBuild"">
<weekDays>
	<weekDay>Monday</weekDay>
	<weekDay>Tuesday</weekDay>
</weekDays>
</scheduleTrigger>");
			trigger = (ScheduleTrigger) NetReflector.Read(xml);
			Assert.AreEqual("12:00:00", trigger.Time);
			Assert.AreEqual(DayOfWeek.Monday, trigger.WeekDays[0]);
			Assert.AreEqual(DayOfWeek.Tuesday, trigger.WeekDays[1]);
			Assert.AreEqual(BuildCondition.ForceBuild, trigger.BuildCondition);
			Assert.AreEqual("nightly", trigger.Name);
		}

		[Test]
		public void ShouldMinimallyPopulateFromReflector()
		{
			string xml = string.Format(@"<scheduleTrigger time=""10:00:00"" />");
			trigger = (ScheduleTrigger) NetReflector.Read(xml);
			Assert.AreEqual("10:00:00", trigger.Time);
			Assert.AreEqual(7, trigger.WeekDays.Length);
			Assert.AreEqual(BuildCondition.IfModificationExists, trigger.BuildCondition);
			Assert.AreEqual("ScheduleTrigger", trigger.Name);
		}

		[Test]
		public void NextBuildTimeShouldBeSameTimeNextDay()
		{
			mockDateTime.SetupResult("Now", new DateTime(2005, 2, 4, 13, 13, 0));
			trigger.Time = "10:00";
			trigger.WeekDays = new DayOfWeek[] {DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday};
			trigger.IntegrationCompleted();
			DateTime expectedDate = new DateTime(2005, 2, 5, 10, 0, 0);
			Assert.AreEqual(expectedDate, trigger.NextBuild);
		}

		[Test]
		public void NextBuildTimeShouldBeTheNextSpecifiedDay()
		{
			mockDateTime.SetupResult("Now", new DateTime(2005, 2, 4, 13, 13, 0));		// Friday
			trigger.Time = "10:00";
			trigger.WeekDays = new DayOfWeek[] {DayOfWeek.Friday, DayOfWeek.Sunday};
			Assert.AreEqual(new DateTime(2005, 2, 6, 10, 0, 0), trigger.NextBuild);		// Sunday

			mockDateTime.SetupResult("Now", new DateTime(2005, 2, 6, 13, 13, 0));		// Sunday
			Assert.AreEqual(ModificationExistRequest(), trigger.Fire());
			trigger.IntegrationCompleted();
			Assert.AreEqual(new DateTime(2005, 2, 11, 10, 0, 0), trigger.NextBuild);	// Friday
		}

		[Test]
		public void NextBuildTimeShouldBeTheNextSpecifiedDayWithTheNextDayFarAway()
		{
			mockDateTime.SetupResult("Now", new DateTime(2005, 2, 4, 13, 13, 0));
			trigger.Time = "10:00";
			trigger.WeekDays = new DayOfWeek[] {DayOfWeek.Friday, DayOfWeek.Thursday};
			Assert.AreEqual(new DateTime(2005, 2, 10, 10, 0, 0), trigger.NextBuild);

			mockDateTime.SetupResult("Now", new DateTime(2005, 2, 10, 13, 13, 0));
			Assert.AreEqual(ModificationExistRequest(), trigger.Fire());
			trigger.IntegrationCompleted();
			Assert.AreEqual(new DateTime(2005, 2, 11, 10, 0, 0), trigger.NextBuild);
		}

		[Test]
		public void ShouldNotUpdateNextBuildTimeUnlessScheduledBuildHasRun()
		{
			mockDateTime.SetupResult("Now", new DateTime(2005, 2, 4, 9, 0, 1));
			trigger.Time = "10:00";
			Assert.AreEqual(new DateTime(2005, 2, 4, 10, 0, 0), trigger.NextBuild);			

			mockDateTime.SetupResult("Now", new DateTime(2005, 2, 4, 10, 0, 1));
			trigger.IntegrationCompleted();
			Assert.AreEqual(new DateTime(2005, 2, 4, 10, 0, 0), trigger.NextBuild);			
		}


        [Test]
        public void RandomOffSetInMinutesFromTimeShouldBePositive()
        {
            Assert.That(delegate { trigger.RandomOffSetInMinutesFromTime = -10; },
                        Throws.TypeOf<ThoughtWorks.CruiseControl.Core.Config.ConfigurationException>());

        }

        [Test]
        public void RandomOffSetInMinutesFromTimeMayNotExceedMidnight()
        {
            mockDateTime.SetupResult("Now", new DateTime(2005, 2, 4, 9, 0, 1));
         //whatever random time is choosen, the resulted time will still be after midnight
            trigger.Time = "23:59";
            trigger.RandomOffSetInMinutesFromTime = 1;
            Assert.That(delegate { DateTime x = trigger.NextBuild; },
                        Throws.TypeOf<ThoughtWorks.CruiseControl.Core.Config.ConfigurationException>());

        }

        [Test]
        public void TimeFailsWithInvalidDate()
        {
            var trigger = new ScheduleTrigger();
            Assert.Throws<ConfigurationException>(() => trigger.Time = "plain wrong!");
        }
	}
}