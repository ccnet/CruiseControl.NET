using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Triggers
{
	[ReflectorType("intervalTrigger")]
	public class IntervalTrigger : ITrigger
	{
		public const double DefaultIntervalSeconds = 60;
		private readonly DateTimeProvider dateTimeProvider;

		private DateTime lastIntegrationCompleteTime;
		private DateTime nextBuildTime;

		public IntervalTrigger() : this(new DateTimeProvider()) { }

		public IntervalTrigger(DateTimeProvider dtProvider)
		{
			this.dateTimeProvider = dtProvider;
			lastIntegrationCompleteTime = DateTime.MinValue;
			nextBuildTime = dtProvider.Now;
		}

		[ReflectorProperty("seconds", Required=false)]
		public double IntervalSeconds = DefaultIntervalSeconds;

		[ReflectorProperty("buildCondition", Required=false)]
		public BuildCondition BuildCondition = BuildCondition.IfModificationExists;

		public virtual void IntegrationCompleted()
		{
			DateTime now = dateTimeProvider.Now;
			lastIntegrationCompleteTime = now;
			nextBuildTime = now.AddSeconds(IntervalSeconds);
		}

		public DateTime NextBuild
		{
			get {  return nextBuildTime;}
		}

		public virtual BuildCondition ShouldRunIntegration()
		{
			TimeSpan timeSinceLastBuild = dateTimeProvider.Now - lastIntegrationCompleteTime;
			if (timeSinceLastBuild.TotalSeconds < IntervalSeconds)
				return BuildCondition.NoBuild;

			return BuildCondition;
		}
	}
}