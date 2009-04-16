using NUnit.Framework;
using System;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    /// <summary>
    /// Unit tests for PermissionDeniedException. 
    /// </summary>
    [TestFixture]
    public class PermissionDeniedExceptionTests
    {
        [Test]
        public void CreateWithpermission()
        {
            TestHelpers.EnsureLanguageIsValid();
            string permission = "Something";
            PermissionDeniedException exception = new PermissionDeniedException(permission);
            Assert.AreEqual("Permission to execute 'Something' has been denied.", exception.Message);
            Assert.AreEqual(permission, exception.Permission);
        }

        [Test]
        public void CreateWithpermissionAndMessage()
        {
            TestHelpers.EnsureLanguageIsValid();
            string permission = "Something";
            string message = "An error has occured";
            PermissionDeniedException exception = new PermissionDeniedException(permission, message);
            Assert.AreEqual(message, exception.Message);
            Assert.AreEqual(permission, exception.Permission);
        }

        [Test]
        public void CreateWithpermissionMessageAndException()
        {
            TestHelpers.EnsureLanguageIsValid();
            string permission = "Something";
            string message = "An error has occured";
            Exception innerException = new Exception("An inner exception");
            PermissionDeniedException exception = new PermissionDeniedException(permission, message, innerException);
            Assert.AreEqual(message, exception.Message);
            Assert.AreEqual(permission, exception.Permission);
            Assert.AreEqual(innerException, exception.InnerException);
        }

        [Test]
        public void PassThroughSerialisation()
        {
            TestHelpers.EnsureLanguageIsValid();
            string permission = "Something";
            PermissionDeniedException exception = new PermissionDeniedException(permission);
            object result = TestHelpers.RunSerialisationTest(exception);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(typeof(PermissionDeniedException), result);
            Assert.AreEqual("Permission to execute 'Something' has been denied.", exception.Message);
            Assert.AreEqual(permission, (result as PermissionDeniedException).Permission);
        }
    }
}
