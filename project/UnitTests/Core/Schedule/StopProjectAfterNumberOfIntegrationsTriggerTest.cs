using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Schedules;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Schedule
{
	[TestFixture]
	public class StopProjectAfterNumberOfIntegrationsTriggerTest
	{
		[Test]
		public void PopulateFromReflector()
		{
			string xml = string.Format(@"<stopAfterNumberOfIntegrations integrations=""5""/>");
			StopProjectAfterNumberOfIntegrationsTrigger trigger = (StopProjectAfterNumberOfIntegrationsTrigger)NetReflector.Read(xml);
			Assert.AreEqual(5, trigger.TotalIntegrations);
		}

		[Test]
		public void ShouldAlwaysStopProjectIfNumberOfIntegrationsNotSpecified()
		{
			StopProjectAfterNumberOfIntegrationsTrigger IntegrationsProjectAfterNumberOfIntegrationsTrigger = new StopProjectAfterNumberOfIntegrationsTrigger();

			Assert.IsTrue(IntegrationsProjectAfterNumberOfIntegrationsTrigger.ShouldStopProject());

			IntegrationsProjectAfterNumberOfIntegrationsTrigger.IntegrationCompleted();
			Assert.IsTrue(IntegrationsProjectAfterNumberOfIntegrationsTrigger.ShouldStopProject());
		}

		[Test]
		public void ShouldAlwaysStopProjectIfNumberOfIntegrationsZero()
		{
			StopProjectAfterNumberOfIntegrationsTrigger IntegrationsProjectAfterNumberOfIntegrationsTrigger = new StopProjectAfterNumberOfIntegrationsTrigger();
			IntegrationsProjectAfterNumberOfIntegrationsTrigger.TotalIntegrations = 0;

			Assert.IsTrue(IntegrationsProjectAfterNumberOfIntegrationsTrigger.ShouldStopProject());

			IntegrationsProjectAfterNumberOfIntegrationsTrigger.IntegrationCompleted();
			Assert.IsTrue(IntegrationsProjectAfterNumberOfIntegrationsTrigger.ShouldStopProject());
		}

		[Test]
		public void ShouldAlwaysStopProjectIfNumberOfIntegrationsNegative()
		{
			StopProjectAfterNumberOfIntegrationsTrigger IntegrationsProjectAfterNumberOfIntegrationsTrigger = new StopProjectAfterNumberOfIntegrationsTrigger();
			IntegrationsProjectAfterNumberOfIntegrationsTrigger.TotalIntegrations = -1;

			Assert.IsTrue(IntegrationsProjectAfterNumberOfIntegrationsTrigger.ShouldStopProject());

			IntegrationsProjectAfterNumberOfIntegrationsTrigger.IntegrationCompleted();
			Assert.IsTrue(IntegrationsProjectAfterNumberOfIntegrationsTrigger.ShouldStopProject());
		}

		[Test]
		public void ShouldNotStopProjectIfNumberOfIntegrationsNotReached()
		{
			StopProjectAfterNumberOfIntegrationsTrigger IntegrationsProjectAfterNumberOfIntegrationsTrigger = new StopProjectAfterNumberOfIntegrationsTrigger();
			IntegrationsProjectAfterNumberOfIntegrationsTrigger.TotalIntegrations = 3;

			Assert.IsFalse(IntegrationsProjectAfterNumberOfIntegrationsTrigger.ShouldStopProject());

			IntegrationsProjectAfterNumberOfIntegrationsTrigger.IntegrationCompleted();
			Assert.IsFalse(IntegrationsProjectAfterNumberOfIntegrationsTrigger.ShouldStopProject());
		}

		[Test]
		public void ShouldStopProjectIfNumberOfIntegrationsReached()
		{
			StopProjectAfterNumberOfIntegrationsTrigger IntegrationsProjectAfterNumberOfIntegrationsTrigger = new StopProjectAfterNumberOfIntegrationsTrigger();
			IntegrationsProjectAfterNumberOfIntegrationsTrigger.TotalIntegrations = 3;

			IntegrationsProjectAfterNumberOfIntegrationsTrigger.IntegrationCompleted();
			IntegrationsProjectAfterNumberOfIntegrationsTrigger.IntegrationCompleted();
			IntegrationsProjectAfterNumberOfIntegrationsTrigger.IntegrationCompleted();
			Assert.IsTrue(IntegrationsProjectAfterNumberOfIntegrationsTrigger.ShouldStopProject());
		}

		[Test]
		public void ShouldStopProjectIfNumberOfIntegrationsExceeded()
		{
			StopProjectAfterNumberOfIntegrationsTrigger IntegrationsProjectAfterNumberOfIntegrationsTrigger = new StopProjectAfterNumberOfIntegrationsTrigger();
			IntegrationsProjectAfterNumberOfIntegrationsTrigger.TotalIntegrations = 3;

			IntegrationsProjectAfterNumberOfIntegrationsTrigger.IntegrationCompleted();
			IntegrationsProjectAfterNumberOfIntegrationsTrigger.IntegrationCompleted();
			IntegrationsProjectAfterNumberOfIntegrationsTrigger.IntegrationCompleted();
			IntegrationsProjectAfterNumberOfIntegrationsTrigger.IntegrationCompleted();
			IntegrationsProjectAfterNumberOfIntegrationsTrigger.IntegrationCompleted();
			Assert.IsTrue(IntegrationsProjectAfterNumberOfIntegrationsTrigger.ShouldStopProject());
		}
	}
}
