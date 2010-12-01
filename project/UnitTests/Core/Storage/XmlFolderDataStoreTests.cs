namespace ThoughtWorks.CruiseControl.UnitTests.Core.Storage
{
    using NUnit.Framework;
    using Rhino.Mocks;
    using ThoughtWorks.CruiseControl.Core.Storage;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Remote;
    using System;
    using System.IO;
    using Exortech.NetReflector;

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
            mocks = new MockRepository();
        }
        #endregion

        #region Tests
        #region XML tests
        [Test]
        public void ShouldLoadMinimalValuesFromConfiguration()
        {
            const string xml = @"<xmlFolderData />";
            var dataStore = NetReflector.Read(xml) as XmlFolderDataStore;
            Assert.AreEqual("snapshots", dataStore.Folder);
        }
        #endregion

        #region StoreProjectSnapshot() tests
        [Test]
        public void StoreProjectSnapshotStoresTheSnapShotAsXml()
        {
            // Arrange
            var expected = "<projectSnapshot/>";
            var snapshotsDir = Path.Combine("workingDir", "snapshots");
            var snapshotFile = Path.Combine(snapshotsDir, "log20100101120000Lbuild.1.0.snapshot");
            var outputStream = new MemoryStream();
            var resultMock = InitialiseResultMock(snapshotsDir, defaultFolder);
            var snapShotMock = InitialiseSnapshotMock(expected);
            var fileSystemMock = InitialiseFileSystemMock(snapshotsDir, snapshotFile, outputStream);
            var dataStore = new XmlFolderDataStore
            {
                FileSystem = fileSystemMock
            };
            this.mocks.ReplayAll();

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
            var resultMock = InitialiseResultMock(snapshotsDir, folder);
            var snapShotMock = InitialiseSnapshotMock(expected);
            var fileSystemMock = InitialiseFileSystemMock(snapshotsDir, snapshotFile, outputStream);
            var dataStore = new XmlFolderDataStore
            {
                FileSystem = fileSystemMock,
                Folder = folder
            };
            this.mocks.ReplayAll();

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
            var resultMock = InitialiseResultMock("nowhere", defaultFolder);
            var snapShotMock = InitialiseSnapshotMock(expected);
            var fileSystemMock = InitialiseFileSystemMock(snapshotsDir, snapshotFile, outputStream);
            var dataStore = new XmlFolderDataStore
                {
                    FileSystem = fileSystemMock,
                    Folder = snapshotsDir
                };
            this.mocks.ReplayAll();

            // Act
            dataStore.StoreProjectSnapshot(resultMock, snapShotMock);

            // Assert
            this.mocks.VerifyAll();
            VerifyOutput(expected, outputStream);
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

        private IIntegrationResult InitialiseResultMock(string snapshotsDir, string folder)
        {
            var resultMock = this.mocks.StrictMock<IIntegrationResult>();
            SetupResult.For(resultMock.BaseFromArtifactsDirectory(folder))
                .Return(snapshotsDir);
            SetupResult.For(resultMock.StartTime).Return(new DateTime(2010, 1, 1, 12, 0, 0));
            SetupResult.For(resultMock.Label).Return("1.0");
            SetupResult.For(resultMock.Succeeded).Return(true);
            return resultMock;
        }

        private IFileSystem InitialiseFileSystemMock(string snapshotsDir, string snapshotFile, MemoryStream outputStream)
        {
            var fileSystemMock = this.mocks.StrictMock<IFileSystem>();
            Expect.Call(() => fileSystemMock.EnsureFolderExists(snapshotsDir));
            Expect.Call(fileSystemMock.OpenOutputStream(snapshotFile))
                .Return(outputStream);
            return fileSystemMock;
        }

        private ItemStatus InitialiseSnapshotMock(string expected)
        {
            var snapShotMock = this.mocks.StrictMock<ItemStatus>();
            Expect.Call(snapShotMock.ToString()).Return(expected);
            return snapShotMock;
        }
        #endregion
    }
}
