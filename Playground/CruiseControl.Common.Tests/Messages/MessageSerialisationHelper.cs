namespace CruiseControl.Common.Tests.Messages
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using CruiseControl.Common.Messages;
    using NUnit.Framework;

    public static class MessageSerialisationHelper
    {
        #region Public methods
        public static TMessage DoRoundTripTest<TMessage>(this TMessage message)
            where TMessage : BaseMessage
        {
            var serialiser = new DataContractSerializer(message.GetType());
            using (var stream = new MemoryStream())
            {
                serialiser.WriteObject(stream, message);
                var data = new byte[stream.Length];
                Array.Copy(stream.GetBuffer(), data, stream.Length);
                using (var inStream = new MemoryStream(data))
                {
                    var output = (TMessage)serialiser.ReadObject(inStream);
                    Assert.IsNotNull(output, "Unable to round trip message");
                    return output;
                }
            }
        }
        #endregion
    }
}
