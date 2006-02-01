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
	public class IntervalTriggerTest : CustomAssertion
	{
		private IMock mockDateTime;
		private IntervalTrigger trigger;
		private DateTime initialDateTimeNow;

		[SetUp]
		public void Setup()
		{
			mockDateTime = new DynamicMock(typeof (DateTimeProvider));
			initialDateTimeNow = new DateTime(2002, 1, 2, 3, 0, 0, 0);
			mockDateTime.SetupResult("Now", this.initialDateTimeNow);
			trigger = new IntervalTrigger((DateTimeProvider) mockDateTime.MockInstance);
		}

		public void VerifyAll()
		{
			mockDateTime.Verify();
		}

		[Test]
		public void ShouldFullyPopulateFromReflector()
		{
			string xml = string.Format(@"<intervalTrigger seconds=""1"" buildCondition=""ForceBuild"" />");
			trigger = (IntervalTrigger) NetReflector.Read(xml);
			Assert.AreEqual(1, trigger.IntervalSeconds);
			Assert.AreEqual(BuildCondition.ForceBuild, trigger.BuildCondition);
		}

		[Test]
		public void ShouldDefaultPopulateFromReflector()
		{
			string xml = string.Format(@"<intervalTrigger />");
			trigger = (IntervalTrigger) NetReflector.Read(xml);
			Assert.AreEqual(IntervalTrigger.DefaultIntervalSeconds, trigger.IntervalSeconds);
			Assert.AreEqual(BuildCondition.IfModificationExists, trigger.BuildCondition);
		}

		[Test]
		public void VerifyThatShouldRunIntegrationAfterTenSeconds()
		{
			trigger.IntervalSeconds = 10;
			trigger.BuildCondition = BuildCondition.IfModificationExists;

			mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 0, 0));
			Assert.AreEqual(BuildCondition.IfModificationExists, trigger.ShouldRunIntegration());
			trigger.IntegrationCompleted();

			mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 5, 0)); // 5 seconds later
			Assert.AreEqual(BuildCondition.NoBuild, trigger.ShouldRunIntegration());

			mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 9, 0)); // 4 seconds later

			// still before 1sec
			Assert.AreEqual(BuildCondition.NoBuild, trigger.ShouldRunIntegration());

			// sleep beyond the 1sec mark
			mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 14, 0)); // 5 seconds later

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

			mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 0, 0));
			Assert.AreEqual(BuildCondition.IfModificationExists, trigger.ShouldRunIntegration());

			mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 0, 550));

			Assert.AreEqual(BuildCondition.IfModificationExists, trigger.ShouldRunIntegration());
			trigger.IntegrationCompleted();
			Assert.AreEqual(BuildCondition.NoBuild, trigger.ShouldRunIntegration());

			mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 1, 50));

			Assert.AreEqual(BuildCondition.IfModificationExists, trigger.ShouldRunIntegration());
			trigger.IntegrationCompleted();
			Assert.AreEqual(BuildCondition.NoBuild, trigger.ShouldRunIntegration());

			mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 1, 550));

			Assert.AreEqual(BuildCondition.IfModificationExists, trigger.ShouldRunIntegration());
			VerifyAll();
		}

		[Test]
		public void ShouldReturnForceBuildIfSpecified()
		{
			trigger.IntervalSeconds = 10;
			trigger.BuildCondition = BuildCondition.ForceBuild;
			mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 0, 0));
			Assert.AreEqual(BuildCondition.ForceBuild, trigger.ShouldRunIntegration());
			VerifyAll();
		}

		[Test]
		public void ShouldReturnCurrentTimeForNextBuildOnServerStart()
		{
			Assert.AreEqual(initialDateTimeNow, trigger.NextBuild);
			VerifyAll();
		}

		[Test]
		public void ShouldReturnIntervalTimeIfLastBuildJustOccured()
		{
			trigger.IntervalSeconds = 10;
			DateTime stubNow = new DateTime(2004, 1, 1, 1, 0, 0, 0);
			mockDateTime.SetupResult("Now", stubNow);
			trigger.IntegrationCompleted();
			Assert.AreEqual(stubNow.AddSeconds(10), trigger.NextBuild);
		}
	}
}