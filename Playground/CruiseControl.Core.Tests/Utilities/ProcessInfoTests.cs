namespace CruiseControl.Core.Tests.Utilities
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using CruiseControl.Core.Interfaces;
    using CruiseControl.Core.Utilities;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class ProcessInfoTests
    {
        #region Tests
        [Test]
        public void MinimalConstructorSetsFilename()
        {
            var filename = "nameoffile";
            var info = new ProcessInfo(filename);
            Assert.AreEqual(filename, info.FileName);
            Assert.AreEqual(2, info.TimeOut.Minutes);
        }

        [Test]
        public void ConstructorSetsProperties()
        {
            var filename = "nameoffile";
            var args = new SecureArguments("value 1", new PrivateString("value 2"));
            var workingDirectory = "C:\\somewhere";
            var priority = ProcessPriorityClass.AboveNormal;
            var exitCodes = new[] { 0, 1, 2, 3 };
            var info = new ProcessInfo(filename, args, workingDirectory, priority, exitCodes);
            Assert.AreEqual(filename, info.FileName);
            Assert.AreSame(args, info.Arguments);
            var argsValue = "value 1 value 2";
            Assert.AreEqual(argsValue, info.PrivateArguments);
            Assert.AreNotEqual(argsValue, info.PublicArguments);
            Assert.AreEqual(workingDirectory, info.WorkingDirectory);
            Assert.AreEqual(priority, info.Priority);
            Assert.AreEqual(exitCodes, info.SuccessExitCodes);
            Assert.IsNotNull(info.EnvironmentVariables);
        }

        [Test]
        public void PublicArgumentsHandlesNull()
        {
            var info = new ProcessInfo("somewhere");
            Assert.IsNull(info.PublicArguments);
        }

        [Test]
        public void CheckIfSuccessReturnsTrueIfInSuccessCodes()
        {
            var info = new ProcessInfo("somewhere", null, null, ProcessPriorityClass.Normal, new[] { 0, 1 });
            var actual = info.CheckIfSuccess(1);
            Assert.IsTrue(actual);
        }

        [Test]
        public void CheckIfSuccessReturnsFalseIfNotInSuccessCodes()
        {
            var info = new ProcessInfo("somewhere", null, null, ProcessPriorityClass.Normal, new[] { 0, 1 });
            var actual = info.CheckIfSuccess(3);
            Assert.IsFalse(actual);
        }

        [Test]
        public void StreamEncodingCanBeChanged()
        {
            var info = new ProcessInfo("somewhere");
            var encoding = Encoding.UTF32;
            info.StreamEncoding = encoding;
            Assert.AreEqual(encoding, info.StreamEncoding);
        }

        [Test]
        public void StandardInputContentCanBeChanged()
        {
            var info = new ProcessInfo("somewhere");
            var expected = "some data to pass in";
            info.StandardInputContent = expected;
            Assert.AreEqual(expected, info.StandardInputContent);
        }

        [Test]
        public void TimeOutCanBeChanged()
        {
            var info = new ProcessInfo("somewhere");
            info.TimeOut = TimeSpan.FromHours(1);
            Assert.AreEqual(1, info.TimeOut.Hours);
        }

        [Test]
        public void CreateProcessCreatesTheProcess()
        {
            var fileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);
            fileSystemMock.Setup(fs => fs.CheckIfFileExists(It.IsAny<string>())).Returns(false);
            var filename = "somewhere";
            var info = new ProcessInfo(filename);
            var process = info.CreateProcess(fileSystemMock.Object);
            Assert.AreEqual(filename, process.StartInfo.FileName);
        }

        [Test]
        public void CreateProcessUpdatesTheFilename()
        {
            var fileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);
            fileSystemMock.Setup(fs => fs.CheckIfFileExists(It.IsAny<string>())).Returns(true);
            fileSystemMock.Setup(fs => fs.CheckIfDirectoryExists(It.IsAny<string>())).Returns(true);
            var filename = "somewhere";
            var directory = Path.GetTempPath();
            var info = new ProcessInfo(filename, null, directory);
            var process = info.CreateProcess(fileSystemMock.Object);
            Assert.AreEqual(Path.Combine(directory, filename), process.StartInfo.FileName);
        }

        [Test]
        public void CreateProcessFailsIfTheDirectoryDoesNotExist()
        {
            var fileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);
            fileSystemMock.Setup(fs => fs.CheckIfFileExists(It.IsAny<string>())).Returns(false);
            fileSystemMock.Setup(fs => fs.CheckIfDirectoryExists(It.IsAny<string>())).Returns(false);
            var filename = "somewhere";
            var info = new ProcessInfo(filename);
            info.WorkingDirectory = "d:\\somewhereelse";
            Assert.Throws<DirectoryNotFoundException>(() => info.CreateProcess(fileSystemMock.Object));
        }
        #endregion
    }
}
