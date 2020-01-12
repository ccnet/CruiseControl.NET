using System;
using Exortech.NetReflector;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Triggers;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Triggers
{
	[TestFixture]
	public class MultipleTriggerTest : IntegrationFixture
	{
		private Mock<ITrigger> subTrigger1Mock;
		private Mock<ITrigger> subTrigger2Mock;
		private ITrigger subTrigger1;
		private ITrigger subTrigger2;
		private MultipleTrigger trigger;

		[SetUp]
		public void Setup()
		{
			subTrigger1Mock = new Mock<ITrigger>();
			subTrigger2Mock = new Mock<ITrigger>();
			subTrigger1 = (ITrigger) subTrigger1Mock.Object;
			subTrigger2 = (ITrigger) subTrigger2Mock.Object;
			trigger = new MultipleTrigger();
			trigger.Triggers = new ITrigger[] {subTrigger1, subTrigger2};
		}

		private void VerifyAll()
		{
			subTrigger1Mock.Verify();
			subTrigger2Mock.Verify();
		}

		[Test]
		public void ShouldReturnNoBuildWhenNoTriggers()
		{
			trigger = new MultipleTrigger();
			Assert.IsNull(trigger.Fire());
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
			subTrigger1Mock.Setup(_trigger => _trigger.IntegrationCompleted()).Verifiable();
			subTrigger2Mock.Setup(_trigger => _trigger.IntegrationCompleted()).Verifiable();
			trigger.IntegrationCompleted();
			VerifyAll();
		}

		[Test]
		public void ShouldReturnNoBuildIfAllNoBuild()
		{
			subTrigger1Mock.Setup(_trigger => _trigger.Fire()).Returns(() => null).Verifiable();
			subTrigger2Mock.Setup(_trigger => _trigger.Fire()).Returns(() => null).Verifiable();
			Assert.IsNull(trigger.Fire());
			VerifyAll();
		}

		[Test]
		public void ShouldReturnIfModificationExistsNoForceBuild()
		{
			subTrigger1Mock.Setup(_trigger => _trigger.Fire()).Returns(() => null).Verifiable();
			subTrigger2Mock.Setup(_trigger => _trigger.Fire()).Returns(ModificationExistRequest()).Verifiable();
			Assert.AreEqual(ModificationExistRequest(), trigger.Fire());
			VerifyAll();
		}

		[Test]
		public void ShouldNotCareAboutOrderingForChecking()
		{
			subTrigger1Mock.Setup(_trigger => _trigger.Fire()).Returns(() => null).Verifiable();
			subTrigger2Mock.Setup(_trigger => _trigger.Fire()).Returns(ModificationExistRequest()).Verifiable();
			Assert.AreEqual(ModificationExistRequest(), trigger.Fire());
			VerifyAll();
		}

		[Test]
		public void ShouldReturnForceBuildIfOneForceBuildAndOneNoBuild()
		{
			subTrigger1Mock.Setup(_trigger => _trigger.Fire()).Returns(() => null).Verifiable();
			subTrigger2Mock.Setup(_trigger => _trigger.Fire()).Returns(ForceBuildRequest()).Verifiable();
			Assert.AreEqual(ForceBuildRequest(), trigger.Fire());
			VerifyAll();
		}

		[Test]
		public void ShouldReturnForceBuildIfOneForceBuildAndOneIfModifications()
		{
			subTrigger1Mock.Setup(_trigger => _trigger.Fire()).Returns(ModificationExistRequest()).Verifiable();
			subTrigger2Mock.Setup(_trigger => _trigger.Fire()).Returns(ForceBuildRequest()).Verifiable();
			Assert.AreEqual(ForceBuildRequest(), trigger.Fire());
			VerifyAll();
		}

		[Test]
		public void ShouldNotCareAboutOrderingForCheckingForceBuild()
		{
			subTrigger1Mock.Setup(_trigger => _trigger.Fire()).Returns(ForceBuildRequest()).Verifiable();
			subTrigger2Mock.Setup(_trigger => _trigger.Fire()).Returns(ModificationExistRequest()).Verifiable();
			Assert.AreEqual(ForceBuildRequest(), trigger.Fire());
			VerifyAll();
		}

		[Test]
		public void ShouldReturnForceBuildIfAllForceBuild()
		{
			subTrigger1Mock.Setup(_trigger => _trigger.Fire()).Returns(ForceBuildRequest()).Verifiable();
			subTrigger2Mock.Setup(_trigger => _trigger.Fire()).Returns(ForceBuildRequest()).Verifiable();
			Assert.AreEqual(ForceBuildRequest(), trigger.Fire());
			VerifyAll();
		}

		[Test]
		public void ShouldReturnNeverIfNoTriggerExists()
		{
			trigger = new MultipleTrigger();
			Assert.AreEqual(DateTime.MaxValue, trigger.NextBuild);
		}

		[Test]
		public void ShouldReturnEarliestTriggerTimeForNextBuild()
		{
			DateTime earlierDate = new DateTime(2005, 1, 1);
			subTrigger1Mock.SetupGet(_trigger => _trigger.NextBuild).Returns(earlierDate).Verifiable();
			DateTime laterDate = new DateTime(2005, 1, 2);
			subTrigger2Mock.SetupGet(_trigger => _trigger.NextBuild).Returns(laterDate).Verifiable();
			Assert.AreEqual(earlierDate, trigger.NextBuild);
		}

		[Test]
		public void ShouldPopulateFromConfiguration()
		{
			string xml = @"<multiTrigger operator=""And""><triggers><intervalTrigger /></triggers></multiTrigger>";
			trigger = (MultipleTrigger) NetReflector.Read(xml);
			Assert.AreEqual(1, trigger.Triggers.Length);
			Assert.AreEqual(Operator.And, trigger.Operator);
		}

		[Test]
		public void ShouldPopulateFromConfigurationWithComment()
		{
			string xml = @"<multiTrigger><!-- foo --><triggers><intervalTrigger /></triggers></multiTrigger>";
			trigger = (MultipleTrigger) NetReflector.Read(xml);
			Assert.AreEqual(1, trigger.Triggers.Length);
			Assert.AreEqual(typeof(IntervalTrigger), trigger.Triggers[0].GetType());
		}

		[Test]
		public void ShouldPopulateFromMinimalConfiguration()
		{
			string xml = @"<multiTrigger />";
			trigger = (MultipleTrigger) NetReflector.Read(xml);
			Assert.AreEqual(0, trigger.Triggers.Length);
			Assert.AreEqual(Operator.Or, trigger.Operator);			
		}

		[Test]
		public void UsingAndConditionOnlyTriggersBuildIfBothTriggersShouldBuild()
		{
			trigger.Operator = Operator.And;
			subTrigger1Mock.Setup(_trigger => _trigger.Fire()).Returns(() => null).Verifiable();
			subTrigger2Mock.Setup(_trigger => _trigger.Fire()).Returns(ForceBuildRequest()).Verifiable();
			Assert.IsNull(trigger.Fire());
		}
	}
}