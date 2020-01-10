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
            var workingDir = "workingDir\\NDependResults";
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock("workingDir\\NDepend.Console", "workingDir\\NDependResults /OutDir workingDir\\NDependResults", "workingDir", 600000);
            var fileSystem = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            MockSequence sequence = new MockSequence();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[0]).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists("workingDir\\NDependResults\\ReportResources.xml")).Returns(false).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[] {
                "workingDir\\NDependResults\\file1.txt",
                "workingDir\\NDependResults\\file2.xml"
            }).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists("artefactDir\\1\\NDepend")).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists("workingDir\\NDependResults\\file1.txt")).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy("workingDir\\NDependResults\\file1.txt", "artefactDir\\1\\NDepend\\file1.txt")).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists("workingDir\\NDependResults\\file2.xml")).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy("workingDir\\NDependResults\\file2.xml", "artefactDir\\1\\NDepend\\file2.xml")).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GenerateTaskResultFromFile("workingDir\\NDependResults\\file2.xml")).Returns(mocks.Create<ITaskResult>().Object).Verifiable();
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
            var workingDir = "workingDir\\NDependResults";
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock("workingDir\\NDepend.Console", "workingDir\\NDependResults /OutDir workingDir\\NDependResults", "workingDir", 600000);
            var fileSystem = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            MockSequence sequence = new MockSequence();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[0]).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists("workingDir\\NDependResults\\ReportResources.xml")).Returns(false).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[] {
                "workingDir\\NDependResults\\file1.txt",
                "workingDir\\NDependResults\\file2.xml"
            }).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists("artefactDir\\1\\NDepend")).Returns(false).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.CreateDirectory("artefactDir\\1\\NDepend")).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists("workingDir\\NDependResults\\file1.txt")).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy("workingDir\\NDependResults\\file1.txt", "artefactDir\\1\\NDepend\\file1.txt")).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists("workingDir\\NDependResults\\file2.xml")).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy("workingDir\\NDependResults\\file2.xml", "artefactDir\\1\\NDepend\\file2.xml")).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GenerateTaskResultFromFile("workingDir\\NDependResults\\file2.xml")).Returns(mocks.Create<ITaskResult>().Object).Verifiable();
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
            var workingDir = "workingDir\\NDependResults";
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock("workingDir\\NDepend.Console", "workingDir\\NDependResults /OutDir workingDir\\NDependResults", "workingDir", 600000);
            var fileSystem = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            MockSequence sequence = new MockSequence();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[0]).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists("workingDir\\NDependResults\\ReportResources.xml")).Returns(true).Verifiable();
            using (var reader = new StringReader("<ReportResources><File>file1.txt</File><File>file2.xml</File></ReportResources>"))
            {
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Load("workingDir\\NDependResults\\ReportResources.xml")).Returns(reader).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists("artefactDir\\1\\NDepend")).Returns(false).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.CreateDirectory("artefactDir\\1\\NDepend")).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists("workingDir\\NDependResults\\file1.txt")).Returns(true).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy("workingDir\\NDependResults\\file1.txt", "artefactDir\\1\\NDepend\\file1.txt")).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists("workingDir\\NDependResults\\file2.xml")).Returns(true).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy("workingDir\\NDependResults\\file2.xml", "artefactDir\\1\\NDepend\\file2.xml")).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GenerateTaskResultFromFile("workingDir\\NDependResults\\file2.xml")).Returns(mocks.Create<ITaskResult>().Object).Verifiable();
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
            var workingDir = "workingDir\\NDependResults";
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock("workingDir\\NDepend.Console", "workingDir\\NDependResults /OutDir workingDir\\NDependResults", "workingDir", 600000);
            var fileSystem = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            MockSequence sequence = new MockSequence();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[0]).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists("workingDir\\NDependResults\\ReportResources.xml")).Returns(true).Verifiable();
            using (var reader = new StringReader("<garbage/>"))
            {
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Load("workingDir\\NDependResults\\ReportResources.xml")).Returns(reader).Verifiable();
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
            var workingDir = "workingDir\\NDependResults";
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock("workingDir\\NDepend.Console", "workingDir\\NDependResults /OutDir workingDir\\NDependResults", "workingDir", 600000);
            var fileSystem = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            MockSequence sequence = new MockSequence();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[0]).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists("workingDir\\NDependResults\\ReportResources.xml")).Returns(true).Verifiable();
            using (var reader = new StringReader("garbage"))
            {
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Load("workingDir\\NDependResults\\ReportResources.xml")).Returns(reader).Verifiable();
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
            var workingDir = "workingDir\\NDependResults";
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock("workingDir\\NDepend.Console", "workingDir\\NDependResults /OutDir workingDir\\NDependResults", "workingDir", 600000);
            var fileSystem = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            MockSequence sequence = new MockSequence();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[0]).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists("workingDir\\NDependResults\\ReportResources.xml")).Returns(true).Verifiable();
            using (var reader = new StringReader("<ReportResources><File>file1.txt</File><File>file2.xml</File></ReportResources>"))
            {
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Load("workingDir\\NDependResults\\ReportResources.xml")).Returns(reader).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists("artefactDir\\1\\NDepend")).Returns(false).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.CreateDirectory("artefactDir\\1\\NDepend")).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists("workingDir\\NDependResults\\file1.txt")).Returns(false).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists("workingDir\\NDependResults\\file2.xml")).Returns(false).Verifiable();
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
            var workingDir = "workingDir\\NDependResults";
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock("workingDir\\NDepend.Console", "workingDir\\NDependResults /OutDir workingDir\\NDependResults", "workingDir", 600000);
            var fileSystem = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            MockSequence sequence = new MockSequence();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[0]).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists("workingDir\\NDependResults\\ReportResources.xml")).Returns(true).Verifiable();
            using (var reader = new StringReader("<ReportResources><Directory>images</Directory></ReportResources>"))
            {
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Load("workingDir\\NDependResults\\ReportResources.xml")).Returns(reader).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory("workingDir\\NDependResults\\images", true)).Returns(new[] { "workingDir\\NDependResults\\images\\test.png" }).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists("artefactDir\\1\\NDepend\\images")).Returns(false).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.CreateDirectory("artefactDir\\1\\NDepend\\images")).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists("workingDir\\NDependResults\\images\\test.png")).Returns(true).Verifiable();
                Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy("workingDir\\NDependResults\\images\\test.png", "artefactDir\\1\\NDepend\\images\\test.png")).Verifiable();
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
            var workingDir = "workingDir\\NDependResults";
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock("workingDir\\ndepend-app.exe", "workingDir\\NDependResults /OutDir workingDir\\NDependResults", "workingDir", 600000);
            var fileSystem = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            MockSequence sequence = new MockSequence();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[0]).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists("workingDir\\NDependResults\\ReportResources.xml")).Returns(false).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[] {
                "workingDir\\NDependResults\\file1.txt",
                "workingDir\\NDependResults\\file2.xml"
            }).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists("artefactDir\\1\\NDepend")).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists("workingDir\\NDependResults\\file1.txt")).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy("workingDir\\NDependResults\\file1.txt", "artefactDir\\1\\NDepend\\file1.txt")).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists("workingDir\\NDependResults\\file2.xml")).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy("workingDir\\NDependResults\\file2.xml", "artefactDir\\1\\NDepend\\file2.xml")).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GenerateTaskResultFromFile("workingDir\\NDependResults\\file2.xml")).Returns(mocks.Create<ITaskResult>().Object).Verifiable();
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
            var workingDir = "working Dir\\NDependResults";
            var result = GenerateResultMock("working Dir", "artefact Dir");
            var executor = GenerateExecutorMock("working Dir\\ndepend-app.exe", "\"working Dir\\NDependResults\" /OutDir \"working Dir\\NDependResults\"", "working Dir", 600000);
            var fileSystem = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            MockSequence sequence = new MockSequence();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[0]).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists("working Dir\\NDependResults\\ReportResources.xml")).Returns(false).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[] {
                "working Dir\\NDependResults\\file1.txt",
                "working Dir\\NDependResults\\file2.xml"
            }).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists("artefact Dir\\1\\NDepend")).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists("working Dir\\NDependResults\\file1.txt")).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy("working Dir\\NDependResults\\file1.txt", "artefact Dir\\1\\NDepend\\file1.txt")).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists("working Dir\\NDependResults\\file2.xml")).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy("working Dir\\NDependResults\\file2.xml", "artefact Dir\\1\\NDepend\\file2.xml")).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GenerateTaskResultFromFile("working Dir\\NDependResults\\file2.xml")).Returns(mocks.Create<ITaskResult>().Object).Verifiable();
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
            var workingDir = "somewhere-else\\NDependResults";
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock("somewhere-else\\NDepend.Console", "somewhere-else\\NDependResults /OutDir somewhere-else\\NDependResults", "somewhere-else", 600000);
            var fileSystem = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            MockSequence sequence = new MockSequence();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[0]).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists("somewhere-else\\NDependResults\\ReportResources.xml")).Returns(false).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[] {
                "somewhere-else\\NDependResults\\file1.txt",
                "somewhere-else\\NDependResults\\file2.xml"
            }).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists("artefactDir\\1\\NDepend")).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists("somewhere-else\\NDependResults\\file1.txt")).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy("somewhere-else\\NDependResults\\file1.txt", "artefactDir\\1\\NDepend\\file1.txt")).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists("somewhere-else\\NDependResults\\file2.xml")).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy("somewhere-else\\NDependResults\\file2.xml", "artefactDir\\1\\NDepend\\file2.xml")).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GenerateTaskResultFromFile("somewhere-else\\NDependResults\\file2.xml")).Returns(mocks.Create<ITaskResult>().Object).Verifiable();
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
            var workingDir = "workingDir\\out-dir";
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock("workingDir\\NDepend.Console",
                "workingDir\\test.project /Silent /EmitVisualNDependBinXml  /InDirs workingDir\\dir1 workingDir\\dir2 /OutDir workingDir\\out-dir /XslForReport  workingDir\\report.xslt", 
                "workingDir", 600000);
            var fileSystem = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            MockSequence sequence = new MockSequence();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[0]).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists("workingDir\\out-dir\\ReportResources.xml")).Returns(false).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[] {
                "workingDir\\out-dir\\file1.txt",
                "workingDir\\out-dir\\file2.xml"
            }).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists("artefactDir\\1\\NDepend")).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists("workingDir\\out-dir\\file1.txt")).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy("workingDir\\out-dir\\file1.txt", "artefactDir\\1\\NDepend\\file1.txt")).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists("workingDir\\out-dir\\file2.xml")).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy("workingDir\\out-dir\\file2.xml", "artefactDir\\1\\NDepend\\file2.xml")).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GenerateTaskResultFromFile("workingDir\\out-dir\\file2.xml")).Returns(mocks.Create<ITaskResult>().Object).Verifiable();
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
            var workingDir = "workingDir\\NDependResults";
            var result = GenerateResultMock();
            var executor = GenerateExecutorMock("workingDir\\NDepend.Console", "workingDir\\NDependResults /OutDir workingDir\\NDependResults", "workingDir", 600000);
            var fileSystem = mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            MockSequence sequence = new MockSequence();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[] {
                "workingDir\\NDependResults\\file1.txt",
                "workingDir\\NDependResults\\file2.xml"
            }).Verifiable();
            var baseTime = DateTime.Now;
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetLastWriteTime("workingDir\\NDependResults\\file1.txt")).Returns(baseTime).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetLastWriteTime("workingDir\\NDependResults\\file2.xml")).Returns(baseTime).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists("workingDir\\NDependResults\\ReportResources.xml")).Returns(false).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists(workingDir)).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetFilesInDirectory(workingDir)).Returns(new string[] {
                "workingDir\\NDependResults\\file1.txt",
                "workingDir\\NDependResults\\file2.xml"
            }).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetLastWriteTime("workingDir\\NDependResults\\file1.txt")).Returns(baseTime.AddMinutes(1)).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.GetLastWriteTime("workingDir\\NDependResults\\file2.xml")).Returns(baseTime).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.DirectoryExists("artefactDir\\1\\NDepend")).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.FileExists("workingDir\\NDependResults\\file1.txt")).Returns(true).Verifiable();
            Mock.Get(fileSystem).InSequence(sequence).Setup(_fileSystem => _fileSystem.Copy("workingDir\\NDependResults\\file1.txt", "artefactDir\\1\\NDepend\\file1.txt")).Verifiable();
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
            Mock.Get(result).Setup(_result => _result.BaseFromArtifactsDirectory("1")).Returns(string.Concat(artefactDir, "\\1"));
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
