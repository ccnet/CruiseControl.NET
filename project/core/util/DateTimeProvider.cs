using System;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class DateTimeProvider
	{
		public virtual DateTime Now
		{
			get { return DateTime.Now; }
		}
	}
}
