namespace ThoughtWorks.CruiseControl.UnitTests.Core.Triggers
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using Exortech.NetReflector;
    using Moq;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Core.Triggers;
    using ThoughtWorks.CruiseControl.Remote;

    [TestFixture]
    public class ProjectTriggerTest : IntegrationFixture
    {
        private MockRepository mocks;
        private DateTime now;
        private DateTime later;

        [SetUp]
        protected void SetUp()
        {
            this.mocks = new MockRepository(MockBehavior.Default);
            now = DateTime.Now;
            later = now.AddHours(1);
        }

        [Test]
        public void ShouldNotTriggerOnFirstIntegration()
        {
            var innerTriggerMock = this.InitialiseTriggerMock();
            var clientMock = InitialiseClientMock(true, NewProjectStatus("project", IntegrationStatus.Success, now));
            var factoryMock = this.InitialiseFactoryMock(clientMock);
            var trigger = new ProjectTrigger(factoryMock)
                {
                    Project = "project",
                    InnerTrigger = innerTriggerMock
                };

            var actual = trigger.Fire();

            this.InitialiseTriggerMockVerify(innerTriggerMock, 1);
            this.mocks.VerifyAll();
            Assert.IsNull(actual);
        }

        [Test]
        public void ShouldTriggerOnFirstIntegrationIfDependentProjectBuildSucceededAndTriggerFirstTimeIsSet()
        {
            var innerTriggerMock = this.InitialiseTriggerMock();
            var clientMock = InitialiseClientMock(
                true,
                NewProjectStatus("project", IntegrationStatus.Success, now));
            var factoryMock = this.InitialiseFactoryMock(clientMock);
            var trigger = new ProjectTrigger(factoryMock)
                {
                    Project = "project",
                    InnerTrigger = innerTriggerMock,
                    TriggerFirstTime = true
                };

            var actual = trigger.Fire();

            this.InitialiseTriggerMockVerify(innerTriggerMock, 1);
            this.mocks.VerifyAll();
            var expected = ModificationExistRequest();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldNotTriggerOnFirstIntegrationIfDependentProjectBuildFailedAndTriggerFirstTimeIsSet()
        {
            var innerTriggerMock = this.InitialiseTriggerMock();
            var clientMock = InitialiseClientMock(true, NewProjectStatus("project", IntegrationStatus.Failure, now));
            var factoryMock = this.InitialiseFactoryMock(clientMock);
            var trigger = new ProjectTrigger(factoryMock)
                {
                    Project = "project",
                    InnerTrigger = innerTriggerMock,
                    TriggerFirstTime = true
                };

            var actual = trigger.Fire();

            this.InitialiseTriggerMockVerify(innerTriggerMock, 1);
            this.mocks.VerifyAll();
            Assert.IsNull(actual);
        }

        [Test]
        public void TriggerWhenDependentProjectBuildsSuccessfully()
        {
            var innerTriggerMock = this.InitialiseTriggerMock();
            var clientMocks = new[] 
                {
                    InitialiseClientMock(true, NewProjectStatus("project", IntegrationStatus.Success, now)),
                    InitialiseClientMock(true, NewProjectStatus("project", IntegrationStatus.Success, later))
                };
            var factoryMock = this.InitialiseFactoryMock(clientMocks);
            var trigger = new ProjectTrigger(factoryMock)
                {
                    Project = "project",
                    InnerTrigger = innerTriggerMock
                };

            trigger.Fire();
            var actual = trigger.Fire();

            this.InitialiseTriggerMockVerify(innerTriggerMock, 2);
            this.mocks.VerifyAll();
            var expected = ModificationExistRequest();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DoNotTriggerWhenInnerTriggerReturnsNoBuild()
        {
            var innerTriggerMock = this.mocks.Create<ITrigger>(MockBehavior.Strict).Object;
            Mock.Get(innerTriggerMock).Setup(_innerTrigger => _innerTrigger.Fire()).Returns(() => null).Verifiable();
            var factoryMock = this.mocks.Create<ICruiseServerClientFactory>(MockBehavior.Strict).Object;
            var trigger = new ProjectTrigger(factoryMock)
            {
                Project = "project",
                InnerTrigger = innerTriggerMock
            };

            var actual = trigger.Fire();

            this.mocks.VerifyAll();
            Assert.IsNull(actual);
        }

        [Test]
        public void DoNotTriggerWhenDependentProjectBuildFails()
        {
            var innerTriggerMock = this.InitialiseTriggerMock();
            var clientMocks = new[] 
                {
                    InitialiseClientMock(true, NewProjectStatus("project", IntegrationStatus.Success, now)),
                    InitialiseClientMock(true, NewProjectStatus("project", IntegrationStatus.Failure, later))
                };
            var factoryMock = this.InitialiseFactoryMock(clientMocks);
            var trigger = new ProjectTrigger(factoryMock)
            {
                Project = "project",
                InnerTrigger = innerTriggerMock
            };

            trigger.Fire();
            var actual = trigger.Fire();

            this.InitialiseTriggerMockVerify(innerTriggerMock, 2);
            this.mocks.VerifyAll();
            Assert.IsNull(actual);
        }

        [Test]
        public void DoNotTriggerIfProjectHasNotBuiltSinceLastPoll()
        {
            var innerTriggerMock = this.InitialiseTriggerMock();
            var clientMocks = new[] 
                {
                    InitialiseClientMock(true, NewProjectStatus("project", IntegrationStatus.Success, now)),
                    InitialiseClientMock(true, NewProjectStatus("project", IntegrationStatus.Success, now))
                };
            var factoryMock = this.InitialiseFactoryMock(clientMocks);
            var trigger = new ProjectTrigger(factoryMock)
            {
                Project = "project",
                InnerTrigger = innerTriggerMock
            };

            trigger.Fire();
            var actual = trigger.Fire();

            this.InitialiseTriggerMockVerify(innerTriggerMock, 2);
            this.mocks.VerifyAll();
            Assert.IsNull(actual);
        }

        [Test]
        public void IntegrationCompletedShouldDelegateToInnerTrigger()
        {
            var innerTriggerMock = this.mocks.Create<ITrigger>(MockBehavior.Strict).Object;
            Mock.Get(innerTriggerMock).Setup(_innerTrigger => _innerTrigger.IntegrationCompleted()).Verifiable();
            var trigger = new ProjectTrigger
                {
                    InnerTrigger = innerTriggerMock
                };
            trigger.IntegrationCompleted();
            mocks.VerifyAll();
        }

        [Test]
        public void NextBuildShouldReturnInnerTriggerNextBuildIfUnknown()
        {
            var innerTriggerMock = this.mocks.Create<ITrigger>(MockBehavior.Strict).Object;
            Mock.Get(innerTriggerMock).SetupGet(_innerTrigger => _innerTrigger.NextBuild).Returns(now).Verifiable();
            var trigger = new ProjectTrigger
                {
                    InnerTrigger = innerTriggerMock
                };
            var actual = trigger.NextBuild;

            mocks.VerifyAll();
            Assert.AreEqual(now, actual);
        }

        [Test]
        public void PopulateFromConfiguration()
        {
            string xml = @"<projectTrigger>
	<serverUri>http://fooserver:12342/CruiseManager.rem</serverUri>
	<project>Foo</project>
	<triggerStatus>Failure</triggerStatus>
	<triggerFirstTime>True</triggerFirstTime>
	<innerTrigger type=""intervalTrigger"">
		<buildCondition>ForceBuild</buildCondition>
		<seconds>10</seconds>
	</innerTrigger>
</projectTrigger>";
            var trigger = NetReflector.Read(xml) as ProjectTrigger;
            Assert.IsNotNull(trigger);
            Assert.AreEqual("http://fooserver:12342/CruiseManager.rem", trigger.ServerUri);
            Assert.AreEqual("Foo", trigger.Project);
            Assert.IsNotNull(trigger.InnerTrigger);
            Assert.AreEqual(IntegrationStatus.Failure, trigger.TriggerStatus);
            Assert.IsTrue(trigger.TriggerFirstTime);
        }

        [Test]
        public void PopulateFromMinimalConfiguration()
        {
            string xml = @"<projectTrigger><project>Foo</project></projectTrigger>";
            var trigger = NetReflector.Read(xml) as ProjectTrigger;
            Assert.IsNotNull(trigger);
            Assert.AreEqual("tcp://localhost:21234/CruiseManager.rem", trigger.ServerUri);
            Assert.AreEqual("Foo", trigger.Project);
            Assert.IsNotNull(trigger.InnerTrigger);
            Assert.AreEqual(IntegrationStatus.Success, trigger.TriggerStatus);
            Assert.IsFalse(trigger.TriggerFirstTime);
        }

        [Test]
        public void HandleExceptionInProjectLocator()
        {
            var innerTriggerMock = this.InitialiseTriggerMock();
            var clientMock = this.InitialiseClientMock(true, NewProjectStatus("wrong", IntegrationStatus.Failure, now));
            var factoryMock = this.InitialiseFactoryMock(clientMock);
            var trigger = new ProjectTrigger(factoryMock)
            {
                Project = "project",
                InnerTrigger = innerTriggerMock
            };

            Assert.Throws<NoSuchProjectException>(() => trigger.Fire());

            this.InitialiseTriggerMockVerify(innerTriggerMock, 1);
            this.mocks.VerifyAll();
        }

        [Test]
        public void HandleSocketError()
        {
            var innerTriggerMock = this.InitialiseTriggerMock();
            var clientMock = this.mocks.Create<CruiseServerClientBase>(MockBehavior.Strict).Object;
            var factoryMock = this.InitialiseFactoryMock(clientMock);
            Mock.Get(clientMock).SetupSet(_clientMock => _clientMock.SessionToken = It.IsAny<string>()).Verifiable();
            Mock.Get(clientMock).Setup(_clientMock => _clientMock.GetProjectStatus())
                .Throws(new SocketException()).Verifiable();
            var trigger = new ProjectTrigger(factoryMock)
            {
                Project = "project",
                InnerTrigger = innerTriggerMock
            };

            var actual = trigger.Fire();

            this.InitialiseTriggerMockVerify(innerTriggerMock, 1);
            this.mocks.VerifyAll();
            Assert.IsNull(actual);
        }

        [Test]
        public void FirePassesInLoginCredentialsAndLogsOut()
        {
            var credentials = new[] 
                {
                    new NameValuePair("user", "me")
                };
            var innerTriggerMock = this.InitialiseTriggerMock();
            var clientMock = InitialiseClientMock(
                false,
                NewProjectStatus("project", IntegrationStatus.Success, now));
            Mock.Get(clientMock).Setup(_clientMock => _clientMock.Login(new List<NameValuePair>(credentials))).Returns(true).Verifiable();
            Mock.Get(clientMock).SetupGet(_clientMock => _clientMock.SessionToken).Returns("token").Verifiable();
            Mock.Get(clientMock).Setup(_clientMock => _clientMock.Logout()).Verifiable();
            var factoryMock = this.InitialiseFactoryMock(clientMock);
            var trigger = new ProjectTrigger(factoryMock)
            {
                Project = "project",
                InnerTrigger = innerTriggerMock,
                TriggerFirstTime = true,
                SecurityCredentials = credentials
            };

            var actual = trigger.Fire();

            this.InitialiseTriggerMockVerify(innerTriggerMock, 1);
            this.mocks.VerifyAll();
            var expected = ModificationExistRequest();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FirePassesInLoginCredentialsAndHandlesFailure()
        {
            var credentials = new[] 
                {
                    new NameValuePair("user", "me")
                };
            var innerTriggerMock = this.InitialiseTriggerMock();
            var clientMock = InitialiseClientMock(
                false,
                NewProjectStatus("project", IntegrationStatus.Success, now));
            Mock.Get(clientMock).Setup(_clientMock => _clientMock.Login(new List<NameValuePair>(credentials))).Returns(false).Verifiable();
            var factoryMock = this.InitialiseFactoryMock(clientMock);
            var trigger = new ProjectTrigger(factoryMock)
            {
                Project = "project",
                InnerTrigger = innerTriggerMock,
                TriggerFirstTime = true,
                SecurityCredentials = credentials
            };

            var actual = trigger.Fire();

            this.InitialiseTriggerMockVerify(innerTriggerMock, 1);
            this.mocks.VerifyAll();
            var expected = ModificationExistRequest();
            Assert.AreEqual(expected, actual);
        }

        private ProjectStatus NewProjectStatus(string name, IntegrationStatus integrationStatus, DateTime dateTime)
        {
            return ProjectStatusFixture.New(name, integrationStatus, dateTime);
        }

        private ITrigger InitialiseTriggerMock()
        {
            var innerTriggerMock = this.mocks.Create<ITrigger>(MockBehavior.Strict).Object;
            Mock.Get(innerTriggerMock).Setup(_innerTriggerMock => _innerTriggerMock.Fire()).Returns(ModificationExistRequest()).Verifiable();
            Mock.Get(innerTriggerMock).Setup(_innerTriggerMock => _innerTriggerMock.IntegrationCompleted()).Verifiable();
            return innerTriggerMock;
        }
        private void InitialiseTriggerMockVerify(ITrigger innerTriggerMock, int times)
        {
            Mock.Get(innerTriggerMock).Verify(_innerTriggerMock => _innerTriggerMock.Fire(), Times.Exactly(times));
            Mock.Get(innerTriggerMock).Verify(_innerTriggerMock => _innerTriggerMock.IntegrationCompleted(), Times.Exactly(times));
        }

        private CruiseServerClientBase InitialiseClientMock(bool expectSessionOverride, params ProjectStatus[] statuses)
        {
            var clientMock = this.mocks.Create<CruiseServerClientBase>(MockBehavior.Strict).Object;
            if (expectSessionOverride)
            {
                Mock.Get(clientMock).SetupSet(_clientMock => _clientMock.SessionToken = It.IsAny<string>()).Verifiable();
            }

            Mock.Get(clientMock).Setup(_clientMock => _clientMock.GetProjectStatus()).Returns(statuses).Verifiable();
            return clientMock;
        }

        private ICruiseServerClientFactory InitialiseFactoryMock(params CruiseServerClientBase[] clientMocks)
        {
            var factoryMock = this.mocks.Create<ICruiseServerClientFactory>(MockBehavior.Strict);
            var setupSequence = factoryMock.SetupSequence(_factoryMock => _factoryMock.GenerateClient("tcp://localhost:21234/CruiseManager.rem"));
            foreach (var clientMock in clientMocks)
            {
                setupSequence = setupSequence.Returns(clientMock);
            }
            return factoryMock.Object;
        }
    }
}
