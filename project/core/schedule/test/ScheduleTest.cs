using System;
using Exortech.NetReflector;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Schedules.Test
{
	[TestFixture]
	public class ScheduleTest : CustomAssertion
	{
		private IMock _mockDateTime;
		private Schedule _schedule;

		[SetUp]
		public void CreateSchedule()
		{
			_mockDateTime = new DynamicMock(typeof(DateTimeProvider));
			_schedule = new Schedule((DateTimeProvider) _mockDateTime.MockInstance);
		}

		[TearDown]
		public void VerifyMocks()
		{
			_mockDateTime.Verify();
		}

		[Test]
		public void PopulateFromReflector()
		{
			string xml = string.Format(@"<schedule sleepSeconds=""1"" iterations=""1"" buildCondition=""ForceBuild"" />");
			Schedule schedule = (Schedule)NetReflector.Read(xml);
			AssertEquals(1, schedule.SleepSeconds);
			AssertEquals(1, schedule.TotalIterations);
			AssertEquals(BuildCondition.ForceBuild, schedule.BuildCondition);
		}

		[Test]
		public void VerifyThatShouldRunIntegrationAfterOneSecond()
		{
			_schedule.SleepSeconds = 10;

			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 0, 0));
			AssertEquals(BuildCondition.IfModificationExists, _schedule.ShouldRunIntegration());
			_schedule.IntegrationCompleted();

			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 5, 0)); // 5 seconds later
			AssertEquals(BuildCondition.NoBuild, _schedule.ShouldRunIntegration());

			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 9, 0)); // 4 seconds later

			// still before 1sec
			AssertEquals(BuildCondition.NoBuild, _schedule.ShouldRunIntegration());
			
			// sleep beyond the 1sec mark
			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 14, 0)); // 5 seconds later
			
			AssertEquals(BuildCondition.IfModificationExists, _schedule.ShouldRunIntegration());
			_schedule.IntegrationCompleted();
			AssertEquals(BuildCondition.NoBuild, _schedule.ShouldRunIntegration());
		}

		[Test]
		public void ShouldRunIntegration_SleepsFromEndOfIntegration()
		{
			_schedule.SleepSeconds = 0.5;

			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 0, 0));
			AssertEquals(BuildCondition.IfModificationExists, _schedule.ShouldRunIntegration());

			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 0, 550));

			AssertEquals(BuildCondition.IfModificationExists, _schedule.ShouldRunIntegration());
			_schedule.IntegrationCompleted();
			AssertEquals(BuildCondition.NoBuild, _schedule.ShouldRunIntegration());

			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 1, 50));

			AssertEquals(BuildCondition.IfModificationExists, _schedule.ShouldRunIntegration());
			_schedule.IntegrationCompleted();
			AssertEquals(BuildCondition.NoBuild, _schedule.ShouldRunIntegration());

			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 1, 550));

			AssertEquals(BuildCondition.IfModificationExists, _schedule.ShouldRunIntegration());
		}

		[Test]
		public void ShouldStopIntegrationAfterTwoIterations()
		{
			Schedule schedule = new Schedule();
			schedule.TotalIterations = 2;

			Assert(!schedule.ShouldStopIntegration());
			schedule.IntegrationCompleted();
			Assert(!schedule.ShouldStopIntegration());
			schedule.IntegrationCompleted();
			Assert(schedule.ShouldStopIntegration());
			AssertEquals(BuildCondition.NoBuild, schedule.ShouldRunIntegration());
		}

		[Test]
		public void ShouldReturnSpecifiedBuildConditionWhenShouldRunIntegration()
		{
			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 0, 0));

			_schedule.BuildCondition = BuildCondition.ForceBuild;
			AssertEquals(BuildCondition.ForceBuild, _schedule.ShouldRunIntegration());
		}
	}
}
