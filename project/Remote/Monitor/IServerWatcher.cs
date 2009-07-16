using System;

namespace ThoughtWorks.CruiseControl.Remote.Monitor
{
    /// <summary>
    /// A watcher that watches a remote server and returns status snapshots.
    /// </summary>
    public interface IServerWatcher
    {
        #region Methods
        #region Refresh()
        /// <summary>
        /// Checks the server for a refresh.
        /// </summary>
        void Refresh();
        #endregion
        #endregion

        #region Events
        #region Update
        /// <summary>
        /// An update has been received from a remote server.
        /// </summary>
        event EventHandler<ServerUpdateArgs> Update;
        #endregion
        #endregion
    }
}
