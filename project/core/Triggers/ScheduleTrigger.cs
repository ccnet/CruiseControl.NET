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
		private DateTime nextIntegration;

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
					SetNextIntegrationDateTime();
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

		private void SetNextIntegrationDateTime()
		{
			DateTime now = dtProvider.Now;
			nextIntegration = new DateTime(now.Year, now.Month, now.Day, integrationTime.Hours, integrationTime.Minutes, 0, 0);
			if (now >= nextIntegration)
			{
				nextIntegration = nextIntegration.AddDays(1);
			}

			nextIntegration = CalculateNextIntegrationTime(nextIntegration);
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
			SetNextIntegrationDateTime();
		}

		public DateTime NextBuild
		{
			get { return nextIntegration; }
		}

		public virtual BuildCondition ShouldRunIntegration()
		{
			DateTime now = dtProvider.Now;
			if (now > nextIntegration && IsValidWeekDay(now.DayOfWeek))
			{
				return BuildCondition;
			}
			return BuildCondition.NoBuild;
		}
	}
}