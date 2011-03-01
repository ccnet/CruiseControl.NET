namespace CruiseControl.Common.Tests
{
    using System;
    using NUnit.Framework;

    public class RemoteServerExceptionTests
    {
        #region Tests
        [Test]
        public void ConstructorSetsProperties()
        {
            var logId = Guid.NewGuid();
            var resultCode = RemoteResultCode.InvalidInput;
            var exception = new RemoteServerException(resultCode, logId);
            var expected = RemoteServerException.GenerateMessage(resultCode);
            Assert.AreEqual(resultCode, exception.ResultCode);
            Assert.AreEqual(logId, exception.LogId);
            Assert.AreEqual(expected, exception.Message);
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public void InnerExceptionIsSet()
        {
            var logId = Guid.NewGuid();
            var resultCode = RemoteResultCode.InvalidInput;
            var message = "duh-duh";
            var inner = new Exception("Oops");
            var exception = new RemoteServerException(resultCode, logId, message, inner);
            Assert.AreEqual(resultCode, exception.ResultCode);
            Assert.AreEqual(logId, exception.LogId);
            Assert.AreEqual(message, exception.Message);
            Assert.AreSame(inner, exception.InnerException);
        }

        [Test]
        public void GenerateMessageHandlesInvalidInput()
        {
            var expected = "An invalid request message was passed to the remote server";
            var actual = RemoteServerException.GenerateMessage(RemoteResultCode.InvalidInput);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GenerateMessageHandlesUnknownAction()
        {
            var expected = "Unable to find specified action - check that the action is correct";
            var actual = RemoteServerException.GenerateMessage(RemoteResultCode.UnknownAction);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GenerateMessageHandlesUnknownUrn()
        {
            var expected = "Unable to find target item - check that the URN is correct";
            var actual = RemoteServerException.GenerateMessage(RemoteResultCode.UnknownUrn);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GenerateMessageHandlesSuccess()
        {
            var expected = "An error has occurred at the remote server, code: " + RemoteResultCode.Success;
            var actual = RemoteServerException.GenerateMessage(RemoteResultCode.Success);
            Assert.AreEqual(expected, actual);
        }
        #endregion
    }
}
