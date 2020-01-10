using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote.Monitor;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Monitor
{
    [TestFixture]
    public class ServerTests
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
                var monitor = new Server(string.Empty);
                Assert.Fail("ArgumentNullException was expected");
            }
            catch (ArgumentNullException) { }
        }

        [Test]
        public void ConstructorDoesNotAllowNullWatcher()
        {
            var client = mocks.Create<CruiseServerClientBase>().Object;
            try
            {
                var monitor = new Server(client, (IServerWatcher)null);
                Assert.Fail("ArgumentNullException was expected");
            }
            catch (ArgumentNullException) { }
        }
        #endregion

        #region Refresh() tests
        [Test]
        public void RefreshCallsRefreshOnWatcher()
        {
            var watcher = new TestWatcher();
            var monitor = InitialiseServer(watcher);
            monitor.Refresh();
            Assert.IsTrue(watcher.Refreshed);
        }

        [Test]
        public void RefreshFiresProjectAddedWhenANewProjectIsFound()
        {
            var watcher = new TestWatcher();
            var monitor = InitialiseServer(watcher);
            var hasFired = false;
            monitor.ProjectAdded += (o, e) =>
            {
                hasFired = true;
            };
            monitor.Refresh();
            watcher.Snapshot.ProjectStatuses = new ProjectStatus[]{
                new ProjectStatus {
                    Name = "Project1"
                },
                new ProjectStatus{
                    Name = "Project2"
                },
                new ProjectStatus{
                    Name = "Project3"
                }
            };
            monitor.Refresh();
            Assert.IsTrue(hasFired);
        }

        [Test]
        public void RefreshFiresProjectRemovedWhenAProjectIsMissing()
        {
            var watcher = new TestWatcher();
            var monitor = InitialiseServer(watcher);
            var hasFired = false;
            monitor.ProjectRemoved += (o, e) =>
            {
                hasFired = true;
                Assert.AreEqual(e.Project.Name, "Project2");
            };
            monitor.Refresh();
            watcher.Snapshot.ProjectStatuses = new ProjectStatus[]{
                new ProjectStatus {
                    Name = "Project1"
                }
            };
            monitor.Refresh();
            Assert.IsTrue(hasFired);
        }

        [Test]
        public void RefreshUpdatesProjectDetails()
        {
            var watcher = new TestWatcher();
            var monitor = InitialiseServer(watcher);
            monitor.Refresh();
            watcher.Snapshot.ProjectStatuses = new ProjectStatus[]{
                new ProjectStatus {
                    Name = "Project1",
                    BuildStatus = IntegrationStatus.Success
                },
                new ProjectStatus {
                    Name = "Project2",
                    BuildStatus = IntegrationStatus.Failure
                }
            };
            monitor.Refresh();
            Assert.AreEqual(IntegrationStatus.Success, monitor.FindProject("Project1").BuildStatus);
            Assert.AreEqual(IntegrationStatus.Failure, monitor.FindProject("Project2").BuildStatus);
        }
        #endregion
        #endregion

        #region Helper methods
        private Server InitialiseServer()
        {
            var client = mocks.Create<CruiseServerClientBase>().Object;
            var monitor = new Server(client, new TestWatcher());
            return monitor;
        }

        private Server InitialiseServer(IServerWatcher watcher)
        {
            var client = new CruiseServerClientMock();
            var monitor = new Server(client, watcher);
            return monitor;
        }
        #endregion

        #region Private classes
        #region TestWatcher
        private class TestWatcher
            : IServerWatcher
        {
            public TestWatcher()
            {
                Snapshot = new CruiseServerSnapshot
                {
                    ProjectStatuses = new ProjectStatus[] {
                            new ProjectStatus {
                                Name = "Project1",
                                BuildStatus = IntegrationStatus.Success
                            },
                            new ProjectStatus{
                                Name = "Project2",
                                BuildStatus = IntegrationStatus.Success
                            }
                        },
                    QueueSetSnapshot = new QueueSetSnapshot
                    {
                        Queues = {
                                new QueueSnapshot {
                                    QueueName = "Queue1"
                                }
                            }
                    }
                };
            }

            public CruiseServerSnapshot Snapshot { get; set; }
            public bool Refreshed { get; private set; }

            public void Refresh()
            {
                Refreshed = true;
                if (Update != null)
                {
                    Update(this, new ServerUpdateArgs(Snapshot));
                }
            }

            public event EventHandler<ServerUpdateArgs> Update;
        }
        #endregion
        #endregion
    }
}
