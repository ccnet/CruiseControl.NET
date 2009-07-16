using System;
using System.Collections.Generic;
using System.Text;
using Rhino.Mocks;
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
            mocks = new MockRepository();
        }
        #endregion

        #region Tests
        #region Constructor tests
        [Test]
        public void ConstructorDoesNotAllowNullClient()
        {
            try
            {
                var project = new BuildQueue(null, null, null);
                Assert.Fail("ArgumentNullException was expected");
            }
            catch (ArgumentNullException) { }
        }

        [Test]
        public void ConstructorDoesNotAllowNullServer()
        {
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            mocks.ReplayAll();
            try
            {
                var project = new BuildQueue(client, null, null);
                Assert.Fail("ArgumentNullException was expected");
            }
            catch (ArgumentNullException) { }
        }

        [Test]
        public void ConstructorDoesNotAllowNullStatus()
        {
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            mocks.ReplayAll();
            try
            {
                var project = new BuildQueue(client, server, null);
                Assert.Fail("ArgumentNullException was expected");
            }
            catch (ArgumentNullException) { }
        }
        #endregion

        #region Server tests
        [Test]
        public void ServerReturnsUnderlyingServer()
        {
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var status = new QueueSnapshot();
            var project = new BuildQueue(client, server, status);
            mocks.ReplayAll();
            Assert.AreSame(server, project.Server);
        }
        #endregion

        #region Name tests
        [Test]
        public void NameReturnsQueueNameFromSnapshot()
        {
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var status = new QueueSnapshot { QueueName = "Test BuildQueue" };
            var project = new BuildQueue(client, server, status);
            mocks.ReplayAll();
            Assert.AreEqual(status.QueueName, project.Name);
        }
        #endregion

        #region Requests tests
        [Test]
        public void RequestsReturnsRequestsFromStatus()
        {
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var status = new QueueSnapshot
            {
                Requests = {
                        new QueuedRequestSnapshot{
                            ProjectName = "Project 1"
                        }
                    }
            };
            var project = new BuildQueue(client, server, status);
            mocks.ReplayAll();
            Assert.AreEqual(status.Requests.ToArray(), project.Requests);
        }
        #endregion

        #region Update() tests
        [Test]
        public void UpdateValidatesArguments()
        {
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var status = new QueueSnapshot { QueueName = "Testing" };
            var project = new BuildQueue(client, server, status);
            mocks.ReplayAll();
            try
            {
                project.Update(null);
                Assert.Fail("ArgumentNullException was expected");
            }
            catch (ArgumentNullException) { }
        }

        [Test]
        public void UpdateFiresPropertyChangedWhenMessageIsAdded()
        {
            mocks = new MockRepository();
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var status = new QueueSnapshot
                {
                    Requests = {
                        new QueuedRequestSnapshot{
                            ProjectName = "Project 1"
                        }
                    }
                };
            var project = new BuildQueue(client, server, status);
            mocks.ReplayAll();
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
            project.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == "Requests") eventFired = true;
            };
            project.Update(newStatus);
            Assert.IsTrue(eventFired, "PropertyChanged for Requests change not fired");
        }

        [Test]
        public void UpdateFiresPropertyChangedWhenMessageIsRemoved()
        {
            mocks = new MockRepository();
            var client = mocks.DynamicMock<CruiseServerClientBase>();
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
            var project = new BuildQueue(client, server, status);
            mocks.ReplayAll();
            var eventFired = false;

            var newStatus = new QueueSnapshot
            {
                Requests = {
                        new QueuedRequestSnapshot{
                            ProjectName = "Project 2"
                        }
                    }
            };
            project.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == "Requests") eventFired = true;
            };
            project.Update(newStatus);
            Assert.IsTrue(eventFired, "PropertyChanged for Requests change not fired");
        }

        [Test]
        public void UpdateFiresPropertyChangedWhenMessageIsChanged()
        {
            mocks = new MockRepository();
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var status = new QueueSnapshot
            {
                Requests = {
                        new QueuedRequestSnapshot{
                            ProjectName = "Project 1"
                        }
                    }
            };
            var project = new BuildQueue(client, server, status);
            mocks.ReplayAll();
            var eventFired = false;

            var newStatus = new QueueSnapshot
            {
                Requests = {
                        new QueuedRequestSnapshot{
                            ProjectName = "Project 2"
                        }
                    }
            };
            project.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == "Requests") eventFired = true;
            };
            project.Update(newStatus);
            Assert.IsTrue(eventFired, "PropertyChanged for Requests change not fired");
        }
        #endregion
        #endregion

        #region Helper methods
        private Server InitialiseServer()
        {
            var watcher = mocks.Stub<IServerWatcher>();
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var monitor = new Server(client, watcher);
            return monitor;
        }
        #endregion
    }
}
