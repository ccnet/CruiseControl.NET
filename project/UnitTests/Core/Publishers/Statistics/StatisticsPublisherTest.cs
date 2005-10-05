using System;
using System.Collections;
using System.IO;
using System.Xml;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers.Statistics
{
	

	
	[TestFixture]
	public class StatisticsPublisherTest : XmlLogFixture
	{
		private const string outDir = "temp";
		private StatisticsPublisher publisher;
		IntegrationResult result;

		[SetUp]
		public void SetUp()
		{
			publisher = new StatisticsPublisher();
			result = IntegrationResultMother.Create(true);
			Directory.CreateDirectory(outDir);
		}

		private string xmlResult()
		{
			return IntegrationResultHelper.toXml(result);
		}

		[TearDown]
		public void TearDown()
		{
			Directory.Delete("temp", true);
		}

		public void AssertHasStatistic(string name, object value)
		{
			Assert.AreEqual(value, publisher.Statistic(name), "Wrong statistic for {0}", name);
		}

		[Test]
		public void ShouldPopulateNUnitSummaryFromLog()
		{
			string xml =
				@"<task>
					<test-results total=""6"" failures=""1"" not-run=""2"" date=""2005-04-29"" time=""9:02 PM"">
						<test-suite />
					</test-results>
					<test-results total=""1"" failures=""1"" not-run=""1"" date=""2005-04-29"" time=""9:02 PM"">
						<test-suite />
					</test-results>
				</task>";

			result.AddTaskResult(xml);

			publisher.ProcessBuildResults(xmlResult());

			AssertHasStatistic("TestCount", 7);
			AssertHasStatistic("TestFailures", 2);
			AssertHasStatistic("TestIgnored", 3);

		}

		[Test]
		public void ShouldCollectFxCopStatistics()
		{
			publisher.ProcessFile(@"E:\dev\dotnet\ccnet-buildfiles\log20050303163138Lbuild.0_8_0_782.xml");

			AssertHasStatistic("FxCop Warnings", 8);
			AssertHasStatistic("FxCop Errors", 3079);

		}

		[Test]
		public void ShouldPopulateTimingsFromIntegrationResult()
		{
			result.StartTime = new DateTime(2005, 03, 12, 01, 13, 00);
			result.EndTime = new DateTime(2005, 03, 12, 01, 45, 00);
			result.ProjectName = "Foo";

			publisher.ProcessBuildResults(xmlResult());

			AssertHasStatistic("StartTime", result.StartTime.ToString());
			AssertHasStatistic("Duration", new TimeSpan(0, 32, 0).ToString());
			AssertHasStatistic("ProjectName", "Foo");

		}

		[Test]
		public void ShouldWriteStatisticsAsXml()
		{
			publisher.ProcessBuildResults(xmlResult());
			StringWriter writer = new StringWriter();
			publisher.Save(writer);
			string xml = writer.ToString();
			AssertXPath(xml, "//statistics/statistic/@name", "ProjectName");
		}

		private void AssertXPath(string xml, string xpath, string expectedValue)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);
			XmlNodeList node = doc.SelectNodes(xpath);
			ArrayList actuals = new ArrayList();
			foreach (XmlNode n in node)
			{
				string actual = n.Value;
				actuals.Add(actual);
				if (actual == expectedValue) return;
			}
			Assert.Fail("No node found matching {0}. Actuals were {1}", expectedValue, actuals);
		}

	}
}