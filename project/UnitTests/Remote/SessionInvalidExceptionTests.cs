using NUnit.Framework;
using System;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    /// <summary>
    /// Unit tests for SessionInvalidException. 
    /// </summary>
    [TestFixture]
    public class SessionInvalidExceptionTests
    {
        [Test]
        public void CreateDefault()
        {
            TestHelpers.EnsureLanguageIsValid();
            SessionInvalidException exception = new SessionInvalidException();
            Assert.AreEqual("The session token is either invalid or is for a session that has expired.", exception.Message);
        }

        [Test]
        public void CreateWithMessage()
        {
            TestHelpers.EnsureLanguageIsValid();
            string message = "An error has occured";
            SessionInvalidException exception = new SessionInvalidException(message);
            Assert.AreEqual(message, exception.Message);
        }

        [Test]
        public void CreateWithMessageAndException()
        {
            TestHelpers.EnsureLanguageIsValid();
            string message = "An error has occured";
            Exception innerException = new Exception("An inner exception");
            SessionInvalidException exception = new SessionInvalidException(message, innerException);
            Assert.AreEqual(message, exception.Message);
            Assert.AreEqual(innerException, exception.InnerException);
        }

        [Test]
        public void PassThroughSerialisation()
        {
            TestHelpers.EnsureLanguageIsValid();
            SessionInvalidException exception = new SessionInvalidException();
            object result = TestHelpers.RunSerialisationTest(exception);
            Assert.IsNotNull(result);
            Assert.That(result, Is.InstanceOf<SessionInvalidException>());
            Assert.AreEqual("The session token is either invalid or is for a session that has expired.", (result as SessionInvalidException).Message);
        }
    }
}
