using Exortech.NetReflector;
using System;
using System.Globalization;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Schedules
{
	[ReflectorType("daily")]
	public class DailySchedule : ISchedule
	{
		private DateTimeProvider _dtProvider;
		private TimeSpan _integrationTime;
		private DateTime _nextIntegration;

		public DailySchedule() : this(new DateTimeProvider()) {}

		public DailySchedule(DateTimeProvider dtProvider)
		{
			_dtProvider = dtProvider;
		}

		[ReflectorProperty("integrationTime")]
		public string IntegrationTime
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

		[ReflectorProperty("buildCondition", Required=false)]
		public BuildCondition BuildCondition = BuildCondition.IfModificationExists;

		private void SetNextIntegrationDateTime()
		{
			DateTime now = _dtProvider.Now;
			_nextIntegration = new DateTime(now.Year, now.Month, now.Day, _integrationTime.Hours, _integrationTime.Minutes, 0, 0);
			if (now >= _nextIntegration)
			{
				_nextIntegration = _nextIntegration.AddDays(1);
			}
		}

		public bool ShouldStopIntegration()
		{
			return false;
		}

		public void IntegrationCompleted()
		{
			SetNextIntegrationDateTime();
		}

		public BuildCondition ShouldRunIntegration()
		{
			if (_dtProvider.Now > _nextIntegration)
			{
				return BuildCondition.IfModificationExists;
			}
			return BuildCondition.NoBuild;
		}
	}
}
