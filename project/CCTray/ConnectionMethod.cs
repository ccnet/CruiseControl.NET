using System;

namespace ThoughtWorks.CruiseControl.Remote.Monitor
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
