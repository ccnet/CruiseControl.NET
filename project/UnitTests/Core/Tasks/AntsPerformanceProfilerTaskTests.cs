namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Exortech.NetReflector;
    using Moq;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Config;
    using ThoughtWorks.CruiseControl.Core.Tasks;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;

    [TestFixture]
    public class AntsPerformanceProfilerTaskTests
    {
        #region Private constants
        private const string defaultWorkingDir = "workingDir";
        #endregion

        #region Private fields
        private MockRepository mocks;
        private readonly string defaultExecutableArgs = "/wd:" + defaultWorkingDir +
            " /out:" + Path.Combine(defaultWorkingDir, "AntsPerformanceAnalysis.txt") +
            " /t:120 /ml /f /is:on /in:on /comp:on /simp:on /notriv:on";
        #endregion

        #region Setup
        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository(MockBehavior.Default);
        }
        #endregion

        #region Tests
        [Test]
        public void DefaultConstructorSetsFileSystemAndLogger()
        {
            var task = new AntsPerformanceProfilerTask();
            Assert.IsInstanceOf<SystemIoFileSystem>(task.FileSystem);
            Assert.IsInstanceOf<DefaultLogger>(task.Logger);
        }

        [Test]
        public void ShouldLoadMinimalValuesFromConfiguration()
        {
            const string xml = @"<antsPerformance><app>testapp.exe</app></antsPerformance>";
            var task = NetReflector.Read(xml) as AntsPerformanceProfilerTask;
            Assert.IsNull(task.Executable);
            Assert.AreEqual("testapp.exe", task.Application);
        }

        [Test]
        public void RunsWithDefaultParametersForServiceProfile()
        {
            var appName = "WinService";
            var result = GenerateResultMock();
            var executablePath = Path.Combine(defaultWorkingDir, "Profile");
            var executor = GenerateExecutorMock(
                executablePath,
                this.defaultExecutableArgs + " /service:" + appName,
                defaultWorkingDir,
                600000);
            var fileSystemMock = this.InitialiseFileSystemMock(executablePath);
            var logger = mocks.Create<ILogger>().Object;
            var task = new AntsPerformanceProfilerTask(executor, fileSystemMock, logger) { PublishFiles = false };
            task.Service = appName;

            Mock.Get(result).SetupProperty(_result => _result.Status);
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.Verify();
        }

        [Test]
        public void RunsWithDefaultParametersForComPlusProfile()
        {
            var appName = "ComPlusService";
            var result = GenerateResultMock();
            var executablePath = Path.Combine(defaultWorkingDir, "Profile");
            var executor = GenerateExecutorMock(
                executablePath,
                this.defaultExecutableArgs + " /complus:" + appName,
                defaultWorkingDir,
                600000);
            var fileSystemMock = this.InitialiseFileSystemMock(executablePath);
            var logger = mocks.Create<ILogger>().Object;
            var task = new AntsPerformanceProfilerTask(executor, fileSystemMock, logger) { PublishFiles = false };
            task.ComPlus = appName;

            Mock.Get(result).SetupProperty(_result => _result.Status);
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.Verify();
        }

        [Test]
        public void RunsWithDefaultParametersForSilverlightProfile()
        {
            var appName = "http://somewhere/silverlight.html";
            var result = GenerateResultMock();
            var executablePath = Path.Combine(defaultWorkingDir, "Profile");
            var executor = GenerateExecutorMock(
                executablePath,
                this.defaultExecutableArgs + " /silverlight:" + appName,
                defaultWorkingDir,
                600000);
            var fileSystemMock = this.InitialiseFileSystemMock(executablePath);
            var logger = mocks.Create<ILogger>().Object;
            var task = new AntsPerformanceProfilerTask(executor, fileSystemMock, logger) { PublishFiles = false };
            task.Silverlight = appName;

            Mock.Get(result).SetupProperty(_result => _result.Status);
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.Verify();
        }

        [Test]
        public void RunsWithDefaultParametersForAppProfile()
        {
            var appName = "testApp.exe";
            var result = GenerateResultMock();
            var executablePath = Path.Combine(defaultWorkingDir, "Profile");
            var executor = GenerateExecutorMock(
                executablePath,
                this.defaultExecutableArgs + " /e:" + Path.Combine(defaultWorkingDir, appName),
                defaultWorkingDir,
                600000);
            var fileSystemMock = this.InitialiseFileSystemMock(executablePath);
            var logger = mocks.Create<ILogger>().Object;
            var task = new AntsPerformanceProfilerTask(executor, fileSystemMock, logger) { PublishFiles = false };
            task.Application = appName;

            Mock.Get(result).SetupProperty(_result => _result.Status);
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.Verify();
        }

        [Test]
        public void RunsWithBaseDirectoryForAppProfile()
        {
            var baseDir = "aDirSomewhere";
            var appName = "testApp.exe";
            var result = GenerateResultMock();
            var executablePath = Path.Combine(baseDir, "Profile");
            var executor = GenerateExecutorMock(
                executablePath,
                "/wd:" + baseDir +
                    " /out:" + Path.Combine(baseDir, "AntsPerformanceAnalysis.txt") +
                    " /t:120 /ml /f /is:on /in:on /comp:on /simp:on /notriv:on /e:" + Path.Combine(baseDir, appName),
                baseDir,
                600000);
            var fileSystemMock = this.InitialiseFileSystemMock(executablePath);
            var logger = mocks.Create<ILogger>().Object;
            var task = new AntsPerformanceProfilerTask(executor, fileSystemMock, logger) { PublishFiles = false };
            task.Application = appName;
            task.BaseDirectory = baseDir;

            Mock.Get(result).SetupProperty(_result => _result.Status);
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.Verify();
        }

        [Test]
        public void RunsWithDefaultParametersForAppProfileAndAllFiles()
        {
            var appName = "testApp.exe";
            var result = GenerateResultMock();
            var executablePath = Path.Combine(defaultWorkingDir, "Profile");
            var executor = GenerateExecutorMock(
                executablePath,
                this.defaultExecutableArgs + " /e:" + Path.Combine(defaultWorkingDir, appName) +
                    " /csv:" + Path.Combine(defaultWorkingDir, "summary.csv") +
                    " /xml:" + Path.Combine(defaultWorkingDir, "summary.xml") +
                    " /h:" + Path.Combine(defaultWorkingDir, "summary.html") +
                    " /calltree:" + Path.Combine(defaultWorkingDir, "calltree.xml") +
                    " /cth:" + Path.Combine(defaultWorkingDir, "calltree.html") +
                    " /data:" + Path.Combine(defaultWorkingDir, "data.out"),
                defaultWorkingDir,
                600000);
            var fileSystemMock = this.InitialiseFileSystemMock(executablePath);
            var logger = mocks.Create<ILogger>().Object;
            var task = new AntsPerformanceProfilerTask(executor, fileSystemMock, logger) { PublishFiles = false };
            task.Application = appName;
            task.SummaryCsvFile = "summary.csv";
            task.SummaryXmlFile = "summary.xml";
            task.SummaryHtmlFile = "summary.html";
            task.CallTreeXmlFile = "calltree.xml";
            task.CallTreeHtmlFile = "calltree.html";
            task.DataFile = "data.out";

            Mock.Get(result).SetupProperty(_result => _result.Status);
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.Verify();
        }

        [Test]
        public void RunsWithDefaultParametersForAppProfilePublishesAllFiles()
        {
            var appName = "testApp.exe";
            var result = GenerateResultMock();
            var executablePath = Path.Combine(defaultWorkingDir, "Profile");
            var executor = GenerateExecutorMock(
                executablePath,
                this.defaultExecutableArgs + " /e:" + Path.Combine(defaultWorkingDir, appName) +
                    " /csv:" + Path.Combine(defaultWorkingDir, "summary.csv") +
                    " /xml:" + Path.Combine(defaultWorkingDir, "summary.xml") +
                    " /h:" + Path.Combine(defaultWorkingDir, "summary.html") +
                    " /calltree:" + Path.Combine(defaultWorkingDir, "calltree.xml") +
                    " /cth:" + Path.Combine(defaultWorkingDir, "calltree.html") +
                    " /data:" + Path.Combine(defaultWorkingDir, "data.out"),
                defaultWorkingDir,
                600000);
            var fileSystemMock = this.InitialiseFileSystemMock(executablePath);

            ExpectFileCheckAndCopy(fileSystemMock, "AntsPerformanceAnalysis.txt");
            ExpectFileCheckAndCopy(fileSystemMock, "summary.csv");
            ExpectFileCheckAndCopy(fileSystemMock, "summary.xml");
            ExpectFileCheckAndCopy(fileSystemMock, "summary.html");
            ExpectFileCheckAndCopy(fileSystemMock, "calltree.xml");
            ExpectFileCheckAndCopy(fileSystemMock, "calltree.html");
            ExpectFileCheckAndCopy(fileSystemMock, "data.out");

            var logger = mocks.Create<ILogger>().Object;
            var task = new AntsPerformanceProfilerTask(executor, fileSystemMock, logger);
            task.Application = appName;
            task.SummaryCsvFile = "summary.csv";
            task.SummaryXmlFile = "summary.xml";
            task.SummaryHtmlFile = "summary.html";
            task.CallTreeXmlFile = "calltree.xml";
            task.CallTreeHtmlFile = "calltree.html";
            task.DataFile = "data.out";

            Mock.Get(result).SetupProperty(_result => _result.Status);
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.Verify();
        }

        private static void ExpectFileCheckAndCopy(IFileSystem fileSystem, string fileName)
        {
            var sourceFile = Path.Combine(defaultWorkingDir, fileName);
            var destFile = Path.Combine(
                Path.Combine("artefactDir", "1"),
                Path.Combine("AntsPerformance", fileName));
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.FileExists(sourceFile)).Returns(true).Verifiable();
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.Copy(sourceFile, destFile)).Verifiable();
        }

        [Test]
        public void RunsWithBaseDirectoryForAppProfileAndAllFiles()
        {
            var baseDir = "aDirSomewhere";
            var appName = "testApp.exe";
            var result = GenerateResultMock();
            var executablePath = Path.Combine(baseDir, "Profile");
            var executor = GenerateExecutorMock(
                executablePath,
                "/wd:" + baseDir +
                    " /out:" + Path.Combine(baseDir, "AntsPerformanceAnalysis.txt") +
                    " /t:120 /ml /f /is:on /in:on /comp:on /simp:on /notriv:on /e:" + Path.Combine(baseDir, appName) +
                    " /csv:" + Path.Combine(baseDir, "summary.csv") +
                    " /xml:" + Path.Combine(baseDir, "summary.xml") +
                    " /h:" + Path.Combine(baseDir, "summary.html") +
                    " /calltree:" + Path.Combine(baseDir, "calltree.xml") +
                    " /cth:" + Path.Combine(baseDir, "calltree.html") +
                    " /data:" + Path.Combine(baseDir, "data.out"),
                baseDir,
                600000);
            var fileSystemMock = this.InitialiseFileSystemMock(executablePath);
            var logger = mocks.Create<ILogger>().Object;
            var task = new AntsPerformanceProfilerTask(executor, fileSystemMock, logger) { PublishFiles = false };
            task.Application = appName;
            task.BaseDirectory = baseDir;
            task.SummaryCsvFile = "summary.csv";
            task.SummaryXmlFile = "summary.xml";
            task.SummaryHtmlFile = "summary.html";
            task.CallTreeXmlFile = "calltree.xml";
            task.CallTreeHtmlFile = "calltree.html";
            task.DataFile = "data.out";

            Mock.Get(result).SetupProperty(_result => _result.Status);
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.Verify();
        }

        [Test]
        public void RunsWithOptionalParametersForAppProfile()
        {
            var appName = "testApp.exe";
            var result = GenerateResultMock();
            var executablePath = Path.Combine(defaultWorkingDir, "Profile");
            var executor = GenerateExecutorMock(
                executablePath,
                "/wd:" + defaultWorkingDir +
                    " /out:" + Path.Combine(defaultWorkingDir, "somewhereElse.txt") +
                    " /t:240 /ll /ows /sm /sp /rs /is:off /in:off /comp:off /simp:off /notriv:off /e:" + Path.Combine(defaultWorkingDir, appName) +
                    " /args:args /q /v /argfile:" + Path.Combine(defaultWorkingDir, "args.xml") +
                    " /threshold:10",
                defaultWorkingDir,
                600000);
            var fileSystemMock = this.InitialiseFileSystemMock(executablePath);
            var logger = mocks.Create<ILogger>().Object;
            var task = new AntsPerformanceProfilerTask(executor, fileSystemMock, logger) { PublishFiles = false };
            task.Application = appName;
            task.Quiet = true;
            task.Verbose = true;
            task.XmlArgsFile = "args.xml";
            task.OutputFile = "somewhereElse.txt";
            task.ApplicationArguments = "args";
            task.ProfilerTimeOut = 240;
            task.TraceLevelValue = AntsPerformanceProfilerTask.TraceLevel.Line;
            task.OnlyWithSource = true;
            task.UseSampling = true;
            task.IncludeSource = false;
            task.AllowInlining = false;
            task.Compensate = false;
            task.SimplifyStackTraces = false;
            task.AvoidTrivial = false;
            task.IncludeSubProcesses = true;
            task.RecordSqlAndIO = true;
            task.ForceOverwrite = false;
            task.Threshold = 10;

            Mock.Get(result).SetupProperty(_result => _result.Status);
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.Verify();
        }

        [Test]
        public void XmlArgsFilesIsQuotedWhenContainingSpaces()
        {
            var appName = "testApp.exe";
            var result = GenerateResultMock();
            var executablePath = Path.Combine(defaultWorkingDir, "Profile");
            var executor = GenerateExecutorMock(
                executablePath,
                this.defaultExecutableArgs + " /e:" + Path.Combine(defaultWorkingDir, appName) +
                " /argfile:\"" + Path.Combine(defaultWorkingDir, "args config.xml") + "\"",
                defaultWorkingDir,
                600000);
            var fileSystemMock = this.InitialiseFileSystemMock(executablePath);
            var logger = mocks.Create<ILogger>().Object;
            var task = new AntsPerformanceProfilerTask(executor, fileSystemMock, logger) { PublishFiles = false };
            task.Application = appName;
            task.XmlArgsFile = "args config.xml";

            Mock.Get(result).SetupProperty(_result => _result.Status);
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.Verify();
        }

        [Test]
        public void XmlArgsFilesExcludesWorkingDirectoryWhenRooted()
        {
            string filename = Platform.IsWindows ? @"c:\args.xml" : "/args.xml";
        
            var appName = "testApp.exe";
            var result = GenerateResultMock();
            var executablePath = Path.Combine(defaultWorkingDir, "Profile");
            var executor = GenerateExecutorMock(
                executablePath,
                this.defaultExecutableArgs + " /e:" + Path.Combine(defaultWorkingDir, appName) + " /argfile:" + filename,
                defaultWorkingDir,
                600000);
            var fileSystemMock = this.InitialiseFileSystemMock(executablePath);
            var logger = mocks.Create<ILogger>().Object;
            var task = new AntsPerformanceProfilerTask(executor, fileSystemMock, logger) { PublishFiles = false };
            task.Application = appName;
            task.XmlArgsFile = filename;

            Mock.Get(result).SetupProperty(_result => _result.Status);
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.Verify();
        }

        [Test]
        public void ValidateDoesNotAllowBothQuietAndVerbose()
        {
            var trace = new ConfigurationTrace(null, null);
            var errorProcesser = this.mocks.Create<IConfigurationErrorProcesser>(MockBehavior.Strict).Object;
            Mock.Get(errorProcesser).Setup(_errorProcesser => _errorProcesser.ProcessError("Cannot have both verbose and quiet set")).Verifiable();
            var task = new AntsPerformanceProfilerTask();
            task.Quiet = true;
            task.Verbose = true;
            task.Validate(null, trace, errorProcesser);
            this.mocks.VerifyAll();
        }

        [Test]
        public void ValidateDoesNotAllowaNegativeProfilerTimeout()
        {
            var trace = new ConfigurationTrace(null, null);
            var errorProcesser = this.mocks.Create<IConfigurationErrorProcesser>(MockBehavior.Strict).Object;
            Mock.Get(errorProcesser).Setup(_errorProcesser => _errorProcesser.ProcessError("profilerTimeout cannot be negative")).Verifiable();
            var task = new AntsPerformanceProfilerTask();
            task.ProfilerTimeOut = -1;
            task.Validate(null, trace, errorProcesser);
            this.mocks.VerifyAll();
        }

        [Test]
        public void ShouldFailIfProcessTimesOut()
        {
            var executorStub = mocks.Create<ProcessExecutor>(MockBehavior.Strict).Object;
            Mock.Get(executorStub).Setup(_executorStub => _executorStub.Execute(It.IsAny<ProcessInfo>())).Returns(ProcessResultFixture.CreateTimedOutResult());

            var task = CreateTask(executorStub);
            var result = IntegrationResultMother.CreateUnknown();

            task.Run(result);
            mocks.Verify();

            Assert.That(result.Status, Is.EqualTo(IntegrationStatus.Failure));
            Assert.That(result.TaskOutput, Does.Match("Command line '.*' timed out after \\d+ seconds"));
        }
        #endregion

        #region Private methods
        private AntsPerformanceProfilerTask CreateTask(ProcessExecutor executor)
        {
            var fileSystemMock = this.InitialiseFileSystemMock("Profile");
            Mock.Get(fileSystemMock).Setup(_fileSystemMock => _fileSystemMock.FileExists(It.IsAny<string>()))
                .Returns(false);
            var logger = mocks.Create<ILogger>().Object;

            return new AntsPerformanceProfilerTask(executor, fileSystemMock, logger);
        }

        private IIntegrationResult GenerateResultMock()
        {
            return GenerateResultMock(defaultWorkingDir, "artefactDir");
        }

        private IIntegrationResult GenerateResultMock(string workingDir, string artefactDir)
        {
            var buildInfo = mocks.Create<BuildProgressInformation>(string.Empty, string.Empty).Object;
            var result = mocks.Create<IIntegrationResult>(MockBehavior.Strict).Object;
            Mock.Get(result).SetupGet(_result => _result.BuildProgressInformation).Returns(buildInfo);
            Mock.Get(result).SetupGet(_result => _result.WorkingDirectory).Returns(workingDir);
            Mock.Get(result).SetupGet(_result => _result.ArtifactDirectory).Returns(artefactDir);
            Mock.Get(result).SetupGet(_result => _result.IntegrationProperties).Returns(new Dictionary<string, string>());
            Mock.Get(result).SetupGet(_result => _result.Label).Returns("1");
            Mock.Get(result).Setup(_result => _result.AddTaskResult(It.IsAny<ITaskResult>())).Verifiable();
            Mock.Get(result).Setup(_result => _result.BaseFromArtifactsDirectory("1")).Returns(System.IO.Path.Combine(artefactDir, "1"));
            return result;
        }

        private ProcessExecutor GenerateExecutorMock(string fileName, string args, string workingDir, int timeout)
        {
            var executor = mocks.Create<ProcessExecutor>(MockBehavior.Strict).Object;
            Mock.Get(executor).Setup(_executor => _executor.Execute(It.IsAny<ProcessInfo>()))
                .Callback<ProcessInfo>(info =>
                {
                    Assert.AreEqual(fileName, info.FileName);
                    Assert.AreEqual(args, info.Arguments);
                    Assert.AreEqual(workingDir, info.WorkingDirectory);
                    Assert.AreEqual(timeout, info.TimeOut);
                }).Returns(new ProcessResult(string.Empty, string.Empty, 0, false)).Verifiable();
            return executor;
        }

        private IFileSystem InitialiseFileSystemMock(string executablePath)
        {
            var fileSystemMock = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            Mock.Get(fileSystemMock).Setup(_fileSystemMock => _fileSystemMock.FileExists(executablePath)).Returns(true);
            Mock.Get(fileSystemMock).Setup(_fileSystemMock => _fileSystemMock.GetFileVersion(executablePath))
                .Returns(new Version(6, 1));
            return fileSystemMock;
        }
        #endregion
    }
}
