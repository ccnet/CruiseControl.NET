using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Schedules;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Schedule
{
	[TestFixture]
	public class NullTriggerTriggerTest
	{
		private NullTrigger trigger;

		[SetUp]
		public void Setup()
		{
			trigger = new NullTrigger();	
		}

		[Test]
		public void PopulateFromReflector()
		{
			string xml = string.Format(@"<neverTriggerIntegration />");
			NullTrigger trigger = (NullTrigger)NetReflector.Read(xml);
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
