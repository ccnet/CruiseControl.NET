using System;

namespace ThoughtWorks.CruiseControl.Remote.monitor
{
	/// <summary>
	/// Enumeration of methods via which CCTray may connect to the CCNet server.
	/// </summary>
	public enum ConnectionMethod
	{
		Remoting,
		WebService
	}
}
