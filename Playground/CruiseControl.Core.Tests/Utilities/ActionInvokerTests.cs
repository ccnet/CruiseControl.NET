namespace CruiseControl.Core.Tests.Utilities
{
    using System;
    using System.Collections.Generic;
    using CruiseControl.Common.Messages;
    using CruiseControl.Core.Utilities;
    using NUnit.Framework;

    [TestFixture]
    public class ActionInvokerTests
    {
        #region Tests
        [Test]
        public void InvokeFailsIfTheNameCannotBeFound()
        {
            var server = new Server("Test");
            var invoker = new ActionInvoker(server);
            Assert.Throws<InvalidOperationException>(
                () => invoker.Invoke("urn:ccnet:test:ghost", "DoSomething", null));
        }

        [Test]
        public void InvokeFailsIfInputIsIncorrectType()
        {
            var testItem = new TestItem("Baby");
            var server = new Server("Test", testItem);
            var invoker = new ActionInvoker(server);
            Assert.Throws<InvalidOperationException>(
                () => invoker.Invoke("urn:ccnet:test:baby", "TestAction", new ServerMessage()));
        }

        [Test]
        public void InvokeInvokesMethod()
        {
            var testItem = new TestItem("Baby");
            var server = new Server("Test", testItem);
            var invoker = new ActionInvoker(server);
            var actual = invoker.Invoke("urn:ccnet:test:baby", "TestAction", new ProjectMessage());
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<ServerMessage>(actual);
            Assert.AreEqual("SetAfterCalled", (actual as ServerMessage).ServerName);
            Assert.IsTrue(testItem.WasInvoked);
        }

        [Test]
        public void InvokeFailsIfTheActionCannotBeFound()
        {
            var testItem = new TestItem("Baby");
            var server = new Server("Test", testItem);
            var invoker = new ActionInvoker(server);
            Assert.Throws<InvalidOperationException>(
                () => invoker.Invoke("urn:ccnet:test:baby", "DoSomething", null));
        }

        [Test]
        public void ListFailsIfTheNameCannotBeFound()
        {
            var server = new Server("Test");
            var invoker = new ActionInvoker(server);
            Assert.Throws<InvalidOperationException>(
                () => invoker.List("urn:ccnet:test:ghost"));
        }

        [Test]
        public void ListReturnsActionsAndDescriptions()
        {
            var testItem = new TestItem("Baby");
            var server = new Server("Test", testItem);
            var invoker = new ActionInvoker(server);
            var actions = invoker.List("urn:ccnet:test:baby");
            Assert.AreEqual(1, actions.Length);
            Assert.AreEqual("TestAction", actions[0].Name);
            Assert.AreEqual("This is a test action", actions[0].Description);
        }

        [Test]
        public void QueryFailsIfTheNameCannotBeFound()
        {
            var server = new Server("Test");
            var invoker = new ActionInvoker(server);
            Assert.Throws<InvalidOperationException>(
                () => invoker.Query("urn:ccnet:test:ghost", "DoSomething"));
        }

        [Test]
        public void QueryFailsIfTheActionCannotBeFound()
        {
            var testItem = new TestItem("Baby");
            var server = new Server("Test", testItem);
            var invoker = new ActionInvoker(server);
            Assert.Throws<InvalidOperationException>(
                () => invoker.Query("urn:ccnet:test:baby", "DoSomething"));
        }

        [Test]
        public void QueryReturnsActionDetails()
        {
            var testItem = new TestItem("Baby");
            var server = new Server("Test", testItem);
            var invoker = new ActionInvoker(server);
            var action = invoker.Query("urn:ccnet:test:baby", "TestAction");
            Assert.AreEqual("TestAction", action.Name);
            Assert.AreEqual("This is a test action", action.Description);
            Assert.AreEqual("CruiseControl.Common.Messages.ProjectMessage", action.InputMessage);
            Assert.AreEqual("CruiseControl.Common.Messages.ServerMessage", action.OutputMessage);
        }
        #endregion

        #region Private classes
        private class TestItem
            : ServerItem
        {
            public TestItem(string name)
                : base(name)
            {
            }

            public bool WasInvoked { get; private set; }

            public override void AskToIntegrate(IntegrationContext context)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<Project> ListProjects()
            {
                return new Project[0];
            }

            [RemoteAction]
            [System.ComponentModel.Description("This is a test action")]
            public ServerMessage TestAction(ProjectMessage message)
            {
                this.WasInvoked = true;
                return new ServerMessage
                           {
                               ServerName = "SetAfterCalled"
                           };
            }
        }
        #endregion
    }
}
