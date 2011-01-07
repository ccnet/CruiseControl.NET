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
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
        }

        [Test]
        public void CheckIfFileExistsReturnsFalseIfFileDoesNotExist()
        {
            var fileSystem = new FileSystem();
            var fileName = Path.GetTempFileName();
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
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
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
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
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
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
        #endregion
    }
}
