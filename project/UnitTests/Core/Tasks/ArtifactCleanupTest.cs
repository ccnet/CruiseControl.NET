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
        public static readonly string SOURCE_DIR = "SourceDir";
        public static readonly string SOURCE_DIR_PATH = Path.GetFullPath(TempFileUtil.GetTempPath(SOURCE_DIR));


        public static readonly string ARTIFACTS_DIR = "Artifacts";
        public static readonly string ARTIFACTS_DIR_PATH = Path.GetFullPath(TempFileUtil.GetTempPath(ARTIFACTS_DIR));
        
        private XmlLogPublisher logPublisher;
        private BuildPublisher buildPublisher;
        private ArtifactCleanUpTask artifactCleaner;


        [SetUp]
        public void SetUp()
        {
            TempFileUtil.DeleteTempDir(FULL_CONFIGURED_LOG_DIR);
            TempFileUtil.DeleteTempDir(ARTIFACTS_DIR);

            TempFileUtil.CreateTempDir(FULL_CONFIGURED_LOG_DIR);
            TempFileUtil.CreateTempDir(ARTIFACTS_DIR);
            TempFileUtil.CreateTempDir(SOURCE_DIR);

            TempFileUtil.CreateTempFile(SOURCE_DIR_PATH, "myfile.txt", "some content");

            logPublisher = new XmlLogPublisher();
            buildPublisher = new BuildPublisher();
            artifactCleaner = new ArtifactCleanUpTask();
            
            buildPublisher.AlwaysPublish = true;
            buildPublisher.UseLabelSubDirectory = true;
            buildPublisher.SourceDir = SOURCE_DIR_PATH;

        }

        [TearDown]
        public void TearDown()
        {
            TempFileUtil.DeleteTempDir(FULL_CONFIGURED_LOG_DIR);
            TempFileUtil.DeleteTempDir(ARTIFACTS_DIR);
            TempFileUtil.DeleteTempDir(SOURCE_DIR);
        }


        [Test]
        public void DeleteAllBuildLogs()
        {
            // Setup
            IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, 1, ARTIFACTS_DIR_PATH);      
            
            // make a build
            logPublisher.Run(result);

            //clear the data of this build, so delete all build files
            artifactCleaner.CleaningUpMethod = ArtifactCleanUpTask.CleanUpMethod.KeepLastXBuilds;
            artifactCleaner.CleaningUpValue = 0;

            // run the cleaning procedure
            artifactCleaner.Run(result);

            // verify if all files are removed
            Assert.AreEqual(0, System.IO.Directory.GetFiles(result.BuildLogDirectory).Length, "logs are not removed");        
        }



        [Test]
        public void NoErrorWhenBuildLogFolderIsUnknown()
        {
            IntegrationResult result = null;

            // make a build
            for (int i = 1; i < 10; i++)
            {
                // Setup
                result = CreateIntegrationResult(IntegrationStatus.Success, i, ARTIFACTS_DIR_PATH);

                // make a build
                logPublisher.Run(result);

            }

            //clear the data of this build, so delete all build files
            artifactCleaner.CleaningUpMethod = ArtifactCleanUpTask.CleanUpMethod.KeepLastXBuilds;
            artifactCleaner.CleaningUpValue = 5;
            
            result.BuildLogDirectory = null;

            // run the cleaning procedure
            artifactCleaner.Run(result);
            
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
                logPublisher.Run(result);

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



        [Test]
        public void KeepLast5PublishedBuilds()
        {
            IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, 1, ARTIFACTS_DIR_PATH);
                    
            for (int i = 1; i <= 10; i++)
            {
                // Setup
                result = CreateIntegrationResult(IntegrationStatus.Success, i, ARTIFACTS_DIR_PATH);
                // publish the build
                logPublisher.Run(result);
                buildPublisher.Run(result);
            }


            //clear the data of this build, so delete all build files
            artifactCleaner.CleaningUpMethod = ArtifactCleanUpTask.CleanUpMethod.KeepLastXSubDirs;
            artifactCleaner.CleaningUpValue = 2;

            // run the cleaning procedure
            artifactCleaner.Run(result);

            // verify if 3 builds are still available ( 32wanted + 1 buildlog folder that also is in artifacts folder
            Assert.AreEqual(3, System.IO.Directory.GetDirectories(result.ArtifactDirectory).Length, "published builds are not removed");

        }

    }
}
