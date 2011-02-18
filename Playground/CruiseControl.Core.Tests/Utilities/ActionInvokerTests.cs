namespace CruiseControl.Core.Tests.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using CruiseControl.Common;
    using CruiseControl.Core.Utilities;
    using NUnit.Framework;
    using Messages = CruiseControl.Common.Messages;

    [TestFixture]
    public class ActionInvokerTests
    {
        #region Tests
        [Test]
        public void InvokeFailsIfTheNameCannotBeFound()
        {
            var server = new Server("Test");
            var invoker = new ActionInvoker(server);
            var result = invoker.Invoke("urn:ccnet:test:ghost", new InvokeArguments());
            Assert.IsNotNull(result);
            Assert.AreEqual(RemoteResultCode.UnknownUrn, result.ResultCode);
        }

        [Test]
        public void InvokeFailsIfInputIsIncorrectType()
        {
            var testItem = new TestItem("Baby");
            var server = new Server("Test", testItem);
            var invoker = new ActionInvoker(server);
            var arguments = new InvokeArguments
                                {
                                    Action = "TestAction",
                                    Data = "<Blank xmlns=\"urn:cruisecontrol:common\" />"
                                };
            var result = invoker.Invoke("urn:ccnet:test:baby", arguments);
            Assert.IsNotNull(result);
            Assert.AreEqual(RemoteResultCode.InvalidInput, result.ResultCode);
        }

        [Test]
        public void InvokeFailsIfInputIsMissing()
        {
            var testItem = new TestItem("Baby");
            var server = new Server("Test", testItem);
            var invoker = new ActionInvoker(server);
            var arguments = new InvokeArguments
                                {
                                    Action = "TestAction",
                                    Data = string.Empty
                                };
            var result = invoker.Invoke("urn:ccnet:test:baby", arguments);
            Assert.IsNotNull(result);
            Assert.AreEqual(RemoteResultCode.InvalidInput, result.ResultCode);
        }

        [Test]
        public void InvokeInvokesMethod()
        {
            var testItem = new TestItem("Baby");
            var server = new Server("Test", testItem);
            var invoker = new ActionInvoker(server);
            var arguments = new InvokeArguments
                                {
                                    Action = "DoSomething",
                                    Data = "<Blank xmlns=\"urn:cruisecontrol:common\" />"
                                };
            var result = invoker.Invoke("urn:ccnet:test:baby", arguments);
            Assert.AreEqual(RemoteResultCode.Success, result.ResultCode);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            var message = MessageSerialiser.Deserialise(result.Data);
            Assert.IsInstanceOf<Messages.Blank>(message);
            Assert.IsTrue(testItem.WasInvoked);
        }

        [Test]
        public void InvokeFailsIfTheActionCannotBeFound()
        {
            var testItem = new TestItem("Baby");
            var server = new Server("Test", testItem);
            var invoker = new ActionInvoker(server);
            var result = invoker.Invoke("urn:ccnet:test:baby", new InvokeArguments { Action = "DoesNotExist" });
            Assert.IsNotNull(result);
            Assert.AreEqual(RemoteResultCode.UnknownAction, result.ResultCode);
        }

        [Test]
        public void InvokeFailsIfTheArgumentsAreMissing()
        {
            var testItem = new TestItem("Baby");
            var server = new Server("Test", testItem);
            var invoker = new ActionInvoker(server);
            var result = invoker.Invoke("urn:ccnet:test:baby", null);
            Assert.IsNotNull(result);
            Assert.AreEqual(RemoteResultCode.MissingArguments, result.ResultCode);
        }

        [Test]
        public void QueryFailsIfTheNameCannotBeFound()
        {
            var server = new Server("Test");
            var invoker = new ActionInvoker(server);
            var result = invoker.Query("urn:ccnet:test:ghost", null);
            Assert.IsNotNull(result);
            Assert.AreEqual(RemoteResultCode.UnknownUrn, result.ResultCode);
        }

        [Test]
        public void QueryReturnsActions()
        {
            var testItem = new TestItem("Baby");
            var server = new Server("Test", testItem);
            var invoker = new ActionInvoker(server);
            var result = invoker.Query("urn:ccnet:test:baby", null);
            Assert.IsNotNull(result);
            Assert.AreEqual(RemoteResultCode.Success, result.ResultCode);
            var expected = new[]
                               {
                                   new RemoteActionDefinition {Name = "TestAction", Description = "This is a test action"},
                                   new RemoteActionDefinition {Name = "DoSomething", Description = "This will do something"}
                               };
            CollectionAssert.AreEqual(expected, result.Actions, new DefinitionComparer());
        }

        [Test]
        public void QueryFiltersActions()
        {
            var testItem = new TestItem("Baby");
            var server = new Server("Test", testItem);
            var invoker = new ActionInvoker(server);
            var args = new QueryArguments { FilterPattern = "DoSomething" };
            var result = invoker.Query("urn:ccnet:test:baby", args);
            Assert.IsNotNull(result);
            Assert.AreEqual(RemoteResultCode.Success, result.ResultCode);
            var expected = new[]
                               {
                                   new RemoteActionDefinition {Name = "DoSomething", Description = "This will do something"}
                               };
            CollectionAssert.AreEqual(expected, result.Actions, new DefinitionComparer());
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
            [Description("This is a test action")]
            public Messages.Blank TestAction(Messages.BuildRequest message)
            {
                this.WasInvoked = true;
                return new Messages.Blank();
            }

            [RemoteAction]
            [Description("This will do something")]
            public Messages.Blank DoSomething(Messages.Blank message)
            {
                this.WasInvoked = true;
                return new Messages.Blank();
            }
        }
        #endregion

        #region Helper classes
        public class DefinitionComparer
            : IComparer
        {
            public int Compare(object x, object y)
            {
                var xDefinition = x as RemoteActionDefinition;
                var yDefinition = x as RemoteActionDefinition;
                var areEqual = xDefinition != null &&
                    yDefinition != null &&
                    xDefinition.Description == yDefinition.Description &&
                    xDefinition.InputData == yDefinition.InputData &&
                    xDefinition.Name == yDefinition.Name &&
                    xDefinition.OutputData == yDefinition.OutputData;
                return areEqual ? 0 : 1;
            }
        }
        #endregion
    }
}
