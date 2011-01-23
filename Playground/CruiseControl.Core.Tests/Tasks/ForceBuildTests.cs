namespace CruiseControl.Core.Tests.Tasks
{
    using System.Linq;
    using CruiseControl.Common.Messages;
    using CruiseControl.Core.Interfaces;
    using CruiseControl.Core.Tasks;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class ForceBuildTests
    {
        #region Tests
        [Test]
        public void RunForcesABuildUsingUniversalName()
        {
            var urnName = "urn:test:otherProject";
            var task = new ForceBuild
                           {
                               ProjectName = urnName
                           };
            var thisProject = new Project("thisProject", task);
            var invokerMock = new Mock<IActionInvoker>(MockBehavior.Strict);
            invokerMock.SetupSet(ai => ai.Server = It.IsAny<Server>());
            invokerMock.Setup(ai => ai.Invoke(urnName, "ForceBuild", It.IsAny<ProjectMessage>()))
                .Returns(new BuildMessage())
                .Verifiable();
            new Server("test", thisProject)
                {
                    ActionInvoker = invokerMock.Object
                };
            var context = new TaskExecutionContext(new TaskExecutionParameters());
            task.Run(context).Count();
            invokerMock.Verify();
        }

        [Test]
        public void ValidateChecksTheProjectName()
        {
            var task = new ForceBuild();
            var validationMock = new Mock<IValidationLog>(MockBehavior.Strict);
            validationMock.Setup(vl => vl.AddError("ProjectName has not been set")).Verifiable();
            task.Validate(validationMock.Object);
            validationMock.Verify();
        }
        #endregion
    }
}
