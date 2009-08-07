
using System.ComponentModel;
using System;
using System.Collections.Generic;
namespace ThoughtWorks.CruiseControl.Remote.Monitor
{
    /// <summary>
    /// A build queue that is being monitored on a remote server.
    /// </summary>
    public class BuildQueue
        : INotifyPropertyChanged
    {
        #region Private fields
        private readonly CruiseServerClientBase client;
        private readonly Server server;
        private Dictionary<string, BuildQueueRequest> requests = new Dictionary<string, BuildQueueRequest>();
        private QueueSnapshot buildQueue;
        private Exception exception;
        private object syncLock = new object();
        private DataBag data = new DataBag();
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new build queue.
        /// </summary>
        /// <param name="client">The underlying client.</param>
        /// <param name="server">The server this queue belongs to.</param>
        /// <param name="buildQueue">The actual build queue details.</param>
        public BuildQueue(CruiseServerClientBase client,Server server, QueueSnapshot buildQueue)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (server == null) throw new ArgumentNullException("server");
            if (buildQueue == null) throw new ArgumentNullException("buildQueue");

            this.client = client;
            this.server = server;
            this.buildQueue = buildQueue;
        }
        #endregion

        #region Public properties
        #region Server
        /// <summary>
        /// The server this build queue belongs to.
        /// </summary>
        public Server Server
        {
            get { return server; }
        }
        #endregion

        #region Name
        /// <summary>
        /// The name of the build queue.
        /// </summary>
        public string Name
        {
            get { return InnerBuildQueue.QueueName; }
        }
        #endregion

        #region Requests
        /// <summary>
        /// Any current or pending requests.
        /// </summary>
        public IEnumerable<BuildQueueRequest> Requests
        {
            get
            {
                lock (syncLock) { return requests.Values; }
            }
        }
        #endregion

        #region Exception
        /// <summary>
        /// Any server exception details.
        /// </summary>
        public Exception Exception
        {
            get { return exception; }
            set
            {
                if (((value != null) && (exception != null) && (value.Message != exception.Message)) ||
                    ((value == null) && (exception != null)) ||
                    ((value != null) && (exception == null)))
                {
                    exception = value;
                    FirePropertyChanged("Exception");
                }
            }
        }
        #endregion

        #region Data
        /// <summary>
        /// Gets the data bag.
        /// </summary>
        public DataBag Data
        {
            get { return data; }
        }
        #endregion
        #endregion

        #region Public methods
        #region Update()
        /// <summary>
        /// Updates the details on a build queue.
        /// </summary>
        /// <param name="value">The new build queue details.</param>
        public void Update(QueueSnapshot value)
        {
            // Validate the arguments
            if (value == null) throw new ArgumentNullException("value");

            // Find all the changed properties
            var changes = new List<string>();
            var newRequests = new List<BuildQueueRequest>();
            var oldRequests = new List<BuildQueueRequest>();

            lock (syncLock)
            {
                // Check for any request differences
                var requestValues = new Dictionary<BuildQueueRequest, QueuedRequestSnapshot>();
                var oldRequestNames = new List<string>(requests.Keys);
                foreach (var request in value.Requests)
                {
                    // Check if this request has already been loaded
                    var requestName = request.ProjectName;
                    if (oldRequestNames.Contains(requestName))
                    {
                        requestValues.Add(requests[requestName], request);
                        oldRequestNames.Remove(requestName);
                    }
                    else
                    {
                        // Otherwise this is a new request
                        var newRequest = new BuildQueueRequest(client, this, request);
                        newRequests.Add(newRequest);
                    }
                }

                // Store the old request
                foreach (var request in oldRequestNames)
                {
                    oldRequests.Add(requests[request]);
                }

                // Perform the actual update
                foreach (var request in oldRequestNames)
                {
                    requests.Remove(request);
                }
                foreach (var request in newRequests)
                {
                    if (!requests.ContainsKey(request.Name))
                    {
                        requests.Add(request.Name, request);
                    }
                }
                buildQueue = value;

                // Update all the requests
                foreach (var requestValue in requestValues)
                {
                    requestValue.Key.Update(requestValue.Value);
                }
            }

            // Tell any listeners about any changes
            foreach (var request in newRequests)
            {
                FireBuildQueueRequestAdded(request);
            }
            foreach (var request in oldRequests)
            {
                FireBuildQueueRequestRemoved(request);
            }
            foreach (var change in changes)
            {
                FirePropertyChanged(change);
            }
        }
        #endregion

        #region Equals()
        /// <summary>
        /// Compare if two queues are the same.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as BuildQueue);
        }

        /// <summary>
        /// Compare if two queues are the same.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool Equals(BuildQueue obj)
        {
            if (obj == null) return false;
            return obj.Server.Equals(Server) &&
                (obj.Name == Name);
        }
        #endregion

        #region GetHashCode()
        /// <summary>
        /// Return the hash code for this queue.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Server.GetHashCode() +
                Name.GetHashCode();
        }
        #endregion
        #endregion

        #region Public events
        #region BuildQueueRequestAdded
        /// <summary>
        /// A new request has been added.
        /// </summary>
        public event EventHandler<BuildQueueRequestChangedArgs> BuildQueueRequestAdded;
        #endregion

        #region BuildQueueRequestRemoved
        /// <summary>
        /// An existing request has been removed.
        /// </summary>
        public event EventHandler<BuildQueueRequestChangedArgs> BuildQueueRequestRemoved;
        #endregion

        #region PropertyChanged
        /// <summary>
        /// A property has been changed on this queue.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #endregion

        #region Protected properties
        #region InnerBuildQueue
        /// <summary>
        /// The underlying build queue status.
        /// </summary>
        protected QueueSnapshot InnerBuildQueue
        {
            get { return buildQueue; }
        }
        #endregion
        #endregion

        #region Protected methods
        #region FireBuildQueueRequestAdded()
        /// <summary>
        /// Fires the <see cref="BuildQueueRequestAdded"/> event.
        /// </summary>
        /// <param name="request">The request that was added.</param>
        protected void FireBuildQueueRequestAdded(BuildQueueRequest request)
        {
            if (BuildQueueRequestAdded != null)
            {
                var args = new BuildQueueRequestChangedArgs(request);
                BuildQueueRequestAdded(this, args);
            }
        }
        #endregion

        #region FireBuildQueueRequestRemoved()
        /// <summary>
        /// Fires the <see cref="BuildQueueRequestRemoved"/> event.
        /// </summary>
        /// <param name="request">The request that was removed.</param>
        protected void FireBuildQueueRequestRemoved(BuildQueueRequest request)
        {
            if (BuildQueueRequestRemoved != null)
            {
                var args = new BuildQueueRequestChangedArgs(request);
                BuildQueueRequestRemoved(this, args);
            }
        }
        #endregion

        #region FirePropertyChanged()
        /// <summary>
        /// Fires the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The property that has changed.</param>
        protected void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                var args = new PropertyChangedEventArgs(propertyName);
                PropertyChanged(this, args);
            }
        }
        #endregion
        #endregion
    }
}
