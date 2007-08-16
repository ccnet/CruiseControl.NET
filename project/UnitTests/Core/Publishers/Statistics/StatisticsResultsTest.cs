using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Publishers.Statistics;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers.Statistics
{
    [TestFixture]
    public class StatisticsResultsTest : CustomAssertion
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            builder = new StatisticsBuilder();
            successful = IntegrationResultMother.CreateSuccessful();
        }

        #endregion

        private StatisticsBuilder builder;
        private IntegrationResult successful;

        [Test]
        public void ShouldWriteStatisticsAsXml()
        {
            StatisticsResults results =
                builder.ProcessBuildResults(successful);
            StringWriter writer = new StringWriter();

            results.Save(writer);
            string xml = writer.ToString();
            Console.Out.WriteLine("xml = {0}", xml);
            AssertXPathExists(xml, "//statistics/statistic[@name='ProjectName']");
        }

        [Test]
        public void WriteHeadingShouldHaveCorrectNumberOfColumns()
        {
            StringBuilder buffer = new StringBuilder();
            StatisticsResults.WriteHeadings(new StringWriter(buffer), builder.Statistics);
            AssertStartsWith("\"BuildErrorType\", \"BuildErrorMessage\"", buffer.ToString());
        }

        [Test]
        public void WriteStatsShouldWriteStatValue()
        {
            StringBuilder buffer = new StringBuilder();
            StatisticsResults results = builder.ProcessBuildResults(successful);
            results.WriteStats(new StringWriter(buffer));
            AssertContains(successful.ProjectName, buffer.ToString());
        }
    }
}