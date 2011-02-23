namespace CruiseControl.Core.Tests
{
    using System;
    using CruiseControl.Common;
    using CruiseControl.Core.Channels;
    using CruiseControl.Core.Interfaces;
    using Moq;
    using NUnit.Framework;
    using Messages = CruiseControl.Common.Messages;

    [TestFixture(Description = "These are general tests to check that the communications API works")]
    public class CommunicationsTests
    {
        #region Tests
        [Test]
        public void RetrieveServerNameOverWcf()
        {
            var serverName = "urn:ccnet:server";
            var invokerMock = new Mock<IActionInvoker>(MockBehavior.Strict);
            invokerMock.Setup(ai => ai.RetrieveServerName()).Returns(serverName);
            var result = this.RunTest(invokerMock.Object, c => c.RetrieveServerName());
            Assert.AreEqual(serverName, result);
        }

        [Test]
        public void InvokeHandlesSuccess()
        {
            var urn = "urn:ccnet:local";
            var action = "DoSomething";
            var args = new Messages.Blank();
            var returnValue = new InvokeResult
                                  {
                                      ResultCode = RemoteResultCode.Success,
                                      Data = MessageSerialiser.Serialise(new Messages.Blank())
                                  };
            var invokerMock = new Mock<IActionInvoker>(MockBehavior.Strict);
            invokerMock.Setup(ai => ai.Invoke(urn, It.IsAny<InvokeArguments>()))
                .Callback((string u, InvokeArguments a) =>
                              {
                                  Assert.AreEqual(action, a.Action);
                                  Assert.AreEqual(returnValue.Data, a.Data);
                              })
                .Returns(returnValue);
            var result = this.RunTest(invokerMock.Object, c => c.Invoke(urn, action, args));
            Assert.IsInstanceOf<Messages.Blank>(result);
        }

        [Test]
        public void InvokeWorksWithoutArgs()
        {
            var urn = "urn:ccnet:local";
            var action = "DoSomething";
            var returnValue = new InvokeResult
                                  {
                                      ResultCode = RemoteResultCode.Success,
                                      Data = MessageSerialiser.Serialise(new Messages.Blank())
                                  };
            var invokerMock = new Mock<IActionInvoker>(MockBehavior.Strict);
            invokerMock.Setup(ai => ai.Invoke(urn, It.IsAny<InvokeArguments>()))
                .Callback((string u, InvokeArguments a) =>
                              {
                                  Assert.AreEqual(action, a.Action);
                                  Assert.IsNull(a.Data);
                              })
                .Returns(returnValue);
            var result = this.RunTest(invokerMock.Object, c => c.Invoke(urn, action));
            Assert.IsInstanceOf<Messages.Blank>(result);
        }

        [Test]
        public void InvokeHandlesFailure()
        {
            var urn = "urn:ccnet:local";
            var action = "DoSomething";
            var args = new Messages.Blank();
            var returnValue = new InvokeResult
                                  {
                                      ResultCode = RemoteResultCode.InvalidInput
                                  };
            var invokerMock = new Mock<IActionInvoker>(MockBehavior.Strict);
            invokerMock.Setup(ai => ai.Invoke(urn, It.IsAny<InvokeArguments>()))
                .Callback((string u, InvokeArguments a) => Assert.AreEqual(action, a.Action))
                .Returns(returnValue);
            var error = Assert.Throws<RemoteServerException>(
                () => this.RunTest(invokerMock.Object, c => c.Invoke(urn, action, args)));
            Assert.IsNotNull(error.LogId);
            Assert.AreEqual(returnValue.ResultCode, error.ResultCode);
        }

        [Test]
        public void InvokeHandlesExceptionOnInvoker()
        {
            var urn = "urn:ccnet:local";
            var action = "DoSomething";
            var args = new Messages.Blank();
            var invokerMock = new Mock<IActionInvoker>(MockBehavior.Strict);
            invokerMock.Setup(ai => ai.Invoke(urn, It.IsAny<InvokeArguments>()))
                .Throws(new Exception("Oops, something bad happened!"));
            var error = Assert.Throws<RemoteServerException>(
                () => this.RunTest(invokerMock.Object, c => c.Invoke(urn, action, args)));
            Assert.IsNotNull(error.LogId);
            Assert.AreEqual(RemoteResultCode.FatalError, error.ResultCode);
        }

        [Test]
        public void QueryHandlesSuccess()
        {
            var urn = "urn:ccnet:local";
            var returnValue = new QueryResult
                                  {
                                      ResultCode = RemoteResultCode.Success,
                                      Actions = new[]
                                                    {
                                                        new RemoteActionDefinition()
                                                    }
                                  };
            var invokerMock = new Mock<IActionInvoker>(MockBehavior.Strict);
            invokerMock.Setup(ai => ai.Query(urn, It.IsAny<QueryArguments>()))
                .Callback((string u, QueryArguments a) =>
                              {
                                  Assert.AreEqual(DataDefinitions.None, a.DataToInclude);
                                  Assert.IsNull(a.FilterPattern);
                              })
                .Returns(returnValue);
            var result = this.RunTest(invokerMock.Object, c => c.Query(urn));
            Assert.AreEqual(1, result.Length);
        }

        [Test]
        public void QueryHandlesFailure()
        {
            var urn = "urn:ccnet:local";
            var returnValue = new QueryResult
                                  {
                                      ResultCode = RemoteResultCode.InvalidInput
                                  };
            var invokerMock = new Mock<IActionInvoker>(MockBehavior.Strict);
            invokerMock.Setup(ai => ai.Query(urn, It.IsAny<QueryArguments>()))
                .Returns(returnValue);
            var error = Assert.Throws<RemoteServerException>(
                () => this.RunTest(invokerMock.Object, c => c.Query(urn)));
            Assert.IsNotNull(error.LogId);
            Assert.AreEqual(returnValue.ResultCode, error.ResultCode);
        }

        [Test]
        public void QueryHandlesExceptionOnInvoker()
        {
            var urn = "urn:ccnet:local";
            var invokerMock = new Mock<IActionInvoker>(MockBehavior.Strict);
            invokerMock.Setup(ai => ai.Query(urn, It.IsAny<QueryArguments>()))
                .Throws(new Exception("Oops, something bad happened!"));
            var error = Assert.Throws<RemoteServerException>(
                () => this.RunTest(invokerMock.Object, c => c.Query(urn)));
            Assert.IsNotNull(error.LogId);
            Assert.AreEqual(RemoteResultCode.FatalError, error.ResultCode);
        }
        #endregion

        #region Helper methods
        private TResult RunTest<TResult>(IActionInvoker invoker, Func<ServerConnection, TResult> test)
        {
            var channel = new Wcf();
            var address = "net.tcp://localhost/client";
            channel.Endpoints.Add(new NetTcp { Address = address });
            var opened = false;
            try
            {
                opened = channel.Initialise(invoker);
                Assert.IsTrue(opened);
                var connection = new ServerConnection(address);
                return test(connection);
            }
            finally
            {
                if (opened)
                {
                    channel.CleanUp();
                }
            }
        }
        #endregion
    }
}
