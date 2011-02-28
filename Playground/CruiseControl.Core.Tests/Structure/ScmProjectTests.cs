namespace CruiseControl.Core.Tests.Structure
{
    using Core.Structure;
    using Interfaces;
    using Moq;
    using NUnit.Framework;
    using Stubs;

    [TestFixture]
    public class ScmProjectTests
    {
        #region Tests
        [Test]
        public void ValidateValidatesPreBuild()
        {
            var validateCalled = false;
            var project = new ScmProject("Test");
            project.PreBuild.Add(new TaskStub
                                     {
                                         OnValidateAction = vl => validateCalled = true
                                     });
            var validationMock = new Mock<IValidationLog>();
            project.Validate(validationMock.Object);
            Assert.IsTrue(validateCalled);
        }

        [Test]
        public void ValidateValidatesPublishers()
        {
            var validateCalled = false;
            var project = new ScmProject("Test");
            project.Publishers.Add(new TaskStub
                                       {
                                           OnValidateAction = vl => validateCalled = true
                                       });
            var validationMock = new Mock<IValidationLog>();
            project.Validate(validationMock.Object);
            Assert.IsTrue(validateCalled);
        }
        #endregion
    }
}
