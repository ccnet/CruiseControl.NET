using System;
using System.Globalization;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Schedules
{
	public class ScheduleTrigger : ITrigger
	{
		private DateTimeProvider _dtProvider;
		private BuildCondition buildCondition;
		private TimeSpan _integrationTime;
		private DateTime _nextIntegration;

		public ScheduleTrigger() : this(new DateTimeProvider()) {}
		public ScheduleTrigger(DateTimeProvider dtProvider)
		{
			_dtProvider = dtProvider;
			this.buildCondition = BuildCondition.NoBuild;
		}

		public virtual string Time
		{
			get { return _integrationTime.ToString(); }
			set 
			{ 
				try
				{
					_integrationTime = TimeSpan.Parse(value);
					SetNextIntegrationDateTime();
				}
				catch (Exception ex)
				{
					string msg = "Unable to parse daily schedule integration time: {0}.  The integration time should be specified in the format: {1}.";
					throw new ConfigurationException(string.Format(msg, value, CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern), ex);
				}
			}
		}

		public virtual BuildCondition BuildCondition
		{
			get { return buildCondition; }
			set { buildCondition = value; }
		}

		private void SetNextIntegrationDateTime()
		{
			DateTime now = _dtProvider.Now;
			_nextIntegration = new DateTime(now.Year, now.Month, now.Day, _integrationTime.Hours, _integrationTime.Minutes, 0, 0);
			if (now >= _nextIntegration)
			{
				_nextIntegration = _nextIntegration.AddDays(1);
			}
		}

		public virtual void IntegrationCompleted()
		{
			SetNextIntegrationDateTime();
		}

		public virtual BuildCondition ShouldRunIntegration()
		{
			if (_dtProvider.Now > _nextIntegration)
			{
				return BuildCondition;
			}
			return BuildCondition.NoBuild;
		}
	}
}
