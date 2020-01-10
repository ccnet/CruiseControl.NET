namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks.Conditions
{
    using Moq;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Tasks.Conditions;
    using ThoughtWorks.CruiseControl.Core.Util;

    public class FolderExistsTaskConditionTests
    {
        private MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            this.mocks = new MockRepository(MockBehavior.Default);
        }

        [Test]
        public void EvaluateReturnsTrueIfConditionIsMatched()
        {
            var fileSystemMock = this.mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            var condition = new FolderExistsTaskCondition
                {
                    FileSystem = fileSystemMock,
                    FolderName = "TestFolder"
                };
            var result = this.mocks.Create<IIntegrationResult>(MockBehavior.Strict).Object;
            Mock.Get(result).Setup(_result => _result.BaseFromWorkingDirectory("TestFolder")).Returns("TestFolder").Verifiable();
            Mock.Get(fileSystemMock).Setup(_fileSystemMock => _fileSystemMock.DirectoryExists("TestFolder")).Returns(true).Verifiable();

            var actual = condition.Eval(result);

            this.mocks.VerifyAll();
            Assert.IsTrue(actual);
        }

        [Test]
        public void EvaluateReturnsFalseIfConditionIsNotMatched()
        {
            var fileSystemMock = this.mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            var condition = new FolderExistsTaskCondition
                {
                    FileSystem = fileSystemMock,
                    FolderName = "TestFolder"
                };
            var result = this.mocks.Create<IIntegrationResult>(MockBehavior.Strict).Object;
            Mock.Get(result).Setup(_result => _result.BaseFromWorkingDirectory("TestFolder")).Returns("TestFolder").Verifiable();
            Mock.Get(fileSystemMock).Setup(_fileSystemMock => _fileSystemMock.DirectoryExists("TestFolder")).Returns(false).Verifiable();

            var actual = condition.Eval(result);

            this.mocks.VerifyAll();
            Assert.IsFalse(actual);
        }
    }
}
