using Exortech.NetReflector;
using System;
using System.Globalization;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Schedules
{
	[ReflectorType("daily")]
	public class DailySchedule : ISchedule
	{
		private TimeSpan _integrationTime;
		private DateTime _nextIntegration;

		[ReflectorProperty("integrationTime")]
		public string IntegrationTime
		{
			get { return _integrationTime.ToString(); }
			set 
			{ 
				try
				{
					_integrationTime = TimeSpan.Parse(value);
					SetNextIntegration();
				}
				catch (Exception ex)
				{
					string msg = "Unable to parse daily schedule integration time: {0}.  The integration time should be specified in the format: {1}.";
					throw new ConfigurationException(string.Format(msg, value, CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern), ex);
				}
			}
		}

		private void SetNextIntegration()
		{
			DateTime now = Now;
			_nextIntegration = new DateTime(now.Year, now.Month, now.Day, _integrationTime.Hours, _integrationTime.Minutes, 0, 0);
		}

		public bool ShouldStopIntegration()
		{
			return false;
		}

		public void ForceBuild()
		{
		
		}

		public void IntegrationCompleted()
		{
		}

		public BuildCondition ShouldRunIntegration()
		{
			if (Now > _nextIntegration)
			{
				return BuildCondition.IfModificationExists;
			}
			return BuildCondition.NoBuild;
		}

		protected virtual DateTime Now
		{
			get { return DateTime.Now; }
		}
	}
}
