namespace CruiseControl.Core.Tests.Exceptions
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using NUnit.Framework;

    public static class ExceptionTestHelpers
    {
        public static void SerialiseTo(this Exception error, Stream stream)
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, error);
        }

        public static TException DeserialiseException<TException>(this Stream stream)
            where TException : Exception
        {
            var formatter = new BinaryFormatter();
            var error = formatter.Deserialize(stream) as TException;
            Assert.IsNotNull(error, "The stream did not contain an instance of " + typeof(TException).FullName);
            return error;
        }

        public static TException RunSerialisationTest<TException>(this TException error)
            where TException : Exception
        {
            var stream = new MemoryStream();
            error.SerialiseTo(stream);
            stream.Seek(0, SeekOrigin.Begin);
            var result = stream.DeserialiseException<TException>();
            Assert.AreEqual(error.Message, result.Message, "Message was not correctly serialised/deserialised");
            return result;
        }
    }
}
