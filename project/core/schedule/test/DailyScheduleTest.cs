using Exortech.NetReflector;
using NMock;
using NUnit.Framework;
using System;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Config;

namespace ThoughtWorks.CruiseControl.Core.Schedules.Test
{
	[TestFixture]
	public class DailyScheduleTest : Assertion
	{
		private IMock _mockDateTime;
		private DailySchedule _schedule;

		[SetUp]
		public void CreateSchedule()
		{
			_mockDateTime = new DynamicMock(typeof(DateTimeProvider));
			_schedule = new DailySchedule((DateTimeProvider) _mockDateTime.MockInstance);
		}

		[TearDown]
		public void VerifyMocks()
		{
			_mockDateTime.Verify();
		}

		[Test]
		public void PopulateFromConfiguration()
		{
			DailySchedule schedule = (DailySchedule)NetReflector.Read(@"<daily integrationTime=""23:59"" buildCondition=""ForceBuild"" />");
			AssertEquals(new TimeSpan(23, 59, 0).ToString(), schedule.IntegrationTime);
			AssertEquals(BuildCondition.ForceBuild, schedule.BuildCondition);
		}

		[Test, ExpectedException(typeof(ConfigurationException))]
		public void PopulateFromConfigurationWithInvalidIntegrationTime()
		{
			NetReflector.Read(@"<daily integrationTime=""23b59""/>");
		}

		[Test]
		public void ShouldRunIntegrationIfCalendarTimeIsAfterIntegrationTime()
		{
			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 23, 25, 0, 0));
			_schedule.IntegrationTime = "23:30";
			AssertEquals(BuildCondition.NoBuild, _schedule.ShouldRunIntegration());
			
			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 23, 31, 0, 0));
			AssertEquals(BuildCondition.IfModificationExists, _schedule.ShouldRunIntegration());
		}

		[Test]
		public void ShouldRunIntegrationOnTheNextDay()
		{
			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 23, 25, 0, 0));
			_schedule.IntegrationTime = "23:30";

			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 2, 1, 1, 0, 0));
			AssertEquals(BuildCondition.IfModificationExists, _schedule.ShouldRunIntegration());
		}

		[Test]
		public void ShouldIncrementTheIntegrationTimeToTheNextDayAfterIntegrationIsCompleted()
		{
			_mockDateTime.SetupResult("Now", new DateTime(2004, 6, 27, 13, 00, 0, 0));
			_schedule.IntegrationTime = "14:30";
			_mockDateTime.SetupResult("Now", new DateTime(2004, 6, 27, 15, 00, 0, 0));

			AssertEquals(BuildCondition.IfModificationExists, _schedule.ShouldRunIntegration());			
			_schedule.IntegrationCompleted();
			AssertEquals(BuildCondition.NoBuild, _schedule.ShouldRunIntegration());			

			_mockDateTime.SetupResult("Now", new DateTime(2004, 6, 28, 15, 00, 0, 0));
			AssertEquals(BuildCondition.IfModificationExists, _schedule.ShouldRunIntegration());			
		}
	}
}
