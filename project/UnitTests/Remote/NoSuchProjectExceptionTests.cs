using NUnit.Framework;
using System;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    /// <summary>
    /// Unit tests for NoSuchProjectException. 
    /// </summary>
    [TestFixture]
    public class NoSuchProjectExceptionTests
    {
        [Test]
        public void CreateDefault()
        {
            TestHelpers.EnsureLanguageIsValid();
            NoSuchProjectException exception = new NoSuchProjectException();
            Assert.AreEqual("The project '' does not exist on the CCNet server.", exception.Message);
            Assert.IsNull(exception.RequestedProject);
        }

        [Test]
        public void CreateWithRequestedProject()
        {
            TestHelpers.EnsureLanguageIsValid();
            string requestedProject = "Something";
            NoSuchProjectException exception = new NoSuchProjectException(requestedProject);
            Assert.AreEqual("The project 'Something' does not exist on the CCNet server.", exception.Message);
            Assert.AreEqual(requestedProject, exception.RequestedProject);
        }

        [Test]
        public void CreateWithRequestedProjectAndException()
        {
            TestHelpers.EnsureLanguageIsValid();
            string requestedProject = "Something";
            Exception innerException = new Exception("An inner exception");
            NoSuchProjectException exception = new NoSuchProjectException(requestedProject, innerException);
            Assert.AreEqual("The project 'Something' does not exist on the CCNet server.", exception.Message);
            Assert.AreEqual(requestedProject, exception.RequestedProject);
            Assert.AreEqual(innerException, exception.InnerException);
        }

        [Test]
        public void PassThroughSerialisation()
        {
            TestHelpers.EnsureLanguageIsValid();
            string requestedProject = "Something";
            NoSuchProjectException exception = new NoSuchProjectException(requestedProject);
            object result = TestHelpers.RunSerialisationTest(exception);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(typeof(NoSuchProjectException), result);
            Assert.AreEqual("The project 'Something' does not exist on the CCNet server.", (result as NoSuchProjectException).Message);
            Assert.AreEqual(requestedProject, (result as NoSuchProjectException).RequestedProject);
        }
    }
}
