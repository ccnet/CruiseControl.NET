namespace CruiseControl.Common.Tests.Messages
{
    using NUnit.Framework;

    public static class MessageSerialisationHelper
    {
        #region Public methods
        public static TMessage DoRoundTripTest<TMessage>(this TMessage message)
        {
            var input = MessageSerialiser.Serialise(message);
            var output = MessageSerialiser.Deserialise(input);
            Assert.IsNotNull(output);
            Assert.IsInstanceOf<TMessage>(output);
            return (TMessage)output;
        }
        #endregion
    }
}
