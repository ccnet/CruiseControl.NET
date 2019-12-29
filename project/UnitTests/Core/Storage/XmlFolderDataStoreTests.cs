namespace ThoughtWorks.CruiseControl.UnitTests.Core.Storage
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using Exortech.NetReflector;
    using Moq;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Storage;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;

    [TestFixture]
    public class XmlFolderDataStoreTests
    {
        #region Private fields
        private MockRepository mocks;
        private readonly string defaultFolder = "snapshots";
        #endregion

        #region Setup
        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository(MockBehavior.Default);
        }
        #endregion

        #region Tests
        #region XML tests
        [Test]
        public void ShouldLoadMinimalValuesFromConfiguration()
        {
            const string xml = @"<xmlFolderData />";
            var dataStore = NetReflector.Read(xml) as XmlFolderDataStore;
            Assert.AreEqual(string.Empty, dataStore.BaseFolder);
            Assert.AreEqual(defaultFolder, dataStore.SnapshotsFolder);
        }
        #endregion

        #region StoreProjectSnapshot() tests
        [Test]
        public void StoreProjectSnapshotStoresTheSnapShotAsXml()
        {
            // Arrange
            var expected = "<projectSnapshot/>";
            var snapshotsDir = Path.Combine("workingDir", defaultFolder);
            var snapshotFile = Path.Combine(snapshotsDir, "log20100101120000Lbuild.1.0.snapshot");
            var outputStream = new MemoryStream();
            var resultMock = InitialiseResultMock("workingDir");
            var snapShotMock = InitialiseSnapshotMock(expected);
            var fileSystemMock = InitialiseFileSystemMockForOutput(snapshotFile, outputStream);
            var dataStore = new XmlFolderDataStore
                {
                    FileSystem = fileSystemMock
                };

            // Act
            dataStore.StoreProjectSnapshot(resultMock, snapShotMock);

            // Assert
            this.mocks.VerifyAll();
            VerifyOutput(expected, outputStream);
        }

        [Test]
        public void StoreProjectSnapshotStoresTheSnapShotAsXmlInRelativeFolder()
        {
            // Arrange
            var expected = "<projectSnapshot/>";
            var folder = "somewhereElse";
            var snapshotsDir = Path.Combine("workingDir", folder);
            var snapshotFile = Path.Combine(snapshotsDir, "log20100101120000Lbuild.1.0.snapshot");
            var outputStream = new MemoryStream();
            var resultMock = InitialiseResultMock("workingDir");
            var snapShotMock = InitialiseSnapshotMock(expected);
            var fileSystemMock = InitialiseFileSystemMockForOutput(snapshotFile, outputStream);
            var dataStore = new XmlFolderDataStore
                {
                    FileSystem = fileSystemMock,
                    SnapshotsFolder = folder
                };

            // Act
            dataStore.StoreProjectSnapshot(resultMock, snapShotMock);

            // Assert
            this.mocks.VerifyAll();
            VerifyOutput(expected, outputStream);
        }

        [Test]
        public void StoreProjectSnapshotStoresTheSnapShotAsXmlInAbsoluteFolder()
        {
            // Arrange
            var expected = "<projectSnapshot/>";
            var snapshotsDir = Path.GetTempPath();
            var snapshotFile = Path.Combine(snapshotsDir, "log20100101120000Lbuild.1.0.snapshot");
            var outputStream = new MemoryStream();
            var resultMock = InitialiseResultMock("workingDir");
            var snapShotMock = InitialiseSnapshotMock(expected);
            var fileSystemMock = InitialiseFileSystemMockForOutput(snapshotFile, outputStream);
            var dataStore = new XmlFolderDataStore
                {
                    FileSystem = fileSystemMock,
                    SnapshotsFolder = snapshotsDir
                };

            // Act
            dataStore.StoreProjectSnapshot(resultMock, snapShotMock);

            // Assert
            this.mocks.VerifyAll();
            VerifyOutput(expected, outputStream);
        }

        [Test]
        public void StoreProjectSnapshotStoresTheSnapShotAsXmlInRelativeBaseFolder()
        {
            // Arrange
            var expected = "<projectSnapshot/>";
            var folder = "somewhereElse";
            var snapshotsDir = Path.Combine(Path.Combine("workingDir", folder), defaultFolder);
            var snapshotFile = Path.Combine(snapshotsDir, "log20100101120000Lbuild.1.0.snapshot");
            var outputStream = new MemoryStream();
            var resultMock = InitialiseResultMock("workingDir");
            var snapShotMock = InitialiseSnapshotMock(expected);
            var fileSystemMock = InitialiseFileSystemMockForOutput(snapshotFile, outputStream);
            var dataStore = new XmlFolderDataStore
                {
                    FileSystem = fileSystemMock,
                    BaseFolder = folder
                };

            // Act
            dataStore.StoreProjectSnapshot(resultMock, snapShotMock);

            // Assert
            this.mocks.VerifyAll();
            VerifyOutput(expected, outputStream);
        }

        [Test]
        public void StoreProjectSnapshotStoresTheSnapShotAsXmlInAbsoluteBaseFolder()
        {
            // Arrange
            var expected = "<projectSnapshot/>";
            var snapshotsDir = Path.GetTempPath();
            var snapshotFile = Path.Combine(Path.Combine(snapshotsDir, defaultFolder), "log20100101120000Lbuild.1.0.snapshot");
            var outputStream = new MemoryStream();
            var resultMock = InitialiseResultMock("workingDir");
            var snapShotMock = InitialiseSnapshotMock(expected);
            var fileSystemMock = InitialiseFileSystemMockForOutput(snapshotFile, outputStream);
            var dataStore = new XmlFolderDataStore
                {
                    FileSystem = fileSystemMock,
                    BaseFolder = snapshotsDir
                };

            // Act
            dataStore.StoreProjectSnapshot(resultMock, snapShotMock);

            // Assert
            this.mocks.VerifyAll();
            VerifyOutput(expected, outputStream);
        }
        #endregion

        #region LoadProjectSnapshot() tests
        [Test]
        public void LoadProjectSnapshotLoadsExistingSnapshot()
        {
            // Arrange
            var logFile = "log20100101120000Lbuild.1.0";
            var projectSnapshot = "<projectStatusSnapshot xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" name=\"UnitTest\" status=\"CompletedSuccess\">" +
                    "<timeStarted>2010-01-01T12:00:00.00000+00:00</timeStarted>" +
                    "<timeCompleted>2010-01-01T12:00:01.00000+00:00</timeCompleted>" +
                    "<childItems />" +
                    "<timeOfSnapshot>2010-01-01T12:00:01.00000+00:00</timeOfSnapshot>" +
                "</projectStatusSnapshot>";
            var snapshotsDir = Path.Combine("workingDir", defaultFolder);
            var snapshotFile = Path.Combine(snapshotsDir, logFile + ".snapshot");
            var outputStream = new MemoryStream();
            var fileSystemMock = InitialiseFileSystemMockForInput(snapshotFile, projectSnapshot);
            var projectMock = InitialiseProjectMock("workingDir");
            var dataStore = new XmlFolderDataStore
                {
                    FileSystem = fileSystemMock
                };

            // Act
            var snapshot = dataStore.LoadProjectSnapshot(projectMock, logFile + ".xml");

            // Assert
            this.mocks.VerifyAll();
            Assert.IsNotNull(snapshot);
            Assert.AreEqual("UnitTest", snapshot.Name);
            Assert.AreEqual(
                DateTime.Parse("2010-01-01T12:00:00.00000+00:00", CultureInfo.InvariantCulture), 
                snapshot.TimeStarted);
            Assert.AreEqual(ItemBuildStatus.CompletedSuccess, snapshot.Status);
        }

        [Test]
        public void LoadProjectSnapshotReturnsNullForMissingSnapshot()
        {
            // Arrange
            var logFile = "log20100101120000Lbuild.1.0";
            var snapshotsDir = Path.Combine("workingDir", defaultFolder);
            var snapshotFile = Path.Combine(snapshotsDir, logFile + ".snapshot");
            var outputStream = new MemoryStream();
            var fileSystemMock = InitialiseFileSystemMockForInput(snapshotFile, null);
            var projectMock = InitialiseProjectMock("workingDir");
            var dataStore = new XmlFolderDataStore
                {
                    FileSystem = fileSystemMock
                };

            // Act
            var snapshot = dataStore.LoadProjectSnapshot(projectMock, logFile + ".xml");

            // Assert
            this.mocks.VerifyAll();
            Assert.IsNull(snapshot);
        }
        #endregion
        #endregion

        #region Helpers
        private static void VerifyOutput(string expected, MemoryStream outputStream)
        {
            using (var inputStream = new MemoryStream(outputStream.GetBuffer()))
            {
                using (var reader = new StreamReader(inputStream))
                {
                    var actual = reader.ReadToEnd();
                    if (actual.Contains("\x0"))
                    {
                        actual = actual.Substring(0, actual.IndexOf('\x0'));
                    }

                    Assert.AreEqual(expected, actual);
                }
            }
        }

        private IIntegrationResult InitialiseResultMock(string artefactDir)
        {
            var resultMock = this.mocks.Create<IIntegrationResult>(MockBehavior.Strict).Object;
            Mock.Get(resultMock).SetupGet(_resultMock => _resultMock.ArtifactDirectory).Returns(artefactDir);
            Mock.Get(resultMock).SetupGet(_resultMock => _resultMock.StartTime).Returns(new DateTime(2010, 1, 1, 12, 0, 0));
            Mock.Get(resultMock).SetupGet(_resultMock => _resultMock.Label).Returns("1.0");
            Mock.Get(resultMock).SetupGet(_resultMock => _resultMock.Succeeded).Returns(true);
            return resultMock;
        }

        private IFileSystem InitialiseFileSystemMockForOutput(string snapshotFile, MemoryStream outputStream)
        {
            var fileSystemMock = this.mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            Mock.Get(fileSystemMock).Setup(_fileSystemMock => _fileSystemMock.EnsureFolderExists(snapshotFile)).Verifiable();
            Mock.Get(fileSystemMock).Setup(_fileSystemMock => _fileSystemMock.OpenOutputStream(snapshotFile)).Returns(outputStream).Verifiable();
            return fileSystemMock;
        }

        private IFileSystem InitialiseFileSystemMockForInput(string snapshotFile, string input)
        {
            var fileSystemMock = this.mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            if (input != null)
            {
                var data = Encoding.UTF8.GetBytes(input);
                var inputStream = new MemoryStream(data);
                Mock.Get(fileSystemMock).Setup(_fileSystemMock => _fileSystemMock.FileExists(snapshotFile)).Returns(true).Verifiable();
                Mock.Get(fileSystemMock).Setup(_fileSystemMock => _fileSystemMock.OpenInputStream(snapshotFile)).Returns(inputStream).Verifiable();
            }
            else
            {
                Mock.Get(fileSystemMock).Setup(_fileSystemMock => _fileSystemMock.FileExists(snapshotFile)).Returns(false).Verifiable();
            }

            return fileSystemMock;
        }

        private ItemStatus InitialiseSnapshotMock(string expected)
        {
            var snapShotMock = this.mocks.Create<ItemStatus>(MockBehavior.Strict).Object;
            Mock.Get(snapShotMock).Setup(_snapShotMock => _snapShotMock.ToString()).Returns(expected).Verifiable();
            return snapShotMock;
        }

        private IProject InitialiseProjectMock(string artefactDir)
        {
            var projectMock = this.mocks.Create<IProject>(MockBehavior.Strict).Object;
            Mock.Get(projectMock).SetupGet(_projectMock => _projectMock.ArtifactDirectory).Returns(artefactDir);
            return projectMock;
        }
        #endregion
    }
}
