using System;
using NUnit.Framework;
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
			Assert.AreEqual(1, stats.GetTotalSuccessfulBuilds());
		}

		public void TestTotalSuccessfulBuilds_NoLogs()
		{
			stats = new LogStatistics(new string[] {});
			Assert.AreEqual(0, stats.GetTotalSuccessfulBuilds());
		}

		public void TestTotalFailedBuilds()
		{
			Assert.AreEqual(3, stats.GetTotalFailedBuilds());
		}

		public void TestTotalFailedBuilds_NoLogs()
		{
			stats = new LogStatistics(new string[] {});
			Assert.AreEqual(0, stats.GetTotalFailedBuilds());
		}
	
		public void TestSuccessRatio()
		{
			Assert.AreEqual(.25, stats.GetSuccessRatio());
		}

		public void TestSuccessRatio_NoLog()
		{
			stats = new LogStatistics(new string[] {});
			Assert.AreEqual(0, stats.GetSuccessRatio());
		}

		public void TestIsCurrentBuildSuccessful()
		{
			Assert.AreEqual(false, stats.IsLatestBuildSuccessful());
		}

		public void TestIsCurrentBuildSuccessful_NoLog()
		{
			stats = new LogStatistics(new string[] {});
			Assert.AreEqual(false, stats.IsLatestBuildSuccessful());
		}

		public void TestGetTimeSinceLatestBuild()
		{
			DateTime latest = new DateTime(2002, 10, 02, 09, 55, 08);
			TimeSpan span = DateTime.Now - latest;
			Assert.IsTrue(stats.GetTimeSinceLatestBuild().Ticks >= span.Ticks, "Returned date time is incorrect");
		}

		public void TestGetTimeSinceLatestBuild_NoLog()
		{
			stats = new LogStatistics(new string[] {});
			Assert.AreEqual(new TimeSpan(0), stats.GetTimeSinceLatestBuild());
		}

		public void TestGetTimeSinceLatestBuildString_NoLog()
		{
			stats = new LogStatistics(new string[] {});
			Assert.AreEqual("0 minutes", stats.GetTimeSinceLatestBuildString());
		}
	}
}
