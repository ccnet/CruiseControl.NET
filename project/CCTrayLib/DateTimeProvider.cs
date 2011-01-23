using System;
using System.Threading;

namespace ThoughtWorks.CruiseControl.CCTrayLib
{
	public class DateTimeProvider
	{
		public virtual DateTime Now
		{
			get { return DateTime.Now; }
		}

		public virtual void Sleep(int milliseconds)
		{
			Thread.Sleep(milliseconds);
		}
	}
}
