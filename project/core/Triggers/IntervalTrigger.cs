using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Triggers
{
	[ReflectorType("intervalTrigger")]
	public class IntervalTrigger : ITrigger
	{
		public static readonly double DefaultIntervalSeconds = 60;
		private double intervalSeconds;
		private BuildCondition buildCondition;
		private readonly DateTimeProvider dateTimeProvider;

		private DateTime lastIntegrationCompleteTime;
		private DateTime nextBuildTime;

		public IntervalTrigger() : this(new DateTimeProvider()) { }

		public IntervalTrigger(DateTimeProvider dtProvider)
		{
			this.dateTimeProvider = dtProvider;
			this.intervalSeconds = DefaultIntervalSeconds;
			this.buildCondition = BuildCondition.IfModificationExists;
			lastIntegrationCompleteTime = DateTime.MinValue;
			nextBuildTime = dtProvider.Now;
		}

		[ReflectorProperty("seconds", Required=false)]
		public virtual double IntervalSeconds
		{
			get { return intervalSeconds; }
			set { intervalSeconds = value; }
		}

		[ReflectorProperty("buildCondition", Required=false)]
		public virtual BuildCondition BuildCondition
		{
			get { return buildCondition; }
			set { buildCondition = value; }
		}

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
			if (timeSinceLastBuild.TotalSeconds < intervalSeconds)
				return BuildCondition.NoBuild;

			return buildCondition;
		}
	}
}
