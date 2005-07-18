using System.Diagnostics;
using System.Threading;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	/// <summary>
	///  Polls a an IPollable thing
	/// </summary>
	public class Poller
	{
		private Timer timer;
		private IPollable itemToPoll;
		private readonly int pollIntervalMilliseconds;

		public Poller(int pollIntervalMilliseconds, IPollable itemToPoll)
		{
			this.itemToPoll = itemToPoll;
			this.pollIntervalMilliseconds = pollIntervalMilliseconds;
		}


		private void Timer_Elapsed(object args)
		{
			Debug.WriteLine("Polling...");
			itemToPoll.Poll();
		}

		public void Start()
		{
			Stop();
			timer = new Timer(new TimerCallback(Timer_Elapsed), null, 0, pollIntervalMilliseconds);
		}

		public void Stop()
		{
			if (timer != null)
			{
				timer.Dispose();
				timer = null;
			}
		}
	}
}