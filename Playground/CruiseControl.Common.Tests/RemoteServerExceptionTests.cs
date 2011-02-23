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
        #endregion
    }
}
