using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Schedules
{
	[Serializable]
	[ReflectorType("stopAfterNumberOfIntegrations")]
	public class StopProjectAfterNumberOfIntegrationsTrigger : IStopProjectTrigger
	{
		[ReflectorProperty("integrations", Required=false)]
		public int TotalIntegrations = 0;

		private int completedIntegrations = 0;

		public bool ShouldStopProject()
		{
			return completedIntegrations >= TotalIntegrations;
		}

		public void IntegrationCompleted()
		{
			completedIntegrations++;
		}
	}
}
