using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Triggers
{
	[ReflectorType("filterTrigger")]
	public class FilterTrigger : ITrigger
	{
		private readonly DateTimeProvider dtProvider;
		private TimeSpan startTime;
		private TimeSpan endTime;

		public FilterTrigger() : this(new DateTimeProvider())
		{
		}

		public FilterTrigger(DateTimeProvider dtProvider)
		{
			this.dtProvider = dtProvider;
		}

		[ReflectorProperty("trigger", InstanceTypeKey="type")]
		public ITrigger InnerTrigger;

		[ReflectorProperty("startTime")]
		public string StartTime
		{
			get { return startTime.ToString(); }
			set { startTime = ParseTime(value); }
		}

		[ReflectorProperty("endTime")]
		public string EndTime
		{
			get { return endTime.ToString(); }
			set { endTime = ParseTime(value); }
		}

		private TimeSpan ParseTime(string timeString)
		{
			return TimeSpan.Parse(timeString);
		}

		[ReflectorArray("weekDays", Required=false)]
		public DayOfWeek[] WeekDays = (DayOfWeek[]) DayOfWeek.GetValues(typeof (DayOfWeek));

		[ReflectorProperty("buildCondition", Required=false)]
		public BuildCondition BuildCondition = BuildCondition.NoBuild;

		public BuildCondition ShouldRunIntegration()
		{
			DateTime now = dtProvider.Now;
			if (IsNowOutsideOfWeekDayRange(now) && IsNowOutsideOfTimeRange(now))
			{
				return InnerTrigger.ShouldRunIntegration();
			}
			return BuildCondition;
		}

		private bool IsNowOutsideOfWeekDayRange(DateTime now)
		{
			return Array.IndexOf(WeekDays, now.DayOfWeek) >= 0;
		}

		private bool IsNowOutsideOfTimeRange(DateTime now)
		{
			TimeSpan timeOfDay = now.TimeOfDay;
			return timeOfDay < startTime || now.TimeOfDay > endTime;
		}

		public void IntegrationCompleted()
		{
			InnerTrigger.IntegrationCompleted();
		}
	}
}
