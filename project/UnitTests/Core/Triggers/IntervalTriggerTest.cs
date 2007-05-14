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
	public class IntervalTriggerTest : IntegrationFixture
	{
		private IMock mockDateTime;
		private IntervalTrigger trigger;
		private DateTime initialDateTimeNow;

		[SetUp]
		public void Setup()
		{
			Source = "IntervalTrigger";
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
			string xml = string.Format(@"<intervalTrigger name=""continuous"" seconds=""1"" buildCondition=""ForceBuild"" />");
			trigger = (IntervalTrigger) NetReflector.Read(xml);
			Assert.AreEqual(1, trigger.IntervalSeconds);
			Assert.AreEqual(BuildCondition.ForceBuild, trigger.BuildCondition);
			Assert.AreEqual("continuous", trigger.Name);
		}

		[Test]
		public void ShouldDefaultPopulateFromReflector()
		{
			string xml = string.Format(@"<intervalTrigger />");
			trigger = (IntervalTrigger) NetReflector.Read(xml);
			Assert.AreEqual(IntervalTrigger.DefaultIntervalSeconds, trigger.IntervalSeconds);
			Assert.AreEqual(BuildCondition.IfModificationExists, trigger.BuildCondition);
			Assert.AreEqual("IntervalTrigger", trigger.Name);
		}

		[Test]
		public void VerifyThatShouldRequestIntegrationAfterTenSeconds()
		{
			trigger.IntervalSeconds = 10;
			trigger.BuildCondition = BuildCondition.IfModificationExists;

			mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 0, 0));
			Assert.AreEqual(ModificationExistRequest(), trigger.Fire());
			trigger.IntegrationCompleted();

			mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 5, 0)); // 5 seconds later
			Assert.IsNull(trigger.Fire());

			mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 9, 0)); // 4 seconds later

			// still before 1sec
			Assert.IsNull(trigger.Fire());

			// sleep beyond the 1sec mark
			mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 14, 0)); // 5 seconds later

			Assert.AreEqual(ModificationExistRequest(), trigger.Fire());
			trigger.IntegrationCompleted();
			Assert.IsNull(trigger.Fire());
			VerifyAll();
		}

		[Test]
		public void ProcessTrigger()
		{
			trigger.IntervalSeconds = 0.5;
			trigger.BuildCondition = BuildCondition.IfModificationExists;

			mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 0, 0));
			Assert.AreEqual(ModificationExistRequest(), trigger.Fire());

			mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 0, 550));

			Assert.AreEqual(ModificationExistRequest(), trigger.Fire());
			trigger.IntegrationCompleted();
			Assert.IsNull(trigger.Fire());

			mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 1, 50));

			Assert.AreEqual(ModificationExistRequest(), trigger.Fire());
			trigger.IntegrationCompleted();
			Assert.AreEqual(null, trigger.Fire());

			mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 1, 550));

			Assert.AreEqual(ModificationExistRequest(), trigger.Fire());
			VerifyAll();
		}

		[Test]
		public void ShouldReturnForceBuildRequestIfSpecified()
		{
			trigger.IntervalSeconds = 10;
			trigger.BuildCondition = BuildCondition.ForceBuild;
			mockDateTime.SetupResult("Now", new DateTime(2004, 1, 1, 1, 0, 0, 0));
			Assert.AreEqual(ForceBuildRequest(), trigger.Fire());
			VerifyAll();			
		}

		[Test]
        public void ShouldReturnIntervalTimeForNextBuildOnServerStart()
		{
            trigger.IntervalSeconds = 10;
            Assert.AreEqual(initialDateTimeNow.AddSeconds(10), trigger.NextBuild);
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