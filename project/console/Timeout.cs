using System;
using System.Threading;

namespace tw.ccnet.console
{
	public class Timeout : ITimeout
	{
		public void Wait()
		{
			new AutoResetEvent(false).WaitOne();
		}
	}
}
