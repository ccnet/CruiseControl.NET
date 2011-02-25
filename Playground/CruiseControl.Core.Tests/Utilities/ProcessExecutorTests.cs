namespace CruiseControl.Core.Tests.Utilities
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using CruiseControl.Core.Exceptions;
    using CruiseControl.Core.Interfaces;
    using CruiseControl.Core.Utilities;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class ProcessExecutorTests
    {
        #region Tests
        [Test]
        public void KillByPIdKillsProcess()
        {
            var sleeper = new Process
                              {
                                  StartInfo = new ProcessStartInfo("sleeper")
                              };
            sleeper.Start();
            var started = SpinWait.SpinUntil(() => Process.GetProcessesByName("sleeper").Length > 0,
                                             TimeSpan.FromSeconds(5));
            Assert.IsTrue(started);
            var fileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);
            fileSystemMock.Setup(fs => fs.CheckIfFileExists(It.IsAny<string>())).Returns(true);
            ProcessExecutor.KillByPId(fileSystemMock.Object, sleeper.Id);
            var stopped = SpinWait.SpinUntil(() => Process.GetProcessesByName("sleeper").Length == 0,
                                             TimeSpan.FromSeconds(5));
            Assert.IsTrue(stopped);
        }

        [Test]
        public void KillByPIdThrowsExceptionIfKillMissing()
        {
            var fileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);
            fileSystemMock.Setup(fs => fs.CheckIfFileExists(It.IsAny<string>())).Returns(false);
            Assert.Throws<CruiseControlException>(
                () => ProcessExecutor.KillByPId(fileSystemMock.Object, int.MinValue));
        }

        [Test]
        public void PopulateKillProcessHandlesWindows()
        {
            var killProcess = new Process();
            var operatingSystem = new OperatingSystem(PlatformID.Win32NT, new Version(7, 0));
            var platform = ProcessExecutor.PopulateKillProcess(killProcess, 1, operatingSystem);
            Assert.AreEqual("Windows", platform);
            var expectedPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.System),
                "taskkill.exe");
            Assert.AreEqual(expectedPath, killProcess.StartInfo.FileName);
            Assert.AreEqual("/pid 1 /t /f", killProcess.StartInfo.Arguments);
        }

        [Test]
        public void PopulateKillProcessHandlesWindows2000()
        {
            var killProcess = new Process();
            var operatingSystem = new OperatingSystem(PlatformID.Win32NT, new Version(5, 0));
            var platform = ProcessExecutor.PopulateKillProcess(killProcess, 1, operatingSystem);
            Assert.AreEqual("Windows", platform);
            var expectedPath = Path.Combine(
                ProcessExecutor.Win2KSupportToolsDir,
                "kill.exe");
            Assert.AreEqual(expectedPath, killProcess.StartInfo.FileName);
            Assert.AreEqual("-f 1", killProcess.StartInfo.Arguments);
        }

        [Test]
        public void PopulateKillProcessThrowsExceptionForUnknownPlatform()
        {
            var killProcess = new Process();
            var operatingSystem = new OperatingSystem(PlatformID.Xbox, new Version(5, 0));
            Assert.Throws<CruiseControlException>(
                () => ProcessExecutor.PopulateKillProcess(killProcess, 1, operatingSystem));
        }

        [Test]
        public void PopulateKillProcessHandlesUnix()
        {
            var killProcess = new Process();
            var operatingSystem = new OperatingSystem(PlatformID.Unix, new Version(7, 0));
            var platform = ProcessExecutor.PopulateKillProcess(killProcess, 1, operatingSystem);
            Assert.AreEqual("Unix", platform);
            Assert.AreEqual("/usr/bin/pkill", killProcess.StartInfo.FileName);
            Assert.AreEqual("-9 -g 1", killProcess.StartInfo.Arguments);
        }
        #endregion
    }
}
