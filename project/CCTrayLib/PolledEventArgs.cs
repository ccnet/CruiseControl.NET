using System;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib
{
	public class PolledEventArgs : EventArgs
	{
		public readonly ProjectStatus ProjectStatus;

		public PolledEventArgs (ProjectStatus projectStatus)
		{
			ProjectStatus = projectStatus;
		}
	}

	public delegate void PolledEventHandler (object sauce, PolledEventArgs e);

}