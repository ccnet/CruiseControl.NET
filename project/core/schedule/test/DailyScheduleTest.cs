using Exortech.NetReflector;
using NUnit.Framework;
using System;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Config;

namespace ThoughtWorks.CruiseControl.Core.Schedules.Test
{
	[TestFixture]
	public class DailyScheduleTest : Assertion
	{
		[Test]
		public void PopulateFromConfiguration()
		{
			DailySchedule schedule = (DailySchedule)NetReflector.Read(@"<daily integrationTime=""23:59""/>");
			AssertEquals(new TimeSpan(23, 59, 0).ToString(), schedule.IntegrationTime);
		}

		[Test, ExpectedException(typeof(ConfigurationException))]
		public void PopulateFromConfigurationWithInvalidIntegrationTime()
		{
			NetReflector.Read(@"<daily integrationTime=""23b59""/>");
		}

		[Test]
		public void ShouldRunIntegrationIfCalendarTimeIsAfterIntegrationTime()
		{
			DailyScheduleExtension schedule = new DailyScheduleExtension();
			schedule.now = new DateTime(2004, 1, 1, 23, 25, 0, 0);

			schedule.IntegrationTime = "23:30";
			AssertEquals(BuildCondition.NoBuild, schedule.ShouldRunIntegration());
			
			schedule.now = new DateTime(2004, 1, 1, 23, 31, 0, 0);
			AssertEquals(BuildCondition.IfModificationExists, schedule.ShouldRunIntegration());
		}

		[Test]
		public void ShouldRunIntegrationOnTheNextDay()
		{
			DailyScheduleExtension schedule = new DailyScheduleExtension();
			schedule.now = new DateTime(2004, 1, 1, 23, 25, 0, 0);
			schedule.IntegrationTime = "23:30";

			schedule.now = new DateTime(2004, 1, 2, 1, 1, 0, 0);
			AssertEquals(BuildCondition.IfModificationExists, schedule.ShouldRunIntegration());
		}

		class DailyScheduleExtension : DailySchedule
		{
			public DateTime now;
			protected override DateTime Now { get { return now; } }
		}
	}
}
