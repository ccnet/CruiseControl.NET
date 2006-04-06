using System;
using System.IO;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Publishers.Statistics;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
	namespace Statistics
	{
		[TestFixture]
		public class MyStatisticsPublisherTest
		{
			private readonly string artifactDirectory = "./ArtifactDirectory";
			private string filePath;
			private MyStatisticsPublisher publisher;
			private BuildStatisticsProcessor processor;
			private DateTime now = DateTime.Now;
			private IntegrationResult failedBuild;
			private TimeSpan integrationTime;
			private IntegrationResult successfulBuild;
			private string successfulLabel = "successful123";
			private string failedLabel = "failed123";

			[TestFixtureSetUp]
			public void FixtureSetUp()
			{
				if (!Directory.Exists(artifactDirectory))
				{
					Directory.CreateDirectory(artifactDirectory);
				}
			}

			[SetUp]
			public void SetUp()
			{
				publisher = new MyStatisticsPublisher();
				publisher.FileName = "buildstatistics.xml";
				processor = new BuildStatisticsProcessor();

				integrationTime = new TimeSpan(0, 3, 0);
				failedBuild = IntegrationResultMother.Create(false, now.Add(-integrationTime));
				failedBuild.ProjectName = "Statistics Publisher";
				failedBuild.EndTime = now;
				failedBuild.Label = failedLabel;

				successfulBuild = IntegrationResultMother.Create(true, now.Add(-integrationTime));
				successfulBuild.ArtifactDirectory = artifactDirectory;
				successfulBuild.ProjectName = "Statistics Publisher";
				successfulBuild.EndTime = now;
				successfulBuild.Label = successfulLabel;
			}

			[Test]
			public void ShouldGenerateAFileInTheArtifactDirectory()
			{
				IMock mockIntegrationResult = new DynamicMock(typeof (IIntegrationResult));
				mockIntegrationResult.ExpectAndReturn("ArtifactDirectory", artifactDirectory);
				IIntegrationResult mockIntegrationResultInstance = (IIntegrationResult) mockIntegrationResult.MockInstance;
				IMock mockProcessor = new DynamicMock(typeof(BuildStatisticsProcessor));
				IntegrationStatistics statistics = new IntegrationStatistics();
				mockProcessor.ExpectAndReturn("ProcessBuildResults", statistics, mockIntegrationResultInstance);
				publisher.Processor = (BuildStatisticsProcessor) mockProcessor.MockInstance;
				publisher.Run(mockIntegrationResultInstance);
				filePath = Path.Combine(artifactDirectory, publisher.FileName);
				bool fileExists = File.Exists(filePath);
				Assert.IsTrue(fileExists);
				if (fileExists) File.Delete(filePath);
			}

			[Test]
			public void ShouldUpdateBuildStatistics()
			{
				
			}

			[Test]
			public void ShouldProcessIntegrationResultToGetIntegrationStatistics()
			{
				IntegrationStatistics statistics = processor.ProcessBuildResults(failedBuild);
				Assert.AreEqual(failedLabel, statistics.BuildLabel);
				Assert.AreEqual(integrationTime, statistics.IntegrationTime);
				Assert.AreEqual(IntegrationStatus.Failure, statistics.IntegrationStatus);
				Assert.AreEqual("Statistics Publisher", statistics.ProjectName);

				Assert.AreEqual(10, statistics.ConfiguredStats["test-count"]);
			}

			[Test]
			public void ShouldReturnIntegrationStatisticsWithCorrectBuildLabelAndStatusForSuccessfulBuild()
			{
				IntegrationStatistics integrationStatistics = processor.ProcessBuildResults(successfulBuild);
				Assert.AreEqual(successfulLabel, integrationStatistics.BuildLabel);
				Assert.AreEqual("Statistics Publisher", integrationStatistics.ProjectName);
				Assert.AreEqual(IntegrationStatus.Success, integrationStatistics.IntegrationStatus);
				Assert.AreEqual(integrationTime, integrationStatistics.IntegrationTime);
			}

			[Test]
			public void ShouldReturnIntegrationStatisticsWithCorrectBuildLabelAndStatusForFailedBuild()
			{
				IntegrationStatistics integrationStatistics = processor.ProcessBuildResults(failedBuild);
				Assert.AreEqual(failedLabel, integrationStatistics.BuildLabel);
				Assert.AreEqual(IntegrationStatus.Failure, integrationStatistics.IntegrationStatus);
			}

			[TearDown]
			public void TearDown()
			{
				if(File.Exists(filePath))
				{
					File.Delete(filePath);
				}
			}

			[TestFixtureTearDown]
			public void FixtureTearDown()
			{
				if (Directory.Exists(artifactDirectory))
				{
					foreach (string file in Directory.GetFiles(artifactDirectory))
					{
						File.Delete(file);
					}
					Directory.Delete(artifactDirectory);
				}
			}
		}
	}
}