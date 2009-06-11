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
		private IProjectMonitor monitor;
		
		public X10Controller(IProjectMonitor monitor, DateTimeProvider dateTimeProvider, X10Configuration configuration, ILampController lampController)
		{
			if (configuration != null && configuration.Enabled)
			{
				Trace.WriteLine("New X10Controller created");
                this.lampController = lampController;
				this.dateTimeProvider = dateTimeProvider;
				this.configuration = configuration;
				this.monitor = monitor;
	
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
					SetRedYellowGreenLamps(false,false,false);
                    return;
				}

				ProjectState state = monitor.ProjectState;
				IntegrationStatus status = ((IProjectMonitor)sender).IntegrationStatus;

				if (state.Equals(ProjectState.NotConnected) || status.Equals(IntegrationStatus.Unknown)) {
	             	SetRedYellowGreenLamps(false, false, false);
                 }
                 else {
					bool red = status.Equals(IntegrationStatus.Exception) ||
						       status.Equals(IntegrationStatus.Failure) ||
							   state.Equals(ProjectState.Broken) || 
							   state.Equals(ProjectState.BrokenAndBuilding);
					bool yellow = state.Equals(ProjectState.Building) || 
					              state.Equals(ProjectState.BrokenAndBuilding);
					bool green = (status.Equals(IntegrationStatus.Success) &&
								(state.Equals(ProjectState.Success) || (state.Equals(ProjectState.Building))));
	
  					SetRedYellowGreenLamps(red,yellow,green);
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
		
		private void SetRedYellowGreenLamps(bool red, bool yellow, bool green) {
		    Cursor.Current = Cursors.WaitCursor;
			lampController.RedLightOn = red;
		    lampController.YellowLightOn = yellow;
			lampController.GreenLightOn = green;
		    Cursor.Current = Cursors.Default;
		}
	}
}
