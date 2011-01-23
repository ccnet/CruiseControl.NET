namespace CruiseControl.Core.Tests.Exceptions
{
    using System;
    using CruiseControl.Core.Exceptions;
    using NUnit.Framework;

    [TestFixture]
    public class CruiseControlExceptionTests
    {
        #region Tests
        [Test]
        public void SingleArgConstructorSetsMessage()
        {
            var testMessage = "This is a test message";
            var error = new CruiseControlException(testMessage);
            Assert.AreEqual(testMessage, error.Message);
        }

        [Test]
        public void DoubleArgConstructorSetsMessageAndInner()
        {
            var testMessage = "This is a test message";
            var inner = new Exception("Inner");
            var error = new CruiseControlException(testMessage, inner);
            Assert.AreEqual(testMessage, error.Message);
            Assert.AreSame(inner, error.InnerException);
        }

        [Test]
        public void SerialisationWorksCorrectly()
        {
            var testMessage = "This is a test message";
            var error = new CruiseControlException(testMessage);
            error.RunSerialisationTest();
        }
        #endregion
    }
}
