using Exortech.NetReflector;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Triggers;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Triggers
{
	[TestFixture]
	public class PollingIntervalTriggerTest : CustomAssertion
	{
		private DynamicMock intervalTriggerMock;
		private PollingIntervalTrigger trigger;

		[SetUp]
		public void Setup()
		{
			intervalTriggerMock = new DynamicMock(typeof(IntervalTrigger));
		}

		public void VerifyAll()
		{
			intervalTriggerMock.Verify();
		}

		// We actually want to test more here, but NetReflector will always use the default constructor
		[Test]
		public void ShouldFullyPopulateFromReflector()
		{
			string xml = string.Format(@"<pollingInterval seconds=""1"" />");
			trigger = (PollingIntervalTrigger)NetReflector.Read(xml);
			Assert.AreEqual(1, trigger.IntervalSeconds);
		}

		// We actually want to test more here, but NetReflector will always use the default constructor
		[Test]
		public void ShouldDefaultPopulateFromReflector()
		{
			string xml = string.Format(@"<pollingInterval />");
			trigger = (PollingIntervalTrigger)NetReflector.Read(xml);
			Assert.AreEqual(PollingIntervalTrigger.DefaultIntervalSeconds, trigger.IntervalSeconds);
		}

		[Test]
		public void ShouldSetUpSubTriggerForModificationChecking()
		{
			intervalTriggerMock.Expect("BuildCondition", BuildCondition.IfModificationExists);
			trigger = new PollingIntervalTrigger((IntervalTrigger) intervalTriggerMock.MockInstance);
			VerifyAll();
		}

		[Test]
		public void ShouldPassThroughDefaultInterval()
		{
			intervalTriggerMock.Expect("IntervalSeconds", ForceBuildIntervalTrigger.DefaultIntervalSeconds);
			trigger = new PollingIntervalTrigger((IntervalTrigger) intervalTriggerMock.MockInstance);
			VerifyAll();
		}

		[Test]
		public void ShouldPassThroughIntervalWhenSet()
		{
			intervalTriggerMock.Expect("IntervalSeconds", ForceBuildIntervalTrigger.DefaultIntervalSeconds);
			trigger = new PollingIntervalTrigger((IntervalTrigger) intervalTriggerMock.MockInstance);
			intervalTriggerMock.Expect("IntervalSeconds", 3d);
			trigger.IntervalSeconds = 3d;
			VerifyAll();
		}

		[Test]
		public void ShouldPassThroughIntegrationCompletedMessage()
		{
			trigger = new PollingIntervalTrigger((IntervalTrigger) intervalTriggerMock.MockInstance);

			intervalTriggerMock.Expect("IntegrationCompleted");
			trigger.IntegrationCompleted();

			VerifyAll();
		}

		[Test]
		public void ShouldReturnCorrectCondition()
		{
			trigger = new PollingIntervalTrigger((IntervalTrigger) intervalTriggerMock.MockInstance);

			intervalTriggerMock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.NoBuild);
			Assert.AreEqual(BuildCondition.NoBuild, trigger.ShouldRunIntegration());

			intervalTriggerMock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.ForceBuild);
			Assert.AreEqual(BuildCondition.ForceBuild, trigger.ShouldRunIntegration());

			VerifyAll();
		}
	}
}
