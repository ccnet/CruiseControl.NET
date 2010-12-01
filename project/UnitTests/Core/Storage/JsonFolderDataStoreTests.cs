namespace ThoughtWorks.CruiseControl.UnitTests.Core.Storage
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using Exortech.NetReflector;
    using NUnit.Framework;
    using Rhino.Mocks;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Storage;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;
    using System.Reflection;

    [TestFixture]
    public class JsonFolderDataStoreTests
    {
        #region Private fields
        private MockRepository mocks;
        private readonly string defaultFolder = "snapshots";
        private readonly string defaultJson = "{\"TimeOfSnapshot\":\"\\/Date(1262347260000)\\/\"," +
            "\"Identifier\":\"8a3c688e-abcd-44d9-b15c-b1339bbd776d\"," +
            "\"Name\":\"TestProject\"," + 
            "\"Status\":2," +
            "\"TimeStarted\":\"\\/Date(1262347200000)\\/\"," +
            "\"TimeCompleted\":\"\\/Date(1262347260000)\\/\"," +
            "\"ChildItems\":[]}";
        #endregion

        #region Setup
        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
        }
        #endregion

        #region Tests
        #region XML tests
        [Test]
        public void ShouldLoadMinimalValuesFromConfiguration()
        {
            const string xml = @"<jsonFolderData />";
            var dataStore = NetReflector.Read(xml) as JsonFolderDataStore;
            Assert.AreEqual(string.Empty, dataStore.BaseFolder);
            Assert.AreEqual(defaultFolder, dataStore.SnapshotsFolder);
        }
        #endregion

        #region StoreProjectSnapshot() tests
        [Test]
        public void StoreProjectSnapshotStoresTheSnapShotAsXml()
        {
            // Arrange
            var snapshotsDir = Path.Combine("workingDir", defaultFolder);
            var snapshotFile = Path.Combine(snapshotsDir, "log20100101120000Lbuild.1.0.snapshot");
            var outputStream = new MemoryStream();
            var resultMock = InitialiseResultMock("workingDir");
            var snapShotMock = InitialiseSnapshot();
            var fileSystemMock = InitialiseFileSystemMockForOutput(snapshotFile, outputStream);
            var dataStore = new JsonFolderDataStore
                {
                    FileSystem = fileSystemMock
                };
            this.mocks.ReplayAll();

            // Act
            dataStore.StoreProjectSnapshot(resultMock, snapShotMock);

            // Assert
            this.mocks.VerifyAll();
            VerifyOutput(defaultJson, outputStream);
        }

        [Test]
        public void StoreProjectSnapshotStoresTheSnapShotAsXmlInRelativeFolder()
        {
            // Arrange
            var folder = "somewhereElse";
            var snapshotsDir = Path.Combine("workingDir", folder);
            var snapshotFile = Path.Combine(snapshotsDir, "log20100101120000Lbuild.1.0.snapshot");
            var outputStream = new MemoryStream();
            var resultMock = InitialiseResultMock("workingDir");
            var snapShotMock = InitialiseSnapshot();
            var fileSystemMock = InitialiseFileSystemMockForOutput(snapshotFile, outputStream);
            var dataStore = new JsonFolderDataStore
                {
                    FileSystem = fileSystemMock,
                    SnapshotsFolder = folder
                };
            this.mocks.ReplayAll();

            // Act
            dataStore.StoreProjectSnapshot(resultMock, snapShotMock);

            // Assert
            this.mocks.VerifyAll();
            VerifyOutput(defaultJson, outputStream);
        }

        [Test]
        public void StoreProjectSnapshotStoresTheSnapShotAsXmlInAbsoluteFolder()
        {
            // Arrange
            var snapshotsDir = Path.GetTempPath();
            var snapshotFile = Path.Combine(snapshotsDir, "log20100101120000Lbuild.1.0.snapshot");
            var outputStream = new MemoryStream();
            var resultMock = InitialiseResultMock("workingDir");
            var snapShotMock = InitialiseSnapshot();
            var fileSystemMock = InitialiseFileSystemMockForOutput(snapshotFile, outputStream);
            var dataStore = new JsonFolderDataStore
                {
                    FileSystem = fileSystemMock,
                    SnapshotsFolder = snapshotsDir
                };
            this.mocks.ReplayAll();

            // Act
            dataStore.StoreProjectSnapshot(resultMock, snapShotMock);

            // Assert
            this.mocks.VerifyAll();
            VerifyOutput(defaultJson, outputStream);
        }

        [Test]
        public void StoreProjectSnapshotStoresTheSnapShotAsXmlInRelativeBaseFolder()
        {
            // Arrange
            var folder = "somewhereElse";
            var snapshotsDir = Path.Combine(Path.Combine("workingDir", folder), defaultFolder);
            var snapshotFile = Path.Combine(snapshotsDir, "log20100101120000Lbuild.1.0.snapshot");
            var outputStream = new MemoryStream();
            var resultMock = InitialiseResultMock("workingDir");
            var snapShotMock = InitialiseSnapshot();
            var fileSystemMock = InitialiseFileSystemMockForOutput(snapshotFile, outputStream);
            var dataStore = new JsonFolderDataStore
                {
                    FileSystem = fileSystemMock,
                    BaseFolder = folder
                };
            this.mocks.ReplayAll();

            // Act
            dataStore.StoreProjectSnapshot(resultMock, snapShotMock);

            // Assert
            this.mocks.VerifyAll();
            VerifyOutput(defaultJson, outputStream);
        }

        [Test]
        public void StoreProjectSnapshotStoresTheSnapShotAsXmlInAbsoluteBaseFolder()
        {
            // Arrange
            var snapshotsDir = Path.GetTempPath();
            var snapshotFile = Path.Combine(Path.Combine(snapshotsDir, defaultFolder), "log20100101120000Lbuild.1.0.snapshot");
            var outputStream = new MemoryStream();
            var resultMock = InitialiseResultMock("workingDir");
            var snapShotMock = InitialiseSnapshot();
            var fileSystemMock = InitialiseFileSystemMockForOutput(snapshotFile, outputStream);
            var dataStore = new JsonFolderDataStore
                {
                    FileSystem = fileSystemMock,
                    BaseFolder = snapshotsDir
                };
            this.mocks.ReplayAll();

            // Act
            dataStore.StoreProjectSnapshot(resultMock, snapShotMock);

            // Assert
            this.mocks.VerifyAll();
            VerifyOutput(defaultJson, outputStream);
        }
        #endregion

        #region LoadProjectSnapshot() tests
        [Test]
        public void LoadProjectSnapshotLoadsExistingSnapshot()
        {
            // Arrange
            var logFile = "log20100101120000Lbuild.1.0";
            var snapshotsDir = Path.Combine("workingDir", defaultFolder);
            var snapshotFile = Path.Combine(snapshotsDir, logFile + ".snapshot");
            var outputStream = new MemoryStream();
            var fileSystemMock = InitialiseFileSystemMockForInput(snapshotFile, defaultJson);
            var projectMock = InitialiseProjectMock("workingDir");
            var dataStore = new JsonFolderDataStore
                {
                    FileSystem = fileSystemMock
                };
            this.mocks.ReplayAll();

            // Act
            var snapshot = dataStore.LoadProjectSnapshot(projectMock, logFile + ".xml");

            // Assert
            this.mocks.VerifyAll();
            Assert.IsNotNull(snapshot);
            Assert.AreEqual("TestProject", snapshot.Name);
            Assert.AreEqual(
                DateTime.Parse("2010-01-01T12:00:00.00000+00:00", CultureInfo.InvariantCulture).ToUniversalTime(), 
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
            var dataStore = new JsonFolderDataStore
                {
                    FileSystem = fileSystemMock
                };
            this.mocks.ReplayAll();

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
            var resultMock = this.mocks.StrictMock<IIntegrationResult>();
            SetupResult.For(resultMock.ArtifactDirectory).Return(artefactDir);
            SetupResult.For(resultMock.StartTime).Return(new DateTime(2010, 1, 1, 12, 0, 0));
            SetupResult.For(resultMock.Label).Return("1.0");
            SetupResult.For(resultMock.Succeeded).Return(true);
            return resultMock;
        }

        private IFileSystem InitialiseFileSystemMockForOutput(string snapshotFile, MemoryStream outputStream)
        {
            var fileSystemMock = this.mocks.StrictMock<IFileSystem>();
            Expect.Call(() => fileSystemMock.EnsureFolderExists(snapshotFile));
            Expect.Call(fileSystemMock.OpenOutputStream(snapshotFile)).Return(outputStream);
            return fileSystemMock;
        }

        private IFileSystem InitialiseFileSystemMockForInput(string snapshotFile, string input)
        {
            var fileSystemMock = this.mocks.StrictMock<IFileSystem>();
            if (input != null)
            {
                var data = Encoding.UTF8.GetBytes(input);
                var inputStream = new MemoryStream(data);
                Expect.Call(fileSystemMock.FileExists(snapshotFile)).Return(true);
                Expect.Call(fileSystemMock.OpenInputStream(snapshotFile)).Return(inputStream);
            }
            else
            {
                Expect.Call(fileSystemMock.FileExists(snapshotFile)).Return(false);
            }

            return fileSystemMock;
        }

        private ItemStatus InitialiseSnapshot()
        {
            var snapShot = new ProjectStatusSnapshot
                {
                    Name = "TestProject",
                    TimeStarted = DateTime.Parse("2010-01-01T12:00:00.00000+00:00", CultureInfo.InvariantCulture).ToUniversalTime(),
                    TimeCompleted = DateTime.Parse("2010-01-01T12:01:00.00000+00:00", CultureInfo.InvariantCulture).ToUniversalTime(),
                    TimeOfSnapshot = DateTime.Parse("2010-01-01T12:01:00.00000+00:00", CultureInfo.InvariantCulture).ToUniversalTime(),
                    Status = ItemBuildStatus.CompletedSuccess
                };
            var idField = typeof(ItemStatus)
                .GetField("identifier", BindingFlags.NonPublic | BindingFlags.Instance);
            idField.SetValue(snapShot, new Guid("8A3C688E-ABCD-44d9-B15C-B1339BBD776D"));
            return snapShot;
        }

        private IProject InitialiseProjectMock(string artefactDir)
        {
            var projectMock = this.mocks.StrictMock<IProject>();
            SetupResult.For(projectMock.ArtifactDirectory).Return(artefactDir);
            return projectMock;
        }
        #endregion
    }
}
