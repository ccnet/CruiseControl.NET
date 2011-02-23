namespace CruiseControl.Core.Tests.Tasks
{
    using System.Linq;
    using CruiseControl.Common;
    using CruiseControl.Core.Interfaces;
    using CruiseControl.Core.Tasks;
    using Moq;
    using NUnit.Framework;
    using Messages = CruiseControl.Common.Messages;

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
        public void RunForcesARemoteBuildUsingUniversalName()
        {
            var urnName = "urn:ccnet:remote:otherproject";
            var connectionMock = new Mock<ServerConnection>(MockBehavior.Strict);
            connectionMock.Setup(sc => sc.Invoke(urnName, "ForceBuild"))
                .Returns(new Messages.Blank());
            var factoryMock = new Mock<IServerConnectionFactory>(MockBehavior.Strict);
            var address = "http://somewhere/ccnet";
            factoryMock.Setup(scf => scf.GenerateConnection(address))
                .Returns(connectionMock.Object);
            var task = new ForceBuild
                           {
                               ServerAddress = address,
                               ProjectName = urnName,
                               ServerConnectionFactory = factoryMock.Object
                           };
            var contextMock = new Mock<TaskExecutionContext>(MockBehavior.Strict, new TaskExecutionParameters());
            contextMock.Setup(c => c.AddEntryToBuildLog("Force build successfully sent to '" + urnName + "' at '" + address + "'")).Verifiable();
            task.Run(contextMock.Object).Count();
            contextMock.Verify();
        }

        [Test]
        public void RunForcesARemoteBuildUsingProjectName()
        {
            var projectName = "otherproject";
            var urnName = "urn:ccnet:test:" + projectName;
            var connectionMock = new Mock<ServerConnection>(MockBehavior.Strict);
            connectionMock.Setup(sc => sc.Invoke(urnName, "ForceBuild"))
                .Returns(new Messages.Blank());
            var factoryMock = new Mock<IServerConnectionFactory>(MockBehavior.Strict);
            var address = "http://somewhere/ccnet";
            factoryMock.Setup(scf => scf.GenerateConnection(address))
                .Returns(connectionMock.Object);
            factoryMock.Setup(scf => scf.GenerateUrn(address, projectName))
                .Returns(urnName);
            var task = new ForceBuild
                           {
                               ServerAddress = address,
                               ProjectName = projectName,
                               ServerConnectionFactory = factoryMock.Object
                           };
            var contextMock = new Mock<TaskExecutionContext>(MockBehavior.Strict, new TaskExecutionParameters());
            contextMock.Setup(c => c.AddEntryToBuildLog("Force build successfully sent to '" + projectName + "' at '" + address + "'")).Verifiable();
            task.Run(contextMock.Object).Count();
            contextMock.Verify();
        }

        [Test]
        public void RunReturnsFailureIfRemoteForceFails()
        {
            var urnName = "urn:ccnet:remote:otherproject";
            var connectionMock = new Mock<ServerConnection>(MockBehavior.Strict);
            connectionMock.Setup(sc => sc.Invoke(urnName, "ForceBuild"))
                .Throws(new RemoteServerException(RemoteResultCode.FatalError, null));
            var factoryMock = new Mock<IServerConnectionFactory>(MockBehavior.Strict);
            var address = "http://somewhere/ccnet";
            factoryMock.Setup(scf => scf.GenerateConnection(address))
                .Returns(connectionMock.Object);
            var task = new ForceBuild
                           {
                               ServerAddress = address,
                               ProjectName = urnName,
                               ServerConnectionFactory = factoryMock.Object
                           };
            var contextMock = new Mock<TaskExecutionContext>(MockBehavior.Strict, new TaskExecutionParameters());
            var message = "Force build failed for '" + urnName + "' at '" + address + "' - result code " + RemoteResultCode.FatalError;
            contextMock.Setup(c => c.AddEntryToBuildLog(message)).Verifiable();
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
