using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Triggers
{
	[Serializable]
	[ReflectorType("forceBuildInterval")]
	public class ForceBuildIntervalTrigger : ITrigger
	{
		public static readonly double DefaultIntervalSeconds = 60;
		private readonly IntervalTrigger intervalTrigger;

		public ForceBuildIntervalTrigger() : this(new IntervalTrigger()) { }

		public ForceBuildIntervalTrigger(IntervalTrigger intervalTrigger)
		{
			this.intervalTrigger = intervalTrigger;
			intervalTrigger.BuildCondition = BuildCondition.ForceBuild;
			intervalTrigger.IntervalSeconds = DefaultIntervalSeconds;
		}

		[ReflectorProperty("seconds", Required=false)]
		public double IntervalSeconds
		{
			get { return intervalTrigger.IntervalSeconds;}
			set { intervalTrigger.IntervalSeconds = value; }
		}

		public void IntegrationCompleted()
		{
			intervalTrigger.IntegrationCompleted();
		}

		public BuildCondition ShouldRunIntegration()
		{
			return intervalTrigger.ShouldRunIntegration();
		}
	}
}
