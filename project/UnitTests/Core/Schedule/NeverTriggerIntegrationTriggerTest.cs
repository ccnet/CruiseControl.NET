using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Schedules;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Schedule
{
	[TestFixture]
	public class NeverTriggerIntegrationTriggerTest
	{
		private NeverTriggerIntegrationTrigger trigger;

		[SetUp]
		public void Setup()
		{
			trigger = new NeverTriggerIntegrationTrigger();	
		}

		[Test]
		public void PopulateFromReflector()
		{
			string xml = string.Format(@"<neverTriggerIntegration />");
			NeverTriggerIntegrationTrigger trigger = (NeverTriggerIntegrationTrigger)NetReflector.Read(xml);
			Assert.AreEqual(BuildCondition.NoBuild, trigger.ShouldRunIntegration());
		}

		[Test]
		public void ShouldNotTriggerAfterNoIntegrations()
		{
			Assert.AreEqual(BuildCondition.NoBuild, trigger.ShouldRunIntegration());
		}

		[Test]
		public void ShouldNotStopAfterOneIntegration()
		{
			trigger.IntegrationCompleted();
			Assert.AreEqual(BuildCondition.NoBuild, trigger.ShouldRunIntegration());
		}

		[Test]
		public void ShouldNotStopAfterTenIntegrations()
		{
			for(int i = 1; i < 10; i++)
			{
				trigger.IntegrationCompleted();	
			}
			Assert.AreEqual(BuildCondition.NoBuild, trigger.ShouldRunIntegration());
		}
	}
}
