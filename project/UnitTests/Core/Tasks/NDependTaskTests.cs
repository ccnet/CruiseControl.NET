using System;
using System.Collections.Generic;
using Exortech.NetReflector;
using NUnit.Framework;
using Rhino.Mocks;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using Rhino.Mocks.Constraints;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    [TestFixture]
    public class NDependTaskTests
    {
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
        public void ExecuteRunsNDependWithDefaults()
        {
            var workingDir = "workingDir\\NDependResults";
            var result = GenerateResultMock();
            var executor = mocks.StrictMock<ProcessExecutor>();
            Expect.Call(executor.Execute(null))
                .IgnoreArguments()
                .Do(new Function<ProcessInfo, ProcessResult>(info =>
                {
                    Assert.AreEqual("workingDir\\NDepend.Console", info.FileName);
                    Assert.AreEqual("workingDir\\NDependResults /OutDir workingDir\\NDependResults", info.Arguments);
                    Assert.AreEqual("workingDir\\workingDir", info.WorkingDirectory);
                    return new ProcessResult(string.Empty, string.Empty, 0, false);
                }));
            var fileSystem = mocks.StrictMock<IFileSystem>();
            Expect.Call(fileSystem.DirectoryExists(workingDir)).Return(true).Repeat.Times(2);
            Expect.Call(fileSystem.GetFilesInDirectory(workingDir)).Return(new string[0]);
            Expect.Call(fileSystem.GetFilesInDirectory(workingDir)).Return(new string[] {
                "workingDir\\workingDir\\file1.txt",
                "workingDir\\workingDir\\file2.xml"
            });
            Expect.Call(() => fileSystem.EnsureFolderExists("artefactDir\\1\\NDepend"));
            Expect.Call(() => fileSystem.Copy("workingDir\\workingDir\\file1.txt", "artefactDir\\1\\NDepend\\file1.txt"));
            Expect.Call(() => fileSystem.Copy("workingDir\\workingDir\\file2.xml", "artefactDir\\1\\NDepend\\file2.xml"));
            Expect.Call(fileSystem.GenerateTaskResultFromFile("workingDir\\workingDir\\file2.xml")).Return(mocks.DynamicMock<ITaskResult>());
            var logger = mocks.DynamicMock<ILogger>();
            var task = new NDependTask(executor, fileSystem, logger);

            mocks.ReplayAll();
            task.Run(result);
            mocks.VerifyAll();
        }
        #endregion

        #region Private methods
        private IIntegrationResult GenerateResultMock()
        {
            var buildInfo = mocks.DynamicMock<BuildProgressInformation>(string.Empty, string.Empty);
            var result = mocks.StrictMock<IIntegrationResult>();
            SetupResult.For(result.BuildProgressInformation).Return(buildInfo);
            SetupResult.For(result.WorkingDirectory).Return("workingDir");
            SetupResult.For(result.ArtifactDirectory).Return("artefactDir");
            SetupResult.For(result.IntegrationProperties).Return(new Dictionary<string, string>());
            SetupResult.For(result.Label).Return("1");
            Expect.Call(() => result.AddTaskResult(mocks.DynamicMock<ITaskResult>())).IgnoreArguments().Repeat.Any();
            SetupResult.For(result.BaseFromArtifactsDirectory("1")).Return("artefactDir\\1");
            return result;
        }
        #endregion
    }
}
