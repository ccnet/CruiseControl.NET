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

		private DateTime _lastIntegrationCompleteTime;

		public IntervalTrigger() : this(new DateTimeProvider()) { }

		public IntervalTrigger(DateTimeProvider dtProvider)
		{
			this.dateTimeProvider = dtProvider;
			this.intervalSeconds = DefaultIntervalSeconds;
			this.buildCondition = BuildCondition.IfModificationExists;
			_lastIntegrationCompleteTime = DateTime.MinValue;
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
			_lastIntegrationCompleteTime = dateTimeProvider.Now;
		}

		public virtual BuildCondition ShouldRunIntegration()
		{
			TimeSpan timeSinceLastBuild = dateTimeProvider.Now - _lastIntegrationCompleteTime;
			if (timeSinceLastBuild.TotalSeconds < intervalSeconds)
				return BuildCondition.NoBuild;

			return buildCondition;
		}
	}
}
