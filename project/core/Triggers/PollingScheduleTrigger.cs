using Exortech.NetReflector;
using System;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Triggers
{
	[Serializable]
	[ReflectorType("pollingSchedule")]
	public class PollingScheduleTrigger : ITrigger
	{
		private readonly ScheduleTrigger scheduleTrigger;

		public PollingScheduleTrigger() : this(new ScheduleTrigger()) { }

		public PollingScheduleTrigger(ScheduleTrigger scheduleTrigger)
		{
			this.scheduleTrigger = scheduleTrigger;
			scheduleTrigger.BuildCondition = BuildCondition.IfModificationExists;
		}

		[ReflectorProperty("time")]
		public string Time
		{
			get { return scheduleTrigger.Time; }
			set { scheduleTrigger.Time = value; }
		}

		[ReflectorArray("weekDays")]
		public DayOfWeek[] WeekDays
		{
			get { return scheduleTrigger.WeekDays; }
			set { scheduleTrigger.WeekDays = value; }
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
