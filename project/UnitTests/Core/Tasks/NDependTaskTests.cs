using System;
using System.Collections.Generic;
using System.IO;
using Moq;
using NUnit.Framework;
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
            mocks = new MockRepository(MockBehavior.Default);
        }
        #endregion

        #region Tests
        [Test]
        public void ExecuteRunsNDependWithDefaults()
        {
            var workingDir = System.IO.Path.Combine("workingDir", "NDependResults");
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock(System.IO.Path.Combine("workingDir", "NDepend.Console"), workingDir + " /OutDir " + workingDir, "workingDir", 600000);
            var fileSystem = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            MockSequence sequence = new MockSequence();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[0]).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists(System.IO.Path.Combine("workingDir", "NDependResults", "ReportResources.xml"))).Returns(false).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[] {
                System.IO.Path.Combine("workingDir", "NDependResults", "file1.txt"),
                System.IO.Path.Combine("workingDir", "NDependResults", "file2.xml")
            }).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(System.IO.Path.Combine("artefactDir", "1", "NDepend"))).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists(System.IO.Path.Combine("workingDir", "NDependResults", "file1.txt"))).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy(System.IO.Path.Combine("workingDir", "NDependResults", "file1.txt"), System.IO.Path.Combine("artefactDir", "1", "NDepend", "file1.txt"))).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists(System.IO.Path.Combine("workingDir", "NDependResults", "file2.xml"))).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy(System.IO.Path.Combine("workingDir", "NDependResults", "file2.xml"), System.IO.Path.Combine("artefactDir", "1", "NDepend", "file2.xml"))).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GenerateTaskResultFromFile(System.IO.Path.Combine("workingDir", "NDependResults", "file2.xml"))).Returns(mocks.Create<ITaskResult>().Object).Verifiable();
            var logger = mocks.Create<ILogger>().Object;
            var task = new NDependTask(executor, fileSystem, logger);

            Mock.Get(result).SetupProperty(_result => _result.Status);
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.Verify();
        }

        [Test]
        public void ExecuteWillCreateANewDirectory()
        {
            var workingDir = System.IO.Path.Combine("workingDir", "NDependResults");
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock(System.IO.Path.Combine("workingDir", "NDepend.Console"), workingDir + " /OutDir " + workingDir, "workingDir", 600000);
            var fileSystem = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            MockSequence sequence = new MockSequence();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[0]).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists(System.IO.Path.Combine("workingDir", "NDependResults", "ReportResources.xml"))).Returns(false).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[] {
                System.IO.Path.Combine("workingDir", "NDependResults", "file1.txt"),
                System.IO.Path.Combine("workingDir", "NDependResults", "file2.xml")
            }).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(System.IO.Path.Combine("artefactDir", "1", "NDepend"))).Returns(false).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.CreateDirectory(System.IO.Path.Combine("artefactDir", "1", "NDepend"))).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists(System.IO.Path.Combine("workingDir", "NDependResults", "file1.txt"))).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy(System.IO.Path.Combine("workingDir", "NDependResults", "file1.txt"), System.IO.Path.Combine("artefactDir", "1", "NDepend", "file1.txt"))).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists(System.IO.Path.Combine("workingDir", "NDependResults", "file2.xml"))).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy(System.IO.Path.Combine("workingDir", "NDependResults", "file2.xml"), System.IO.Path.Combine("artefactDir", "1", "NDepend", "file2.xml"))).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GenerateTaskResultFromFile(System.IO.Path.Combine("workingDir", "NDependResults", "file2.xml"))).Returns(mocks.Create<ITaskResult>().Object).Verifiable();
            var logger = mocks.Create<ILogger>().Object;
            var task = new NDependTask(executor, fileSystem, logger);

            Mock.Get(result).SetupProperty(_result => _result.Status);
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.Verify();
        }

        [Test]
        public void ExecuteLoadsContentsFileIfItExists()
        {
            var workingDir = System.IO.Path.Combine("workingDir", "NDependResults");
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock(System.IO.Path.Combine("workingDir", "NDepend.Console"), workingDir + " /OutDir " + workingDir, "workingDir", 600000);
            var fileSystem = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            MockSequence sequence = new MockSequence();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[0]).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists(System.IO.Path.Combine("workingDir", "NDependResults", "ReportResources.xml"))).Returns(true).Verifiable();
            using (var reader = new StringReader("<ReportResources><File>file1.txt</File><File>file2.xml</File></ReportResources>"))
            {
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Load(System.IO.Path.Combine("workingDir", "NDependResults", "ReportResources.xml"))).Returns(reader).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(System.IO.Path.Combine("artefactDir", "1", "NDepend"))).Returns(false).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.CreateDirectory(System.IO.Path.Combine("artefactDir", "1", "NDepend"))).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists(System.IO.Path.Combine("workingDir", "NDependResults", "file1.txt"))).Returns(true).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy(System.IO.Path.Combine("workingDir", "NDependResults", "file1.txt"), System.IO.Path.Combine("artefactDir", "1", "NDepend", "file1.txt"))).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists(System.IO.Path.Combine("workingDir", "NDependResults", "file2.xml"))).Returns(true).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy(System.IO.Path.Combine("workingDir", "NDependResults", "file2.xml"), System.IO.Path.Combine("artefactDir", "1", "NDepend", "file2.xml"))).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GenerateTaskResultFromFile(System.IO.Path.Combine("workingDir", "NDependResults", "file2.xml"))).Returns(mocks.Create<ITaskResult>().Object).Verifiable();
                var logger = mocks.Create<ILogger>().Object;
                var task = new NDependTask(executor, fileSystem, logger);

                Mock.Get(result).SetupProperty(_result => _result.Status);
                result.Status = IntegrationStatus.Unknown;
                task.Run(result);
                mocks.Verify();
            }
        }

        [Test]
        public void ExecuteFailsIfContentFileHasInvalidRootNode()
        {
            var workingDir = System.IO.Path.Combine("workingDir", "NDependResults");
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock(System.IO.Path.Combine("workingDir", "NDepend.Console"), workingDir + " /OutDir " + workingDir, "workingDir", 600000);
            var fileSystem = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            MockSequence sequence = new MockSequence();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[0]).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists(System.IO.Path.Combine("workingDir", "NDependResults", "ReportResources.xml"))).Returns(true).Verifiable();
            using (var reader = new StringReader("<garbage/>"))
            {
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Load(System.IO.Path.Combine("workingDir", "NDependResults", "ReportResources.xml"))).Returns(reader).Verifiable();
                var logger = mocks.Create<ILogger>().Object;
                var task = new NDependTask(executor, fileSystem, logger);

                Mock.Get(result).SetupProperty(_result => _result.Status);
                result.Status = IntegrationStatus.Unknown;
                Assert.Throws<CruiseControlException>(() => task.Run(result));
                mocks.Verify();
            }
        }

        [Test]
        public void ExecuteFailsIfContentFileIsInvalid()
        {
            var workingDir = System.IO.Path.Combine("workingDir", "NDependResults");
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock(System.IO.Path.Combine("workingDir", "NDepend.Console"), workingDir + " /OutDir " + workingDir, "workingDir", 600000);
            var fileSystem = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            MockSequence sequence = new MockSequence();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[0]).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists(System.IO.Path.Combine("workingDir", "NDependResults", "ReportResources.xml"))).Returns(true).Verifiable();
            using (var reader = new StringReader("garbage"))
            {
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Load(System.IO.Path.Combine("workingDir", "NDependResults", "ReportResources.xml"))).Returns(reader).Verifiable();
                var logger = mocks.Create<ILogger>().Object;
                var task = new NDependTask(executor, fileSystem, logger);

                Mock.Get(result).SetupProperty(_result => _result.Status);
                result.Status = IntegrationStatus.Unknown;
                Assert.Throws<CruiseControlException>(() => task.Run(result));
                mocks.Verify();
            }
        }

        [Test]
        public void ExecuteLoadsContentsFileIfItExistsAndSkipsFilesIfMissing()
        {
            var workingDir = System.IO.Path.Combine("workingDir", "NDependResults");
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock(System.IO.Path.Combine("workingDir", "NDepend.Console"), workingDir + " /OutDir " + workingDir, "workingDir", 600000);
            var fileSystem = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            MockSequence sequence = new MockSequence();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[0]).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists(System.IO.Path.Combine("workingDir", "NDependResults", "ReportResources.xml"))).Returns(true).Verifiable();
            using (var reader = new StringReader("<ReportResources><File>file1.txt</File><File>file2.xml</File></ReportResources>"))
            {
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Load(System.IO.Path.Combine("workingDir", "NDependResults", "ReportResources.xml"))).Returns(reader).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(System.IO.Path.Combine("artefactDir", "1", "NDepend"))).Returns(false).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.CreateDirectory(System.IO.Path.Combine("artefactDir", "1", "NDepend"))).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists(System.IO.Path.Combine("workingDir", "NDependResults", "file1.txt"))).Returns(false).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists(System.IO.Path.Combine("workingDir", "NDependResults", "file2.xml"))).Returns(false).Verifiable();
                var logger = mocks.Create<ILogger>().Object;
                var task = new NDependTask(executor, fileSystem, logger);

                Mock.Get(result).SetupProperty(_result => _result.Status);
                result.Status = IntegrationStatus.Unknown;
                task.Run(result);
                mocks.Verify();
            }
        }

        [Test]
        public void ExecuteLoadsContentsFileIfItExistsAndImportsDirectory()
        {
            var workingDir = System.IO.Path.Combine("workingDir", "NDependResults");
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock(System.IO.Path.Combine("workingDir", "NDepend.Console"), workingDir + " /OutDir " + workingDir, "workingDir", 600000);
            var fileSystem = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            MockSequence sequence = new MockSequence();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[0]).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists(System.IO.Path.Combine("workingDir", "NDependResults", "ReportResources.xml"))).Returns(true).Verifiable();
            using (var reader = new StringReader("<ReportResources><Directory>images</Directory></ReportResources>"))
            {
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Load(System.IO.Path.Combine("workingDir", "NDependResults", "ReportResources.xml"))).Returns(reader).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(System.IO.Path.Combine("workingDir", "NDependResults", "images"), true)).Returns(new[] { System.IO.Path.Combine("workingDir", "NDependResults", "images", "test.png") }).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(System.IO.Path.Combine("artefactDir", "1", "NDepend", "images"))).Returns(false).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.CreateDirectory(System.IO.Path.Combine("artefactDir", "1", "NDepend", "images"))).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists(System.IO.Path.Combine("workingDir", "NDependResults", "images", "test.png"))).Returns(true).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy(System.IO.Path.Combine("workingDir", "NDependResults", "images", "test.png"), System.IO.Path.Combine("artefactDir", "1", "NDepend", "images", "test.png"))).Verifiable();
                var logger = mocks.Create<ILogger>().Object;
                var task = new NDependTask(executor, fileSystem, logger);

                Mock.Get(result).SetupProperty(_result => _result.Status);
                result.Status = IntegrationStatus.Unknown;
                task.Run(result);
                mocks.Verify();
            }
        }

        [Test]
        public void CanOverrideExecutable()
        {
            var workingDir = System.IO.Path.Combine("workingDir", "NDependResults");
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock(System.IO.Path.Combine("workingDir", "ndepend-app.exe"), workingDir + " /OutDir " + workingDir, "workingDir", 600000);
            var fileSystem = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            MockSequence sequence = new MockSequence();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[0]).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists(System.IO.Path.Combine("workingDir", "NDependResults", "ReportResources.xml"))).Returns(false).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[] {
                System.IO.Path.Combine("workingDir", "NDependResults", "file1.txt"),
                System.IO.Path.Combine("workingDir", "NDependResults", "file2.xml")
            }).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(System.IO.Path.Combine("artefactDir", "1", "NDepend"))).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists(System.IO.Path.Combine("workingDir", "NDependResults", "file1.txt"))).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy(System.IO.Path.Combine("workingDir", "NDependResults", "file1.txt"), System.IO.Path.Combine("artefactDir", "1", "NDepend", "file1.txt"))).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists(System.IO.Path.Combine("workingDir", "NDependResults", "file2.xml"))).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy(System.IO.Path.Combine("workingDir", "NDependResults", "file2.xml"), System.IO.Path.Combine("artefactDir", "1", "NDepend", "file2.xml"))).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GenerateTaskResultFromFile(System.IO.Path.Combine("workingDir", "NDependResults", "file2.xml"))).Returns(mocks.Create<ITaskResult>().Object).Verifiable();
            var logger = mocks.Create<ILogger>().Object;
            var task = new NDependTask(executor, fileSystem, logger);
            task.Executable = "ndepend-app.exe";

            Mock.Get(result).SetupProperty(_result => _result.Status);
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.Verify();
        }

        [Test]
        public void CanUsePathsWithSpaces()
        {
            var workingDir = System.IO.Path.Combine("working Dir", "NDependResults");
            var result = GenerateResultMock("working Dir", "artefact Dir");
            var executor = GenerateExecutorMock(System.IO.Path.Combine("working Dir", "ndepend-app.exe"), "\"" + workingDir + "\" /OutDir \"" + workingDir + "\"", "working Dir", 600000);
            var fileSystem = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            MockSequence sequence = new MockSequence();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[0]).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists(System.IO.Path.Combine("working Dir", "NDependResults", "ReportResources.xml"))).Returns(false).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[] {
                System.IO.Path.Combine("working Dir", "NDependResults", "file1.txt"),
                System.IO.Path.Combine("working Dir", "NDependResults", "file2.xml")
            }).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(System.IO.Path.Combine("artefact Dir", "1", "NDepend"))).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists(System.IO.Path.Combine("working Dir", "NDependResults", "file1.txt"))).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy(System.IO.Path.Combine("working Dir", "NDependResults", "file1.txt"), System.IO.Path.Combine("artefact Dir", "1", "NDepend", "file1.txt"))).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists(System.IO.Path.Combine("working Dir", "NDependResults", "file2.xml"))).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy(System.IO.Path.Combine("working Dir", "NDependResults", "file2.xml"), System.IO.Path.Combine("artefact Dir", "1", "NDepend", "file2.xml"))).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GenerateTaskResultFromFile(System.IO.Path.Combine("working Dir", "NDependResults", "file2.xml"))).Returns(mocks.Create<ITaskResult>().Object).Verifiable();
            var logger = mocks.Create<ILogger>().Object;
            var task = new NDependTask(executor, fileSystem, logger);
            task.Executable = "ndepend-app.exe";

            Mock.Get(result).SetupProperty(_result => _result.Status);
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.Verify();
        }

        [Test]
        public void CanOverrideBaseDirectory()
        {
            var workingDir = System.IO.Path.Combine("somewhere-else", "NDependResults");
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock(System.IO.Path.Combine("somewhere-else", "NDepend.Console"), workingDir + " /OutDir " + workingDir, "somewhere-else", 600000);
            var fileSystem = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            MockSequence sequence = new MockSequence();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[0]).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists(System.IO.Path.Combine("somewhere-else", "NDependResults", "ReportResources.xml"))).Returns(false).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[] {
                System.IO.Path.Combine("somewhere-else", "NDependResults", "file1.txt"),
                System.IO.Path.Combine("somewhere-else", "NDependResults", "file2.xml")
            }).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(System.IO.Path.Combine("artefactDir", "1", "NDepend"))).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists(System.IO.Path.Combine("somewhere-else", "NDependResults", "file1.txt"))).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy(System.IO.Path.Combine("somewhere-else", "NDependResults", "file1.txt"), System.IO.Path.Combine("artefactDir", "1", "NDepend", "file1.txt"))).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists(System.IO.Path.Combine("somewhere-else", "NDependResults", "file2.xml"))).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy(System.IO.Path.Combine("somewhere-else", "NDependResults", "file2.xml"), System.IO.Path.Combine("artefactDir", "1", "NDepend", "file2.xml"))).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GenerateTaskResultFromFile(System.IO.Path.Combine("somewhere-else", "NDependResults", "file2.xml"))).Returns(mocks.Create<ITaskResult>().Object).Verifiable();
            var logger = mocks.Create<ILogger>().Object;
            var task = new NDependTask(executor, fileSystem, logger);
            task.BaseDirectory = "somewhere-else";

            Mock.Get(result).SetupProperty(_result => _result.Status);
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.Verify();
        }

        [Test]
        public void AllApplicationArgsAreSet()
        {
            var workingDir = System.IO.Path.Combine("workingDir", "out-dir");
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock(System.IO.Path.Combine("workingDir", "NDepend.Console"),
                System.IO.Path.Combine("workingDir", "test.project") + " /Silent /EmitVisualNDependBinXml  /InDirs " + System.IO.Path.Combine("workingDir", "dir1") + " " + System.IO.Path.Combine("workingDir", "dir2") + 
                " /OutDir " + workingDir + " /XslForReport  " + System.IO.Path.Combine("workingDir", "report.xslt"), 
                "workingDir", 600000);
            var fileSystem = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            MockSequence sequence = new MockSequence();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[0]).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists(System.IO.Path.Combine("workingDir", "out-dir", "ReportResources.xml"))).Returns(false).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[] {
                System.IO.Path.Combine("workingDir", "out-dir", "file1.txt"),
                System.IO.Path.Combine("workingDir", "out-dir", "file2.xml")
            }).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(System.IO.Path.Combine("artefactDir", "1", "NDepend"))).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists(System.IO.Path.Combine("workingDir", "out-dir", "file1.txt"))).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy(System.IO.Path.Combine("workingDir", "out-dir", "file1.txt"), System.IO.Path.Combine("artefactDir", "1", "NDepend", "file1.txt"))).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists(System.IO.Path.Combine("workingDir", "out-dir", "file2.xml"))).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy(System.IO.Path.Combine("workingDir", "out-dir", "file2.xml"), System.IO.Path.Combine("artefactDir", "1", "NDepend", "file2.xml"))).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GenerateTaskResultFromFile(System.IO.Path.Combine("workingDir", "out-dir", "file2.xml"))).Returns(mocks.Create<ITaskResult>().Object).Verifiable();
            var logger = mocks.Create<ILogger>().Object;
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

            Mock.Get(result).SetupProperty(_result => _result.Status);
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.Verify();
        }

        [Test]
        public void RunningOnExistingDirectoryChecksFilesAndCopiesWhenNewer()
        {
            var workingDir = System.IO.Path.Combine("workingDir", "NDependResults");
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock(System.IO.Path.Combine("workingDir", "NDepend.Console"), workingDir + " /OutDir " + workingDir, "workingDir", 600000);
            var fileSystem = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            MockSequence sequence = new MockSequence();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[] {
                System.IO.Path.Combine("workingDir", "NDependResults", "file1.txt"),
                System.IO.Path.Combine("workingDir", "NDependResults", "file2.xml")
            }).Verifiable();
            var baseTime = DateTime.Now;
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetLastWriteTime(System.IO.Path.Combine("workingDir", "NDependResults", "file1.txt"))).Returns(baseTime).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetLastWriteTime(System.IO.Path.Combine("workingDir", "NDependResults", "file2.xml"))).Returns(baseTime).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists(System.IO.Path.Combine("workingDir", "NDependResults", "ReportResources.xml"))).Returns(false).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[] {
                System.IO.Path.Combine("workingDir", "NDependResults", "file1.txt"),
                System.IO.Path.Combine("workingDir", "NDependResults", "file2.xml")
            }).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetLastWriteTime(System.IO.Path.Combine("workingDir", "NDependResults", "file1.txt"))).Returns(baseTime.AddMinutes(1)).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetLastWriteTime(System.IO.Path.Combine("workingDir", "NDependResults", "file2.xml"))).Returns(baseTime).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(System.IO.Path.Combine("artefactDir", "1", "NDepend"))).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists(System.IO.Path.Combine("workingDir", "NDependResults", "file1.txt"))).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy(System.IO.Path.Combine("workingDir", "NDependResults", "file1.txt"), System.IO.Path.Combine("artefactDir", "1", "NDepend", "file1.txt"))).Verifiable();
            var logger = mocks.Create<ILogger>().Object;
            var task = new NDependTask(executor, fileSystem, logger);

            Mock.Get(result).SetupProperty(_result => _result.Status);
            result.Status = IntegrationStatus.Unknown;
            task.Run(result);
            mocks.Verify();
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
        #endregion
    }
}
