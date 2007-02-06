using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Publishers.Statistics;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers.Statistics
{
	[TestFixture]
	public class StatisticsPublisherTest
	{
		private SystemPath testDir;

		[SetUp]
		public void CreateFakeOutputDir()
		{
			testDir = SystemPath.UniqueTempPath();
		}

		[TearDown]
		public void DeleteTempDirectory()
		{
			testDir.DeleteDirectory();
		}

		[Test]
		public void CreatesStatisticsFileInArtifactDirectory()
		{
			IntegrationResult result1 = SimulateBuild(1);

			string statsFile = Path.Combine(result1.ArtifactDirectory, StatisticsPublisher.XmlFileName);
			Assert.IsTrue(File.Exists(statsFile));

			CountNodes(statsFile, "//statistics/integration", 1);

			IntegrationResult result2 = SimulateBuild(2);

			string statsFile2 = Path.Combine(result2.ArtifactDirectory, StatisticsPublisher.XmlFileName);
			Assert.IsTrue(File.Exists(statsFile2));

			CountNodes(statsFile2, "//statistics/integration", 2);
		}

		[Test]
		public void CreatesCsvFileInArtifactsDirectory()
		{
			IntegrationResult result1 = SimulateBuild(1);

			string statsFile = Path.Combine(result1.ArtifactDirectory, StatisticsPublisher.CsvFileName);
			Assert.IsTrue(File.Exists(statsFile));

			CountLines(statsFile, 2);

			IntegrationResult result2 = SimulateBuild(2);

			string statsFile2 = Path.Combine(result2.ArtifactDirectory, StatisticsPublisher.CsvFileName);
			Assert.IsTrue(File.Exists(statsFile2));

			CountLines(statsFile2, 3);
		}

		[Test]
		public void LoadStatistics()
		{
			testDir.CreateDirectory().CreateTextFile(StatisticsPublisher.XmlFileName, "<statistics />");
			XmlDocument statisticsDoc = StatisticsPublisher.LoadStatistics(testDir.ToString());

			XPathNavigator navigator = statisticsDoc.CreateNavigator();
			XPathNodeIterator nodeIterator = navigator.Select("//timestamp/@day");
			
			nodeIterator.MoveNext();
			int day = Convert.ToInt32(nodeIterator.Current.Value);
			Assert.AreEqual(DateTime.Now.Day, day);

			nodeIterator = navigator.Select("//timestamp/@month");
			nodeIterator.MoveNext();
			string month = nodeIterator.Current.Value;
			Assert.AreEqual(DateTime.Now.ToString("MMM"), month);

			nodeIterator = navigator.Select("//timestamp/@year");
			nodeIterator.MoveNext();
			int year = Convert.ToInt32(nodeIterator.Current.Value);
			Assert.AreEqual(DateTime.Now.Year, year);
		}

		private IntegrationResult SimulateBuild(int buildLabel)
		{
			StatisticsPublisher publisher = new StatisticsPublisher();

			IntegrationResult result = IntegrationResultMother.CreateSuccessful(buildLabel.ToString());
			result.ArtifactDirectory = testDir.ToString();
			
			publisher.Run(result);
			
			return result;
		}

		private void CountLines(string file, int expectedCount)
		{
			StreamReader text = File.OpenText(file);
			string s = text.ReadToEnd();
			string[] split = s.Split('\n');
			int count = 0;
			foreach(string line in split)
			{
				if (line.Length>0) count++;
			}
			Assert.AreEqual(expectedCount, count);
			text.Close();
		}

		private static void CountNodes(string statsFile2, string xpath, int count)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(statsFile2);
			XmlNodeList node = doc.SelectNodes(xpath);
			Assert.AreEqual(count, node.Count);
		}
	}
}