using System;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Triggers
{
	public class IntervalTrigger : ITrigger
	{
		private double intervalSeconds;
		private BuildCondition buildCondition;
		private readonly DateTimeProvider dateTimeProvider;

		private DateTime _lastIntegrationCompleteTime;

		public IntervalTrigger() : this(new DateTimeProvider()) { }
		public IntervalTrigger(DateTimeProvider dtProvider)
		{
			this.dateTimeProvider = dtProvider;
			this.intervalSeconds = 0;
			this.buildCondition = BuildCondition.NoBuild;
			_lastIntegrationCompleteTime = DateTime.MinValue;
		}

		public virtual double IntervalSeconds
		{
			get { return intervalSeconds; }
			set { intervalSeconds = value; }
		}

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
