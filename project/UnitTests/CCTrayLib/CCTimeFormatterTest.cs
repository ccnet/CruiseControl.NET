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
			Assert.AreEqual("2 Days 2 Hours 6 Minutes", formatter.ToString());
		}

		[Test]
		public void ShouldDisplayInDDHHMMFormatIgnoringPluralsIfNumberIsOne()
		{
			CCTimeFormatter formatter = new CCTimeFormatter(new TimeSpan(1, 1, 1, 30));
			Assert.AreEqual("1 Day 1 Hour 1 Minute", formatter.ToString());
		}

		[Test]
		public void ShouldNotDisplayMinutesIfZero()
		{
			CCTimeFormatter formatter = new CCTimeFormatter(new TimeSpan(1, 1, 0, 30));
			Assert.AreEqual("1 Day 1 Hour", formatter.ToString());
		}

		[Test]
		public void ShouldDisplayNowBuildingIfTimeSpanIsLessThanTwoSeconds()
		{
			CCTimeFormatter formatter = new CCTimeFormatter(new TimeSpan(0, 0, 0, 1));
			Assert.AreEqual("Now Building", formatter.ToString());
		}

		[Test]
		public void ShouldDisplayInSecondsIfLessThanOneMinute()
		{
			CCTimeFormatter formatter = new CCTimeFormatter(new TimeSpan(0, 0, 0, 30));
			Assert.AreEqual("30 seconds", formatter.ToString());
		}
	}
}