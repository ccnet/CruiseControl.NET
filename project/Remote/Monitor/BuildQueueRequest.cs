using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ThoughtWorks.CruiseControl.Remote.Monitor
{
    /// <summary>
    /// A build queue request that is being monitored on a remote server.
    /// </summary>
    public class BuildQueueRequest
        : INotifyPropertyChanged, IEquatable<BuildQueueRequest>
    {
        #region Private fields
        private readonly CruiseServerClientBase client;
        private readonly BuildQueue buildQueue;
        private QueuedRequestSnapshot snapshot;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new build queue request.
        /// </summary>
        /// <param name="client">The underlying client.</param>
        /// <param name="buildQueue">The queue this project belongs to.</param>
        /// <param name="snapshot">The actual build queue request details.</param>
        public BuildQueueRequest(CruiseServerClientBase client, BuildQueue buildQueue, QueuedRequestSnapshot snapshot)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (buildQueue == null) throw new ArgumentNullException("buildQueue");
            if (snapshot == null) throw new ArgumentNullException("snapshot");

            this.client = client;
            this.buildQueue = buildQueue;
            this.snapshot = snapshot;
        }
        #endregion

        #region Public properties
        #region BuildQueue
        /// <summary>
        /// The queue this request belongs to.
        /// </summary>
        public BuildQueue BuildQueue
        {
            get { return buildQueue; }
        }
        #endregion

        #region ProjectName
        /// <summary>
        /// The name of the project this request is for.
        /// </summary>
        public string Name
        {
            get { return InnerBuildQueueRequest.ProjectName; }
        }
        #endregion

        #region Project
        /// <summary>
        /// The project that this request is for.
        /// </summary>
        public Project Project
        {
            get { return buildQueue.Server.FindProject(InnerBuildQueueRequest.ProjectName); }
        }
        #endregion

        #region Activity
        /// <summary>
        /// The current activity.
        /// </summary>
        public ProjectActivity Activity
        {
            get { return InnerBuildQueueRequest.Activity; }
        }
        #endregion

        #region RequestTime
        /// <summary>
        /// The date and time this request was added.
        /// </summary>
        public DateTime RequestTime
        {
            get { return InnerBuildQueueRequest.RequestTime; }
        }
        #endregion
        #endregion

        #region Public methods
        #region Update()
        /// <summary>
        /// Updates the details on a build queue.
        /// </summary>
        /// <param name="value">The new build queue details.</param>
        public void Update(QueuedRequestSnapshot value)
        {
            // Validate the arguments
            if (value == null) throw new ArgumentNullException("value");

            // Find all the changed properties
            var changes = new List<string>();
            if (snapshot.Activity != value.Activity) changes.Add("Activity");
            if (snapshot.RequestTime != value.RequestTime) changes.Add("RequestTime");

            // Make the actual change
            snapshot = value;

            // Fire any change notifications
            foreach (var change in changes)
            {
                FirePropertyChanged(change);
            }
        }
        #endregion

        #region Equals()
        /// <summary>
        /// Compares if two objects are the same.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as BuildQueueRequest);
        }

        /// <summary>
        /// Compares if two build requests are the same.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Equals(BuildQueueRequest obj)
        {
            if (obj == null) return false;
            return (obj.Project.Equals(Project) &&
                obj.RequestTime.Equals(RequestTime));
        }
        #endregion

        #region GetHashCode()
        /// <summary>
        /// Get the hash code for this request.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (this.BuildQueue == null ? 0 : this.BuildQueue.GetHashCode()) +
                (this.Name ?? string.Empty).GetHashCode() + 
                (InnerBuildQueueRequest == null ? 0 : this.RequestTime.GetHashCode());
        }
        #endregion
        #endregion

        #region Public events
        #region PropertyChanged
        /// <summary>
        /// A property has been changed on this project.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #endregion

        #region Protected properties
        #region InnerBuildQueueRequest
        /// <summary>
        /// The underlying build queue status.
        /// </summary>
        protected QueuedRequestSnapshot InnerBuildQueueRequest
        {
            get { return snapshot; }
        }
        #endregion
        #endregion

        #region Protected methods
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
