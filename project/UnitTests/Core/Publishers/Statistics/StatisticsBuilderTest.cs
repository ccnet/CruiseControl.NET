using System;
using System.Collections;
using System.IO;
using System.Xml;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Core.Publishers.Statistics;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers.Statistics
{
	[TestFixture]
	public class StatisticsBuilderTest : XmlLogFixture
	{
		private const string outDir = "temp";
		private StatisticsBuilder builder;
		IntegrationResult result;
		private string xml;

		[TestFixtureSetUp]
		public void LoadXML()
		{
			StreamReader reader = File.OpenText("buildlog.xml");
			xml = reader.ReadToEnd();
			reader.Close();
		}

		[SetUp]
		public void SetUp()
		{
			builder = new StatisticsBuilder();
			result = IntegrationResultMother.Create(true);
			Directory.CreateDirectory(outDir);
		}


		private string xmlResult()
		{
			return toXml(result);
		}

		private string toXml(IIntegrationResult result)
		{
			StringWriter xmlResultString = new StringWriter();
			XmlIntegrationResultWriter writer = new XmlIntegrationResultWriter(xmlResultString);
			writer.Write(result);
			return xmlResultString.ToString();
		}

		[TearDown]
		public void TearDown()
		{
			Directory.Delete("temp", true);
		}

		public void AssertHasStatistic(string name, object value)
		{
			Assert.AreEqual(value, builder.Statistic(name), "Wrong statistic for {0}", name);
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

			builder.ProcessBuildResults(xmlResult());

			AssertHasStatistic("TestCount", 7);
			AssertHasStatistic("TestFailures", 2);
			AssertHasStatistic("TestIgnored", 3);

		}

		[Test]
		public void ShouldCollectFxCopStatistics()
		{
			builder.ProcessBuildResults(xml);

			AssertHasStatistic("FxCop Warnings", 1);
			AssertHasStatistic("FxCop Errors", 205);

		}

		[Test]
		public void ShouldPopulateTimingsFromIntegrationResult()
		{
			result.StartTime = new DateTime(2005, 03, 12, 01, 13, 00);
			result.EndTime = new DateTime(2005, 03, 12, 01, 45, 00);
			result.ProjectName = "Foo";

			builder.ProcessBuildResults(xmlResult());

			AssertHasStatistic("StartTime", result.StartTime.ToString());
			AssertHasStatistic("Duration", new TimeSpan(0, 32, 0).ToString());
			AssertHasStatistic("ProjectName", "Foo");

		}

		[Test]
		public void ShouldWriteStatisticsAsXml()
		{
			builder.ProcessBuildResults(xmlResult());
			StringWriter writer = new StringWriter();
			builder.Save(writer);
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