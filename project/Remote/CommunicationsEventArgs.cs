using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// The arguments from a communications event.
    /// </summary>
    public class CommunicationsEventArgs
        : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationsEventArgs"/> class.
        /// </summary>
        /// <param name="action">The action that the event is for.</param>
        /// <param name="message">The message that triggered the event.</param>
        public CommunicationsEventArgs(string action, CommunicationsMessage message)
        {
            this.Message = message;
            this.Action = action;
        }
        #endregion

        #region Public properties
        #region Message
        /// <summary>
        /// Gets the message that triggered this event.
        /// </summary>
        public CommunicationsMessage Message { get; private set; }
        #endregion

        #region Action
        /// <summary>
        /// Gets the action that triggered this event.
        /// </summary>
        public string Action { get; private set; }
        #endregion
        #endregion
    }
}
