using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Schedules
{
	[Serializable]
	[ReflectorType("neverTriggerIntegration")]
	public class NullTrigger : ITrigger
	{
		public BuildCondition ShouldRunIntegration()
		{
			return BuildCondition.NoBuild;
		}

		public void IntegrationCompleted()
		{
			return;
		}
	}
}
