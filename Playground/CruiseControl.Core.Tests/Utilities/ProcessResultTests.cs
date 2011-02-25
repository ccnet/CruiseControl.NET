namespace CruiseControl.Core.Tests.Utilities
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using CruiseControl.Core.Interfaces;
    using CruiseControl.Core.Utilities;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class ProcessResultTests
    {
        #region Tests
        [Test]
        public void ConstructorSetsPropertiesForSuccess()
        {
            var filename = "testoutput.txt";
            var fileSystemMock = GenerateFileSystemMock(filename, "OLine 1", "OLine 2");
            var exitCode = 0;
            var result = new ProcessResult(
                fileSystemMock.Object,
                filename,
                exitCode,
                false);
            Assert.AreEqual(filename, result.OutputPath);
            Assert.AreEqual(exitCode, result.ExitCode);
            Assert.IsFalse(result.TimedOut);
            Assert.IsFalse(result.Failed);
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(2, result.NumberOfOutputLines);
            Assert.AreEqual(0, result.NumberOfErrorLines);
            Assert.IsFalse(result.HasErrorOutput);
        }

        [Test]
        public void ConstructorSetsPropertiesForError()
        {
            var filename = "testoutput.txt";
            var fileSystemMock = GenerateFileSystemMock(filename, "OLine 1", "ELine 2");
            var exitCode = 32;
            var result = new ProcessResult(
                fileSystemMock.Object,
                filename,
                exitCode,
                false);
            Assert.AreEqual(filename, result.OutputPath);
            Assert.AreEqual(exitCode, result.ExitCode);
            Assert.IsFalse(result.TimedOut);
            Assert.IsTrue(result.Failed);
            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(1, result.NumberOfOutputLines);
            Assert.AreEqual(1, result.NumberOfErrorLines);
            Assert.IsTrue(result.HasErrorOutput);
        }

        [Test]
        public void ReadStandardOutputOnlyReturnsOutputLines()
        {
            var filename = "testoutput.txt";
            var fileSystemMock = GenerateFileSystemMock(filename, "OLine 1", "ELine 2");
            var result = new ProcessResult(
                fileSystemMock.Object,
                filename,
                0,
                false);
            var actual = result.ReadStandardOutput().ToArray();
            var expected = new[] { "Line 1" };
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void ReadStandardErrorOnlyReturnsOutputLines()
        {
            var filename = "testoutput.txt";
            var fileSystemMock = GenerateFileSystemMock(filename, "OLine 1", "ELine 2");
            var result = new ProcessResult(
                fileSystemMock.Object,
                filename,
                0,
                false);
            var actual = result.ReadStandardError().ToArray();
            var expected = new[] { "Line 2" };
            CollectionAssert.AreEqual(expected, actual);
        }
        #endregion

        #region Helper methods
        private static Mock<IFileSystem> GenerateFileSystemMock(string filename, params string[] lines)
        {
            var fileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);
            var buffer = Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, lines));
            fileSystemMock.Setup(fs => fs.OpenFileForRead(filename))
                .Returns(() => new MemoryStream(buffer));
            return fileSystemMock;
        }
        #endregion
    }
}
