namespace CruiseControl.Core.Tests.Tasks
{
    using System.Linq;
    using CruiseControl.Common;
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
            var urnName = "urn:ccnet:test:otherproject";
            var task = new ForceBuild
                           {
                               ProjectName = urnName
                           };
            var thisProject = new Project("thisProject", task);
            var invokerMock = new Mock<IActionInvoker>(MockBehavior.Strict);
            invokerMock.SetupSet(ai => ai.Server = It.IsAny<Server>());
            invokerMock.Setup(ai => ai.Invoke(urnName, It.IsAny<InvokeArguments>()))
                .Returns(new InvokeResult
                             {
                                 Data = "<Blank xmlns=\"urn:cruisecontrol:common\" />",
                                 ResultCode = RemoteResultCode.Success
                             })
                .Verifiable();
            new Server("test", thisProject)
                {
                    ActionInvoker = invokerMock.Object
                };
            var contextMock = new Mock<TaskExecutionContext>(MockBehavior.Strict, new TaskExecutionParameters());
            contextMock.Setup(c => c.AddEntryToBuildLog("Force build successfully sent to '" + urnName + "'")).Verifiable();
            task.Run(contextMock.Object).Count();
            invokerMock.Verify();
            contextMock.Verify();
        }

        [Test]
        public void RunForcesABuildUsingAProjecName()
        {
            var projectName = "otherproject";
            var urnName = "urn:ccnet:test:" + projectName;
            var task = new ForceBuild
                           {
                               ProjectName = projectName
                           };
            var thisProject = new Project("thisProject", task);
            var invokerMock = new Mock<IActionInvoker>(MockBehavior.Strict);
            invokerMock.SetupSet(ai => ai.Server = It.IsAny<Server>());
            invokerMock.Setup(ai => ai.Invoke(urnName, It.IsAny<InvokeArguments>()))
                .Returns(new InvokeResult
                             {
                                 Data = "<Blank xmlns=\"urn:cruisecontrol:common\" />",
                                 ResultCode = RemoteResultCode.Success
                             })
                .Verifiable();
            new Server("test", thisProject)
                {
                    ActionInvoker = invokerMock.Object
                };
            var contextMock = new Mock<TaskExecutionContext>(MockBehavior.Strict, new TaskExecutionParameters());
            contextMock.Setup(c => c.AddEntryToBuildLog("Force build successfully sent to '" + projectName + "'")).Verifiable();
            task.Run(contextMock.Object).Count();
            invokerMock.Verify();
            contextMock.Verify();
        }

        [Test]
        public void RunReturnsFailureIfForceFails()
        {
            var urnName = "urn:ccnet:test:otherproject";
            var task = new ForceBuild
                           {
                               ProjectName = urnName
                           };
            var thisProject = new Project("thisProject", task);
            var invokerMock = new Mock<IActionInvoker>(MockBehavior.Strict);
            invokerMock.SetupSet(ai => ai.Server = It.IsAny<Server>());
            invokerMock.Setup(ai => ai.Invoke(urnName, It.IsAny<InvokeArguments>()))
                .Returns(new InvokeResult
                             {
                                 ResultCode = RemoteResultCode.FatalError
                             })
                .Verifiable();
            new Server("test", thisProject)
                {
                    ActionInvoker = invokerMock.Object
                };
            var contextMock = new Mock<TaskExecutionContext>(MockBehavior.Strict, new TaskExecutionParameters());
            contextMock
                .Setup(c => c.AddEntryToBuildLog("Force build failed for '" + urnName + "' - result code " + RemoteResultCode.FatalError))
                .Verifiable();
            contextMock.SetupProperty(c => c.CurrentStatus);
            task.Run(contextMock.Object).Count();
            invokerMock.Verify();
            contextMock.Verify();
            Assert.AreEqual(IntegrationStatus.Failure, contextMock.Object.CurrentStatus);
        }

        [Test]
        [Ignore("TODO: Work out logic for performing a remote force build")]
        public void RunForcesARemoteBuildUsingUniversalName()
        {
            var urnName = "urn:ccnet:remote:otherproject";
            var task = new ForceBuild
                           {
                               ServerAddress = "http://somewhere/ccnet",
                               ProjectName = urnName
                           };
            var contextMock = new Mock<TaskExecutionContext>(MockBehavior.Strict, new TaskExecutionParameters());
            contextMock.Setup(c => c.AddEntryToBuildLog("Force build successfully sent to '" + urnName + "'")).Verifiable();
            task.Run(contextMock.Object).Count();
            contextMock.Verify();
        }

        [Test]
        [Ignore("TODO: Work out logic for performing a remote force build")]
        public void RunForcesARemoteBuildUsingProjectName()
        {
            var projectName = "otherproject";
            var urnName = "urn:ccnet:test:" + projectName;
            var task = new ForceBuild
                           {
                               ServerAddress = "http://somewhere/ccnet",
                               ProjectName = projectName
                           };
            var contextMock = new Mock<TaskExecutionContext>(MockBehavior.Strict, new TaskExecutionParameters());
            contextMock.Setup(c => c.AddEntryToBuildLog("Force build successfully sent to '" + urnName + "'")).Verifiable();
            task.Run(contextMock.Object).Count();
            contextMock.Verify();
        }

        [Test]
        [Ignore("TODO: Work out logic for performing a remote force build")]
        public void RunReturnsFailureIfRemoteForceFails()
        {
            var urnName = "urn:ccnet:remote:otherproject";
            var task = new ForceBuild
                           {
                               ServerAddress = "http://somewhere/ccnet",
                               ProjectName = urnName
                           };
            var contextMock = new Mock<TaskExecutionContext>(MockBehavior.Strict, new TaskExecutionParameters());
            contextMock
                .Setup(c => c.AddEntryToBuildLog("Force build failed for '" + urnName + "' - result code " + RemoteResultCode.FatalError))
                .Verifiable();
            contextMock.SetupProperty(c => c.CurrentStatus);
            task.Run(contextMock.Object).Count();
            contextMock.Verify();
            Assert.AreEqual(IntegrationStatus.Failure, contextMock.Object.CurrentStatus);
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
