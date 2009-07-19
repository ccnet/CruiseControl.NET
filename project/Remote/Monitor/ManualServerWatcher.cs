using System;
using System.Threading;

namespace ThoughtWorks.CruiseControl.Remote.Monitor
{
    /// <summary>
    /// Only checks for changes when manually requested.
    /// </summary>
    public class ManualServerWatcher
        : IServerWatcher, IDisposable
    {
        #region Private fields
        private readonly CruiseServerClientBase client;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="ManualServerWatcher"/>.
        /// </summary>
        /// <param name="client">The underlying client to poll.</param>
        public ManualServerWatcher(CruiseServerClientBase client)
        {
            if (client == null) throw new ArgumentNullException("client");
            this.client = client;
        }
        #endregion

        #region Methods
        #region Refresh()
        /// <summary>
        /// Checks the server for a refresh.
        /// </summary>
        public virtual void Refresh()
        {
            RetrieveSnapshot();
        }
        #endregion

        #region Dispose()
        /// <summary>
        /// Cleans up when this watcher is no longer needed.
        /// </summary>
        public void Dispose()
        {
        }
        #endregion
        #endregion

        #region Events
        #region Update
        /// <summary>
        /// An update has been received from a remote server.
        /// </summary>
        public event EventHandler<ServerUpdateArgs> Update;
        #endregion
        #endregion

        #region Private methods
        #region RetrieveSnapshot()
        /// <summary>
        /// Attempt to retrieve a snapshot from the remote server.
        /// </summary>
        private void RetrieveSnapshot()
        {
            // Retrieve the snapshot
            ServerUpdateArgs args;
            try
            {
                CruiseServerSnapshot snapshot = null;
                try
                {
                    snapshot = client.GetCruiseServerSnapshot();
                }
                catch (NotImplementedException)
                {
                    // This is an older style server, try fudging the snapshot
                    snapshot = new CruiseServerSnapshot
                    {
                        ProjectStatuses = client.GetProjectStatus()
                    };
                }
                args = new ServerUpdateArgs(snapshot);
            }
            catch (Exception error)
            {
                args = new ServerUpdateArgs(error);
            }

            // Fire the update
            if (Update != null)
            {
                Update(this, args);
            }
        }
        #endregion
        #endregion
    }
}
