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
	public class ScheduleIntegrationTriggerTest
	{
		private IMock _mockDateTime;
		private ScheduleIntegrationTrigger trigger;

		[SetUp]
		public void Setup()
		{
			_mockDateTime = new DynamicMock(typeof(DateTimeProvider));
			trigger = new ScheduleIntegrationTrigger((DateTimeProvider) _mockDateTime.MockInstance);
		}

		[TearDown]
		public void VerifyMocks()
		{
			_mockDateTime.Verify();
		}

		[Test]
		public void PopulateFromConfiguration()
		{
			ScheduleIntegrationTrigger integrationTrigger = (ScheduleIntegrationTrigger)NetReflector.Read(@"<schedule integrationTime=""23:59"" buildCondition=""ForceBuild"" />");
			Assert.AreEqual(new TimeSpan(23, 59, 0).ToString(), integrationTrigger.IntegrationTime);
			Assert.AreEqual(BuildCondition.ForceBuild, integrationTrigger.BuildCondition);
		}

		[Test, ExpectedException(typeof(ConfigurationException))]
		public void PopulateFromConfigurationWithInvalidIntegrationTime()
		{
			NetReflector.Read(@"<schedule integrationTime=""23b59""/>");
		}

		[Test]
		public void ShouldRunIntegrationIfCalendarTimeIsAfterIntegrationTime()
		{
			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 23, 25, 0, 0));
			trigger.IntegrationTime = "23:30";
			Assert.AreEqual(BuildCondition.NoBuild, trigger.ShouldRunIntegration());
			
			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 23, 31, 0, 0));
			Assert.AreEqual(BuildCondition.IfModificationExists, trigger.ShouldRunIntegration());
		}

		[Test]
		public void ShouldRunIntegrationOnTheNextDay()
		{
			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 23, 25, 0, 0));
			trigger.IntegrationTime = "23:30";

			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 2, 1, 1, 0, 0));
			Assert.AreEqual(BuildCondition.IfModificationExists, trigger.ShouldRunIntegration());
		}

		[Test]
		public void ShouldIncrementTheIntegrationTimeToTheNextDayAfterIntegrationIsCompleted()
		{
			_mockDateTime.SetupResult("Now", new DateTime(2004, 6, 27, 13, 00, 0, 0));
			trigger.IntegrationTime = "14:30";
			_mockDateTime.SetupResult("Now", new DateTime(2004, 6, 27, 15, 00, 0, 0));

			Assert.AreEqual(BuildCondition.IfModificationExists, trigger.ShouldRunIntegration());			
			trigger.IntegrationCompleted();
			Assert.AreEqual(BuildCondition.NoBuild, trigger.ShouldRunIntegration());			

			_mockDateTime.SetupResult("Now", new DateTime(2004, 6, 28, 15, 00, 0, 0));
			Assert.AreEqual(BuildCondition.IfModificationExists, trigger.ShouldRunIntegration());			
		}
	}
}
