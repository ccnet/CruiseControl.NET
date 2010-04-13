namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Messages
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Remote.Messages;

    [TestFixture]
    public class EncryptedRequestTests
    {
        #region Tests
        #region Constructor tests
        [Test]
        public void SessionConstructorInitialisesTheValues()
        {
            var sessionId = "MyNewSession";
            var request = new EncryptedRequest(sessionId);
            Assert.AreEqual(sessionId, request.SessionToken);
        }

        [Test]
        public void FullConstructorInitialisesTheValues()
        {
            var sessionId = "MyNewSession";
            var data = "SomeEncryptedData";
            var request = new EncryptedRequest(sessionId, data);
            Assert.AreEqual(sessionId, request.SessionToken);
            Assert.AreEqual(data, request.EncryptedData);
        }
        #endregion

        #region Getter/setter tests
        [Test]
        public void EncryptedDataCanBeSetAndRetrieved()
        {
            var request = new EncryptedRequest();
            var data = "SomeEncryptedData";
            request.EncryptedData = data;
            Assert.AreEqual(data, request.EncryptedData);
        }
        #endregion
        #endregion
    }
}
