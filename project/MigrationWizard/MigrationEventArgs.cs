using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.MigrationWizard
{
    /// <summary>
    /// Arguments for a migration event.
    /// </summary>
    public class MigrationEventArgs
        : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="MigrationEventArgs"/>.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        public MigrationEventArgs(string message, MigrationEventType type)
        {
            Message = message;
            Type = type;
            Time = DateTime.Now;
        }
        #endregion

        #region Public properties
        #region Message
        /// <summary>
        /// The event message.
        /// </summary>
        public string Message { get; private set; }
        #endregion

        #region Type
        /// <summary>
        /// The event type.
        /// </summary>
        public MigrationEventType Type { get; private set; }
        #endregion

        #region Time
        /// <summary>
        /// The time of the event.
        /// </summary>
        public DateTime Time { get; private set; }
        #endregion
        #endregion
    }
}
