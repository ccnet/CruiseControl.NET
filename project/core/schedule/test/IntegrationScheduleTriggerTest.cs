using System;
using Exortech.NetReflector;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Schedules.Test
{
	[TestFixture]
	public class IntegrationScheduleTriggerTest
	{
		private IMock _mockDateTime;
		private IntegrationScheduleTrigger scheduleTrigger;

		[SetUp]
		public void Setup()
		{
			_mockDateTime = new DynamicMock(typeof (DateTimeProvider));
			scheduleTrigger = new IntegrationScheduleTrigger((DateTimeProvider) _mockDateTime.MockInstance);
		}

		[TearDown]
		public void VerifyMocks()
		{
			_mockDateTime.Verify();
		}

		[Test]
		public void PopulateFromConfiguration()
		{
			IntegrationScheduleTrigger integrationScheduleTrigger = (IntegrationScheduleTrigger) NetReflector.Read(@"<integrationSchedule time=""23:59"" buildCondition=""ForceBuild"" />");
			Assert.AreEqual(new TimeSpan(23, 59, 0).ToString(), integrationScheduleTrigger.IntegrationTime);
			Assert.AreEqual(BuildCondition.ForceBuild, integrationScheduleTrigger.BuildCondition);
		}

		[Test, ExpectedException(typeof (ConfigurationException))]
		public void PopulateFromConfigurationWithInvalidIntegrationTime()
		{
			NetReflector.Read(@"<integrationSchedule time=""23b59""/>");
		}

		[Test]
		public void ShouldRunIntegrationIfCalendarTimeIsAfterIntegrationTime()
		{
			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 23, 25, 0, 0));
			scheduleTrigger.IntegrationTime = "23:30";
			Assert.AreEqual(BuildCondition.NoBuild, scheduleTrigger.ShouldRunIntegration());

			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 23, 31, 0, 0));
			Assert.AreEqual(BuildCondition.IfModificationExists, scheduleTrigger.ShouldRunIntegration());
		}

		[Test]
		public void ShouldRunIntegrationOnTheNextDay()
		{
			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 23, 25, 0, 0));
			scheduleTrigger.IntegrationTime = "23:30";

			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 2, 1, 1, 0, 0));
			Assert.AreEqual(BuildCondition.IfModificationExists, scheduleTrigger.ShouldRunIntegration());
		}

		[Test]
		public void ShouldIncrementTheIntegrationTimeToTheNextDayAfterIntegrationIsCompleted()
		{
			_mockDateTime.SetupResult("Now", new DateTime(2004, 6, 27, 13, 00, 0, 0));
			scheduleTrigger.IntegrationTime = "14:30";
			_mockDateTime.SetupResult("Now", new DateTime(2004, 6, 27, 15, 00, 0, 0));

			Assert.AreEqual(BuildCondition.IfModificationExists, scheduleTrigger.ShouldRunIntegration());
			scheduleTrigger.IntegrationCompleted();
			Assert.AreEqual(BuildCondition.NoBuild, scheduleTrigger.ShouldRunIntegration());

			_mockDateTime.SetupResult("Now", new DateTime(2004, 6, 28, 15, 00, 0, 0));
			Assert.AreEqual(BuildCondition.IfModificationExists, scheduleTrigger.ShouldRunIntegration());
		}

		[Test]
		public void ShouldReturnSpecifiedBuildConditionWhenShouldRunIntegration()
		{
			foreach (BuildCondition expectedCondition in Enum.GetValues(typeof (BuildCondition)))
			{
				_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 23, 25, 0, 0));
				scheduleTrigger.IntegrationTime = "23:30";
				scheduleTrigger.BuildCondition = expectedCondition;
				Assert.AreEqual(BuildCondition.NoBuild, scheduleTrigger.ShouldRunIntegration());

				_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 23, 31, 0, 0));
				Assert.AreEqual(expectedCondition, scheduleTrigger.ShouldRunIntegration());
			}
		}
	}
}