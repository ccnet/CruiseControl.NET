using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
	public class GrowlConfiguration
	{
		public bool Enabled = false;
		public string Hostname = string.Empty;
		public int Port = 0;
		public string Password = string.Empty;
        public ToolTipIcon MinimumNotificationLevel;

	}
}
