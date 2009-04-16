using NUnit.Framework;
using System;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    /// <summary>
    /// Unit tests for BadReferenceException. 
    /// </summary>
    [TestFixture]
    public class BadReferenceExceptionTests
    {
        [Test]
        public void CreateWithReference()
        {
            TestHelpers.EnsureLanguageIsValid();
            string reference = "Something";
            BadReferenceException exception = new BadReferenceException(reference);
            Assert.AreEqual("Reference 'Something' is either incorrect or missing.", exception.Message);
            Assert.AreEqual(reference, exception.Reference);
        }

        [Test]
        public void CreateWithReferenceAndMessage()
        {
            TestHelpers.EnsureLanguageIsValid();
            string reference = "Something";
            string message = "An error has occured";
            BadReferenceException exception = new BadReferenceException(reference, message);
            Assert.AreEqual(message, exception.Message);
            Assert.AreEqual(reference, exception.Reference);
        }

        [Test]
        public void CreateWithReferenceMessageAndException()
        {
            TestHelpers.EnsureLanguageIsValid();
            string reference = "Something";
            string message = "An error has occured";
            Exception innerException = new Exception("An inner exception");
            BadReferenceException exception = new BadReferenceException(reference, message, innerException);
            Assert.AreEqual(message, exception.Message);
            Assert.AreEqual(reference, exception.Reference);
            Assert.AreEqual(innerException, exception.InnerException);
        }

        [Test]
        public void PassThroughSerialisation()
        {
            TestHelpers.EnsureLanguageIsValid();
            string reference = "Something";
            BadReferenceException exception = new BadReferenceException(reference);
            object result = TestHelpers.RunSerialisationTest(exception);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(typeof(BadReferenceException), result);
            Assert.AreEqual("Reference 'Something' is either incorrect or missing.", (result as BadReferenceException).Message);
            Assert.AreEqual(reference, (result as BadReferenceException).Reference);
        }
    }
}
