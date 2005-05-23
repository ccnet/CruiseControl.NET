using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	/// <summary>
	///  Polls a number of IPollable things
	/// </summary>
	/// <remarks>
	///	 Deliberately uses the System.Windows.Forms timer to avoid any threading issues
	/// </remarks>
	public class Poller
	{
		Timer timer;
		IPollable[] itemsToPoll;

		public Poller(int pollIntervalMilliseconds, params IPollable[] itemsToPoll)
		{
			timer = new Timer();
			timer.Interval = pollIntervalMilliseconds;
			timer.Tick += new EventHandler(Timer_Tick);
			this.itemsToPoll = itemsToPoll;
		}


		private void Timer_Tick(object sender, EventArgs e)
		{
			Debug.WriteLine("Poller is polling...");

			foreach (IPollable toPoll in itemsToPoll)
			{
				try
				{
					toPoll.Poll();
				}
				catch (Exception ex)
				{
					Debug.WriteLine("Execption ignored during polling: " + ex);
				}
			}

			Debug.WriteLine("Polling complete");
		}

		public void Start()
		{
			timer.Start();
		}
	}
}
