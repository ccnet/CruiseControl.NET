namespace ThoughtWorks.CruiseControl.UnitTests.Core.Triggers
{
    using System;
    using Exortech.NetReflector;
    using Rhino.Mocks;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Core.Triggers;
    using ThoughtWorks.CruiseControl.Remote;
    using System.Collections.Generic;
    using System.Net.Sockets;

    [TestFixture]
    public class ProjectTriggerTest : IntegrationFixture
    {
        private MockRepository mocks;
        private DateTime now;
        private DateTime later;

        [SetUp]
        protected void SetUp()
        {
            this.mocks = new MockRepository();
            now = DateTime.Now;
            later = now.AddHours(1);
        }

        [Test]
        public void ShouldNotTriggerOnFirstIntegration()
        {
            var innerTriggerMock = this.InitialiseTriggerMock(1);
            var clientMock = InitialiseClientMock(true, NewProjectStatus("project", IntegrationStatus.Success, now));
            var factoryMock = this.InitialiseFactoryMock(clientMock);
            var trigger = new ProjectTrigger(factoryMock)
                {
                    Project = "project",
                    InnerTrigger = innerTriggerMock
                };

            this.mocks.ReplayAll();
            var actual = trigger.Fire();

            this.mocks.VerifyAll();
            Assert.IsNull(actual);
        }

        [Test]
        public void ShouldTriggerOnFirstIntegrationIfDependentProjectBuildSucceededAndTriggerFirstTimeIsSet()
        {
            var innerTriggerMock = this.InitialiseTriggerMock(1);
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

            this.mocks.ReplayAll();
            var actual = trigger.Fire();

            this.mocks.VerifyAll();
            var expected = ModificationExistRequest();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldNotTriggerOnFirstIntegrationIfDependentProjectBuildFailedAndTriggerFirstTimeIsSet()
        {
            var innerTriggerMock = this.InitialiseTriggerMock(1);
            var clientMock = InitialiseClientMock(true, NewProjectStatus("project", IntegrationStatus.Failure, now));
            var factoryMock = this.InitialiseFactoryMock(clientMock);
            var trigger = new ProjectTrigger(factoryMock)
                {
                    Project = "project",
                    InnerTrigger = innerTriggerMock,
                    TriggerFirstTime = true
                };

            this.mocks.ReplayAll();
            var actual = trigger.Fire();

            this.mocks.VerifyAll();
            Assert.IsNull(actual);
        }

        [Test]
        public void TriggerWhenDependentProjectBuildsSuccessfully()
        {
            var innerTriggerMock = this.InitialiseTriggerMock(2);
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

            this.mocks.ReplayAll();
            trigger.Fire();
            var actual = trigger.Fire();

            this.mocks.VerifyAll();
            var expected = ModificationExistRequest();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DoNotTriggerWhenInnerTriggerReturnsNoBuild()
        {
            var innerTriggerMock = this.mocks.StrictMock<ITrigger>();
            Expect.Call(innerTriggerMock.Fire()).Return(null);
            var factoryMock = this.mocks.StrictMock<ICruiseServerClientFactory>();
            var trigger = new ProjectTrigger(factoryMock)
            {
                Project = "project",
                InnerTrigger = innerTriggerMock
            };

            this.mocks.ReplayAll();
            var actual = trigger.Fire();

            this.mocks.VerifyAll();
            Assert.IsNull(actual);
        }

        [Test]
        public void DoNotTriggerWhenDependentProjectBuildFails()
        {
            var innerTriggerMock = this.InitialiseTriggerMock(2);
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

            this.mocks.ReplayAll();
            trigger.Fire();
            var actual = trigger.Fire();

            this.mocks.VerifyAll();
            Assert.IsNull(actual);
        }

        [Test]
        public void DoNotTriggerIfProjectHasNotBuiltSinceLastPoll()
        {
            var innerTriggerMock = this.InitialiseTriggerMock(2);
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

            this.mocks.ReplayAll();
            trigger.Fire();
            var actual = trigger.Fire();

            this.mocks.VerifyAll();
            Assert.IsNull(actual);
        }

        [Test]
        public void IntegrationCompletedShouldDelegateToInnerTrigger()
        {
            var innerTriggerMock = this.mocks.StrictMock<ITrigger>();
            Expect.Call(() => innerTriggerMock.IntegrationCompleted());
            var trigger = new ProjectTrigger
                {
                    InnerTrigger = innerTriggerMock
                };
            mocks.ReplayAll();
            trigger.IntegrationCompleted();
            mocks.VerifyAll();
        }

        [Test]
        public void NextBuildShouldReturnInnerTriggerNextBuildIfUnknown()
        {
            var innerTriggerMock = this.mocks.StrictMock<ITrigger>();
            Expect.Call(innerTriggerMock.NextBuild).Return(now);
            var trigger = new ProjectTrigger
                {
                    InnerTrigger = innerTriggerMock
                };
            mocks.ReplayAll();
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
            var innerTriggerMock = this.InitialiseTriggerMock(1);
            var clientMock = this.InitialiseClientMock(true, NewProjectStatus("wrong", IntegrationStatus.Failure, now));
            var factoryMock = this.InitialiseFactoryMock(clientMock);
            var trigger = new ProjectTrigger(factoryMock)
            {
                Project = "project",
                InnerTrigger = innerTriggerMock
            };

            this.mocks.ReplayAll();
            Assert.Throws<NoSuchProjectException>(() => trigger.Fire());

            this.mocks.VerifyAll();
        }

        [Test]
        public void HandleSocketError()
        {
            var innerTriggerMock = this.InitialiseTriggerMock(1);
            var clientMock = this.mocks.StrictMock<CruiseServerClientBase>();
            var factoryMock = this.InitialiseFactoryMock(clientMock);
            Expect.Call(clientMock.SessionToken).SetPropertyAndIgnoreArgument();
            Expect.Call(clientMock.GetProjectStatus())
                .Throw(new SocketException());
            var trigger = new ProjectTrigger(factoryMock)
            {
                Project = "project",
                InnerTrigger = innerTriggerMock
            };

            this.mocks.ReplayAll();
            var actual = trigger.Fire();

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
            var innerTriggerMock = this.InitialiseTriggerMock(1);
            var clientMock = InitialiseClientMock(
                false,
                NewProjectStatus("project", IntegrationStatus.Success, now));
            Expect.Call(clientMock.Login(new List<NameValuePair>(credentials))).Return(true);
            Expect.Call(clientMock.SessionToken).Return("token");
            Expect.Call(() => clientMock.Logout());
            var factoryMock = this.InitialiseFactoryMock(clientMock);
            var trigger = new ProjectTrigger(factoryMock)
            {
                Project = "project",
                InnerTrigger = innerTriggerMock,
                TriggerFirstTime = true,
                SecurityCredentials = credentials
            };

            this.mocks.ReplayAll();
            var actual = trigger.Fire();

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
            var innerTriggerMock = this.InitialiseTriggerMock(1);
            var clientMock = InitialiseClientMock(
                false,
                NewProjectStatus("project", IntegrationStatus.Success, now));
            Expect.Call(clientMock.Login(new List<NameValuePair>(credentials))).Return(false);
            var factoryMock = this.InitialiseFactoryMock(clientMock);
            var trigger = new ProjectTrigger(factoryMock)
            {
                Project = "project",
                InnerTrigger = innerTriggerMock,
                TriggerFirstTime = true,
                SecurityCredentials = credentials
            };

            this.mocks.ReplayAll();
            var actual = trigger.Fire();

            this.mocks.VerifyAll();
            var expected = ModificationExistRequest();
            Assert.AreEqual(expected, actual);
        }

        private ProjectStatus NewProjectStatus(string name, IntegrationStatus integrationStatus, DateTime dateTime)
        {
            return ProjectStatusFixture.New(name, integrationStatus, dateTime);
        }

        private ITrigger InitialiseTriggerMock(int times)
        {
            var innerTriggerMock = this.mocks.StrictMock<ITrigger>();
            Expect.Call(innerTriggerMock.Fire()).Return(ModificationExistRequest()).Repeat.Times(times);
            Expect.Call(() => innerTriggerMock.IntegrationCompleted()).Repeat.Times(times);
            return innerTriggerMock;
        }

        private CruiseServerClientBase InitialiseClientMock(bool expectSessionOverride, params ProjectStatus[] statuses)
        {
            var clientMock = this.mocks.StrictMock<CruiseServerClientBase>();
            if (expectSessionOverride)
            {
                Expect.Call(clientMock.SessionToken).SetPropertyAndIgnoreArgument();
            }

            Expect.Call(clientMock.GetProjectStatus()).Return(statuses);
            return clientMock;
        }

        private ICruiseServerClientFactory InitialiseFactoryMock(params CruiseServerClientBase[] clientMocks)
        {
            var factoryMock = this.mocks.StrictMock<ICruiseServerClientFactory>();
            foreach (var clientMock in clientMocks)
            {
                Expect.Call(factoryMock.GenerateClient("tcp://localhost:21234/CruiseManager.rem"))
                    .Return(clientMock);
            }

            return factoryMock;
        }
    }
}
