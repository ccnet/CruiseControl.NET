using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib
{
	[TestFixture]
	public class CCTimeFormatterTest
	{
		[Test]
		public void ShouldDisplayInDDHHMMFormat()
		{
			CCTimeFormatter formatter = new CCTimeFormatter(new TimeSpan(2, 2, 6, 30));
			Assert.AreEqual("2 days 2 hours 6 minutes", formatter.ToString());
		}

		[Test]
		public void ShouldDisplayInDDHHMMFormatIgnoringPluralsIfNumberIsOne()
		{
			CCTimeFormatter formatter = new CCTimeFormatter(new TimeSpan(1, 1, 1, 30));
			Assert.AreEqual("1 day 1 hour 1 minute", formatter.ToString());
		}

		[Test]
		public void ShouldNotDisplayMinutesIfZero()
		{
			CCTimeFormatter formatter = new CCTimeFormatter(new TimeSpan(1, 1, 0, 30));
			Assert.AreEqual("1 day 1 hour", formatter.ToString());
		}

		[Test]
		public void ShouldDisplayInSecondsIfLessThanOneMinute()
		{
			CCTimeFormatter formatter = new CCTimeFormatter(new TimeSpan(0, 0, 0, 30));
			Assert.AreEqual("30 seconds", formatter.ToString());
		}
	}
}