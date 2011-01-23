using System;

namespace ThoughtWorks.CruiseControl.Remote.Monitor
{
    /// <summary>
    /// The arguments for a server update.
    /// </summary>
    public class ServerUpdateArgs
        : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="ServerUpdateArgs"/>.
        /// </summary>
        /// <param name="snapshot">The current snapshot.</param>
        public ServerUpdateArgs(CruiseServerSnapshot snapshot)
        {
            Snapshot = snapshot;
        }

        /// <summary>
        /// Initialise a new <see cref="ServerUpdateArgs"/>.
        /// </summary>
        /// <param name="error">The error details.</param>
        public ServerUpdateArgs(Exception error)
        {
            Exception = error;
        }
        #endregion

        #region Public properties
        #region Snapshot
        /// <summary>
        /// The snapshot of the server.
        /// </summary>
        public CruiseServerSnapshot Snapshot { get; protected set; }
        #endregion

        #region Exception
        /// <summary>
        /// The exception details for this update. 
        /// </summary>
        public Exception Exception { get; private set; }
        #endregion
        #endregion
    }
}
