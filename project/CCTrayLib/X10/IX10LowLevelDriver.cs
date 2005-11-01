namespace ThoughtWorks.CruiseControl.CCTrayLib.X10
{
	public interface IX10LowLevelDriver
	{
		/// <summary>
		/// This is the interface for actually sending individual commands to a particular device
		/// </summary>
		/// <param name="deviceCode">Device code to be operated on</param>
		/// <param name="deviceCommand">Command to send to device</param>
		/// <param name="lightLevel">if Dimming or Brightening then this is a percentage to raise or lower
		/// light level.  If not Dimming or Brightening then this parameter is ignored </param>
		void ControlDevice(int deviceCode, Function deviceCommand, int lightLevel);
	}
}