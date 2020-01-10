using System;
using System.Collections;
using System.IO;
using System.Xml;
using Exortech.NetReflector;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
	[TestFixture]
	public class MergeFileTaskTest : CustomAssertion
	{
        private MockRepository mocks = new MockRepository(MockBehavior.Default);

        [Test]
        public void ConfigurationIsLoadedCorrectly()
        {
            var reader = new NetReflectorConfigurationReader();
            var xml = new XmlDocument();
            xml.LoadXml(@"
<cruisecontrol>
    <project name=""WebTrunkTest"">
        <tasks>
            <merge target=""somewhere"">
                <files>
                    <file>File 1</file>
                    <file action=""Copy"">File 2</file>
                    <file action=""Merge"">File 3</file>
                </files>
            </merge>
        </tasks>
    </project>
</cruisecontrol>
");
            var configuration = reader.Read(xml, null);
            Assert.IsNotNull(configuration, "Configuration not loaded");
            var project = configuration.Projects["WebTrunkTest"] as Project;
            Assert.IsNotNull(project, "Project not loaded");
            Assert.AreNotEqual(0, project.Tasks.Length, "Tasks not loaded");
            var task = project.Tasks[0] as MergeFilesTask;
            Assert.IsNotNull(task, "Task not correctly loaded");
            Assert.AreEqual("somewhere", task.TargetFolder, "TargetFolder is incorrect");

            var expected = new MergeFileInfo[] {
                new MergeFileInfo{FileName = "File 1", MergeAction = MergeFileInfo.MergeActionType.Merge},
                new MergeFileInfo{FileName = "File 2", MergeAction = MergeFileInfo.MergeActionType.Copy},
                new MergeFileInfo{FileName = "File 3", MergeAction = MergeFileInfo.MergeActionType.Merge}
            };
            Assert.AreEqual(task.MergeFiles.Length, expected.Length, "File count incorrect");
            for (var loop = 0; loop < expected.Length; loop++)
            {
                Assert.AreEqual(expected[loop].FileName, task.MergeFiles[loop].FileName, string.Format(System.Globalization.CultureInfo.CurrentCulture,"FileName on {0} does not match", loop));
                Assert.AreEqual(expected[loop].MergeAction, task.MergeFiles[loop].MergeAction, string.Format(System.Globalization.CultureInfo.CurrentCulture,"MergeAction on {0} does not match", loop));
            }
        }

        [Test]
        public void AttributeOnElementThrowsException()
        {
            var reader = new NetReflectorConfigurationReader();
            var xml = new XmlDocument();
            xml.LoadXml(@"
<cruisecontrol>
    <project name=""WebTrunkTest"">
        <tasks>
            <merge target=""somewhere"">
                <files me=""wrong"">
                    <file>File 1</file>
                    <file action=""Copy"">File 2</file>
                    <file action=""Merge"">File 3</file>
                </files>
            </merge>
        </tasks>
    </project>
</cruisecontrol>
");
            Assert.That(delegate { reader.Read(xml, null); },
                        Throws.TypeOf<ConfigurationException>());
        }

        [Test]
        public void SubItemsOnFileThrowsException()
        {
            var reader = new NetReflectorConfigurationReader();
            var xml = new XmlDocument();
            xml.LoadXml(@"
<cruisecontrol>
    <project name=""WebTrunkTest"">
        <tasks>
            <merge target=""somewhere"">
                <files>
                    <file><type>Wrong</type>File 1</file>
                    <file action=""Copy"">File 2</file>
                    <file action=""Merge"">File 3</file>
                </files>
            </merge>
        </tasks>
    </project>
</cruisecontrol>
");
            Assert.That(delegate { reader.Read(xml, null); },
                        Throws.TypeOf<ConfigurationException>());
        }

        [Test]
        public void UnknownItemThrowsException()
        {
            var reader = new NetReflectorConfigurationReader();
            var xml = new XmlDocument();
            xml.LoadXml(@"
<cruisecontrol>
    <project name=""WebTrunkTest"">
        <tasks>
            <merge target=""somewhere"">
                <files>
                    <wrong>File 1</wrong>
                    <file action=""Copy"">File 2</file>
                    <file action=""Merge"">File 3</file>
                </files>
            </merge>
        </tasks>
    </project>
</cruisecontrol>
");
            Assert.That(delegate { reader.Read(xml, null); },
                        Throws.TypeOf<ConfigurationException>());
        }

        [Test]
        public void UnknownActionThrowsException()
        {
            var reader = new NetReflectorConfigurationReader();
            var xml = new XmlDocument();
            xml.LoadXml(@"
<cruisecontrol>
    <project name=""WebTrunkTest"">
        <tasks>
            <merge target=""somewhere"">
                <files>
                    <file>File 1</file>
                    <file action=""Wrong"">File 2</file>
                    <file action=""Merge"">File 3</file>
                </files>
            </merge>
        </tasks>
    </project>
</cruisecontrol>
");
            Assert.That(delegate { reader.Read(xml, null); },
                        Throws.TypeOf<ConfigurationException>());
        }

        [Test]
        public void RunMergesAndCopiesFilesWithNullTarget()
        {
            string target = null;

            RunMergeTest(target);
        }

        [Test]
        public void RunMergesAndCopiesFilesWithTargetSet()
        {
            string target = "reports";

            RunMergeTest(target);
        }

        private void RunMergeTest(string target)
        {
            var working = Path.Combine(Path.GetTempPath(), "working");
            var artefact = Path.Combine(Path.GetTempPath(), "artefact");
            var label = "1.2.3.4";
            var targetFolder = Path.Combine(
                Path.Combine(artefact, label),
                target ?? string.Empty);

            // Mock the logger
            var logger = mocks.Create<ILogger>(MockBehavior.Strict).Object;
            Mock.Get(logger).Setup(_logger => _logger.Info(It.IsAny<string>(), It.IsAny<object[]>())).Verifiable();
            Mock.Get(logger).Setup(_logger => _logger.Warning(It.IsAny<string>(), It.IsAny<object[]>())).Verifiable();

            // Mock the file system
            var fileSystem = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.FileExists(Path.Combine(working, "text.xml"))).Returns(true).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.FileExists(Path.Combine(working, "text.xml"))).Returns(true).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.FileExists(Path.Combine(working, "text.txt"))).Returns(true).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.FileExists(Path.Combine(working, "blank.txt"))).Returns(false).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.EnsureFolderExists(targetFolder)).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.Copy(
                Path.Combine(working, "text.txt"),
                Path.Combine(targetFolder, "text.txt"))
            ).Verifiable();

            // Initialise the task
            var task = new MergeFilesTask
            {
                Logger = logger,
                FileSystem = fileSystem,
                TargetFolder = target
            };
            task.MergeFiles = new MergeFileInfo[]{
                new MergeFileInfo{ FileName = "text.xml", MergeAction = MergeFileInfo.MergeActionType.Merge},
                new MergeFileInfo{ FileName = "text.txt", MergeAction = MergeFileInfo.MergeActionType.Copy},
                new MergeFileInfo{ FileName = "blank.txt", MergeAction = MergeFileInfo.MergeActionType.Copy}
            };

            // Mock the result
            var result = mocks.Create<IIntegrationResult>(MockBehavior.Strict).Object;
            Mock.Get(result).Setup(_result => _result.AddTaskResult(It.IsAny<ITaskResult>())).Verifiable();
            Mock.Get(result).SetupProperty(_result => _result.Status);
            Mock.Get(result).SetupGet(_result => _result.Succeeded).Returns(true).Verifiable();

            var buildProgress = mocks.Create<BuildProgressInformation>(MockBehavior.Strict, artefact, "Project1").Object;
            Mock.Get(result).SetupGet(_result => _result.BuildProgressInformation).Returns(buildProgress);
            Mock.Get(result).SetupGet(_result => _result.WorkingDirectory).Returns(working);
            Mock.Get(result).SetupGet(_result => _result.ArtifactDirectory).Returns(artefact);
            Mock.Get(result).SetupGet(_result => _result.Label).Returns(label);
            Mock.Get(buildProgress).Setup(_buildProgress => _buildProgress.SignalStartRunTask("Merging Files")).Verifiable();
            Mock.Get(buildProgress).Setup(_buildProgress => _buildProgress.AddTaskInformation(It.IsAny<string>())).Verifiable();

            //var testFile = new FileInfo(Path.Combine(working, "text.xml"));
            //Expect.Call(fileSystem.FileExists(testFile.FullName)).Return(true);
            //Expect.Call(() =>
            //                {
            //                    result.AddTaskResult(new FileTaskResult(
            //                                             testFile, false,
            //                                             fileSystem) {WrapInCData = false});
            //                });

            // Run the test
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);

            // Check the results
            mocks.VerifyAll();
        }
    }
}