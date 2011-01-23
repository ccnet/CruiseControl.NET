namespace CruiseControl.Core.Tests.Exceptions
{
    using System;
    using CruiseControl.Core.Exceptions;
    using NUnit.Framework;

    [TestFixture]
    public class ConfigurationExceptionTests
    {
        #region Tests
        [Test]
        public void SingleArgConstructorSetsMessage()
        {
            var testMessage = "This is a test message";
            var error = new ConfigurationException(testMessage);
            Assert.AreEqual(testMessage, error.Message);
            Assert.AreEqual(testMessage, error.ConfigurationProblem);
        }

        [Test]
        public void SingleArgWithParamsConstructorSetsMessage()
        {
            var error = new ConfigurationException("{0} is a test {1}", "This", "message");
            Assert.AreEqual("This is a test message", error.Message);
            Assert.AreEqual("This is a test message", error.ConfigurationProblem);
        }

        [Test]
        public void DoubleArgConstructorSetsMessageAndInner()
        {
            var testMessage = "This is a test message";
            var inner = new Exception("Inner");
            var error = new ConfigurationException(testMessage, inner);
            Assert.AreEqual(testMessage, error.Message);
            Assert.AreEqual(testMessage, error.ConfigurationProblem);
            Assert.AreSame(inner, error.InnerException);
        }

        [Test]
        public void SerialisationWorksCorrectly()
        {
            var testMessage = "This is a test message";
            var error = new ConfigurationException(testMessage);
            var result = error.RunSerialisationTest();
            Assert.AreEqual(testMessage, result.ConfigurationProblem);
        }
        #endregion
    }
}
