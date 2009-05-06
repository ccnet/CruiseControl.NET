using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    [TestFixture]
    public class CommunicationsExceptionTests
    {
        #region Test methods
        #region Constructors()
        [Test]
        public void NewWithNoParametersSetsDefaultMessage()
        {
            CommunicationsException exception = new CommunicationsException();
            Assert.AreEqual("A communications error has occurred.", exception.Message);
        }

        [Test]
        public void NewWithMessageSetsMessage()
        {
            CommunicationsException exception = new CommunicationsException("Message");
            Assert.AreEqual("Message", exception.Message);
        }

        [Test]
        public void FullNewSetsAllProperties()
        {
            Exception error = new Exception();
            CommunicationsException exception = new CommunicationsException("Message", error);
            Assert.AreEqual("Message", exception.Message);
            Assert.AreEqual(error, exception.InnerException);
        }
        #endregion
        #endregion
    }
}
