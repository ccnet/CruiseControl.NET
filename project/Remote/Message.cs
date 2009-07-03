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
        #region Private fields
        private string message;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new blank <see cref="Message"/>.
        /// </summary>
        public Message()
        {
        }

        /// <summary>
        /// Initialise a new <see cref="Message"/> with a message.
        /// </summary>
        /// <param name="message">The message.</param>
        public Message(string message)
        {
            this.message = message;
        }
        #endregion

        #region Public properties
        #region Text
        /// <summary>
        /// The text of the message.
        /// </summary>
        [XmlText]
        public string Text
        {
            get { return message; }
            set { message = value; }
        }
        #endregion
        #endregion

        #region Public methods
        #region ToString()
        /// <summary>
        /// Returns the message text.
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
            //check if obj isn't null, if it is return false 
            if (obj == null)
            {
                return false;
            }

            Message m = obj as Message;

            //if obj can't be casted as Message, return false 
            if (m == null)
            {
                return false;
            }

            //compare the values 
            return string.Equals(this.Text, m.Text);
        }

        #endregion
        #endregion
    }
}