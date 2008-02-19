using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Core.Tasks;

using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    [TestFixture]
    public class ArtifactCleanupTest
    {
        public static readonly string FULL_CONFIGURED_LOG_DIR = "FullConfiguredLogDir";
        public static readonly string FULL_CONFIGURED_LOG_DIR_PATH = Path.GetFullPath(TempFileUtil.GetTempPath(FULL_CONFIGURED_LOG_DIR));

        public static readonly string ARTIFACTS_DIR = "Artifacts";
        public static readonly string ARTIFACTS_DIR_PATH = Path.GetFullPath(TempFileUtil.GetTempPath(ARTIFACTS_DIR));
        
        private XmlLogPublisher publisher;
        private ArtifactCleanUpTask artifactCleaner;


        [SetUp]
        public void SetUp()
        {
            TempFileUtil.DeleteTempDir(FULL_CONFIGURED_LOG_DIR);
            TempFileUtil.DeleteTempDir(ARTIFACTS_DIR);

            TempFileUtil.CreateTempDir(FULL_CONFIGURED_LOG_DIR);
            TempFileUtil.CreateTempDir(ARTIFACTS_DIR);


            publisher = new XmlLogPublisher();
            artifactCleaner = new ArtifactCleanUpTask();
        }

        [TearDown]
        public void TearDown()
        {
            TempFileUtil.DeleteTempDir(FULL_CONFIGURED_LOG_DIR);
            TempFileUtil.DeleteTempDir(ARTIFACTS_DIR);
        }


        [Test]
        public void DeleteAllBuildLogs()
        {
            // Setup
            IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, 1, ARTIFACTS_DIR_PATH);      
            
            // make a build
            publisher.Run(result);

            //clear the data of this build, so delete all build files
            artifactCleaner.CleaningUpMethod = ArtifactCleanUpTask.CleanUpMethod.KeepLastXBuilds;
            artifactCleaner.CleaningUpValue = 0;

            // run the cleaning procedure
            artifactCleaner.Run(result);

            // verify if all files are removed
            Assert.AreEqual(0, System.IO.Directory.GetFiles(result.BuildLogDirectory).Length, "logs are not removed");        
        }


        [Test]
        public void KeepLast5BuildLogs()
        {
            IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, 1, ARTIFACTS_DIR_PATH);

            for (int i = 2; i < 10; i++ )
            {
                // Setup
                result = CreateIntegrationResult(IntegrationStatus.Success, i, ARTIFACTS_DIR_PATH);

                // make a build
                publisher.Run(result);

            }

            //clear the data of this build, so delete all build files
            artifactCleaner.CleaningUpMethod = ArtifactCleanUpTask.CleanUpMethod.KeepLastXBuilds;
            artifactCleaner.CleaningUpValue = 5;

            // run the cleaning procedure
            artifactCleaner.Run(result);

            // verify if 5 builds are still available
            Assert.AreEqual(5, System.IO.Directory.GetFiles(result.BuildLogDirectory).Length, "logs are not removed");
        }


        private IntegrationResult CreateIntegrationResult(IntegrationStatus status,int buildNumber, string artifactFolder )
        {
            IntegrationResult result = IntegrationResultMother.Create(status, new DateTime(1980, 1, 1,1,0,0,buildNumber));
            result.ProjectName = "proj";
            result.StartTime = new DateTime(1980, 1, 1);
            result.Label = buildNumber.ToString();
            result.Status = status;
            result.ArtifactDirectory = artifactFolder;

            return result;
        }

    }
}
