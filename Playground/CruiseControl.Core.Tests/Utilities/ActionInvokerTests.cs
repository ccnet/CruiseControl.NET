namespace CruiseControl.Core.Tests.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using CruiseControl.Common;
    using CruiseControl.Core.Utilities;
    using NUnit.Framework;
    using Messages = CruiseControl.Common.Messages;

    [TestFixture]
    public class ActionInvokerTests
    {
        #region Tests
        [Test]
        public void PingReturnsTrue()
        {
            var channel = new ActionInvoker(null);
            Assert.IsTrue(channel.Ping());
        }

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
                                   new RemoteActionDefinition
                                       {
                                           Name = "DoSomething",
                                           Description = "This will do something",
                                           InputData = "<definition name=\"Blank\" namespace=\"urn:cruisecontrol:common\" />",
                                           OutputData = "<definition name=\"Blank\" namespace=\"urn:cruisecontrol:common\" />"
                                       },
                                   new RemoteActionDefinition
                                       {
                                           Name = "TestAction",
                                           Description = "This is a test action",
                                           InputData = "<definition name=\"SingleValue\" namespace=\"urn:cruisecontrol:common\">" +
                                                    "<value name=\"Value\" type=\"string\" />" +
                                                "</definition>",
                                           OutputData = "<definition name=\"Blank\" namespace=\"urn:cruisecontrol:common\" />"
                                       }
                               };
            CollectionAssert.AreEqual(expected, result.Actions.OrderBy(rad => rad.Name), new DefinitionComparer());
        }

        [Test]
        public void QueryFiltersActions()
        {
            var testItem = new TestItem("Baby");
            var server = new Server("Test", testItem);
            var invoker = new ActionInvoker(server);
            var args = new QueryArguments { FilterPattern = "DoSomething", DataToInclude = DataDefinitions.None };
            var result = invoker.Query("urn:ccnet:test:baby", args);
            Assert.IsNotNull(result);
            Assert.AreEqual(RemoteResultCode.Success, result.ResultCode);
            var expected = new[]
                               {
                                   new RemoteActionDefinition
                                       {
                                           Name = "DoSomething",
                                           Description = "This will do something"
                                       }
                               };
            CollectionAssert.AreEqual(expected, result.Actions, new DefinitionComparer());
        }

        [Test]
        public void QueryRetrievesInputDefinitions()
        {
            var testItem = new TestItem("Baby");
            var server = new Server("Test", testItem);
            var invoker = new ActionInvoker(server);
            var result = invoker.Query(
                "urn:ccnet:test:baby",
                new QueryArguments {DataToInclude = DataDefinitions.InputOnly});
            Assert.IsNotNull(result);
            Assert.AreEqual(RemoteResultCode.Success, result.ResultCode);
            var expected = new[]
                               {
                                   new RemoteActionDefinition
                                       {
                                           Name = "DoSomething", 
                                           Description = "This will do something",
                                           InputData = "<definition name=\"Blank\" namespace=\"urn:cruisecontrol:common\" />"
                                       },
                                   new RemoteActionDefinition
                                       {
                                           Name = "TestAction",
                                           Description = "This is a test action",
                                           InputData = "<definition name=\"SingleValue\" namespace=\"urn:cruisecontrol:common\">" +
                                                    "<value name=\"Value\" type=\"string\" />" +
                                                "</definition>"
                                       }
                               };
            CollectionAssert.AreEqual(expected, result.Actions.OrderBy(rad => rad.Name), new DefinitionComparer());
        }

        [Test]
        public void QueryRetrievesOutputDefinitions()
        {
            var testItem = new TestItem("Baby");
            var server = new Server("Test", testItem);
            var invoker = new ActionInvoker(server);
            var result = invoker.Query(
                "urn:ccnet:test:baby",
                new QueryArguments { DataToInclude = DataDefinitions.OutputOnly });
            Assert.IsNotNull(result);
            Assert.AreEqual(RemoteResultCode.Success, result.ResultCode);
            var expected = new[]
                               {
                                   new RemoteActionDefinition
                                       {
                                           Name = "DoSomething",
                                           Description = "This will do something",
                                           OutputData = "<definition name=\"Blank\" namespace=\"urn:cruisecontrol:common\" />"
                                       },
                                   new RemoteActionDefinition
                                       {
                                           Name = "TestAction",
                                           Description = "This is a test action",
                                           OutputData = "<definition name=\"Blank\" namespace=\"urn:cruisecontrol:common\" />"
                                       }
                               };
            CollectionAssert.AreEqual(expected, result.Actions.OrderBy(rad => rad.Name), new DefinitionComparer());
        }

        [Test]
        public void RetrieveServerNameRetrievesUrnFromServer()
        {
            var server = new Server("testserver");
            var invoker = new ActionInvoker(server);
            var expected = server.UniversalName;
            var actual = invoker.RetrieveServerName();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GenerateMessageFormatAddsNewNamespace()
        {
            var namespaces = new Dictionary<string, string>();
            ActionInvoker.GenerateMessageFormat(namespaces, typeof (Messages.Blank));
            Assert.IsTrue(namespaces.ContainsKey("CruiseControl.Common.Messages"));
            Assert.AreEqual("urn:cruisecontrol:common", namespaces["CruiseControl.Common.Messages"]);
        }

        [Test]
        public void GenerateMessageIgnoresExistingNamespace()
        {
            var namespaces = new Dictionary<string, string>();
            var ns = "anotherUrn";
            namespaces.Add("CruiseControl.Common.Messages", ns);
            ActionInvoker.GenerateMessageFormat(namespaces, typeof(Messages.Blank));
            Assert.IsTrue(namespaces.ContainsKey("CruiseControl.Common.Messages"));
            Assert.AreEqual(ns, namespaces["CruiseControl.Common.Messages"]);
        }

        [Test]
        public void GenerateMessageFormatHandlesMissingXmlnsDefinition()
        {
            var namespaces = new Dictionary<string, string>();
            ActionInvoker.GenerateMessageFormat(namespaces, typeof(FakeMessage));
            Assert.IsTrue(namespaces.ContainsKey("CruiseControl.Core.Tests.Utilities"));
            Assert.AreEqual(
                "clr-namespace:CruiseControl.Core.Tests.Utilities;assembly=CruiseControl.Core.Tests", 
                namespaces["CruiseControl.Core.Tests.Utilities"]);
        }

        [Test]
        public void GenerateMessageFormatHandlesMessageWithoutProperties()
        {
            var namespaces = new Dictionary<string, string>();
            var actual = ActionInvoker.GenerateMessageFormat(namespaces, typeof(Messages.Blank));
            var expected = "<definition name=\"Blank\" namespace=\"urn:cruisecontrol:common\" />";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GenerateMessageFormatHandlesMessageWithProperties()
        {
            var namespaces = new Dictionary<string, string>();
            var actual = ActionInvoker.GenerateMessageFormat(namespaces, typeof(Messages.SingleValue));
            var expected = "<definition name=\"SingleValue\" namespace=\"urn:cruisecontrol:common\">" +
                            "<value name=\"Value\" type=\"string\" />" +
                           "</definition>";
            Assert.AreEqual(expected, actual);
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
            public Messages.Blank TestAction(Messages.SingleValue message)
            {
                this.WasInvoked = true;
                return new Messages.Blank();
            }

            [RemoteAction]
            [System.ComponentModel.Description("This will do something")]
            public Messages.Blank DoSomething(Messages.Blank message)
            {
                this.WasInvoked = true;
                return new Messages.Blank();
            }
        }

        private class FakeMessage
        {
            
        }
        #endregion

        #region Helper classes
        public class DefinitionComparer
            : IComparer
        {
            public int Compare(object x, object y)
            {
                var xDefinition = x as RemoteActionDefinition;
                var yDefinition = y as RemoteActionDefinition;
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
