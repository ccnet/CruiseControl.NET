using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote.Monitor;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Monitor
{
    [TestFixture]
    public class BuildQueueTests
    {
        #region Private fields
        private MockRepository mocks;
        #endregion

        #region Setup
        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository(MockBehavior.Default);
        }
        #endregion

        #region Tests
        #region Constructor tests
        [Test]
        public void ConstructorDoesNotAllowNullClient()
        {
            try
            {
                var queue = new BuildQueue(null, null, null);
                Assert.Fail("ArgumentNullException was expected");
            }
            catch (ArgumentNullException) { }
        }

        [Test]
        public void ConstructorDoesNotAllowNullServer()
        {
            var client = mocks.Create<CruiseServerClientBase>().Object;
            try
            {
                var queue = new BuildQueue(client, null, null);
                Assert.Fail("ArgumentNullException was expected");
            }
            catch (ArgumentNullException) { }
        }

        [Test]
        public void ConstructorDoesNotAllowNullStatus()
        {
            var client = mocks.Create<CruiseServerClientBase>().Object;
            var server = InitialiseServer();
            try
            {
                var queue = new BuildQueue(client, server, null);
                Assert.Fail("ArgumentNullException was expected");
            }
            catch (ArgumentNullException) { }
        }
        #endregion

        #region Server tests
        [Test]
        public void ServerReturnsUnderlyingServer()
        {
            var client = mocks.Create<CruiseServerClientBase>().Object;
            var server = InitialiseServer();
            var status = new QueueSnapshot();
            var queue = new BuildQueue(client, server, status);
            Assert.AreSame(server, queue.Server);
        }
        #endregion

        #region Name tests
        [Test]
        public void NameReturnsQueueNameFromSnapshot()
        {
            var client = mocks.Create<CruiseServerClientBase>().Object;
            var server = InitialiseServer();
            var status = new QueueSnapshot { QueueName = "Test BuildQueue" };
            var queue = new BuildQueue(client, server, status);
            Assert.AreEqual(status.QueueName, queue.Name);
        }
        #endregion

        #region Requests tests
        [Test]
        public void RequestsReturnsRequestsFromStatus()
        {
            var client = mocks.Create<CruiseServerClientBase>().Object;
            var server = InitialiseServer();
            var status = new QueueSnapshot
            {
                Requests = {
                        new QueuedRequestSnapshot{
                            ProjectName = "Project 1"
                        }
                    }
            };
            var queue = new BuildQueue(client, server, status);
        }
        #endregion

        #region Update() tests
        [Test]
        public void UpdateValidatesArguments()
        {
            var client = mocks.Create<CruiseServerClientBase>().Object;
            var server = InitialiseServer();
            var status = new QueueSnapshot { QueueName = "Testing" };
            var queue = new BuildQueue(client, server, status);
            try
            {
                queue.Update(null);
                Assert.Fail("ArgumentNullException was expected");
            }
            catch (ArgumentNullException) { }
        }

        [Test]
        public void UpdateFiresPropertyChangedWhenMessageIsAdded()
        {
            mocks = new MockRepository(MockBehavior.Default);
            var client = mocks.Create<CruiseServerClientBase>().Object;
            var server = InitialiseServer();
            var status = new QueueSnapshot
                {
                    Requests = {
                        new QueuedRequestSnapshot{
                            ProjectName = "Project 1"
                        }
                    }
                };
            var queue = new BuildQueue(client, server, status);
            var eventFired = false;

            var newStatus = new QueueSnapshot
            {
                Requests = {
                        new QueuedRequestSnapshot{
                            ProjectName = "Project 1"
                        },
                        new QueuedRequestSnapshot{
                            ProjectName = "Project 2"
                        }
                    }
            };
            queue.BuildQueueRequestAdded += (o, e) =>
            {
                eventFired = true;
            };
            queue.Update(newStatus);
            Assert.IsTrue(eventFired, "BuildQueueRequestAdded for Requests change not fired");
        }

        [Test]
        public void UpdateFiresPropertyChangedWhenMessageIsRemoved()
        {
            mocks = new MockRepository(MockBehavior.Default);
            var client = mocks.Create<CruiseServerClientBase>().Object;
            var server = InitialiseServer();
            var status = new QueueSnapshot
            {
                Requests = {
                        new QueuedRequestSnapshot{
                            ProjectName = "Project 1"
                        },
                        new QueuedRequestSnapshot{
                            ProjectName = "Project 2"
                        }
                    }
            };
            var queue = new BuildQueue(client, server, status);
            var eventFired = false;
            queue.Update(status);

            var newStatus = new QueueSnapshot
            {
                Requests = {
                        new QueuedRequestSnapshot{
                            ProjectName = "Project 2"
                        }
                    }
            };
            queue.BuildQueueRequestRemoved += (o, e) =>
            {
                eventFired = true;
            };
            queue.Update(newStatus);
            Assert.IsTrue(eventFired, "BuildQueueRequestRemoved for Requests change not fired");
        }
        #endregion
        #endregion

        #region Helper methods
        private Server InitialiseServer()
        {
            var watcher = mocks.Create<IServerWatcher>().Object;
            var client = new CruiseServerClientMock();
            var monitor = new Server(client, watcher);
            return monitor;
        }
        #endregion
    }
}
