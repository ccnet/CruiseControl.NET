using System;
using System.Collections;
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Core.Config;
using System.Xml;
using Rhino.Mocks;
using System.IO;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
	[TestFixture]
	public class MergeFileTaskTest : CustomAssertion
	{
        private MockRepository mocks = new MockRepository();

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
                Assert.AreEqual(expected[loop].FileName, task.MergeFiles[loop].FileName, string.Format("FileName on {0} does not match", loop));
                Assert.AreEqual(expected[loop].MergeAction, task.MergeFiles[loop].MergeAction, string.Format("MergeAction on {0} does not match", loop));
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
            var logger = mocks.StrictMock<ILogger>();
            Expect.Call(() => { logger.Info(""); }).IgnoreArguments().Repeat.AtLeastOnce();
            Expect.Call(() => { logger.Warning(""); }).IgnoreArguments().Repeat.AtLeastOnce();

            // Mock the file system
            var fileSystem = mocks.StrictMock<IFileSystem>();
            Expect.Call(fileSystem.FileExists(Path.Combine(working, "text.xml"))).Return(true);
            Expect.Call(fileSystem.FileExists(Path.Combine(working, "text.txt"))).Return(true);
            Expect.Call(fileSystem.FileExists(Path.Combine(working, "blank.txt"))).Return(false);
            Expect.Call(() => { fileSystem.EnsureFolderExists(targetFolder); });
            Expect.Call(() =>
            {
                fileSystem.Copy(
                    Path.Combine(working, "text.txt"),
                    Path.Combine(targetFolder, "text.txt"));
            });

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
            var result = mocks.StrictMock<IIntegrationResult>();
            var buildProgress = mocks.StrictMock<BuildProgressInformation>(artefact, "Project1");
            SetupResult.For(result.BuildProgressInformation)
                .Return(buildProgress);
            SetupResult.For(result.WorkingDirectory).Return(working);
            SetupResult.For(result.ArtifactDirectory).Return(artefact);
            SetupResult.For(result.Label).Return(label);
            Expect.Call(() => { buildProgress.SignalStartRunTask("Merging Files"); });
            Expect.Call(() => { result.AddTaskResultFromFile(Path.Combine(working, "text.xml"), false); });

            // Run the test
            mocks.ReplayAll();
            task.Run(result);

            // Check the results
            mocks.VerifyAll();
        }
    }
}