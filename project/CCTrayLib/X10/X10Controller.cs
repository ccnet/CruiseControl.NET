using System;
using System.Diagnostics;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.CCTrayLib.X10
{
	public class X10Controller
	{
		private readonly ILampController lampController;
		private readonly DateTimeProvider dateTimeProvider;
		private readonly X10Configuration configuration;

		public X10Controller(IProjectMonitor monitor, DateTimeProvider dateTimeProvider, X10Configuration configuration, ILampController lampController)
		{
			if (configuration != null && configuration.Enabled)
			{
				Trace.WriteLine("New X10Controller created");
                this.lampController = lampController;
				this.dateTimeProvider = dateTimeProvider;
				this.configuration = configuration;
	
				monitor.Polled += new MonitorPolledEventHandler(Monitor_Polled);
			}
		}

		private void Monitor_Polled(object sender, MonitorPolledEventArgs args)
		{
			Debug.WriteLine("X10Controller.Monitor_Polled");
			try
			{
				if (!IsInsideLampSwitchingHours)
				{
                    Cursor.Current = Cursors.WaitCursor;
					lampController.GreenLightOn = false;
					lampController.RedLightOn = false;
                    Cursor.Current = Cursors.Default;
                    return;
				}

				switch (((IProjectMonitor)sender).IntegrationStatus)
				{
					case IntegrationStatus.Success:
                        Cursor.Current = Cursors.WaitCursor;
                        lampController.GreenLightOn = true;
						lampController.RedLightOn = false;
                        Cursor.Current = Cursors.Default;
                        break;

					case IntegrationStatus.Exception:
					case IntegrationStatus.Failure:
                        Cursor.Current = Cursors.WaitCursor;
                        lampController.GreenLightOn = false;
						lampController.RedLightOn = true;
                        Cursor.Current = Cursors.Default;
                        break;

					default:
                        Cursor.Current = Cursors.WaitCursor;
                        lampController.GreenLightOn = true;
						lampController.RedLightOn = true;
                        Cursor.Current = Cursors.Default;
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
                bool isTodayActive = configuration.ActiveDays[(int)now.DayOfWeek];
                bool isInTimeRange = now.TimeOfDay >= configuration.StartTime.TimeOfDay && now.TimeOfDay < configuration.EndTime.TimeOfDay;
                
                return (isTodayActive && isInTimeRange);
			}
		}
	}
}
