using Exortech.NetReflector;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Schedules;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Schedule
{
	[TestFixture]
	public class MultipleIntegrationTriggerTest
	{
		private DynamicMock subTrigger1Mock;
		private DynamicMock subTrigger2Mock;
		private IIntegrationTrigger subTrigger1;
		private IIntegrationTrigger subTrigger2;
		private MultipleIntegrationTrigger trigger;

		[SetUp]
		public void Setup()
		{
			subTrigger1Mock = new DynamicMock(typeof(IIntegrationTrigger));
			subTrigger2Mock = new DynamicMock(typeof(IIntegrationTrigger));
			subTrigger1 = (IIntegrationTrigger) subTrigger1Mock.MockInstance;
			subTrigger2 = (IIntegrationTrigger) subTrigger2Mock.MockInstance;
			trigger = new MultipleIntegrationTrigger();
			trigger.Triggers = new IIntegrationTrigger[] {subTrigger1, subTrigger2};
		}

		private void VerifyAll()
		{
			subTrigger1Mock.Verify();
			subTrigger2Mock.Verify();
		}

		[Test]
		public void PopulateFromReflector()
		{
			string xml = string.Format(@"<multipleIntegrationTriggers>
	<triggers>
		<interval intervalSeconds=""60"" />
		<schedule integrationTime=""08:00"" buildCondition=""ForceBuild"" />
	</triggers>
</multipleIntegrationTriggers>");
			trigger = (MultipleIntegrationTrigger)NetReflector.Read(xml);
			Assert.AreEqual(2, trigger.Triggers.Length);
		}

		[Test]
		public void ShouldReturnNoBuildWhenNoTriggers()
		{
			trigger = new MultipleIntegrationTrigger();
			Assert.AreEqual(BuildCondition.NoBuild, trigger.ShouldRunIntegration());
		}

		[Test]
		public void ShouldNotFailWhenNoTriggersAndIntegrationCompletedCalled()
		{
			trigger = new MultipleIntegrationTrigger();
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
