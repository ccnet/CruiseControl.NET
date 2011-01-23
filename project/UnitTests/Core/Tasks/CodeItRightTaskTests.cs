namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;
    using Rhino.Mocks;
    using System.IO;
    using ThoughtWorks.CruiseControl.Core.Tasks;
    using ThoughtWorks.CruiseControl.Core.Util;
    using Exortech.NetReflector;
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
            mocks = new MockRepository();
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

            Expect.Call(result.Status).PropertyBehavior();
            mocks.ReplayAll();
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.VerifyAll();
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

            Expect.Call(result.Status).PropertyBehavior();
            mocks.ReplayAll();
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.VerifyAll();
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

            Expect.Call(result.Status).PropertyBehavior();
            mocks.ReplayAll();
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

            Expect.Call(result.Status).PropertyBehavior();
            mocks.ReplayAll();
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
            var buildInfo = mocks.DynamicMock<BuildProgressInformation>(string.Empty, string.Empty);
            var result = mocks.StrictMock<IIntegrationResult>();
            SetupResult.For(result.BuildProgressInformation).Return(buildInfo);
            SetupResult.For(result.WorkingDirectory).Return(workingDir);
            SetupResult.For(result.IntegrationProperties).Return(new Dictionary<string, string>());
            SetupResult.For(result.BaseFromWorkingDirectory(Arg<string>.Is.Anything))
                .Do(new Func<string, string>(p => Path.Combine(workingDir, p)));
            Expect.Call(() => result.AddTaskResult(Arg<ProcessTaskResult>.Is.Anything));
            Expect.Call(() => result.AddTaskResult((ITaskResult)null));
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

        private IFileSystem InitialiseFileSystemMock(string workingDir)
        {
            var xmlFile = Path.Combine(workingDir, "codeitright.xml");
            var fileSystemMock = mocks.StrictMock<IFileSystem>();
            Expect
                .Call(fileSystemMock.GenerateTaskResultFromFile(xmlFile, true))
                .Return(null);
            SetupResult.For(fileSystemMock.FileExists(xmlFile)).Return(true);
            var xml = "<CodeItRightReport>" +
                    "<Violations>" + 
                        "<Violation>" +
                            "<Severity>Error</Severity>" +
                        "</Violation>" +
                    "</Violations>" +
                "</CodeItRightReport>";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            SetupResult.For(fileSystemMock.OpenInputStream(xmlFile)).Return(stream);
            return fileSystemMock;
        }
        #endregion
    }
}
