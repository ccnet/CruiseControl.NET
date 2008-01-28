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
        private TimeSpan startTime = new TimeSpan(0, 0, 0);
	    private TimeSpan endTime = new TimeSpan(23, 59, 59);
		private DayOfWeek[] weekDays = (DayOfWeek[]) DayOfWeek.GetValues(typeof (DayOfWeek));

		public FilterTrigger() : this(new DateTimeProvider())
		{
		}

		public FilterTrigger(DateTimeProvider dtProvider)
		{
			this.dtProvider = dtProvider;
		}

		[ReflectorProperty("trigger", InstanceTypeKey="type")]
		public ITrigger InnerTrigger;

        [ReflectorProperty("startTime", Required = false)]
		public string StartTime
		{
			get { return startTime.ToString(); }
			set { startTime = ParseTime(value); }
		}

		[ReflectorProperty("endTime", Required = false)]
		public string EndTime
		{
			get { return endTime.ToString(); }
			set { endTime = ParseTime(value); }
		}

		private TimeSpan ParseTime(string timeString)
		{
			return TimeSpan.Parse(timeString);
		}


		[ReflectorProperty("buildCondition", Required=false)]
		public BuildCondition BuildCondition = BuildCondition.NoBuild;

		private bool IsInFilterRange(DateTime now)
		{
			return IsDateInFilterWeekDays(now) && IsTimeInFilterTimeRange(now);
		}

		private bool IsDateInFilterWeekDays(DateTime dateTime)
		{
			return Array.IndexOf(WeekDays, dateTime.DayOfWeek) >= 0;
		}

		private bool IsTimeInFilterTimeRange(DateTime dateTime)
		{
			TimeSpan timeOfDay = dateTime.TimeOfDay;
			if (startTime < endTime){
				return timeOfDay >= startTime && dateTime.TimeOfDay <= endTime;
			} 
			else 
			{
				return !(timeOfDay <= startTime) || !(dateTime.TimeOfDay >= endTime);
			}
		}

		public void IntegrationCompleted()
		{
			InnerTrigger.IntegrationCompleted();
		}

		public DateTime NextBuild
		{
			get
			{
				DateTime innerTriggerBuild = InnerTrigger.NextBuild;
				if (IsInFilterRange(innerTriggerBuild))
				{
					DateTime nextBuild = new DateTime(innerTriggerBuild.Year, innerTriggerBuild.Month, innerTriggerBuild.Day);
					nextBuild += endTime;
					return nextBuild;
				}
				return innerTriggerBuild;
			}
		}

		public IntegrationRequest Fire()
		{
			DateTime now = dtProvider.Now;
			if (IsInFilterRange(now))
			{
				return null;
			}
			return InnerTrigger.Fire();
		}

		[ReflectorArray("weekDays", Required=false)]
		public DayOfWeek[] WeekDays
		{
			get { return weekDays; }
			set
			{
				if (value.Length != 0)
				{
					weekDays = value;
				}
			}
		}
	}
}