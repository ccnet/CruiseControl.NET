namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    using System.Collections.Generic;
    using System.IO;
    using Exortech.NetReflector;
    using NUnit.Framework;
    using Rhino.Mocks;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Config;
    using ThoughtWorks.CruiseControl.Core.Tasks;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;
    using System;

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
            mocks = new MockRepository();
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
            var logger = mocks.DynamicMock<ILogger>();
            var task = new AntsPerformanceProfilerTask(executor, fileSystemMock, logger) { PublishFiles = false };
            task.Service = appName;

            Expect.Call(result.Status).PropertyBehavior();
            mocks.ReplayAll();
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.VerifyAll();
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
            var logger = mocks.DynamicMock<ILogger>();
            var task = new AntsPerformanceProfilerTask(executor, fileSystemMock, logger) { PublishFiles = false };
            task.ComPlus = appName;

            Expect.Call(result.Status).PropertyBehavior();
            mocks.ReplayAll();
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.VerifyAll();
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
            var logger = mocks.DynamicMock<ILogger>();
            var task = new AntsPerformanceProfilerTask(executor, fileSystemMock, logger) { PublishFiles = false };
            task.Silverlight = appName;

            Expect.Call(result.Status).PropertyBehavior();
            mocks.ReplayAll();
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.VerifyAll();
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
            var logger = mocks.DynamicMock<ILogger>();
            var task = new AntsPerformanceProfilerTask(executor, fileSystemMock, logger) { PublishFiles = false };
            task.Application = appName;

            Expect.Call(result.Status).PropertyBehavior();
            mocks.ReplayAll();
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.VerifyAll();
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
            var logger = mocks.DynamicMock<ILogger>();
            var task = new AntsPerformanceProfilerTask(executor, fileSystemMock, logger) { PublishFiles = false };
            task.Application = appName;
            task.BaseDirectory = baseDir;

            Expect.Call(result.Status).PropertyBehavior();
            mocks.ReplayAll();
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.VerifyAll();
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
            var logger = mocks.DynamicMock<ILogger>();
            var task = new AntsPerformanceProfilerTask(executor, fileSystemMock, logger) { PublishFiles = false };
            task.Application = appName;
            task.SummaryCsvFile = "summary.csv";
            task.SummaryXmlFile = "summary.xml";
            task.SummaryHtmlFile = "summary.html";
            task.CallTreeXmlFile = "calltree.xml";
            task.CallTreeHtmlFile = "calltree.html";
            task.DataFile = "data.out";

            Expect.Call(result.Status).PropertyBehavior();
            mocks.ReplayAll();
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.VerifyAll();
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

            var logger = mocks.DynamicMock<ILogger>();
            var task = new AntsPerformanceProfilerTask(executor, fileSystemMock, logger);
            task.Application = appName;
            task.SummaryCsvFile = "summary.csv";
            task.SummaryXmlFile = "summary.xml";
            task.SummaryHtmlFile = "summary.html";
            task.CallTreeXmlFile = "calltree.xml";
            task.CallTreeHtmlFile = "calltree.html";
            task.DataFile = "data.out";

            Expect.Call(result.Status).PropertyBehavior();
            mocks.ReplayAll();
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.VerifyAll();
        }

        private static void ExpectFileCheckAndCopy(IFileSystem fileSystem, string fileName)
        {
            var sourceFile = Path.Combine(defaultWorkingDir, fileName);
            var destFile = Path.Combine(
                Path.Combine("artefactDir", "1"),
                Path.Combine("AntsPerformance", fileName));
            Expect.Call(fileSystem.FileExists(sourceFile)).Return(true);
            Expect.Call(() => fileSystem.Copy(sourceFile, destFile));
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
            var logger = mocks.DynamicMock<ILogger>();
            var task = new AntsPerformanceProfilerTask(executor, fileSystemMock, logger) { PublishFiles = false };
            task.Application = appName;
            task.BaseDirectory = baseDir;
            task.SummaryCsvFile = "summary.csv";
            task.SummaryXmlFile = "summary.xml";
            task.SummaryHtmlFile = "summary.html";
            task.CallTreeXmlFile = "calltree.xml";
            task.CallTreeHtmlFile = "calltree.html";
            task.DataFile = "data.out";

            Expect.Call(result.Status).PropertyBehavior();
            mocks.ReplayAll();
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.VerifyAll();
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
            var logger = mocks.DynamicMock<ILogger>();
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

            Expect.Call(result.Status).PropertyBehavior();
            mocks.ReplayAll();
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.VerifyAll();
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
            var logger = mocks.DynamicMock<ILogger>();
            var task = new AntsPerformanceProfilerTask(executor, fileSystemMock, logger) { PublishFiles = false };
            task.Application = appName;
            task.XmlArgsFile = "args config.xml";

            Expect.Call(result.Status).PropertyBehavior();
            mocks.ReplayAll();
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.VerifyAll();
        }

        [Test]
        public void XmlArgsFilesExcludesWorkingDirectoryWhenRooted()
        {
            var appName = "testApp.exe";
            var result = GenerateResultMock();
            var executablePath = Path.Combine(defaultWorkingDir, "Profile");
            var executor = GenerateExecutorMock(
                executablePath,
                this.defaultExecutableArgs + " /e:" + Path.Combine(defaultWorkingDir, appName) + " /argfile:c:\\args.xml",
                defaultWorkingDir,
                600000);
            var fileSystemMock = this.InitialiseFileSystemMock(executablePath);
            var logger = mocks.DynamicMock<ILogger>();
            var task = new AntsPerformanceProfilerTask(executor, fileSystemMock, logger) { PublishFiles = false };
            task.Application = appName;
            task.XmlArgsFile = "c:\\args.xml";

            Expect.Call(result.Status).PropertyBehavior();
            mocks.ReplayAll();
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.VerifyAll();
        }

        [Test]
        public void ValidateDoesNotAllowBothQuietAndVerbose()
        {
            var trace = new ConfigurationTrace(null, null);
            var errorProcesser = this.mocks.StrictMock<IConfigurationErrorProcesser>();
            Expect.Call(() => errorProcesser.ProcessError("Cannot have both verbose and quiet set"));
            var task = new AntsPerformanceProfilerTask();
            task.Quiet = true;
            task.Verbose = true;
            this.mocks.ReplayAll();
            task.Validate(null, trace, errorProcesser);
            this.mocks.VerifyAll();
        }

        [Test]
        public void ValidateDoesNotAllowaNegativeProfilerTimeout()
        {
            var trace = new ConfigurationTrace(null, null);
            var errorProcesser = this.mocks.StrictMock<IConfigurationErrorProcesser>();
            Expect.Call(() => errorProcesser.ProcessError("profilerTimeout cannot be negative"));
            var task = new AntsPerformanceProfilerTask();
            task.ProfilerTimeOut = -1;
            this.mocks.ReplayAll();
            task.Validate(null, trace, errorProcesser);
            this.mocks.VerifyAll();
        }

        [Test]
        public void ShouldFailIfProcessTimesOut()
        {
            var executorStub = mocks.StrictMock<ProcessExecutor>();
            SetupResult.For(executorStub.Execute(null)).IgnoreArguments().Return(ProcessResultFixture.CreateTimedOutResult());

            var task = CreateTask(executorStub);
            var result = IntegrationResultMother.CreateUnknown();

            mocks.ReplayAll();
            task.Run(result);
            mocks.VerifyAll();

            Assert.That(result.Status, Is.EqualTo(IntegrationStatus.Failure));
            Assert.That(result.TaskOutput, Does.Match("Command line '.*' timed out after \\d+ seconds"));
        }
        #endregion

        #region Private methods
        private AntsPerformanceProfilerTask CreateTask(ProcessExecutor executor)
        {
            var fileSystemMock = this.InitialiseFileSystemMock("Profile");
            SetupResult.For(fileSystemMock.FileExists(string.Empty))
                .IgnoreArguments()
                .Return(false);
            var logger = mocks.DynamicMock<ILogger>();

            return new AntsPerformanceProfilerTask(executor, fileSystemMock, logger);
        }

        private IIntegrationResult GenerateResultMock()
        {
            return GenerateResultMock(defaultWorkingDir, "artefactDir");
        }

        private IIntegrationResult GenerateResultMock(string workingDir, string artefactDir)
        {
            var buildInfo = mocks.DynamicMock<BuildProgressInformation>(string.Empty, string.Empty);
            var result = mocks.StrictMock<IIntegrationResult>();
            SetupResult.For(result.BuildProgressInformation).Return(buildInfo);
            SetupResult.For(result.WorkingDirectory).Return(workingDir);
            SetupResult.For(result.ArtifactDirectory).Return(artefactDir);
            SetupResult.For(result.IntegrationProperties).Return(new Dictionary<string, string>());
            SetupResult.For(result.Label).Return("1");
            Expect.Call(() => result.AddTaskResult(mocks.DynamicMock<ITaskResult>())).IgnoreArguments().Repeat.Any();
            SetupResult.For(result.BaseFromArtifactsDirectory("1")).Return(string.Concat(artefactDir, "\\1"));
            return result;
        }

        private ProcessExecutor GenerateExecutorMock(string fileName, string args, string workingDir, int timeout)
        {
            var executor = mocks.StrictMock<ProcessExecutor>();
            Expect.Call(executor.Execute(null))
                .IgnoreArguments()
                .Do(new Function<ProcessInfo, ProcessResult>(info =>
                {
                    Assert.AreEqual(fileName, info.FileName);
                    Assert.AreEqual(args, info.Arguments);
                    Assert.AreEqual(workingDir, info.WorkingDirectory);
                    Assert.AreEqual(timeout, info.TimeOut);
                    return new ProcessResult(string.Empty, string.Empty, 0, false);
                }));
            return executor;
        }

        private IFileSystem InitialiseFileSystemMock(string executablePath)
        {
            var fileSystemMock = mocks.StrictMock<IFileSystem>();
            SetupResult.For(fileSystemMock.FileExists(executablePath)).Return(true);
            SetupResult.For(fileSystemMock.GetFileVersion(executablePath))
                .Return(new Version(6, 1));
            return fileSystemMock;
        }
        #endregion
    }
}
