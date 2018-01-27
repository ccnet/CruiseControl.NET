using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Exortech.NetReflector;
using NUnit.Framework;
using Rhino.Mocks;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    [TestFixture]
    public class FakeTaskTest
    {
        private MockRepository mocks;
        private readonly string DefaultWorkingDirectory = Path.GetFullPath(Path.Combine(".", "source"));
        private readonly string DefaultWorkingDirectoryWithSpaces = Path.GetFullPath(Path.Combine(".", "source code"));
        private ProcessExecutor executor;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
            executor = mocks.StrictMock<ProcessExecutor>();
        }

        [Test]
        public void PopulateFromReflector()
        {
            var task = new FakeTask();
            const string xml = @"
    <fake>
    	<executable>C:\FAKE.exe</executable>
    	<baseDirectory>C:\</baseDirectory>
		<buildFile>mybuild.fx</buildFile>
		<description>Test description</description>
    </fake>";

            NetReflector.Read(xml, task);
            Assert.AreEqual(@"C:\FAKE.exe", task.Executable);
            Assert.AreEqual(@"C:\", task.ConfiguredBaseDirectory);
            Assert.AreEqual("mybuild.fx", task.BuildFile);
            Assert.AreEqual("Test description", task.Description);
        }

        [Test]
        public void PopulateFromConfigurationUsingOnlyRequiredElementsAndCheckDefaultValues()
        {
            
            var task = new FakeTask();
            const string xml = "<fake />";

            NetReflector.Read(xml, task);
            Assert.AreEqual(FakeTask.defaultExecutable, task.Executable);
            Assert.AreEqual(string.Empty, task.ConfiguredBaseDirectory);
            Assert.AreEqual(string.Empty, task.BuildFile);
            Assert.AreEqual(null, task.Description);
        }

        [Test]
        public void ExecuteRunsFakeWithDefaults()
        {
            var workingDir = Path.Combine(DefaultWorkingDirectory, "WorkingDir");
            var artefactDir = Path.Combine(DefaultWorkingDirectoryWithSpaces, "ArtifactsDir");
            var buildFile = Path.Combine(DefaultWorkingDirectory, "ccnet.fsx");

            var result = GenerateResultMock(workingDir, artefactDir);
            var task = new FakeTask(executor);
            task.BuildFile = buildFile;
            SetupExecutorMock(executor, "FAKE.exe", string.Concat(StringUtil.AutoDoubleQuoteString(buildFile), " ", "logfile=", StringUtil.AutoDoubleQuoteString(Path.Combine(artefactDir, string.Format(FakeTask.logFilename, task.LogFileId)))), workingDir, 600000);
            Expect.Call(result.Status).PropertyBehavior();

            mocks.ReplayAll();
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.VerifyAll();
        }

				[Test]
				public void ShouldFailIfProcessTimesOut()
				{
					ExecutorShouldTimeOut(executor);
					mocks.ReplayAll();

					var task = new FakeTask(executor);

					var result = IntegrationResultMother.CreateUnknown();
					task.Run(result);
					
					mocks.VerifyAll();
					Assert.That(result.Status, Is.EqualTo(IntegrationStatus.Failure));
					Assert.That(result.TaskOutput, Does.Match("Command line '.*' timed out after \\d+ seconds"));
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
            SetupResult.For(result.BaseFromWorkingDirectory("")).Return(workingDir);
            return result;
        }

        private void SetupExecutorMock(ProcessExecutor executor, string fileName, string args, string workingDir, int timeout)
        {
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
        }

				private void ExecutorShouldTimeOut(ProcessExecutor executor)
				{
					Expect.Call(executor.Execute(null))
							.IgnoreArguments()
							.Return(ProcessResultFixture.CreateTimedOutResult());
				}
		}
}
