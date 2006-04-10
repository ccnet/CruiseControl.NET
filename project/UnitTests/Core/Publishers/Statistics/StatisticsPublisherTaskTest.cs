using System.IO;
using System.Xml;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Publishers.Statistics;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers.Statistics
{
	[TestFixture, Ignore("")]
	public class StatisticsPublisherTest
	{
		const string TEST_DIR = "build\\temp";
		private DirectoryInfo tmpdir;

		[SetUp]
		public void CreateFakeOutputDir()
		{
			if (Directory.Exists(TEST_DIR))
			{
				Directory.Delete(TEST_DIR, true);
			}
			tmpdir = Directory.CreateDirectory(TEST_DIR);
		}

		[TearDown]
		public void DeleteTempDirectory()
		{
			if (Directory.Exists(TEST_DIR))
			{
				Directory.Delete(TEST_DIR, true);
			}
		}

		[Test]
		public void CreatesStatisticsFileInArtifactDirectory()
		{
			IntegrationResult result1 = simulateBuild(1);

			string statsFile = result1.ArtifactDirectory + "\\statistics.xml";
			Assert.IsTrue(File.Exists(statsFile));

			CountNodes(statsFile, "//statistics/integration", 1);

			IntegrationResult result2 = simulateBuild(2);

			string statsFile2 = result2.ArtifactDirectory + "\\statistics.xml";
			Assert.IsTrue(File.Exists(statsFile2));

			CountNodes(statsFile2, "//statistics/integration", 2);

		}

		[Test]
		public void CreatesCsvFileInArtifactsDirectory()
		{
			IntegrationResult result1 = simulateBuild(1);

			string statsFile = result1.ArtifactDirectory + "\\statistics.csv";
			Assert.IsTrue(File.Exists(statsFile));

			CountLines(statsFile, 2);

			IntegrationResult result2 = simulateBuild(2);

			string statsFile2 = result2.ArtifactDirectory + "\\statistics.csv";
			Assert.IsTrue(File.Exists(statsFile2));

			CountLines(statsFile2, 3);

		}

		private IntegrationResult simulateBuild(int buildLabel)
		{
			StatisticsPublisher publisher = new StatisticsPublisher();

			IntegrationResult result = IntegrationResultMother.CreateSuccessful(buildLabel.ToString());
			result.LastSuccessfulIntegrationLabel = (buildLabel - 1).ToString();
			result.ArtifactDirectory = tmpdir.FullName;
			
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