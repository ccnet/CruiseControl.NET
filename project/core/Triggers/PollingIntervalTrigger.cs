using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Triggers
{
	[ReflectorType("pollingInterval")]
	public class PollingIntervalTrigger : ITrigger
	{
		public static readonly double DefaultIntervalSeconds = 60;
		private readonly IntervalTrigger intervalTrigger;

		public PollingIntervalTrigger() : this(new IntervalTrigger()) { }

		public PollingIntervalTrigger(IntervalTrigger intervalTrigger)
		{
			this.intervalTrigger = intervalTrigger;
			intervalTrigger.BuildCondition = BuildCondition.IfModificationExists;
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
