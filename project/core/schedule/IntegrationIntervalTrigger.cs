using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Schedules
{
	[Serializable]
	[ReflectorType("integrationInterval")]
	public class IntegrationIntervalTrigger : ITrigger
	{
		public const int DefaultIntervalSeconds = 60;

		private DateTime _lastIntegrationCompleteTime = DateTime.MinValue;
		private DateTimeProvider _dtProvider;

		public IntegrationIntervalTrigger() : this(new DateTimeProvider()) { }
		public IntegrationIntervalTrigger(DateTimeProvider dtProvider)
		{
			_dtProvider = dtProvider;
		}

		[ReflectorProperty("seconds")]
		public double IntervalSeconds = DefaultIntervalSeconds;

		[ReflectorProperty("buildCondition", Required=false)]
		public BuildCondition BuildCondition = BuildCondition.IfModificationExists;

		public void IntegrationCompleted()
		{
			_lastIntegrationCompleteTime = _dtProvider.Now;
		}

		public BuildCondition ShouldRunIntegration()
		{
			TimeSpan timeSinceLastBuild = _dtProvider.Now - _lastIntegrationCompleteTime;
			if (timeSinceLastBuild.TotalSeconds < IntervalSeconds)
				return BuildCondition.NoBuild;

			return BuildCondition;
		}
	}
}
