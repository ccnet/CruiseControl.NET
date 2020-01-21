using NUnit.Framework;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace ThoughtWorks.CruiseControl.UnitTests
{
    /// <summary>
    /// Helper methods for testing the serialization / serialization of objects.
    /// </summary>
    public static class TestHelpers
    {
        /// <summary>
        /// Ensures that the correct language is set for the unit tests.
        /// </summary>
        public static void EnsureLanguageIsValid()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
        }

        /// <summary>
        /// Tests that an object can be serialized and deserialized.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns>The deserialized object.</returns>
        /// <remarks>
        /// This does not test that the deserialized object is the same as the source, it only tests
        /// that the object can actually be serialized and deserialized.
        /// </remarks>
        public static object RunSerialisationTest(object value)
        {
            object result = null;
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(stream, value);
            }
            catch (Exception error)
            {
                Assert.Fail(string.Format(CultureInfo.CurrentCulture,"Unable to serialize: {0}", error.Message));
            }

            stream.Position = 0;
            try
            {
                result = formatter.Deserialize(stream);
            }
            catch (Exception error)
            {
                Assert.Fail(string.Format(CultureInfo.CurrentCulture,"Unable to serialize: {0}", error.Message));
            }
            return result;
        }
    }
}
