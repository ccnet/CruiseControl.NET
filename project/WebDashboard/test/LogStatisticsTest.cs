using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Web.Test
{
	[TestFixture]
	public class LogStatisticsTest : CustomAssertion
	{
		private static readonly string[] logfiles = new string[] {
													 "log20021002095508.xml",
													 "log20020924144031.xml",
													 "log20020916143555.xml",
													 "log20020910183219Lbuild.222.xml" };
		private LogStatistics stats;

		[SetUp]
		protected void SetUp()
		{
			stats = new LogStatistics(logfiles);
		}

		public void TestTotalSuccessfulBuilds()
		{
			AssertEquals(1, stats.GetTotalSuccessfulBuilds());
		}

		public void TestTotalSuccessfulBuilds_NoLogs()
		{
			stats = new LogStatistics(new string[] {});
			AssertEquals(0, stats.GetTotalSuccessfulBuilds());
		}

		public void TestTotalFailedBuilds()
		{
			AssertEquals(3, stats.GetTotalFailedBuilds());
		}

		public void TestTotalFailedBuilds_NoLogs()
		{
			stats = new LogStatistics(new string[] {});
			AssertEquals(0, stats.GetTotalFailedBuilds());
		}
	
		public void TestSuccessRatio()
		{
			AssertEquals(.25, stats.GetSuccessRatio());
		}

		public void TestSuccessRatio_NoLog()
		{
			stats = new LogStatistics(new string[] {});
			AssertEquals(0, stats.GetSuccessRatio());
		}

		public void TestIsCurrentBuildSuccessful()
		{
			AssertEquals(false, stats.IsLatestBuildSuccessful());
		}

		public void TestIsCurrentBuildSuccessful_NoLog()
		{
			stats = new LogStatistics(new string[] {});
			AssertEquals(false, stats.IsLatestBuildSuccessful());
		}

		public void TestGetTimeSinceLatestBuild()
		{
			DateTime latest = new DateTime(2002, 10, 02, 09, 55, 08);
			TimeSpan span = DateTime.Now - latest;
			Assert("Returned date time is incorrect", 
				stats.GetTimeSinceLatestBuild().Ticks >= span.Ticks);
		}

		public void TestGetTimeSinceLatestBuild_NoLog()
		{
			stats = new LogStatistics(new string[] {});
			AssertEquals(new TimeSpan(0), stats.GetTimeSinceLatestBuild());
		}

		public void TestGetTimeSinceLatestBuildString_NoLog()
		{
			stats = new LogStatistics(new string[] {});
			AssertEquals("0 minutes", stats.GetTimeSinceLatestBuildString());
		}
	}
}
