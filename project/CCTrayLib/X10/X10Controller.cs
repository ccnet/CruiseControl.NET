using System;
using System.Diagnostics;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.X10
{
	public class X10Controller
	{
		private readonly ILampController lampController;

		public X10Controller(IProjectMonitor monitor, ILampController lampController)
		{
			this.lampController = lampController;
		
			monitor.Polled += new MonitorPolledEventHandler(Monitor_Polled);
		}

		private void Monitor_Polled(object sender, MonitorPolledEventArgs args)
		{
			try
			{
				switch (args.ProjectMonitor.IntegrationStatus)
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
	}
}