using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Util
{
	[TestFixture]
	public class TimeoutTest
	{
		[Test]
		public void DefaultTimeoutIsInMillis()
		{
			Timeout timeout = new Timeout(100);
			Assert.AreEqual(100, timeout.Millis);
			Assert.AreEqual(new Timeout(100, TimeUnits.MILLIS), timeout);
		}

		[Test]
		public void CanSpecifyTimeoutInSeconds()
		{
			Timeout period = new Timeout(1, TimeUnits.SECONDS);
			Assert.AreEqual(1000, period.Millis);
			Assert.AreEqual(new Timeout(1000, TimeUnits.MILLIS), period);
		}

		[Test]
		public void CanSpecifyTimeoutInMinutes()
		{
			Timeout period = new Timeout(1, TimeUnits.MINUTES);
			Assert.AreEqual(60*1000, period.Millis);
			Assert.AreEqual(new Timeout(60*1000, TimeUnits.MILLIS), period);
		}

		[Test]
		public void CanSpecifyTimeoutInHours()
		{
			Timeout period = new Timeout(1, TimeUnits.HOURS);
			Assert.AreEqual(60*60*1000, period.Millis);
			Assert.AreEqual(new Timeout(60*60*1000, TimeUnits.MILLIS), period);
		}
	}
}