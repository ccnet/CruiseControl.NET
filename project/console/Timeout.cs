using System;
using System.Threading;

namespace ThoughtWorks.CruiseControl.Console
{
	public class Timeout : ITimeout
	{
		public void Wait()
		{
			new AutoResetEvent(false).WaitOne();
		}
	}
}
