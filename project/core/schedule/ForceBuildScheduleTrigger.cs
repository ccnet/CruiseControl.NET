using Exortech.NetReflector;
using System;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Schedules
{
	[Serializable]
	[ReflectorType("forceBuildSchedule")]
	public class ForceBuildScheduleTrigger : ITrigger
	{
		private readonly ScheduleTrigger scheduleTrigger;

		public ForceBuildScheduleTrigger() : this(new ScheduleTrigger()) { }

		public ForceBuildScheduleTrigger(ScheduleTrigger scheduleTrigger)
		{
			this.scheduleTrigger = scheduleTrigger;
			scheduleTrigger.BuildCondition = BuildCondition.ForceBuild;
		}

		[ReflectorProperty("time")]
		public string Time
		{
			get { return scheduleTrigger.Time; }
			set { scheduleTrigger.Time = value; }
		}

		public void IntegrationCompleted()
		{
			scheduleTrigger.IntegrationCompleted();
		}

		public BuildCondition ShouldRunIntegration()
		{
			return scheduleTrigger.ShouldRunIntegration();
		}
	}
}
