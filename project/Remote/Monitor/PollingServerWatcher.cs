using System;
using System.Threading;

namespace ThoughtWorks.CruiseControl.Remote.Monitor
{
    /// <summary>
    /// Polls the remote server on a regular basis for any changes.
    /// </summary>
    public class PollingServerWatcher
        : IServerWatcher, IDisposable
    {
        #region Private fields
        private readonly CruiseServerClientBase client;
        private Thread pollingThread;
        private long interval = 5;
        private DateTime nextRefresh;
        private bool disposing;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="PollingServerWatcher"/>.
        /// </summary>
        /// <param name="client">The underlying client to poll.</param>
        public PollingServerWatcher(CruiseServerClientBase client)
        {
            if (client == null) throw new ArgumentNullException("client");
            this.client = client;

            nextRefresh = DateTime.Now.AddSeconds(interval);
            pollingThread = new Thread(Poll);
            pollingThread.Start();
        }
        #endregion

        #region Public properties
        #region Interval
        /// <summary>
        /// The interval to poll (in seconds).
        /// </summary>
        public long Interval
        {
            get { return interval; }
            set
            {
                interval = value;
                nextRefresh = DateTime.Now.AddSeconds(interval);
            }
        }
        #endregion
        #endregion

        #region Methods
        #region Refresh()
        /// <summary>
        /// Checks the server for a refresh.
        /// </summary>
        public virtual void Refresh()
        {
            nextRefresh = DateTime.Now;
        }
        #endregion

        #region Dispose()
        /// <summary>
        /// Cleans up when this watcher is no longer needed.
        /// </summary>
        public void Dispose()
        {
            disposing = true;
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
        #region Poll()
        /// <summary>
        /// Checks to see if the server should be checked.
        /// </summary>
        private void Poll()
        {
            Thread.CurrentThread.Name = "Server watcher: " + client.Address;
            while (!disposing)
            {
                Thread.Sleep(500);
                if (DateTime.Now > nextRefresh) RetrieveSnapshot();
            }
        }
        #endregion

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
                    client.ProcessSingleAction<object>(o =>
                    {
                        snapshot = client.GetCruiseServerSnapshot();
                    }, null);
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
