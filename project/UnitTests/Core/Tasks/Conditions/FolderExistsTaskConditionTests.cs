namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks.Conditions
{
    using System;
    using NUnit.Framework;
    using Rhino.Mocks;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Tasks.Conditions;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Core.Util;

    public class FolderExistsTaskConditionTests
    {
        private MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            this.mocks = new MockRepository();
        }

        [Test]
        public void EvaluateReturnsTrueIfConditionIsMatched()
        {
            Func<IIntegrationResult, bool> evalFunc = ir => true;
            var fileSystemMock = this.mocks.StrictMock<IFileSystem>();
            var condition = new FolderExistsTaskCondition
                {
                    FileSystem = fileSystemMock,
                    FolderName = "TestFolder"
                };
            var result = this.mocks.StrictMock<IIntegrationResult>();
            Expect.Call(result.BaseFromWorkingDirectory("TestFolder")).Return("TestFolder");
            Expect.Call(fileSystemMock.DirectoryExists("TestFolder")).Return(true);

            this.mocks.ReplayAll();
            var actual = condition.Eval(result);

            this.mocks.VerifyAll();
            Assert.IsTrue(actual);
        }

        [Test]
        public void EvaluateReturnsFalseIfConditionIsNotMatched()
        {
            Func<IIntegrationResult, bool> evalFunc = ir => true;
            var fileSystemMock = this.mocks.StrictMock<IFileSystem>();
            var condition = new FolderExistsTaskCondition
                {
                    FileSystem = fileSystemMock,
                    FolderName = "TestFolder"
                };
            var result = this.mocks.StrictMock<IIntegrationResult>();
            Expect.Call(result.BaseFromWorkingDirectory("TestFolder")).Return("TestFolder");
            Expect.Call(fileSystemMock.DirectoryExists("TestFolder")).Return(false);

            this.mocks.ReplayAll();
            var actual = condition.Eval(result);

            this.mocks.VerifyAll();
            Assert.IsFalse(actual);
        }
    }
}
