using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Schedules;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Schedule
{
	[TestFixture]
	public class NeverStopProjectTriggerTest
	{
		[Test]
		public void PopulateFromReflector()
		{
			string xml = string.Format(@"<neverStopProject />");
			NeverStopProjectTrigger trigger = (NeverStopProjectTrigger)NetReflector.Read(xml);
			Assert.IsFalse(trigger.ShouldStopProject());
		}

		[Test]
		public void ShouldNotStopAfterNoIntegrations()
		{
			NeverStopProjectTrigger projectTrigger = new NeverStopProjectTrigger();
			Assert.IsFalse(projectTrigger.ShouldStopProject());
		}

		[Test]
		public void ShouldNotStopAfterOneIntegration()
		{
			NeverStopProjectTrigger projectTrigger = new NeverStopProjectTrigger();
			projectTrigger.IntegrationCompleted();
			Assert.IsFalse(projectTrigger.ShouldStopProject());
		}

		[Test]
		public void ShouldNotStopAfterTenIntegrations()
		{
			NeverStopProjectTrigger projectTrigger = new NeverStopProjectTrigger();
			for(int i = 1; i < 10; i++)
			{
				projectTrigger.IntegrationCompleted();	
			}
			Assert.IsFalse(projectTrigger.ShouldStopProject());
		}
	}
}
