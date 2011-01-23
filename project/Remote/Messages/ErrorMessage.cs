using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote.Messages
{
    /// <summary>
    /// An error message to return in a response.
    /// </summary>
    [Serializable]
    [XmlRoot("errorMessage")]
    public class ErrorMessage
    {
        #region Private fields
        private string type;
        private string message;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialises a new empty <see cref="ErrorMessage"/>.
        /// </summary>
        public ErrorMessage()
        {
        }

        /// <summary>
        /// Initialises a new <see cref="ErrorMessage"/> with a message.
        /// </summary>
        /// <param name="message"></param>
        public ErrorMessage(string message)
        {
            this.message = message;
        }

        /// <summary>
        /// Initialises a new <see cref="ErrorMessage"/> with a message and a type.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        public ErrorMessage(string message, string type)
        {
            this.type = type;
            this.message = message;
        }
        #endregion

        #region Public properties
        #region Type
        /// <summary>
        /// The type of error.
        /// </summary>
        [XmlAttribute("type")]
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        #endregion

        #region Public properties
        /// <summary>
        /// The error message text.
        /// </summary>
        [XmlText]
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        #endregion
        #endregion
    }
}
