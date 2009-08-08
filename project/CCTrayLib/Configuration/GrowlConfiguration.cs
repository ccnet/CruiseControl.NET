using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
	public class GrowlConfiguration
	{
		public bool Enabled = false;
		public string Hostname = string.Empty;
		public int Port = 0;
		public string Password = string.Empty;
		public NotifyInfoFlags MinimumNotificationLevel;

	}
}
