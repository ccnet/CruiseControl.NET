namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Exortech.NetReflector;
    using Moq;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Core.Tasks;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Core;

    [TestFixture]
    public class CodeItRightTaskTests
    {
        #region Private constants
        private const string defaultWorkingDir = "workingDir";
        #endregion

        #region Private fields
        private MockRepository mocks;
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
        public void ShouldLoadMinimalValuesFromConfiguration()
        {
            const string xml = @"<codeItRight />";
            var task = NetReflector.Read(xml) as CodeItRightTask;
            Assert.AreEqual("SubMain.CodeItRight.Cmd", task.Executable);
        }

        [Test]
        public void RunsWithDefaultParameters()
        {
            var result = GenerateResultMock();
            var executablePath = "SubMain.CodeItRight.Cmd";
            var executor = GenerateExecutorMock(
                executablePath,
                "/quiet /severityThreshold:\"None\" /out:\"" + Path.Combine(defaultWorkingDir, "codeitright.xml") +
                "\" /Solution:\"" + Path.Combine(defaultWorkingDir, "test.sln") + "\"",
                defaultWorkingDir,
                600000);
            var fileSystemMock = this.InitialiseFileSystemMock(defaultWorkingDir);
            var task = new CodeItRightTask(executor, fileSystemMock)
            {
                Solution = "test.sln"
            };

            Mock.Get(result).SetupProperty(_result => _result.Status);
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.Verify();
        }

        [Test]
        public void RunsWithFullParameters()
        {
            var result = GenerateResultMock();
            var executablePath = "SubMain.CodeItRight.Cmd";
            var executor = GenerateExecutorMock(
                executablePath,
                "/quiet /severityThreshold:\"None\" /out:\"" + Path.Combine(defaultWorkingDir, "codeitright.xml") +
                "\" /Project:\"" + Path.Combine(defaultWorkingDir, "test.csproj") +
                "\" /outxsl:\"" + Path.Combine(defaultWorkingDir, "test.xsl") + 
                "\" /crdata:\"" + Path.Combine(defaultWorkingDir, "test.crdata") + 
                "\" /profile:\"profile\"",
                defaultWorkingDir,
                600000);
            var fileSystemMock = this.InitialiseFileSystemMock(defaultWorkingDir);
            var task = new CodeItRightTask(executor, fileSystemMock)
            {
                Project = "test.csproj",
                CRData = "test.crdata",
                Xsl = "test.xsl",
                Profile = "profile"
            };

            Mock.Get(result).SetupProperty(_result => _result.Status);
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.Verify();
        }

        [Test]
        public void RunFailsWithoutProjectOrSolution()
        {
            var result = GenerateResultMock();
            var executablePath = "SubMain.CodeItRight.Cmd";
            var executor = GenerateExecutorMock(
                executablePath,
                "/quiet /severityThreshold:\"None\" /out:\"" + Path.Combine(defaultWorkingDir, "codeitright.xml") +
                "\" /Project:\"" + Path.Combine(defaultWorkingDir, "test.csproj") +
                "\" /outxsl:\"" + Path.Combine(defaultWorkingDir, "test.xsl") +
                "\" /crdata:\"" + Path.Combine(defaultWorkingDir, "test.crdata") +
                "\" /profile:\"profile\"",
                defaultWorkingDir,
                600000);
            var fileSystemMock = this.InitialiseFileSystemMock(defaultWorkingDir);
            var task = new CodeItRightTask(executor, fileSystemMock);

            Mock.Get(result).SetupProperty(_result => _result.Status);
            result.Status = IntegrationStatus.Unknown;
            Assert.Throws<CruiseControlException>(() => task.Run(result));
        }

        [Test]
        public void RunsFailsIfSeverityExists()
        {
            var result = GenerateResultMock();
            var executablePath = "SubMain.CodeItRight.Cmd";
            var executor = GenerateExecutorMock(
                executablePath,
                "/quiet /severityThreshold:\"None\" /out:\"" + Path.Combine(defaultWorkingDir, "codeitright.xml") +
                "\" /Project:\"" + Path.Combine(defaultWorkingDir, "test.csproj") +
                "\" /outxsl:\"" + Path.Combine(defaultWorkingDir, "test.xsl") +
                "\" /crdata:\"" + Path.Combine(defaultWorkingDir, "test.crdata") +
                "\" /profile:\"profile\"",
                defaultWorkingDir,
                600000);
            var fileSystemMock = this.InitialiseFileSystemMock(defaultWorkingDir);
            var task = new CodeItRightTask(executor, fileSystemMock)
            {
                Project = "test.csproj",
                CRData = "test.crdata",
                Xsl = "test.xsl",
                Profile = "profile",
                FailureThreshold = CodeItRightTask.Severity.Error
            };

            Mock.Get(result).SetupProperty(_result => _result.Status);
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            Assert.AreEqual(IntegrationStatus.Failure, result.Status);
            mocks.VerifyAll();
        }
        #endregion

        #region Private methods
        private IIntegrationResult GenerateResultMock()
        {
            return GenerateResultMock(defaultWorkingDir);
        }

        private IIntegrationResult GenerateResultMock(string workingDir)
        {
            var buildInfo = mocks.Create<BuildProgressInformation>(string.Empty, string.Empty).Object;
            var result = mocks.Create<IIntegrationResult>(MockBehavior.Strict).Object;
            Mock.Get(result).SetupGet(_result => _result.BuildProgressInformation).Returns(buildInfo);
            Mock.Get(result).SetupGet(_result => _result.WorkingDirectory).Returns(workingDir);
            Mock.Get(result).SetupGet(_result => _result.IntegrationProperties).Returns(new Dictionary<string, string>());
            Mock.Get(result).Setup(_result => _result.BaseFromWorkingDirectory(It.IsAny<string>()))
                .Returns<string>(p => Path.Combine(workingDir, p));
            Mock.Get(result).Setup(_result => _result.AddTaskResult(It.IsAny<ProcessTaskResult>())).Verifiable();
            Mock.Get(result).Setup(_result => _result.AddTaskResult((ITaskResult)null)).Verifiable();
            return result;
        }

        private ProcessExecutor GenerateExecutorMock(string fileName, string args, string workingDir, int timeout)
        {
            var executor = mocks.Create<ProcessExecutor>(MockBehavior.Strict).Object;
            Mock.Get(executor).Setup(_executor => _executor.Execute(It.IsAny<ProcessInfo>()))
                .Callback<ProcessInfo>(info => {
                    Assert.AreEqual(fileName, info.FileName);
                    Assert.AreEqual(args, info.Arguments);
                    Assert.AreEqual(workingDir, info.WorkingDirectory);
                    Assert.AreEqual(timeout, info.TimeOut);
                }).Returns(new ProcessResult(string.Empty, string.Empty, 0, false)).Verifiable();
            return executor;
        }

        private IFileSystem InitialiseFileSystemMock(string workingDir)
        {
            var xmlFile = Path.Combine(workingDir, "codeitright.xml");
            var fileSystemMock = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            Mock.Get(fileSystemMock).Setup(_fileSystemMock => _fileSystemMock.GenerateTaskResultFromFile(xmlFile, true))
                .Returns((ITaskResult)null).Verifiable();
            Mock.Get(fileSystemMock).Setup(_fileSystemMock => _fileSystemMock.FileExists(xmlFile)).Returns(true);
            var xml = "<CodeItRightReport>" +
                    "<Violations>" + 
                        "<Violation>" +
                            "<Severity>Error</Severity>" +
                        "</Violation>" +
                    "</Violations>" +
                "</CodeItRightReport>";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            Mock.Get(fileSystemMock).Setup(_fileSystemMock => _fileSystemMock.OpenInputStream(xmlFile)).Returns(stream);
            return fileSystemMock;
        }
        #endregion
    }
}
