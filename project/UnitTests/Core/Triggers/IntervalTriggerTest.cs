using System;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Triggers;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Triggers
{
	[TestFixture]
	public class IntervalTriggerTest : CustomAssertion
	{
		private IMock _mockDateTime;
		private IntervalTrigger trigger;

		[SetUp]
		public void Setup()
		{
			_mockDateTime = new DynamicMock(typeof(DateTimeProvider));
			trigger = new IntervalTrigger((DateTimeProvider) _mockDateTime.MockInstance);
		}

		public void VerifyAll()
		{
			_mockDateTime.Verify();
		}

		[Test]
		public void VerifyThatShouldRunIntegrationAfterTenSeconds()
		{
			trigger.IntervalSeconds = 10;
			trigger.BuildCondition = BuildCondition.IfModificationExists;

			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 0, 0));
			Assert.AreEqual(BuildCondition.IfModificationExists, trigger.ShouldRunIntegration());
			trigger.IntegrationCompleted();

			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 5, 0)); // 5 seconds later
			Assert.AreEqual(BuildCondition.NoBuild, trigger.ShouldRunIntegration());

			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 9, 0)); // 4 seconds later

			// still before 1sec
			Assert.AreEqual(BuildCondition.NoBuild, trigger.ShouldRunIntegration());
			
			// sleep beyond the 1sec mark
			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 14, 0)); // 5 seconds later
			
			Assert.AreEqual(BuildCondition.IfModificationExists, trigger.ShouldRunIntegration());
			trigger.IntegrationCompleted();
			Assert.AreEqual(BuildCondition.NoBuild, trigger.ShouldRunIntegration());
			VerifyAll();
		}

		[Test]
		public void ShouldRunIntegrationIfTimeChanges()
		{
			trigger.IntervalSeconds = 10;
			trigger.BuildCondition = BuildCondition.IfModificationExists;

			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 0, 0));
			Assert.AreEqual(BuildCondition.IfModificationExists, trigger.ShouldRunIntegration());
			trigger.IntegrationCompleted();

			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 5, 0)); // 5 seconds later
			Assert.AreEqual(BuildCondition.NoBuild, trigger.ShouldRunIntegration());

			trigger.IntervalSeconds = 2;
			Assert.AreEqual(BuildCondition.IfModificationExists, trigger.ShouldRunIntegration());
			trigger.IntegrationCompleted();
			Assert.AreEqual(BuildCondition.NoBuild, trigger.ShouldRunIntegration());
			VerifyAll();
		}

		[Test]
		public void ShouldRunIntegration_SleepsFromEndOfIntegration()
		{
			trigger.IntervalSeconds = 0.5;
			trigger.BuildCondition = BuildCondition.IfModificationExists;

			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 0, 0));
			Assert.AreEqual(BuildCondition.IfModificationExists, trigger.ShouldRunIntegration());

			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 0, 550));

			Assert.AreEqual(BuildCondition.IfModificationExists, trigger.ShouldRunIntegration());
			trigger.IntegrationCompleted();
			Assert.AreEqual(BuildCondition.NoBuild, trigger.ShouldRunIntegration());

			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 1, 50));

			Assert.AreEqual(BuildCondition.IfModificationExists, trigger.ShouldRunIntegration());
			trigger.IntegrationCompleted();
			Assert.AreEqual(BuildCondition.NoBuild, trigger.ShouldRunIntegration());

			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 1, 550));

			Assert.AreEqual(BuildCondition.IfModificationExists, trigger.ShouldRunIntegration());
			VerifyAll();
		}

		[Test]
		public void ShouldReturnForceBuildIfSpecified()
		{
			trigger.IntervalSeconds = 10;
			trigger.BuildCondition = BuildCondition.ForceBuild;
			_mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 0, 0));
			Assert.AreEqual(BuildCondition.ForceBuild, trigger.ShouldRunIntegration());
			VerifyAll();
		}
	}
}
