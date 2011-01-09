namespace CruiseControl.Core.Tests.Utilities
{
    using System;
    using System.IO;
    using CruiseControl.Core.Utilities;
    using NUnit.Framework;

    [TestFixture]
    public class FileSystemTests
    {
        #region Tests
        [Test]
        public void CheckIfFileExistsReturnsTrueIfFileExists()
        {
            var fileSystem = new FileSystem();
            var fileName = Path.GetTempFileName();
            try
            {
                File.WriteAllText(fileName, "Test");
                var result = fileSystem.CheckIfFileExists(fileName);
                Assert.IsTrue(result);
            }
            finally
            {
                CleanUpFile(fileName);
            }
        }

        [Test]
        public void CheckIfFileExistsReturnsFalseIfFileDoesNotExist()
        {
            var fileSystem = new FileSystem();
            var fileName = Path.GetTempFileName();
            CleanUpFile(fileName);
            var result = fileSystem.CheckIfFileExists(fileName);
            Assert.IsFalse(result);
        }

        [Test]
        public void OpenFileForReadOpensStream()
        {
            var fileSystem = new FileSystem();
            var fileName = Path.GetTempFileName();
            try
            {
                File.WriteAllText(fileName, "Test");
                using (var result = fileSystem.OpenFileForRead(fileName))
                {
                    Assert.IsNotNull(result);
                    using (var reader = new StreamReader(result))
                    {
                        var data = reader.ReadToEnd();
                        Assert.AreEqual("Test", data);
                    }
                }
            }
            finally
            {
                CleanUpFile(fileName);
            }
        }

        [Test]
        public void CreateXmlWriterCreatesTheWriter()
        {
            var fileSystem = new FileSystem();
            var fileName = Path.GetTempFileName();
            try
            {
                using (var writer = fileSystem.CreateXmlWriter(fileName))
                {
                    Assert.IsNotNull(writer);
                }
            }
            finally
            {
                CleanUpFile(fileName);
            }
        }

        [Test]
        public void EnsureFolderExistsCreatesFolderIfItDoesNotExist()
        {
            var fileSystem = new FileSystem();
            var folder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            try
            {
                fileSystem.EnsureFolderExists(folder);
                Assert.IsTrue(Directory.Exists(folder));
            }
            finally
            {
                if (Directory.Exists(folder))
                {
                    Directory.Delete(folder, true);
                }
            }
        }

        [Test]
        public void CopyFileCopiesFile()
        {
            var fileSystem = new FileSystem();
            var file1Name = Path.GetTempFileName() + "1";
            var file2Name = Path.GetTempFileName() + "2";
            try
            {
                File.WriteAllText(file1Name, "Test");
                fileSystem.CopyFile(file1Name, file2Name);
                Assert.IsTrue(File.Exists(file1Name));
                Assert.IsTrue(File.Exists(file2Name));
                var text = File.ReadAllText(file2Name);
                Assert.AreEqual("Test", text);
            }
            finally
            {
                CleanUpFile(file1Name);
                CleanUpFile(file2Name);
            }
        }

        [Test]
        public void MoveFileMovesFile()
        {
            var fileSystem = new FileSystem();
            var file1Name = Path.GetTempFileName() + "1";
            var file2Name = Path.GetTempFileName() + "2";
            try
            {
                File.WriteAllText(file1Name, "Test");
                fileSystem.MoveFile(file1Name, file2Name);
                Assert.IsFalse(File.Exists(file1Name));
                Assert.IsTrue(File.Exists(file2Name));
                var text = File.ReadAllText(file2Name);
                Assert.AreEqual("Test", text);
            }
            finally
            {
                CleanUpFile(file1Name);
                CleanUpFile(file2Name);
            }
        }

        [Test]
        public void DeleteFileDeletesFile()
        {
            var fileSystem = new FileSystem();
            var fileName = Path.GetTempFileName();
            try
            {
                File.WriteAllText(fileName, "Test");
                fileSystem.DeleteFile(fileName);
                Assert.IsFalse(File.Exists(fileName));
            }
            finally
            {
                CleanUpFile(fileName);
            }
        }

        [Test]
        public void OpenFileForWriteOpensANewStream()
        {
            var fileSystem = new FileSystem();
            var fileName = Path.GetTempFileName();
            try
            {
                using (var stream = fileSystem.OpenFileForWrite(fileName))
                {
                    Assert.IsNotNull(stream);
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write("This is some test data");
                    }
                }

                Assert.IsTrue(File.Exists(fileName));
                Assert.AreEqual("This is some test data", File.ReadAllText(fileName));
            }
            finally
            {
                CleanUpFile(fileName);
            }
        }

        [Test]
        public void OpenFileForWriteOverwritesExistingFile()
        {
            var fileSystem = new FileSystem();
            var fileName = Path.GetTempFileName();
            try
            {
                File.WriteAllText(fileName, "Some previous text");
                using (var stream = fileSystem.OpenFileForWrite(fileName))
                {
                    Assert.IsNotNull(stream);
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write("This is some test data");
                    }
                }

                Assert.IsTrue(File.Exists(fileName));
                Assert.AreEqual("This is some test data", File.ReadAllText(fileName));
            }
            finally
            {
                CleanUpFile(fileName);
            }
        }
        #endregion

        #region Helper methods
        private static void CleanUpFile(string filename)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
        }
        #endregion
    }
}
