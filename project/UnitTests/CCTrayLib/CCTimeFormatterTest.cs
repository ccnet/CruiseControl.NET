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
			CCTimeFormatter formatter = new CCTimeFormatter(new TimeSpan(1, 1, 6, 30));
			Assert.AreEqual("1 Day(s) 1 Hour(s) 6 Minute(s)", formatter.ToString());
		}

		[Test]
		public void ShouldNotDisplayMinutesIfZero()
		{
			CCTimeFormatter formatter = new CCTimeFormatter(new TimeSpan(1, 1, 0, 30));
			Assert.AreEqual("1 Day(s) 1 Hour(s)", formatter.ToString());
		}
		[Test]
		public void ShouldDisplayNowBuildingIfTimeSpanIsZero()
		{
			CCTimeFormatter formatter = new CCTimeFormatter(new TimeSpan(0, 0, 0, 14));
			Assert.AreEqual("Now Building", formatter.ToString());
		}
	}
}