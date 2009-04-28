using NUnit.Framework;
using System;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    /// <summary>
    /// Unit tests for SecurityException. 
    /// </summary>
    [TestFixture]
    public class SecurityExceptionTests
    {
        [Test]
        public void CreateDefault()
        {
            TestHelpers.EnsureLanguageIsValid();
            SecurityException exception = new SecurityException();
            Assert.AreEqual("A security failure has occurred.", exception.Message);
        }

        [Test]
        public void CreateWithMessage()
        {
            TestHelpers.EnsureLanguageIsValid();
            string message = "An error has occured";
            SecurityException exception = new SecurityException(message);
            Assert.AreEqual(message, exception.Message);
        }

        [Test]
        public void CreateWithMessageAndException()
        {
            TestHelpers.EnsureLanguageIsValid();
            string message = "An error has occured";
            Exception innerException = new Exception("An inner exception");
            SecurityException exception = new SecurityException(message, innerException);
            Assert.AreEqual(message, exception.Message);
            Assert.AreEqual(innerException, exception.InnerException);
        }

        [Test]
        public void PassThroughSerialisation()
        {
            TestHelpers.EnsureLanguageIsValid();
            SecurityException exception = new SecurityException();
            object result = TestHelpers.RunSerialisationTest(exception);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(typeof(SecurityException), result);
            Assert.AreEqual("A security failure has occurred.", (result as SecurityException).Message);
        }
    }
}
