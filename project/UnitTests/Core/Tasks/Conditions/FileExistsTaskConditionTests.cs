namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks.Conditions
{
    using NUnit.Framework;
    using Rhino.Mocks;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Tasks.Conditions;
    using ThoughtWorks.CruiseControl.Core.Util;

    public class FileExistsTaskConditionTests
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
            var fileSystemMock = this.mocks.StrictMock<IFileSystem>();
            var condition = new FileExistsTaskCondition
                {
                    FileSystem = fileSystemMock,
                    FileName = "TestFile"
                };
            var result = this.mocks.StrictMock<IIntegrationResult>();
            Expect.Call(result.BaseFromWorkingDirectory("TestFile")).Return("TestFile");
            Expect.Call(fileSystemMock.FileExists("TestFile")).Return(true);

            this.mocks.ReplayAll();
            var actual = condition.Eval(result);

            this.mocks.VerifyAll();
            Assert.IsTrue(actual);
        }

        [Test]
        public void EvaluateReturnsFalseIfConditionIsNotMatched()
        {
            var fileSystemMock = this.mocks.StrictMock<IFileSystem>();
            var condition = new FileExistsTaskCondition
                {
                    FileSystem = fileSystemMock,
                    FileName = "TestFile"
                };
            var result = this.mocks.StrictMock<IIntegrationResult>();
            Expect.Call(result.BaseFromWorkingDirectory("TestFile")).Return("TestFile");
            Expect.Call(fileSystemMock.FileExists("TestFile")).Return(false);

            this.mocks.ReplayAll();
            var actual = condition.Eval(result);

            this.mocks.VerifyAll();
            Assert.IsFalse(actual);
        }
    }
}
