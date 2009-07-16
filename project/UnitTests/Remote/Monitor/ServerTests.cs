using System;
using System.Collections.Generic;
using System.Text;
using Rhino.Mocks;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote.Monitor;
using ThoughtWorks.CruiseControl.Remote;
using System.Diagnostics;

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
                var monitor = new Server(null);
                Assert.Fail("ArgumentNullException was expected");
            }
            catch (ArgumentNullException) { }
        }

        [Test]
        public void ConstructorDoesNotAllowNullWatcher()
        {
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            mocks.ReplayAll();
            try
            {
                var monitor = new Server(client, null);
                Assert.Fail("ArgumentNullException was expected");
            }
            catch (ArgumentNullException) { }
        }
        #endregion

        #region ProjectCount tests
        [Test]
        public void ProjectCountReturnsZeroForNoProjects()
        {
            var monitor = InitialiseServer();
            mocks.ReplayAll();
            Assert.AreEqual(0, monitor.ProjectCount);
        }

        [Test]
        public void ProjectCountReturnsTheNumberOfLoadedProjects()
        {
            var monitor = InitialiseServer();
            mocks.ReplayAll();
            monitor.Refresh();
            Assert.AreEqual(2, monitor.ProjectCount);
        }
        #endregion

        #region Refresh() tests
        [Test]
        public void RefreshCallsRefreshOnWatcher()
        {
            var watcher = new TestWatcher();
            var monitor = InitialiseServer(watcher);
            mocks.ReplayAll();
            monitor.Refresh();
            Assert.IsTrue(watcher.Refreshed);
        }

        [Test]
        public void RefreshFiresProjectAddedWhenANewProjectIsFound()
        {
            var watcher = new TestWatcher();
            var monitor = InitialiseServer(watcher);
            mocks.ReplayAll();
            var hasFired = false;
            monitor.ProjectAdded += (o, e) =>
            {
                hasFired = true;
                Assert.AreEqual(e.Project.Name, "Project3");
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
            mocks.ReplayAll();
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
            mocks.ReplayAll();
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
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var monitor = new Server(client, new TestWatcher());
            return monitor;
        }

        private Server InitialiseServer(IServerWatcher watcher)
        {
            var client = mocks.DynamicMock<CruiseServerClientBase>();
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
