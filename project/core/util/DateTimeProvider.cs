#pragma warning disable 1591
using System;
using System.Threading;

namespace ThoughtWorks.CruiseControl.Core.Util
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
		
		public virtual void Sleep(TimeSpan duration)
		{
			Thread.Sleep(duration);
		}

		public virtual DateTime Today
		{
			get { return DateTime.Today; }
		}
	}
}
