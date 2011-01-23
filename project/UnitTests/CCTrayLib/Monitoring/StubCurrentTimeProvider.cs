using System;
using ThoughtWorks.CruiseControl.CCTrayLib;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	public class StubCurrentTimeProvider : DateTimeProvider
	{
		private DateTime now;

		public override DateTime Now
		{
			get { return now; }
		}

		public void SetNow(DateTime dateTime)
		{
			now = dateTime;
		}
	}
}