using System;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// A user-readable message.
    /// </summary>
	[Serializable]
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
        #endregion
        #endregion
    }
}