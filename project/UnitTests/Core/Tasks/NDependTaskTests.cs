using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Rhino.Mocks;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

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
            var executor = GenerateExecutorMock("workingDir\\NDepend.Console", "workingDir\\NDependResults /OutDir workingDir\\NDependResults", "workingDir", 600000);
            var fileSystem = mocks.StrictMock<IFileSystem>();
            Expect.Call(fileSystem.DirectoryExists(workingDir)).Return(true).Repeat.Times(2);
            Expect.Call(fileSystem.GetFilesInDirectory(workingDir)).Return(new string[0]);
            Expect.Call(fileSystem.FileExists("workingDir\\NDependResults\\ReportResources.xml")).Return(false);
            Expect.Call(fileSystem.GetFilesInDirectory(workingDir)).Return(new string[] {
                "workingDir\\NDependResults\\file1.txt",
                "workingDir\\NDependResults\\file2.xml"
            });
            Expect.Call(fileSystem.DirectoryExists("artefactDir\\1\\NDepend")).Return(true);
            Expect.Call(fileSystem.FileExists("workingDir\\NDependResults\\file1.txt")).Return(true);
            Expect.Call(() => fileSystem.Copy("workingDir\\NDependResults\\file1.txt", "artefactDir\\1\\NDepend\\file1.txt"));
            Expect.Call(fileSystem.FileExists("workingDir\\NDependResults\\file2.xml")).Return(true);
            Expect.Call(() => fileSystem.Copy("workingDir\\NDependResults\\file2.xml", "artefactDir\\1\\NDepend\\file2.xml"));
            Expect.Call(fileSystem.GenerateTaskResultFromFile("workingDir\\NDependResults\\file2.xml")).Return(mocks.DynamicMock<ITaskResult>());
            var logger = mocks.DynamicMock<ILogger>();
            var task = new NDependTask(executor, fileSystem, logger);

            Expect.Call(result.Status).PropertyBehavior();
            mocks.ReplayAll();
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.VerifyAll();
        }

        [Test]
        public void ExecuteWillCreateANewDirectory()
        {
            var workingDir = "workingDir\\NDependResults";
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock("workingDir\\NDepend.Console", "workingDir\\NDependResults /OutDir workingDir\\NDependResults", "workingDir", 600000);
            var fileSystem = mocks.StrictMock<IFileSystem>();
            Expect.Call(fileSystem.DirectoryExists(workingDir)).Return(true).Repeat.Times(2);
            Expect.Call(fileSystem.GetFilesInDirectory(workingDir)).Return(new string[0]);
            Expect.Call(fileSystem.FileExists("workingDir\\NDependResults\\ReportResources.xml")).Return(false);
            Expect.Call(fileSystem.GetFilesInDirectory(workingDir)).Return(new string[] {
                "workingDir\\NDependResults\\file1.txt",
                "workingDir\\NDependResults\\file2.xml"
            });
            Expect.Call(fileSystem.DirectoryExists("artefactDir\\1\\NDepend")).Return(false);
            Expect.Call(() => fileSystem.CreateDirectory("artefactDir\\1\\NDepend"));
            Expect.Call(fileSystem.FileExists("workingDir\\NDependResults\\file1.txt")).Return(true);
            Expect.Call(() => fileSystem.Copy("workingDir\\NDependResults\\file1.txt", "artefactDir\\1\\NDepend\\file1.txt"));
            Expect.Call(fileSystem.FileExists("workingDir\\NDependResults\\file2.xml")).Return(true);
            Expect.Call(() => fileSystem.Copy("workingDir\\NDependResults\\file2.xml", "artefactDir\\1\\NDepend\\file2.xml"));
            Expect.Call(fileSystem.GenerateTaskResultFromFile("workingDir\\NDependResults\\file2.xml")).Return(mocks.DynamicMock<ITaskResult>());
            var logger = mocks.DynamicMock<ILogger>();
            var task = new NDependTask(executor, fileSystem, logger);

            Expect.Call(result.Status).PropertyBehavior();
            mocks.ReplayAll();
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.VerifyAll();
        }

        [Test]
        public void ExecuteLoadsContentsFileIfItExists()
        {
            var workingDir = "workingDir\\NDependResults";
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock("workingDir\\NDepend.Console", "workingDir\\NDependResults /OutDir workingDir\\NDependResults", "workingDir", 600000);
            var fileSystem = mocks.StrictMock<IFileSystem>();
            Expect.Call(fileSystem.DirectoryExists(workingDir)).Return(true);
            Expect.Call(fileSystem.GetFilesInDirectory(workingDir)).Return(new string[0]);
            Expect.Call(fileSystem.FileExists("workingDir\\NDependResults\\ReportResources.xml")).Return(true);
            using (var reader = new StringReader("<ReportResources><File>file1.txt</File><File>file2.xml</File></ReportResources>"))
            {
                Expect.Call(fileSystem.Load("workingDir\\NDependResults\\ReportResources.xml")).Return(reader);
                Expect.Call(fileSystem.DirectoryExists("artefactDir\\1\\NDepend")).Return(false);
                Expect.Call(() => fileSystem.CreateDirectory("artefactDir\\1\\NDepend"));
                Expect.Call(fileSystem.FileExists("workingDir\\NDependResults\\file1.txt")).Return(true);
                Expect.Call(() => fileSystem.Copy("workingDir\\NDependResults\\file1.txt", "artefactDir\\1\\NDepend\\file1.txt"));
                Expect.Call(fileSystem.FileExists("workingDir\\NDependResults\\file2.xml")).Return(true);
                Expect.Call(() => fileSystem.Copy("workingDir\\NDependResults\\file2.xml", "artefactDir\\1\\NDepend\\file2.xml"));
                Expect.Call(fileSystem.GenerateTaskResultFromFile("workingDir\\NDependResults\\file2.xml")).Return(mocks.DynamicMock<ITaskResult>());
                var logger = mocks.DynamicMock<ILogger>();
                var task = new NDependTask(executor, fileSystem, logger);

                Expect.Call(result.Status).PropertyBehavior();
                mocks.ReplayAll();
                result.Status = IntegrationStatus.Unknown;
                task.Run(result);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void ExecuteFailsIfContentFileHasInvalidRootNode()
        {
            var workingDir = "workingDir\\NDependResults";
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock("workingDir\\NDepend.Console", "workingDir\\NDependResults /OutDir workingDir\\NDependResults", "workingDir", 600000);
            var fileSystem = mocks.StrictMock<IFileSystem>();
            Expect.Call(fileSystem.DirectoryExists(workingDir)).Return(true);
            Expect.Call(fileSystem.GetFilesInDirectory(workingDir)).Return(new string[0]);
            Expect.Call(fileSystem.FileExists("workingDir\\NDependResults\\ReportResources.xml")).Return(true);
            using (var reader = new StringReader("<garbage/>"))
            {
                Expect.Call(fileSystem.Load("workingDir\\NDependResults\\ReportResources.xml")).Return(reader);
                var logger = mocks.DynamicMock<ILogger>();
                var task = new NDependTask(executor, fileSystem, logger);

                Expect.Call(result.Status).PropertyBehavior();
                mocks.ReplayAll();
                result.Status = IntegrationStatus.Unknown;
                Assert.Throws<CruiseControlException>(() => task.Run(result));
                mocks.VerifyAll();
            }
        }

        [Test]
        public void ExecuteFailsIfContentFileIsInvalid()
        {
            var workingDir = "workingDir\\NDependResults";
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock("workingDir\\NDepend.Console", "workingDir\\NDependResults /OutDir workingDir\\NDependResults", "workingDir", 600000);
            var fileSystem = mocks.StrictMock<IFileSystem>();
            Expect.Call(fileSystem.DirectoryExists(workingDir)).Return(true);
            Expect.Call(fileSystem.GetFilesInDirectory(workingDir)).Return(new string[0]);
            Expect.Call(fileSystem.FileExists("workingDir\\NDependResults\\ReportResources.xml")).Return(true);
            using (var reader = new StringReader("garbage"))
            {
                Expect.Call(fileSystem.Load("workingDir\\NDependResults\\ReportResources.xml")).Return(reader);
                var logger = mocks.DynamicMock<ILogger>();
                var task = new NDependTask(executor, fileSystem, logger);

                Expect.Call(result.Status).PropertyBehavior();
                mocks.ReplayAll();
                result.Status = IntegrationStatus.Unknown;
                Assert.Throws<CruiseControlException>(() => task.Run(result));
                mocks.VerifyAll();
            }
        }

        [Test]
        public void ExecuteLoadsContentsFileIfItExistsAndSkipsFilesIfMissing()
        {
            var workingDir = "workingDir\\NDependResults";
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock("workingDir\\NDepend.Console", "workingDir\\NDependResults /OutDir workingDir\\NDependResults", "workingDir", 600000);
            var fileSystem = mocks.StrictMock<IFileSystem>();
            Expect.Call(fileSystem.DirectoryExists(workingDir)).Return(true);
            Expect.Call(fileSystem.GetFilesInDirectory(workingDir)).Return(new string[0]);
            Expect.Call(fileSystem.FileExists("workingDir\\NDependResults\\ReportResources.xml")).Return(true);
            using (var reader = new StringReader("<ReportResources><File>file1.txt</File><File>file2.xml</File></ReportResources>"))
            {
                Expect.Call(fileSystem.Load("workingDir\\NDependResults\\ReportResources.xml")).Return(reader);
                Expect.Call(fileSystem.DirectoryExists("artefactDir\\1\\NDepend")).Return(false);
                Expect.Call(() => fileSystem.CreateDirectory("artefactDir\\1\\NDepend"));
                Expect.Call(fileSystem.FileExists("workingDir\\NDependResults\\file1.txt")).Return(false);
                Expect.Call(fileSystem.FileExists("workingDir\\NDependResults\\file2.xml")).Return(false);
                var logger = mocks.DynamicMock<ILogger>();
                var task = new NDependTask(executor, fileSystem, logger);

                Expect.Call(result.Status).PropertyBehavior();
                mocks.ReplayAll();
                result.Status = IntegrationStatus.Unknown;
                task.Run(result);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void ExecuteLoadsContentsFileIfItExistsAndImportsDirectory()
        {
            var workingDir = "workingDir\\NDependResults";
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock("workingDir\\NDepend.Console", "workingDir\\NDependResults /OutDir workingDir\\NDependResults", "workingDir", 600000);
            var fileSystem = mocks.StrictMock<IFileSystem>();
            Expect.Call(fileSystem.DirectoryExists(workingDir)).Return(true);
            Expect.Call(fileSystem.GetFilesInDirectory(workingDir)).Return(new string[0]);
            Expect.Call(fileSystem.FileExists("workingDir\\NDependResults\\ReportResources.xml")).Return(true);
            using (var reader = new StringReader("<ReportResources><Directory>images</Directory></ReportResources>"))
            {
                Expect.Call(fileSystem.Load("workingDir\\NDependResults\\ReportResources.xml")).Return(reader);
                Expect.Call(fileSystem.GetFilesInDirectory("workingDir\\NDependResults\\images", true)).Return(new[] { "workingDir\\NDependResults\\images\\test.png" });
                Expect.Call(fileSystem.DirectoryExists("artefactDir\\1\\NDepend\\images")).Return(false);
                Expect.Call(() => fileSystem.CreateDirectory("artefactDir\\1\\NDepend\\images"));
                Expect.Call(fileSystem.FileExists("workingDir\\NDependResults\\images\\test.png")).Return(true);
                Expect.Call(() => fileSystem.Copy("workingDir\\NDependResults\\images\\test.png", "artefactDir\\1\\NDepend\\images\\test.png"));
                var logger = mocks.DynamicMock<ILogger>();
                var task = new NDependTask(executor, fileSystem, logger);

                Expect.Call(result.Status).PropertyBehavior();
                mocks.ReplayAll();
                result.Status = IntegrationStatus.Unknown;
                task.Run(result);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CanOverrideExecutable()
        {
            var workingDir = "workingDir\\NDependResults";
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock("workingDir\\ndepend-app.exe", "workingDir\\NDependResults /OutDir workingDir\\NDependResults", "workingDir", 600000);
            var fileSystem = mocks.StrictMock<IFileSystem>();
            Expect.Call(fileSystem.DirectoryExists(workingDir)).Return(true).Repeat.Times(2);
            Expect.Call(fileSystem.GetFilesInDirectory(workingDir)).Return(new string[0]);
            Expect.Call(fileSystem.FileExists("workingDir\\NDependResults\\ReportResources.xml")).Return(false);
            Expect.Call(fileSystem.GetFilesInDirectory(workingDir)).Return(new string[] {
                "workingDir\\NDependResults\\file1.txt",
                "workingDir\\NDependResults\\file2.xml"
            });
            Expect.Call(fileSystem.DirectoryExists("artefactDir\\1\\NDepend")).Return(true);
            Expect.Call(fileSystem.FileExists("workingDir\\NDependResults\\file1.txt")).Return(true);
            Expect.Call(() => fileSystem.Copy("workingDir\\NDependResults\\file1.txt", "artefactDir\\1\\NDepend\\file1.txt"));
            Expect.Call(fileSystem.FileExists("workingDir\\NDependResults\\file2.xml")).Return(true);
            Expect.Call(() => fileSystem.Copy("workingDir\\NDependResults\\file2.xml", "artefactDir\\1\\NDepend\\file2.xml"));
            Expect.Call(fileSystem.GenerateTaskResultFromFile("workingDir\\NDependResults\\file2.xml")).Return(mocks.DynamicMock<ITaskResult>());
            var logger = mocks.DynamicMock<ILogger>();
            var task = new NDependTask(executor, fileSystem, logger);
            task.Executable = "ndepend-app.exe";

            Expect.Call(result.Status).PropertyBehavior();
            mocks.ReplayAll();
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.VerifyAll();
        }

        [Test]
        public void CanUsePathsWithSpaces()
        {
            var workingDir = "working Dir\\NDependResults";
            var result = GenerateResultMock("working Dir", "artefact Dir");
            var executor = GenerateExecutorMock("working Dir\\ndepend-app.exe", "\"working Dir\\NDependResults\" /OutDir \"working Dir\\NDependResults\"", "working Dir", 600000);
            var fileSystem = mocks.StrictMock<IFileSystem>();
            Expect.Call(fileSystem.DirectoryExists(workingDir)).Return(true).Repeat.Times(2);
            Expect.Call(fileSystem.GetFilesInDirectory(workingDir)).Return(new string[0]);
            Expect.Call(fileSystem.FileExists("working Dir\\NDependResults\\ReportResources.xml")).Return(false);
            Expect.Call(fileSystem.GetFilesInDirectory(workingDir)).Return(new string[] {
                "working Dir\\NDependResults\\file1.txt",
                "working Dir\\NDependResults\\file2.xml"
            });
            Expect.Call(fileSystem.DirectoryExists("artefact Dir\\1\\NDepend")).Return(true);
            Expect.Call(fileSystem.FileExists("working Dir\\NDependResults\\file1.txt")).Return(true);
            Expect.Call(() => fileSystem.Copy("working Dir\\NDependResults\\file1.txt", "artefact Dir\\1\\NDepend\\file1.txt"));
            Expect.Call(fileSystem.FileExists("working Dir\\NDependResults\\file2.xml")).Return(true);
            Expect.Call(() => fileSystem.Copy("working Dir\\NDependResults\\file2.xml", "artefact Dir\\1\\NDepend\\file2.xml"));
            Expect.Call(fileSystem.GenerateTaskResultFromFile("working Dir\\NDependResults\\file2.xml")).Return(mocks.DynamicMock<ITaskResult>());
            var logger = mocks.DynamicMock<ILogger>();
            var task = new NDependTask(executor, fileSystem, logger);
            task.Executable = "ndepend-app.exe";

            Expect.Call(result.Status).PropertyBehavior();
            mocks.ReplayAll();
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.VerifyAll();
        }

        [Test]
        public void CanOverrideBaseDirectory()
        {
            var workingDir = "somewhere-else\\NDependResults";
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock("somewhere-else\\NDepend.Console", "somewhere-else\\NDependResults /OutDir somewhere-else\\NDependResults", "somewhere-else", 600000);
            var fileSystem = mocks.StrictMock<IFileSystem>();
            Expect.Call(fileSystem.DirectoryExists(workingDir)).Return(true).Repeat.Times(2);
            Expect.Call(fileSystem.GetFilesInDirectory(workingDir)).Return(new string[0]);
            Expect.Call(fileSystem.FileExists("somewhere-else\\NDependResults\\ReportResources.xml")).Return(false);
            Expect.Call(fileSystem.GetFilesInDirectory(workingDir)).Return(new string[] {
                "somewhere-else\\NDependResults\\file1.txt",
                "somewhere-else\\NDependResults\\file2.xml"
            });
            Expect.Call(fileSystem.DirectoryExists("artefactDir\\1\\NDepend")).Return(true);
            Expect.Call(fileSystem.FileExists("somewhere-else\\NDependResults\\file1.txt")).Return(true);
            Expect.Call(() => fileSystem.Copy("somewhere-else\\NDependResults\\file1.txt", "artefactDir\\1\\NDepend\\file1.txt"));
            Expect.Call(fileSystem.FileExists("somewhere-else\\NDependResults\\file2.xml")).Return(true);
            Expect.Call(() => fileSystem.Copy("somewhere-else\\NDependResults\\file2.xml", "artefactDir\\1\\NDepend\\file2.xml"));
            Expect.Call(fileSystem.GenerateTaskResultFromFile("somewhere-else\\NDependResults\\file2.xml")).Return(mocks.DynamicMock<ITaskResult>());
            var logger = mocks.DynamicMock<ILogger>();
            var task = new NDependTask(executor, fileSystem, logger);
            task.BaseDirectory = "somewhere-else";

            Expect.Call(result.Status).PropertyBehavior();
            mocks.ReplayAll();
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.VerifyAll();
        }

        [Test]
        public void AllApplicationArgsAreSet()
        {
            var workingDir = "workingDir\\out-dir";
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock("workingDir\\NDepend.Console",
                "workingDir\\test.project /Silent /EmitVisualNDependBinXml  /InDirs workingDir\\dir1 workingDir\\dir2 /OutDir workingDir\\out-dir /XslForReport  workingDir\\report.xslt", 
                "workingDir", 600000);
            var fileSystem = mocks.StrictMock<IFileSystem>();
            Expect.Call(fileSystem.DirectoryExists(workingDir)).Return(true).Repeat.Times(2);
            Expect.Call(fileSystem.GetFilesInDirectory(workingDir)).Return(new string[0]);
            Expect.Call(fileSystem.FileExists("workingDir\\out-dir\\ReportResources.xml")).Return(false);
            Expect.Call(fileSystem.GetFilesInDirectory(workingDir)).Return(new string[] {
                "workingDir\\out-dir\\file1.txt",
                "workingDir\\out-dir\\file2.xml"
            });
            Expect.Call(fileSystem.DirectoryExists("artefactDir\\1\\NDepend")).Return(true);
            Expect.Call(fileSystem.FileExists("workingDir\\out-dir\\file1.txt")).Return(true);
            Expect.Call(() => fileSystem.Copy("workingDir\\out-dir\\file1.txt", "artefactDir\\1\\NDepend\\file1.txt"));
            Expect.Call(fileSystem.FileExists("workingDir\\out-dir\\file2.xml")).Return(true);
            Expect.Call(() => fileSystem.Copy("workingDir\\out-dir\\file2.xml", "artefactDir\\1\\NDepend\\file2.xml"));
            Expect.Call(fileSystem.GenerateTaskResultFromFile("workingDir\\out-dir\\file2.xml")).Return(mocks.DynamicMock<ITaskResult>());
            var logger = mocks.DynamicMock<ILogger>();
            var task = new NDependTask(executor, fileSystem, logger);
            task.ProjectFile = "test.project";
            task.Silent = true;
            task.EmitXml = true;
            task.InputDirs = new string[] 
            {
                "dir1",
                "dir2"
            };
            task.OutputDir = "out-dir";
            task.ReportXslt = "report.xslt";

            Expect.Call(result.Status).PropertyBehavior();
            mocks.ReplayAll();
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.VerifyAll();
        }

        [Test]
        public void RunningOnExistingDirectoryChecksFilesAndCopiesWhenNewer()
        {
            var workingDir = "workingDir\\NDependResults";
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock("workingDir\\NDepend.Console", "workingDir\\NDependResults /OutDir workingDir\\NDependResults", "workingDir", 600000);
            var fileSystem = mocks.StrictMock<IFileSystem>();
            Expect.Call(fileSystem.DirectoryExists(workingDir)).Return(true).Repeat.Times(2);
            Expect.Call(fileSystem.GetFilesInDirectory(workingDir)).Return(new string[] {
                "workingDir\\NDependResults\\file1.txt",
                "workingDir\\NDependResults\\file2.xml"
            });
            Expect.Call(fileSystem.FileExists("workingDir\\NDependResults\\ReportResources.xml")).Return(false);
            Expect.Call(fileSystem.GetFilesInDirectory(workingDir)).Return(new string[] {
                "workingDir\\NDependResults\\file1.txt",
                "workingDir\\NDependResults\\file2.xml"
            });
            Expect.Call(fileSystem.DirectoryExists("artefactDir\\1\\NDepend")).Return(true);
            Expect.Call(fileSystem.FileExists("workingDir\\NDependResults\\file1.txt")).Return(true);
            Expect.Call(() => fileSystem.Copy("workingDir\\NDependResults\\file1.txt", "artefactDir\\1\\NDepend\\file1.txt"));
            var baseTime = DateTime.Now;
            Expect.Call(fileSystem.GetLastWriteTime("workingDir\\NDependResults\\file1.txt")).Return(baseTime);
            Expect.Call(fileSystem.GetLastWriteTime("workingDir\\NDependResults\\file2.xml")).Return(baseTime).Repeat.Times(2);
            Expect.Call(fileSystem.GetLastWriteTime("workingDir\\NDependResults\\file1.txt")).Return(baseTime.AddMinutes(1));
            var logger = mocks.DynamicMock<ILogger>();
            var task = new NDependTask(executor, fileSystem, logger);

            Expect.Call(result.Status).PropertyBehavior();
            mocks.ReplayAll();
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.VerifyAll();
        }

        [Test]
        public void DefaultConstructorSetsFileSystemAndLogger()
        {
            var task = new NDependTask();
            Assert.IsInstanceOf<SystemIoFileSystem>(task.FileSystem);
            Assert.IsInstanceOf<DefaultLogger>(task.Logger);
        }
        #endregion

        #region Private methods
        private IIntegrationResult GenerateResultMock()
        {
            return GenerateResultMock("workingDir", "artefactDir");
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
        #endregion
    }
}
