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
    public class ProjectTests
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
                var project = new Project(null, null, null);
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
                var project = new Project(client, null, null);
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
                var project = new Project(client, server, null);
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
            var status = new ProjectStatus();
            var project = new Project(client, server, status);
            mocks.ReplayAll();
            Assert.AreSame(server, project.Server);
        }
        #endregion

        #region Name tests
        [Test]
        public void NameReturnsNameFromStatus()
        {
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var status = new ProjectStatus { Name = "Test Project" };
            var project = new Project(client, server, status);
            mocks.ReplayAll();
            Assert.AreEqual(status.Name, project.Name);
        }
        #endregion

        #region BuildStage tests
        [Test]
        public void BuildStageReturnsBuildStageFromStatus()
        {
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var status = new ProjectStatus { BuildStage = "Old" };
            var project = new Project(client, server, status);
            mocks.ReplayAll();
            Assert.AreEqual(status.BuildStage, project.BuildStage);
        }
        #endregion

        #region BuildStatus tests
        [Test]
        public void BuildStatusReturnsBuildStatusFromStatus()
        {
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var status = new ProjectStatus { BuildStatus = IntegrationStatus.Exception };
            var project = new Project(client, server, status);
            mocks.ReplayAll();
            Assert.AreEqual(status.BuildStatus, project.BuildStatus);
        }
        #endregion

        #region Status tests
        [Test]
        public void StatusReturnsStatusFromStatus()
        {
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var status = new ProjectStatus { Status = ProjectIntegratorState.Stopping };
            var project = new Project(client, server, status);
            mocks.ReplayAll();
            Assert.AreEqual(status.Status, project.Status);
        }
        #endregion

        #region Activity tests
        [Test]
        public void ActivityReturnsActivityFromStatus()
        {
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var status = new ProjectStatus { Activity = ProjectActivity.CheckingModifications };
            var project = new Project(client, server, status);
            mocks.ReplayAll();
            Assert.AreEqual(status.Activity, project.Activity);
        }
        #endregion

        #region Description tests
        [Test]
        public void DescriptionReturnsDescriptionFromStatus()
        {
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var status = new ProjectStatus { Description = "Description" };
            var project = new Project(client, server, status);
            mocks.ReplayAll();
            Assert.AreEqual(status.Description, project.Description);
        }
        #endregion

        #region Category tests
        [Test]
        public void CategoryReturnsCategoryFromStatus()
        {
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var status = new ProjectStatus { Category = "Category" };
            var project = new Project(client, server, status);
            mocks.ReplayAll();
            Assert.AreEqual(status.Category, project.Category);
        }
        #endregion

        #region Queue tests
        [Test]
        public void QueueReturnsQueueFromStatus()
        {
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var status = new ProjectStatus { Queue = "Queue Name" };
            var project = new Project(client, server, status);
            mocks.ReplayAll();
            Assert.AreEqual(status.Queue, project.Queue);
        }
        #endregion

        #region QueuePriority tests
        [Test]
        public void QueuePriorityReturnsQueuePriorityFromStatus()
        {
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var status = new ProjectStatus { QueuePriority = 7 };
            var project = new Project(client, server, status);
            mocks.ReplayAll();
            Assert.AreEqual(status.QueuePriority, project.QueuePriority);
        }
        #endregion

        #region WebURL tests
        [Test]
        public void WebURLReturnsWebURLFromStatus()
        {
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var status = new ProjectStatus { WebURL = "http://somewhere" };
            var project = new Project(client, server, status);
            mocks.ReplayAll();
            Assert.AreEqual(status.WebURL, project.WebURL);
        }
        #endregion

        #region LastBuildDate tests
        [Test]
        public void LastBuildDateReturnsLastBuildDateFromStatus()
        {
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var status = new ProjectStatus { LastBuildDate = new DateTime(2009, 1, 1) };
            var project = new Project(client, server, status);
            mocks.ReplayAll();
            Assert.AreEqual(status.LastBuildDate, project.LastBuildDate);
        }
        #endregion

        #region LastBuildLabel tests
        [Test]
        public void LastBuildLabelReturnsLastBuildLabelFromStatus()
        {
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var status = new ProjectStatus { LastBuildLabel = "Last label" };
            var project = new Project(client, server, status);
            mocks.ReplayAll();
            Assert.AreEqual(status.LastBuildLabel, project.LastBuildLabel);
        }
        #endregion

        #region LastSuccessfulBuildLabel tests
        [Test]
        public void LastSuccessfulBuildLabelReturnsLastSuccessfulBuildLabelFromStatus()
        {
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var status = new ProjectStatus { LastSuccessfulBuildLabel = "Last success label" };
            var project = new Project(client, server, status);
            mocks.ReplayAll();
            Assert.AreEqual(status.LastSuccessfulBuildLabel, project.LastSuccessfulBuildLabel);
        }
        #endregion

        #region NextBuildTime tests
        [Test]
        public void NextBuildTimeReturnsNextBuildTimeFromStatus()
        {
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var status = new ProjectStatus { NextBuildTime = new DateTime(2009, 1, 2) };
            var project = new Project(client, server, status);
            mocks.ReplayAll();
            Assert.AreEqual(status.NextBuildTime, project.NextBuildTime);
        }
        #endregion

        #region Messages tests
        [Test]
        public void MessagesReturnsMessagesFromStatus()
        {
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var messages = new Message[] {
                new Message { Text = "Testing"}
            };
            var status = new ProjectStatus { Messages = messages };
            var project = new Project(client, server, status);
            mocks.ReplayAll();
            Assert.AreSame(status.Messages, project.Messages);
        }
        #endregion

        #region Update() tests
        [Test]
        public void UpdateValidatesArguments()
        {
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var status = new ProjectStatus { BuildStatus = IntegrationStatus.Exception };
            var project = new Project(client, server, status);
            mocks.ReplayAll();
            try
            {
                project.Update(null);
                Assert.Fail("ArgumentNullException was expected");
            }
            catch (ArgumentNullException) { }
        }

        [Test]
        public void UpdateChangesUnderlyingStatus()
        {
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var status = new ProjectStatus { BuildStatus = IntegrationStatus.Exception };
            var project = new Project(client, server, status);
            mocks.ReplayAll();

            var newStatus = new ProjectStatus { BuildStatus = IntegrationStatus.Failure };
            project.Update(newStatus);
            Assert.AreEqual(newStatus.BuildStatus, project.BuildStatus);
        }

        [Test]
        public void UpdateFiresPropertyChangedWhenPropertyHasChanged()
        {
            RunPropertyChangedTest("BuildStage", "Stage 1", "Stage 2");
            RunPropertyChangedTest("BuildStatus", IntegrationStatus.Exception, IntegrationStatus.Failure);
            RunPropertyChangedTest("Status", ProjectIntegratorState.Running, ProjectIntegratorState.Stopped);
            RunPropertyChangedTest("Activity", ProjectActivity.Building, ProjectActivity.Pending);
            RunPropertyChangedTest("Description", "Old", "New");
            RunPropertyChangedTest("Category", "Old", "New");
            RunPropertyChangedTest("Queue", "Old", "New");
            RunPropertyChangedTest("QueuePriority", 1, 2);
            RunPropertyChangedTest("WebURL", "Old", "New");
            RunPropertyChangedTest("LastBuildDate", new DateTime(2009, 1, 1), new DateTime(2009, 1, 2));
            RunPropertyChangedTest("LastBuildLabel", "Old", "New");
            RunPropertyChangedTest("LastSuccessfulBuildLabel", "Old", "New");
            RunPropertyChangedTest("NextBuildTime", new DateTime(2009, 1, 1), new DateTime(2009, 1, 2));
        }

        [Test]
        public void UpdateFiresPropertyChangedWhenMessageIsAdded()
        {
            mocks = new MockRepository();
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var status = new ProjectStatus
                {
                    Messages = new Message[] {
                    new Message{Text = "Message 1"}
                }
            };
            var project = new Project(client, server, status);
            mocks.ReplayAll();
            var eventFired = false;

            var newStatus = new ProjectStatus
            {
                Messages = new Message[] {
                    new Message{Text = "Message 1"},
                    new Message{Text = "Message 2"}
                }
            };
            project.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == "Messages") eventFired = true;
            };
            project.Update(newStatus);
            Assert.IsTrue(eventFired, "PropertyChanged for Messages change not fired");
        }

        [Test]
        public void UpdateFiresPropertyChangedWhenMessageIsRemoved()
        {
            mocks = new MockRepository();
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var status = new ProjectStatus
            {
                Messages = new Message[] {
                    new Message{Text = "Message 1"},
                    new Message{Text = "Message 2"}
                }
            };
            var project = new Project(client, server, status);
            mocks.ReplayAll();
            var eventFired = false;

            var newStatus = new ProjectStatus
            {
                Messages = new Message[] {
                    new Message{Text = "Message 2"}
                }
            };
            project.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == "Messages") eventFired = true;
            };
            project.Update(newStatus);
            Assert.IsTrue(eventFired, "PropertyChanged for Messages change not fired");
        }

        [Test]
        public void UpdateFiresPropertyChangedWhenMessageIsChanged()
        {
            mocks = new MockRepository();
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var status = new ProjectStatus
            {
                Messages = new Message[] {
                    new Message{Text = "Message 1"}
                }
            };
            var project = new Project(client, server, status);
            mocks.ReplayAll();
            var eventFired = false;

            var newStatus = new ProjectStatus
            {
                Messages = new Message[] {
                    new Message{Text = "Message 2"}
                }
            };
            project.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == "Messages") eventFired = true;
            };
            project.Update(newStatus);
            Assert.IsTrue(eventFired, "PropertyChanged for Messages change not fired");
        }
        #endregion

        #region ForceBuild() tests
        [Test]
        public void ForceBuildSendsRequestToClient()
        {
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var status = new ProjectStatus { Name = "Test Project" };
            var project = new Project(client, server, status);
            Expect.Call(() => client.ForceBuild("Test Project"));
            mocks.ReplayAll();
            project.ForceBuild();
            mocks.VerifyAll();
        }

        [Test]
        public void ForceBuildWithParametersSendsRequestToClient()
        {
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var status = new ProjectStatus { Name = "Test Project" };
            var project = new Project(client, server, status);
            var parameters = new List<NameValuePair>();
            Expect.Call(() => client.ForceBuild("Test Project", parameters));
            mocks.ReplayAll();
            project.ForceBuild(parameters);
            mocks.VerifyAll();
        }
        #endregion

        #region AbortBuild() tests
        [Test]
        public void AbortBuildSendsRequestToClient()
        {
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var status = new ProjectStatus { Name = "Test Project" };
            var project = new Project(client, server, status);
            Expect.Call(() => client.AbortBuild("Test Project"));
            mocks.ReplayAll();
            project.AbortBuild();
            mocks.VerifyAll();
        }
        #endregion

        #region Start() tests
        [Test]
        public void StartSendsRequestToClient()
        {
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var status = new ProjectStatus { Name = "Test Project" };
            var project = new Project(client, server, status);
            Expect.Call(() => client.StartProject("Test Project"));
            mocks.ReplayAll();
            project.Start();
            mocks.VerifyAll();
        }
        #endregion

        #region Stop() tests
        [Test]
        public void StopSendsRequestToClient()
        {
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var status = new ProjectStatus { Name = "Test Project" };
            var project = new Project(client, server, status);
            Expect.Call(() => client.StopProject("Test Project"));
            mocks.ReplayAll();
            project.Stop();
            mocks.VerifyAll();
        }
        #endregion
        #endregion

        #region Helper methods
        private void RunPropertyChangedTest(string property, object originalValue, object newValue)
        {
            mocks = new MockRepository();
            var client = mocks.DynamicMock<CruiseServerClientBase>();
            var server = InitialiseServer();
            var statusType = typeof(ProjectStatus);
            var propertyMember = statusType.GetProperty(property);

            var status = new ProjectStatus();
            propertyMember.SetValue(status, originalValue, new object[0]);
            var project = new Project(client, server, status);
            mocks.ReplayAll();
            var eventFired = false;

            var newStatus = new ProjectStatus();
            propertyMember.SetValue(newStatus, newValue, new object[0]);
            project.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == property) eventFired = true;
            };
            project.Update(newStatus);
            Assert.IsTrue(eventFired, "PropertyChanged for " + property + " change not fired");
        }

        private Server InitialiseServer()
        {
            var watcher = mocks.Stub<IServerWatcher>();
            var client = new CruiseServerClientMock();
            var monitor = new Server(client, watcher);
            return monitor;
        }
        #endregion
    }
}
