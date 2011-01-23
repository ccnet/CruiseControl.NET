using System;
using Exortech.NetReflector;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Triggers;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Triggers
{
	[TestFixture]
	public class MultipleTriggerTest : IntegrationFixture
	{
		private DynamicMock subTrigger1Mock;
		private DynamicMock subTrigger2Mock;
		private ITrigger subTrigger1;
		private ITrigger subTrigger2;
		private MultipleTrigger trigger;

		[SetUp]
		public void Setup()
		{
			subTrigger1Mock = new DynamicMock(typeof (ITrigger));
			subTrigger2Mock = new DynamicMock(typeof (ITrigger));
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
			subTrigger1Mock.Expect("IntegrationCompleted");
			subTrigger2Mock.Expect("IntegrationCompleted");
			trigger.IntegrationCompleted();
			VerifyAll();
		}

		[Test]
		public void ShouldReturnNoBuildIfAllNoBuild()
		{
			subTrigger1Mock.ExpectAndReturn("Fire", null);
			subTrigger2Mock.ExpectAndReturn("Fire", null);
			Assert.IsNull(trigger.Fire());
			VerifyAll();
		}

		[Test]
		public void ShouldReturnIfModificationExistsNoForceBuild()
		{
			subTrigger1Mock.ExpectAndReturn("Fire", null);
			subTrigger2Mock.ExpectAndReturn("Fire", ModificationExistRequest());
			Assert.AreEqual(ModificationExistRequest(), trigger.Fire());
			VerifyAll();
		}

		[Test]
		public void ShouldNotCareAboutOrderingForChecking()
		{
			subTrigger1Mock.ExpectAndReturn("Fire", null);
			subTrigger2Mock.ExpectAndReturn("Fire", ModificationExistRequest());
			Assert.AreEqual(ModificationExistRequest(), trigger.Fire());
			VerifyAll();
		}

		[Test]
		public void ShouldReturnForceBuildIfOneForceBuildAndOneNoBuild()
		{
			subTrigger1Mock.ExpectAndReturn("Fire", null);
			subTrigger2Mock.ExpectAndReturn("Fire", ForceBuildRequest());
			Assert.AreEqual(ForceBuildRequest(), trigger.Fire());
			VerifyAll();
		}

		[Test]
		public void ShouldReturnForceBuildIfOneForceBuildAndOneIfModifications()
		{
			subTrigger1Mock.ExpectAndReturn("Fire", ModificationExistRequest());
			subTrigger2Mock.ExpectAndReturn("Fire", ForceBuildRequest());
			Assert.AreEqual(ForceBuildRequest(), trigger.Fire());
			VerifyAll();
		}

		[Test]
		public void ShouldNotCareAboutOrderingForCheckingForceBuild()
		{
			subTrigger1Mock.ExpectAndReturn("Fire", ForceBuildRequest());
			subTrigger2Mock.ExpectAndReturn("Fire", ModificationExistRequest());
			Assert.AreEqual(ForceBuildRequest(), trigger.Fire());
			VerifyAll();
		}

		[Test]
		public void ShouldReturnForceBuildIfAllForceBuild()
		{
			subTrigger1Mock.ExpectAndReturn("Fire", ForceBuildRequest());
			subTrigger2Mock.ExpectAndReturn("Fire", ForceBuildRequest());
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
			subTrigger1Mock.SetupResult("NextBuild", earlierDate);
			DateTime laterDate = new DateTime(2005, 1, 2);
			subTrigger2Mock.SetupResult("NextBuild", laterDate);
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
			subTrigger1Mock.ExpectAndReturn("Fire", null);
			subTrigger2Mock.ExpectAndReturn("Fire", ForceBuildRequest());
			Assert.IsNull(trigger.Fire());
		}
	}
}