using Exortech.NetReflector;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Triggers;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Triggers
{
	[TestFixture]
	public class MultipleTriggerTest
	{
		private DynamicMock subTrigger1Mock;
		private DynamicMock subTrigger2Mock;
		private ITrigger subTrigger1;
		private ITrigger subTrigger2;
		private MultipleTrigger trigger;

		[SetUp]
		public void Setup()
		{
			subTrigger1Mock = new DynamicMock(typeof(ITrigger));
			subTrigger2Mock = new DynamicMock(typeof(ITrigger));
			subTrigger1 = (ITrigger) subTrigger1Mock.MockInstance;
			subTrigger2 = (ITrigger) subTrigger2Mock.MockInstance;
			trigger = new MultipleTrigger();
			trigger.Triggers = new ITrigger[] {subTrigger1, subTrigger2};
		}

		private void VerifyAll()
		{
			subTrigger1Mock.Verify();
			subTrigger2Mock.Verify();
		}

		[Test]
		public void PopulateFromReflector()
		{
			string xml = string.Format(@"<multipleTriggers>
	<triggers>
		<pollingInterval seconds=""60"" />
		<pollingSchedule time=""08:00"" buildCondition=""ForceBuild"" />
	</triggers>
</multipleTriggers>");
			trigger = (MultipleTrigger)NetReflector.Read(xml);
			Assert.AreEqual(2, trigger.Triggers.Length);
		}

		[Test]
		public void ShouldReturnNoBuildWhenNoTriggers()
		{
			trigger = new MultipleTrigger();
			Assert.AreEqual(BuildCondition.NoBuild, trigger.ShouldRunIntegration());
		}

		[Test]
		public void ShouldNotFailWhenNoTriggersAndIntegrationCompletedCalled()
		{
			trigger = new MultipleTrigger();
			trigger.IntegrationCompleted();
		}

		[Test]
		public void ShouldPassThroughIntegrationCompletedCallToAllSubTriggers()
		{
			subTrigger1Mock.Expect("IntegrationCompleted");
			subTrigger2Mock.Expect("IntegrationCompleted");
			trigger.IntegrationCompleted();
			VerifyAll();
		}

		[Test]
		public void ShouldReturnNoBuildIfAllNoBuild()
		{
			subTrigger1Mock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.NoBuild);
			subTrigger2Mock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.NoBuild);
			Assert.AreEqual(BuildCondition.NoBuild, trigger.ShouldRunIntegration());
			VerifyAll();
		}

		[Test]
		public void ShouldReturnIfModificationExistsNoForceBuild()
		{
			subTrigger1Mock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.NoBuild);
			subTrigger2Mock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.IfModificationExists);
			Assert.AreEqual(BuildCondition.IfModificationExists, trigger.ShouldRunIntegration());
			VerifyAll();
		}

		[Test]
		public void ShouldNotCareAboutOrderingForChecking()
		{
			subTrigger1Mock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.NoBuild);
			subTrigger2Mock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.IfModificationExists);
			Assert.AreEqual(BuildCondition.IfModificationExists, trigger.ShouldRunIntegration());
			VerifyAll();
		}

		[Test]
		public void ShouldReturnForceBuildIfOneForceBuildAndOneNoBuild()
		{
			subTrigger1Mock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.NoBuild);
			subTrigger2Mock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.ForceBuild);
			Assert.AreEqual(BuildCondition.ForceBuild, trigger.ShouldRunIntegration());
			VerifyAll();
		}

		[Test]
		public void ShouldReturnForceBuildIfOneForceBuildAndOneIfModifications()
		{
			subTrigger1Mock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.IfModificationExists);
			subTrigger2Mock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.ForceBuild);
			Assert.AreEqual(BuildCondition.ForceBuild, trigger.ShouldRunIntegration());
			VerifyAll();
		}

		[Test]
		public void ShouldNotCareAboutOrderingForCheckingForceBuild()
		{
			subTrigger1Mock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.ForceBuild);
			subTrigger2Mock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.IfModificationExists);
			Assert.AreEqual(BuildCondition.ForceBuild, trigger.ShouldRunIntegration());
			VerifyAll();
		}

		[Test]
		public void ShouldReturnForceBuildIfAllForceBuild()
		{
			subTrigger1Mock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.ForceBuild);
			subTrigger2Mock.ExpectAndReturn("ShouldRunIntegration", BuildCondition.ForceBuild);
			Assert.AreEqual(BuildCondition.ForceBuild, trigger.ShouldRunIntegration());
			VerifyAll();
		}
	}
}
