using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Schedules
{
	[Serializable]
	[ReflectorType("neverStopProject")]
	public class NeverStopProjectTrigger : IStopProjectTrigger
	{
		public bool ShouldStopProject()
		{
			return false;
		}

		public void IntegrationCompleted()
		{
			return;
		}
	}
}
