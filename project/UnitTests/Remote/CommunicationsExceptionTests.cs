namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    using System;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Remote;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.IO;

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

        [Test]
        public void ConstructorSetsType()
        {
            var message = "Test Message";
            var exception = new Exception();
            var type = "Some type";
            var error = new CommunicationsException(message, exception, type);
            Assert.AreEqual(message, error.Message);
            Assert.AreSame(exception, error.InnerException);
            Assert.AreEqual(type, error.ErrorType);
        }

        [Test]
        public void ExceptionCanBeSerialised()
        {
            var message = "Test Message";
            var exception = new Exception("Inner message");
            var type = "Some type";
            var original = new CommunicationsException(message, exception, type);
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream();
            formatter.Serialize(stream, original);
            stream.Position = 0;  
            var error = formatter.Deserialize(stream);
            Assert.IsInstanceOf<CommunicationsException>(error);
            var deserialised = error as CommunicationsException;
            Assert.AreEqual(message, deserialised.Message);
            Assert.IsNotNull(deserialised.InnerException);
            Assert.AreEqual(exception.Message, deserialised.InnerException.Message);
            Assert.AreEqual(type, deserialised.ErrorType);
        }
        #endregion
        #endregion
    }
}
