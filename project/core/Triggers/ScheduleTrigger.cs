using System;
using System.Globalization;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Triggers
{
	[ReflectorType("scheduleTrigger")]
	public class ScheduleTrigger : ITrigger
	{
		private DateTimeProvider dtProvider;
		private TimeSpan integrationTime;
		private DateTime nextBuild;

		public ScheduleTrigger() : this(new DateTimeProvider())
		{
		}

		public ScheduleTrigger(DateTimeProvider dtProvider)
		{
			this.dtProvider = dtProvider;
		}

		[ReflectorProperty("time")]
		public virtual string Time
		{
			get { return integrationTime.ToString(); }
			set
			{
				try
				{
					integrationTime = TimeSpan.Parse(value);
				}
				catch (Exception ex)
				{
					string msg = "Unable to parse daily schedule integration time: {0}.  The integration time should be specified in the format: {1}.";
					throw new ConfigurationException(string.Format(msg, value, CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern), ex);
				}
			}
		}

		[ReflectorProperty("buildCondition", Required=false)] 
		public BuildCondition BuildCondition = BuildCondition.IfModificationExists;

		[ReflectorArray("weekDays", Required=false)] 
		public DayOfWeek[] WeekDays = (DayOfWeek[]) DayOfWeek.GetValues(typeof (DayOfWeek));

		private bool triggered;

		private void SetNextIntegrationDateTime()
		{
			DateTime now = dtProvider.Now;
			nextBuild = new DateTime(now.Year, now.Month, now.Day, integrationTime.Hours, integrationTime.Minutes, 0, 0);
			if (now >= nextBuild)
			{
				nextBuild = nextBuild.AddDays(1);
			}

			nextBuild = CalculateNextIntegrationTime(nextBuild);
		}

		private DateTime CalculateNextIntegrationTime(DateTime nextIntegration)
		{
			while (true)
			{
				if (IsValidWeekDay(nextIntegration.DayOfWeek))
					break;
				nextIntegration = nextIntegration.AddDays(1);
			}
			return nextIntegration;
		}

		private bool IsValidWeekDay(DayOfWeek nextIntegrationDay)
		{
			return Array.IndexOf(WeekDays, nextIntegrationDay) >= 0;
		}

		public virtual void IntegrationCompleted()
		{
			if (triggered) SetNextIntegrationDateTime();
			triggered = false;
		}

		public DateTime NextBuild
		{
			get 
			{
				if (nextBuild == DateTime.MinValue)
				{
					SetNextIntegrationDateTime();
				}
				return nextBuild; 
			}
		}

		public virtual BuildCondition ShouldRunIntegration()
		{
			DateTime now = dtProvider.Now;
			if (now > NextBuild && IsValidWeekDay(now.DayOfWeek))
			{
				triggered = true;
				return BuildCondition;
			}
			return BuildCondition.NoBuild;
		}
	}
}