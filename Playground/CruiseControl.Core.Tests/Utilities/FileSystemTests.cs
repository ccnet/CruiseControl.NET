namespace CruiseControl.Core.Tests.Utilities
{
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
        #endregion
    }
}
