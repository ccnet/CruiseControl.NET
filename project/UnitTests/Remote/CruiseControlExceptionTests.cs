using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    /// <summary>
    /// Unit tests for CruiseControlException. 
    /// </summary>
    [TestFixture]
    public class CruiseControlExceptionTests
    {
        [Test]
        public void CreateDefault()
        {
            TestHelpers.EnsureLanguageIsValid();
            CruiseControlException exception = new CruiseControlException();
            Assert.AreEqual(string.Empty, exception.Message);
        }

        [Test]
        public void CreateWithMessage()
        {
            TestHelpers.EnsureLanguageIsValid();
            string message = "An error has occured";
            CruiseControlException exception = new CruiseControlException(message);
            Assert.AreEqual(message, exception.Message);
        }

        [Test]
        public void CreateWithMessageAndException()
        {
            TestHelpers.EnsureLanguageIsValid();
            string message = "An error has occured";
            Exception innerException = new Exception("An inner exception");
            CruiseControlException exception = new CruiseControlException(message, innerException);
            Assert.AreEqual(message, exception.Message);
            Assert.AreEqual(innerException, exception.InnerException);
        }

        [Test]
        public void PassThroughSerialisation()
        {
            TestHelpers.EnsureLanguageIsValid();
            string message = "An error has occured";
            CruiseControlException exception = new CruiseControlException(message);
            object result = TestHelpers.RunSerialisationTest(exception);
            Assert.IsNotNull(result);
            Assert.That(result, Is.InstanceOf<CruiseControlException>());
            Assert.AreEqual(message, (result as CruiseControlException).Message);
        }
    }
}
