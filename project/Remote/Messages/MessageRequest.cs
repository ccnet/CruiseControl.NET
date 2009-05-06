using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote.Messages
{
    /// <summary>
    /// A message for passing a message to the server.
    /// </summary>
    [XmlRoot("messageMessage")]
    [Serializable]
    public class MessageRequest
        : ProjectRequest
    {
        #region Private fields
        private string message;
        #endregion

        #region Public properties
        #region Message
        /// <summary>
        /// The message being passed.
        /// </summary>
        [XmlElement("message")]
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        #endregion
        #endregion
    }
}
