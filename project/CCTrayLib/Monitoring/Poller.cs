using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Timers;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	/// <summary>
	///  Polls a an IPollable thing
	/// </summary>
	public class Poller
	{
		Timer timer;
		IPollable itemToPoll;

		public Poller(int pollIntervalMilliseconds, IPollable itemToPoll)
		{
			this.itemToPoll = itemToPoll;
			timer = new Timer();
			timer.Interval = pollIntervalMilliseconds;
			timer.AutoReset = true;
			timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
		}


		private void Timer_Elapsed( object sender, ElapsedEventArgs e )
		{
			itemToPoll.Poll();
		}

		public void Start()
		{
			timer.Start();
		}

		public void Stop()
		{
			timer.Stop();
		}
	}
}
