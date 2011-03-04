namespace CruiseControl.Core.Tests.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using CruiseControl.Core.Exceptions;
    using CruiseControl.Core.Interfaces;
    using CruiseControl.Core.Tasks;
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

        [Test]
        public void KillProcessesForProjectKillsAProcess()
        {
            var fileSystemMock = InitialiseFileSystemMockForExecute();
            var info = new ProcessInfo("sleeper");
            var executor = new ProcessExecutor { FileSystem = fileSystemMock.Object };
            var projectName = "aProject";
            var thread = new Thread(
                () => executor.Execute(info, projectName, "aTask", "C:\\somewhere.txt"));
            thread.Start();
            var started = SpinWait.SpinUntil(() => Process.GetProcessesByName("sleeper").Length > 0,
                                             TimeSpan.FromSeconds(5));
            Assert.IsTrue(started);
            ProcessExecutor.KillProcessesForProject(fileSystemMock.Object, projectName);
            var stopped = SpinWait.SpinUntil(() => Process.GetProcessesByName("sleeper").Length == 0,
                                             TimeSpan.FromSeconds(5));
            Assert.IsTrue(stopped);
        }

        [Test]
        public void ExecuteRunsProcess()
        {
            var fileSystemMock = InitialiseFileSystemMockForExecute();
            var info = new ProcessInfo("sleeper", "1");
            var executor = new ProcessExecutor { FileSystem = fileSystemMock.Object };
            var projectName = "aProject";
            var waitHandle = new ManualResetEvent(false);
            ProcessResult result = null;
            var thread = new Thread(
                () =>
                {
                    try
                    {
                        result = executor.Execute(info, projectName, "aTask", "C:\\somewhere.txt");
                    }
                    finally
                    {
                        waitHandle.Set();
                    }
                });
            thread.Start();
            waitHandle.WaitOne(TimeSpan.FromSeconds(30));
            Assert.IsTrue(result.Succeeded);
        }

        [Test]
        public void ExecuteTimesOut()
        {
            var fileSystemMock = InitialiseFileSystemMockForExecute();
            var info = new ProcessInfo("sleeper") { TimeOut = TimeSpan.FromSeconds(1) };
            var executor = new ProcessExecutor { FileSystem = fileSystemMock.Object };
            var projectName = "aProject";
            var waitHandle = new ManualResetEvent(false);
            ProcessResult result = null;
            var thread = new Thread(
                () =>
                {
                    try
                    {
                        result = executor.Execute(info, projectName, "aTask", "C:\\somewhere.txt");
                    }
                    finally
                    {
                        waitHandle.Set();
                    }
                });
            thread.Start();
            waitHandle.WaitOne(TimeSpan.FromSeconds(30));
            Assert.IsTrue(result.TimedOut);
        }

        [Test]
        public void ExecutePassesOnOutput()
        {
            var fileSystemMock = InitialiseFileSystemMockForExecute();
            var info = new ProcessInfo("sleeper", "1");
            var output = new List<ProcessOutputEventArgs>();
            var executor = new ProcessExecutor { FileSystem = fileSystemMock.Object };
            executor.ProcessOutput += (o, e) => output.Add(e);
            var projectName = "aProject";
            var waitHandle = new ManualResetEvent(false);
            ProcessResult result = null;
            var thread = new Thread(
                () =>
                {
                    try
                    {
                        result = executor.Execute(info, projectName, "aTask", "C:\\somewhere.txt");
                    }
                    finally
                    {
                        waitHandle.Set();
                    }
                });
            thread.Start();
            waitHandle.WaitOne(TimeSpan.FromSeconds(30));
            CollectionAssert.IsNotEmpty(output);
            Assert.IsTrue(result.Succeeded);
        }

        [Test]
        public void ExecuteChangesPriority()
        {
            var fileSystemMock = InitialiseFileSystemMockForExecute();
            var info = new ProcessInfo("sleeper", "1", null, ProcessPriorityClass.BelowNormal);
            var executor = new ProcessExecutor { FileSystem = fileSystemMock.Object };
            var projectName = "aProject";
            var waitHandle = new ManualResetEvent(false);
            ProcessResult result = null;
            var thread = new Thread(
                () =>
                {
                    try
                    {
                        result = executor.Execute(info, projectName, "aTask", "C:\\somewhere.txt");
                    }
                    finally
                    {
                        waitHandle.Set();
                    }
                });
            thread.Start();
            waitHandle.WaitOne(TimeSpan.FromSeconds(30));
            Assert.IsTrue(result.Succeeded);
        }

        [Test]
        public void ExecuteWritesToStdIn()
        {
            var fileSystemMock = InitialiseFileSystemMockForExecute();
            var info = new ProcessInfo("sleeper", "1") { StandardInputContent = "SomeData" };
            var executor = new ProcessExecutor { FileSystem = fileSystemMock.Object };
            var projectName = "aProject";
            var waitHandle = new ManualResetEvent(false);
            ProcessResult result = null;
            var thread = new Thread(
                () =>
                {
                    try
                    {
                        result = executor.Execute(info, projectName, "aTask", "C:\\somewhere.txt");
                    }
                    finally
                    {
                        waitHandle.Set();
                    }
                });
            thread.Start();
            waitHandle.WaitOne(TimeSpan.FromSeconds(30));
            Assert.IsTrue(result.Succeeded);
        }

        [Test]
        public void KillProcessesForProjectHandlesAMissingProject()
        {
            ProcessExecutor.KillProcessesForProject(null, "DoesNothingExist");
        }

        [Test]
        public void ExecuteWithProjectItemGeneratesTheRightArguments()
        {
            var info = new ProcessInfo("Test");
            var item = new Comment
                           {
                               Project = new Project("Test")
                           };
            var context = new TaskExecutionContext(
                new TaskExecutionParameters
                    {
                        Project = item.Project
                    });
            var logFile = context.GeneratePathInWorkingDirectory("Comment.log");
            var executor = new ProcessExecutorOverride
                               {
                                   OnExecute = (pi, p, i, o) =>
                                                   {
                                                       Assert.AreSame(info, pi);
                                                       Assert.AreEqual("Test", p);
                                                       Assert.AreEqual("Comment", i);
                                                       Assert.AreEqual(logFile, o);
                                                       return null;
                                                   }
                               };
            executor.Execute(info, item, context);
        }
        #endregion

        #region Helper methods
        private static Mock<IFileSystem> InitialiseFileSystemMockForExecute()
        {
            var fileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);
            fileSystemMock.Setup(fs => fs.CheckIfFileExists(It.IsAny<string>())).Returns(true);
            fileSystemMock.Setup(fs => fs.OpenFileForWrite("C:\\somewhere.txt")).Returns(new MemoryStream());
            fileSystemMock.Setup(fs => fs.OpenFileForRead("C:\\somewhere.txt")).Returns(new MemoryStream());
            return fileSystemMock;
        }
        #endregion

        #region Private classes
        private class ProcessExecutorOverride
            : ProcessExecutor
        {
            public Func<ProcessInfo, string, string, string, ProcessResult> OnExecute { get; set; }
            public override ProcessResult Execute(ProcessInfo processInfo, string projectName, string itemId, string outputFile)
            {
                return this.OnExecute != null ?
                    this.OnExecute(processInfo, projectName, itemId, outputFile) :
                    base.Execute(processInfo, projectName, itemId, outputFile);
            }
        }
        #endregion
    }
}
