using System;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Triggers;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Triggers
{
	[TestFixture]
	public class ScheduleTriggerTest
	{
		private IMock _mockDateTime;
		private ScheduleTrigger trigger;

		[SetUp]
		public void Setup()
		{
			_mockDateTime = new DynamicMock(typeof (DateTimeProvider));
			trigger = new ScheduleTrigger((DateTimeProvider) _mockDateTime.MockInstance);
		}

		[TearDown]
		public void VerifyAll()
		{
			_mockDateTime.Verify();
		}

		[Test]
		public void ShouldRunIntegrationIfCalendarTimeIsAfterIntegrationTime()
		{
			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 23, 25, 0, 0));
			trigger.Time = "23:30";
			trigger.BuildCondition = BuildCondition.IfModificationExists;

			Assert.AreEqual(BuildCondition.NoBuild, trigger.ShouldRunIntegration());

			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 23, 31, 0, 0));
			Assert.AreEqual(BuildCondition.IfModificationExists, trigger.ShouldRunIntegration());
		}

		[Test]
		public void ShouldRunIntegrationOnTheNextDay()
		{
			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 23, 25, 0, 0));
			trigger.Time = "23:30";
			trigger.BuildCondition = BuildCondition.IfModificationExists;

			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 2, 1, 1, 0, 0));
			Assert.AreEqual(BuildCondition.IfModificationExists, trigger.ShouldRunIntegration());
		}

		[Test]
		public void ShouldIncrementTheIntegrationTimeToTheNextDayAfterIntegrationIsCompleted()
		{
			_mockDateTime.SetupResult("Now", new DateTime(2004, 6, 27, 13, 00, 0, 0));
			trigger.Time = "14:30";
			trigger.BuildCondition = BuildCondition.IfModificationExists;
			_mockDateTime.SetupResult("Now", new DateTime(2004, 6, 27, 15, 00, 0, 0));

			Assert.AreEqual(BuildCondition.IfModificationExists, trigger.ShouldRunIntegration());
			trigger.IntegrationCompleted();
			Assert.AreEqual(BuildCondition.NoBuild, trigger.ShouldRunIntegration());

			_mockDateTime.SetupResult("Now", new DateTime(2004, 6, 28, 15, 00, 0, 0));
			Assert.AreEqual(BuildCondition.IfModificationExists, trigger.ShouldRunIntegration());
		}

		[Test]
		public void ShouldReturnSpecifiedBuildConditionWhenShouldRunIntegration()
		{
			foreach (BuildCondition expectedCondition in Enum.GetValues(typeof (BuildCondition)))
			{
				_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 23, 25, 0, 0));
				trigger.Time = "23:30";
				trigger.BuildCondition = expectedCondition;
				Assert.AreEqual(BuildCondition.NoBuild, trigger.ShouldRunIntegration());

				_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 23, 31, 0, 0));
				Assert.AreEqual(expectedCondition, trigger.ShouldRunIntegration());
			}
		}
	}
}