using Exortech.NetReflector;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Triggers;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Triggers
{
	[TestFixture]
	public class ForceBuildScheduleTriggerTest
	{
		private DynamicMock scheduleTriggerMock;
		private ForceBuildScheduleTrigger trigger;

		[SetUp]
		public void Setup()
		{
			scheduleTriggerMock = new DynamicMock(typeof(ScheduleTrigger));
		}

		public void VerifyAll()
		{
			scheduleTriggerMock.Verify();
		}

		// We actually want to test more here, but NetReflector will always use the default constructor
		[Test]
		public void ShouldFullyPopulateFromReflector()
		{
			string xml = string.Format(@"<forceBuildSchedule time=""12:00:00"" />");
			trigger = (ForceBuildScheduleTrigger)NetReflector.Read(xml);
			Assert.AreEqual("12:00:00", trigger.Time);
		}

		[Test]
		public void ShouldSetUpSubTriggerForModificationChecking()
		{
			scheduleTriggerMock.Expect("BuildCondition", BuildCondition.ForceBuild);
			trigger = new ForceBuildScheduleTrigger((ScheduleTrigger) scheduleTriggerMock.MockInstance);
			VerifyAll();
		}

		[Test]
		public void ShouldPassTimeWhenSet()
		{
			trigger = new ForceBuildScheduleTrigger((ScheduleTrigger) scheduleTriggerMock.MockInstance);
			scheduleTriggerMock.Expect("Time", "11:59");
			trigger.Time = "11:59";
			VerifyAll();
		}

		[Test]
		public void ShouldPassThroughIntegrationCompletedMessage()
		{
			trigger = new ForceBuildScheduleTrigger((ScheduleTrigger) scheduleTriggerMock.MockInstance);

			scheduleTriggerMock.Expect("IntegrationCompleted");
			trigger.IntegrationCompleted();

			VerifyAll();
		}

		[Test]
		public void ShouldReturnCorrectCondition()
		{
			trigger = new ForceBuildScheduleTrigger((ScheduleTrigger) scheduleTriggerMock.MockInstance);

			scheduleTriggerMock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.NoBuild);
			Assert.AreEqual(BuildCondition.NoBuild, trigger.ShouldRunIntegration());

			scheduleTriggerMock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.ForceBuild);
			Assert.AreEqual(BuildCondition.ForceBuild, trigger.ShouldRunIntegration());

			VerifyAll();
		}
	}
}