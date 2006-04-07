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
		private string name;

		private DateTime nextBuildTime;

		public IntervalTrigger() : this(new DateTimeProvider()) { }

		public IntervalTrigger(DateTimeProvider dtProvider)
		{
			this.dateTimeProvider = dtProvider;
			nextBuildTime = dtProvider.Now;
		}

		[ReflectorProperty("name", Required=false)]
		public string Name
		{
			get
			{
				if (name == null) name = GetType().Name;
				return name;
			}
			set { name = value; }
		}

		[ReflectorProperty("seconds", Required=false)]
		public double IntervalSeconds = DefaultIntervalSeconds;

		[ReflectorProperty("buildCondition", Required=false)]
		public BuildCondition BuildCondition = BuildCondition.IfModificationExists;

		public virtual void IntegrationCompleted()
		{
			IncrementNextBuildTime();
		}

		protected DateTime IncrementNextBuildTime()
		{
			return nextBuildTime = dateTimeProvider.Now.AddSeconds(IntervalSeconds);
		}

		public DateTime NextBuild
		{
			get {  return nextBuildTime;}
		}

		public virtual IntegrationRequest Fire()
		{
			BuildCondition buildCondition = ShouldRunIntegration();
			if (buildCondition == BuildCondition.NoBuild) return null;
			return new IntegrationRequest(buildCondition, Name);
		}

		private BuildCondition ShouldRunIntegration()
		{
			if (dateTimeProvider.Now < nextBuildTime)
				return BuildCondition.NoBuild;

			return BuildCondition;
		}
	}
}