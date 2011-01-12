namespace CruiseControl.Core.Tests.Tasks.Conditions
{
    using CruiseControl.Core;
    using CruiseControl.Core.Interfaces;
    using CruiseControl.Core.Tasks.Conditions;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class FileExistsTests
    {
        #region Tests
        [Test]
        public void ValidateChecksNameIsSet()
        {
            var condition = new FileExists();
            var validationMock = new Mock<IValidationLog>(MockBehavior.Strict);
            validationMock.Setup(vl => vl.AddError("FileName has not been set")).Verifiable();
            condition.Validate(validationMock.Object);
            validationMock.Verify();
        }

        [Test]
        public void EvaluateReturnsResultOfFileCheck()
        {
            var fileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);
            fileSystemMock.Setup(fs => fs.CheckIfFileExists("c:\\test.xml")).Returns(true);
            var condition = new FileExists
                                {
                                    FileName = "c:\\test.xml",
                                    FileSystem = fileSystemMock.Object
                                };
            var contextMock = new Mock<TaskExecutionContext>(new TaskExecutionParameters());
            var result = condition.Evaluate(contextMock.Object);
            Assert.IsTrue(result);
        }
        #endregion
    }
}
