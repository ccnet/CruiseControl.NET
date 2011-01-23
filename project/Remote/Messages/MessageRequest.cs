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
        private string message;
        private Message.MessageKind kind ;

        /// <summary>
        /// The message being passed.
        /// </summary>
        [XmlElement("message")]
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        /// <summary>
        /// The kind of message
        /// </summary>
        [XmlElement("kind")]
        public Message.MessageKind Kind
        {
            get { return kind; }
            set { kind = value; }
        }
    }
}
