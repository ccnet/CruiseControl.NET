
using System;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// A user-readable message.
    /// </summary>
    [Serializable]
    [XmlRoot("message")]
    public class Message
    {
        public enum MessageKind
        {
            NotDefined = 0,
            Breakers = 1,
            Fixer = 2,
            FailingTasks = 3,
            BuildStatus = 4
        }

        private string message;
        private MessageKind messageKind;

        /// <summary>
        /// Initialise a new blank <see cref="Message"/>.
        /// </summary>
        public Message()
        { }

        /// <summary>
        /// Initialise a new <see cref="Message"/> with a message.
        /// </summary>
        /// <param name="message">The message.</param>
        public Message(string message)
        {
            this.message = message;
            this.Kind = MessageKind.NotDefined;
        }

        /// <summary>
        /// Initialise a new <see cref="Message"/> with a message.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="kind">the message kind</param>
        public Message(string message, MessageKind kind)
        {
            this.message = message;
            this.Kind = kind;
        }

        /// <summary>
        /// The text of the message.
        /// </summary>
        [XmlText]
        public string Text
        {
            get { return message; }
            set { message = value; }
        }

        /// <summary>
        /// The type of message
        /// </summary>
        [XmlAttribute]
        public MessageKind Kind
        {
            get { return messageKind; }
            set { messageKind = value; }
        }

        /// <summary>
        /// Returns the kind and the message text.
        /// </summary>
        /// <returns>The text of the message.</returns>
        public override string ToString()
        {
            return message;
        }

        /// <summary>
        /// compares 2 message objects
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            //if obj can't be casted as Message, return false 
            var m = obj as Message;
            if (m == null)
            {
                return false;
            }

            //compare the values 
            return string.Equals(this.Text, m.Text) && (this.Kind == m.Kind);
        }

        /// <summary>
        /// Retrieves the hash code for this message.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
}