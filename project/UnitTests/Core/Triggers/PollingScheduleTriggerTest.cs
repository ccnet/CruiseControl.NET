using System;
using Exortech.NetReflector;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Triggers;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Triggers
{
	[TestFixture]
	public class PollingScheduleTriggerTest
	{
		private DynamicMock scheduleTriggerMock;
		private PollingScheduleTrigger trigger;

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
			string xml = string.Format(@"<pollingSchedule time=""12:00:00"">
<weekDays>
	<weekDay>Monday</weekDay>
	<weekDay>Tuesday</weekDay>
</weekDays>
</pollingSchedule>");
			trigger = (PollingScheduleTrigger)NetReflector.Read(xml);
			Assert.AreEqual("12:00:00", trigger.Time);
			Assert.AreEqual(DayOfWeek.Monday, trigger.WeekDays[0]);
			Assert.AreEqual(DayOfWeek.Tuesday, trigger.WeekDays[1]);
		}

		[Test]
		public void ShouldSetUpSubTriggerForModificationChecking()
		{
			scheduleTriggerMock.Expect("BuildCondition", BuildCondition.IfModificationExists);
			trigger = new PollingScheduleTrigger((ScheduleTrigger) scheduleTriggerMock.MockInstance);
			VerifyAll();
		}

		[Test]
		public void ShouldPassTimeWhenSet()
		{
			trigger = new PollingScheduleTrigger((ScheduleTrigger) scheduleTriggerMock.MockInstance);
			scheduleTriggerMock.Expect("Time", "11:59");
			trigger.Time = "11:59";
			VerifyAll();
		}

		[Test]
		public void ShouldPassThroughIntegrationCompletedMessage()
		{
			trigger = new PollingScheduleTrigger((ScheduleTrigger) scheduleTriggerMock.MockInstance);

			scheduleTriggerMock.Expect("IntegrationCompleted");
			trigger.IntegrationCompleted();

			VerifyAll();
		}

		[Test]
		public void ShouldReturnCorrectCondition()
		{
			trigger = new PollingScheduleTrigger((ScheduleTrigger) scheduleTriggerMock.MockInstance);

			scheduleTriggerMock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.NoBuild);
			Assert.AreEqual(BuildCondition.NoBuild, trigger.ShouldRunIntegration());

			scheduleTriggerMock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.ForceBuild);
			Assert.AreEqual(BuildCondition.ForceBuild, trigger.ShouldRunIntegration());

			VerifyAll();
		}
	}
}