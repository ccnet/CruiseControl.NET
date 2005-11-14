using System;
using System.Diagnostics;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.X10
{
	public class X10Controller
	{
		private readonly ILampController lampController;
		private readonly DateTimeProvider dateTimeProvider;
		private readonly X10Configuration configuration;

		public X10Controller(IProjectMonitor monitor, ILampController lampController, DateTimeProvider dateTimeProvider,
		                     X10Configuration configuration)
		{
			this.lampController = lampController;
			this.dateTimeProvider = dateTimeProvider;
			this.configuration = configuration;

			monitor.Polled += new MonitorPolledEventHandler(Monitor_Polled);
		}

		private void Monitor_Polled(object sender, MonitorPolledEventArgs args)
		{
			try
			{
				if (!IsInsideLampSwitchingHours)
				{
					lampController.GreenLightOn = false;
					lampController.RedLightOn = false;
					return;
				}

				switch (((IProjectMonitor)sender).IntegrationStatus)
				{
					case IntegrationStatus.Success:
						lampController.GreenLightOn = true;
						lampController.RedLightOn = false;
						break;

					case IntegrationStatus.Exception:
					case IntegrationStatus.Failure:
						lampController.GreenLightOn = false;
						lampController.RedLightOn = true;
						break;

					default:
						lampController.GreenLightOn = true;
						lampController.RedLightOn = true;
						break;
				}
			}
			catch (ApplicationException ex)
			{
				Trace.WriteLine("Failed to update X10 device status: " + ex);
			}
		}

		private bool IsInsideLampSwitchingHours
		{
			get
			{
				DateTime now = dateTimeProvider.Now;
				return ((now.DayOfWeek >= configuration.StartDay && now.DayOfWeek <= configuration.EndDay) &&
				        (now.TimeOfDay >= configuration.StartTime.TimeOfDay && now.TimeOfDay < configuration.EndTime.TimeOfDay));

			}
		}
	}
}