using System;
using Exortech.NetReflector;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Triggers;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Triggers
{
	[TestFixture]
	public class FilterTriggerTest : IntegrationFixture
	{
		private IMock mockTrigger;
		private IMock mockDateTime;
		private FilterTrigger trigger;

		[SetUp]
		protected void CreateMocksAndInitialiseObjectUnderTest()
		{
			mockTrigger = new DynamicMock(typeof (ITrigger));
			mockDateTime = new DynamicMock(typeof (DateTimeProvider));

			trigger = new FilterTrigger((DateTimeProvider) mockDateTime.MockInstance);
			trigger.InnerTrigger = (ITrigger) mockTrigger.MockInstance;
			trigger.StartTime = "10:00";
			trigger.EndTime = "11:00";
			trigger.WeekDays = new DayOfWeek[] {DayOfWeek.Wednesday};
			trigger.BuildCondition = BuildCondition.NoBuild;
		}

		[TearDown]
		protected void VerifyMocks()
		{
			mockDateTime.Verify();
			mockTrigger.Verify();
		}

		[Test]
		public void ShouldNotInvokeDecoratedTriggerWhenTimeAndWeekDayMatch()
		{
			mockTrigger.ExpectNoCall("Fire");
			mockDateTime.ExpectAndReturn("Now", new DateTime(2004, 12, 1, 10, 30, 0, 0));

			Assert.IsNull(trigger.Fire());
		}

		[Test]
		public void ShouldNotInvokeDecoratedTriggerWhenWeekDaysNotSpecified()
		{
			mockTrigger.ExpectNoCall("Fire");
			mockDateTime.ExpectAndReturn("Now", new DateTime(2004, 12, 1, 10, 30, 0, 0));
			trigger.WeekDays = new DayOfWeek[] {};

			Assert.IsNull(trigger.Fire());
		}

		[Test]
		public void ShouldInvokeDecoratedTriggerWhenTimeIsOutsideOfRange()
		{
			mockTrigger.ExpectAndReturn("Fire", ModificationExistRequest());
			mockDateTime.ExpectAndReturn("Now", new DateTime(2004, 12, 1, 11, 30, 0, 0));

			Assert.AreEqual(ModificationExistRequest(), trigger.Fire());
		}

		[Test]
		public void ShouldNotInvokeOverMidnightTriggerWhenCurrentTimeIsBeforeMidnight()
		{
			trigger.StartTime = "23:00";
			trigger.EndTime = "7:00";

			mockTrigger.ExpectNoCall("Fire");
			mockDateTime.ExpectAndReturn("Now", new DateTime(2004, 12, 1, 23, 30, 0, 0));

			Assert.IsNull(trigger.Fire());
		}

		[Test]
		public void ShouldNotInvokeOverMidnightTriggerWhenCurrentTimeIsAfterMidnight()
		{
			trigger.StartTime = "23:00";
			trigger.EndTime = "7:00";

			mockTrigger.ExpectNoCall("Fire");
			mockDateTime.ExpectAndReturn("Now", new DateTime(2004, 12, 1, 00, 30, 0, 0));

			Assert.IsNull(trigger.Fire());
		}

		[Test]
		public void ShouldInvokeOverMidnightTriggerWhenCurrentTimeIsOutsideOfRange()
		{
			trigger.StartTime = "23:00";
			trigger.EndTime = "7:00";

			mockTrigger.ExpectAndReturn("Fire", ModificationExistRequest());
			mockDateTime.ExpectAndReturn("Now", new DateTime(2004, 12, 1, 11, 30, 0, 0));

			Assert.AreEqual(ModificationExistRequest(), trigger.Fire());
		}

		[Test]
		public void ShouldNotInvokeDecoratedTriggerWhenTimeIsEqualToStartTimeOrEndTime()
		{
			mockTrigger.ExpectNoCall("Fire");
			mockDateTime.ExpectAndReturn("Now", new DateTime(2004, 12, 1, 10, 00, 0, 0));
			mockDateTime.ExpectAndReturn("Now", new DateTime(2004, 12, 1, 11, 00, 0, 0));

			Assert.IsNull(trigger.Fire());
			Assert.IsNull(trigger.Fire());
		}

		[Test]
		public void ShouldNotInvokeDecoratedTriggerWhenTodayIsOneOfSpecifiedWeekdays()
		{
			mockTrigger.ExpectAndReturn("Fire", ModificationExistRequest());
			mockDateTime.ExpectAndReturn("Now", new DateTime(2004, 12, 2, 10, 30, 0, 0));

			Assert.AreEqual(ModificationExistRequest(), trigger.Fire());
		}

		[Test]
		public void ShouldDelegateIntegrationCompletedCallToInnerTrigger()
		{
			mockTrigger.Expect("IntegrationCompleted");
			trigger.IntegrationCompleted();
		}

		[Test]
		public void ShouldUseFilterEndTimeIfTriggerBuildTimeIsInFilter()
		{
			DateTime triggerNextBuildTime = new DateTime(2004, 12, 1, 10, 30, 00);
			mockTrigger.SetupResult("NextBuild", triggerNextBuildTime);
			Assert.AreEqual(new DateTime(2004, 12, 1, 11, 00, 00), trigger.NextBuild);
		}

		[Test]
		public void ShouldNotFilterIfTriggerBuildDayIsNotInFilter()
		{
			DateTime triggerNextBuildTime = new DateTime(2004, 12, 4, 10, 00, 00);
			mockTrigger.SetupResult("NextBuild", triggerNextBuildTime);
			Assert.AreEqual(triggerNextBuildTime, trigger.NextBuild);
		}

		[Test]
		public void ShouldNotFilterIfTriggerBuildTimeIsNotInFilter()
		{
			DateTime nextBuildTime = new DateTime(2004, 12, 1, 13, 30, 00);
			mockTrigger.ExpectAndReturn("NextBuild", nextBuildTime);
			Assert.AreEqual(nextBuildTime, trigger.NextBuild);
		}

		[Test]
		public void ShouldFullyPopulateFromReflector()
		{
			string xml = string.Format(@"<filterTrigger startTime=""8:30:30"" endTime=""22:30:30"" buildCondition=""ForceBuild"">
											<trigger type=""scheduleTrigger"" time=""12:00:00""/>
											<weekDays>
												<weekDay>Monday</weekDay>
												<weekDay>Tuesday</weekDay>
											</weekDays>
										</filterTrigger>");
			trigger = (FilterTrigger) NetReflector.Read(xml);
			Assert.AreEqual("08:30:30", trigger.StartTime);
			Assert.AreEqual("22:30:30", trigger.EndTime);
			Assert.AreEqual(typeof (ScheduleTrigger), trigger.InnerTrigger.GetType());
			Assert.AreEqual(DayOfWeek.Monday, trigger.WeekDays[0]);
			Assert.AreEqual(DayOfWeek.Tuesday, trigger.WeekDays[1]);
			Assert.AreEqual(BuildCondition.ForceBuild, trigger.BuildCondition);
		}

		[Test]
		public void ShouldMinimallyPopulateFromReflector()
		{
			string xml = string.Format(@"<filterTrigger startTime=""8:30:30"" endTime=""22:30:30"">
											<trigger type=""scheduleTrigger"" time=""12:00:00"" />
										</filterTrigger>");
			trigger = (FilterTrigger) NetReflector.Read(xml);
			Assert.AreEqual("08:30:30", trigger.StartTime);
			Assert.AreEqual("22:30:30", trigger.EndTime);
			Assert.AreEqual(typeof (ScheduleTrigger), trigger.InnerTrigger.GetType());
			Assert.AreEqual(7, trigger.WeekDays.Length);
			Assert.AreEqual(BuildCondition.NoBuild, trigger.BuildCondition);
		}
	}
}