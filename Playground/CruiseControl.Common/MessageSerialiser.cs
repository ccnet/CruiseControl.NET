namespace CruiseControl.Common
{
    using System.Xaml;

    /// <summary>
    /// Handles the serialisation and deserialisation of messages
    /// </summary>
    public static class MessageSerialiser
    {
        #region Public methods
        #region Deserialise()
        /// <summary>
        /// Deserialises the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// The deserialised message.
        /// </returns>
        public static object Deserialise(string message)
        {
            var output = XamlServices.Parse(message);
            return output;
        }
        #endregion

        #region Serialise()
        /// <summary>
        /// Serialises the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// The serialised message.
        /// </returns>
        public static string Serialise(object message)
        {
            var output = XamlServices.Save(message);
            return output;
        }
        #endregion
        #endregion
    }
}
