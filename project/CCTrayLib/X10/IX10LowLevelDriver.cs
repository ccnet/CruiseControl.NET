using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.X10;

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

        // this is a bit icky - this is used to reset the text on a Forms.Label control to nothing.
        // It has the side effect of passing the driver a reference to the desired Label. LowLevel
        // drivers can use this Label to show output when simulating success or failure. 
        void ResetStatus(Label statusLabel);

        /// <summary>
        /// This closes any open ports and releases any locks the driver might have aquiried.
        /// </summary>
        void CloseDriver();
	}
}
