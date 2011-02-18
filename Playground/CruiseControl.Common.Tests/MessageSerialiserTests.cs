namespace CruiseControl.Common.Tests
{
    using CruiseControl.Common.Messages;
    using NUnit.Framework;

    [TestFixture]
    public class MessageSerialiserTests
    {
        #region Tests
        [Test]
        public void SerialiseConvertsMessageToXaml()
        {
            var input = new Blank();
            var output = MessageSerialiser.Serialise(input);
            var expected = "<Blank xmlns=\"urn:cruisecontrol:common\" />";
            Assert.AreEqual(expected, output);
        }

        [Test]
        public void DeserialiseConvertsXamlToMessage()
        {
            var input = "<Blank xmlns=\"urn:cruisecontrol:common\" />";
            var output = MessageSerialiser.Deserialise(input);
            Assert.IsInstanceOf<Blank>(output);
        }
        #endregion
    }
}
