using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ThoughtWorks.CruiseControl.Remote.Monitor
{
    /// <summary>
    /// A project that is being monitored on a remote server.
    /// </summary>
    public class Project
        : INotifyPropertyChanged
    {
        #region Private fields
        private readonly CruiseServerClientBase client;
        private readonly Server server;
        private ProjectStatus project;
        private Exception exception;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new project.
        /// </summary>
        /// <param name="client">The underlying client.</param>
        /// <param name="server">The server this project belongs to.</param>
        /// <param name="project">The actual project details.</param>
        public Project(CruiseServerClientBase client,Server server,  ProjectStatus project)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (server == null) throw new ArgumentNullException("server");
            if (project == null) throw new ArgumentNullException("project");

            this.client = client;
            this.server = server;
            this.project = project;
        }
        #endregion

        #region Public properties
        #region Server
        /// <summary>
        /// The server this project belongs to.
        /// </summary>
        public Server Server
        {
            get { return server; }
        }
        #endregion

        #region Name
        /// <summary>
        /// The name of the project.
        /// </summary>
        public string Name
        {
            get { return InnerProject.Name; }
        }
        #endregion

        #region BuildStage
        /// <summary>
        /// The current build stage.
        /// </summary>
        public string BuildStage
        {
            get { return InnerProject.BuildStage; }
        }
        #endregion

        #region Status
        /// <summary>
        /// The current project status.
        /// </summary>
        public ProjectIntegratorState Status
        {
            get { return InnerProject.Status; }
        }
        #endregion

        #region BuildStatus
        /// <summary>
        /// The current build status.
        /// </summary>
        public IntegrationStatus BuildStatus
        {
            get { return InnerProject.BuildStatus; }
        }
        #endregion

        #region Activity
        /// <summary>
        /// The current activity.
        /// </summary>
        public ProjectActivity Activity
        {
            get { return InnerProject.Activity; }
        }
        #endregion

        #region Description
        /// <summary>
        /// The description of the project.
        /// </summary>
        public string Description
        {
            get { return InnerProject.Description; }
        }
        #endregion

        #region Category
        /// <summary>
        /// The project category.
        /// </summary>
        public string Category
        {
            get { return InnerProject.Category; }
        }
        #endregion

        #region BuildQueue
        /// <summary>
        /// The build queue this project belongs to.
        /// </summary>
        public BuildQueue BuildQueue
        {
            get { return server.FindBuildQueue(InnerProject.Queue); }
        }
        #endregion

        #region Queue
        /// <summary>
        /// The name of the queue this project belongs to.
        /// </summary>
        public string Queue
        {
            get { return InnerProject.Queue; }
        }
        #endregion

        #region QueuePriority
        /// <summary>
        /// The priority of the project within the queue.
        /// </summary>
        public int QueuePriority
        {
            get { return InnerProject.QueuePriority; }
        }
        #endregion

        #region WebURL
        /// <summary>
        /// The URL for the project.
        /// </summary>
        public string WebURL
        {
            get { return InnerProject.WebURL; }
        }
        #endregion

        #region LastBuildDate
        /// <summary>
        /// The date and time the project was last built.
        /// </summary>
        public DateTime LastBuildDate
        {
            get { return InnerProject.LastBuildDate; }
        }
        #endregion

        #region LastBuildLabel
        /// <summary>
        /// The last build label (independent of the outcome of the build).
        /// </summary>
        public string LastBuildLabel
        {
            get { return InnerProject.LastBuildLabel; }
        }
        #endregion

        #region LastSuccessfulBuildLabel
        /// <summary>
        /// The last successful build label.
        /// </summary>
        public string LastSuccessfulBuildLabel
        {
            get { return InnerProject.LastSuccessfulBuildLabel; }
        }
        #endregion

        #region NextBuildTime
        /// <summary>
        /// The date and time of the next build check.
        /// </summary>
        public DateTime NextBuildTime
        {
            get { return InnerProject.NextBuildTime; }
        }
        #endregion

        #region Messages
        /// <summary>
        /// Any associated messages for the project.
        /// </summary>
        public Message[] Messages
        {
            get { return InnerProject.Messages; }
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
        #endregion

        #region Public methods
        #region Update()
        /// <summary>
        /// Updates the details on a project.
        /// </summary>
        /// <param name="value">The new project details.</param>
        public void Update(ProjectStatus value)
        {
            // Validate the arguments
            if (value == null) throw new ArgumentNullException("value");

            // Find all the changed properties
            var changes = new List<string>();
            if (project.Activity != value.Activity) changes.Add("Activity");
            if (project.BuildStage != value.BuildStage) changes.Add("BuildStage");
            if (project.BuildStatus != value.BuildStatus) changes.Add("BuildStatus");
            if (project.Category != value.Category) changes.Add("Category");
            if (project.Description != value.Description) changes.Add("Description");
            if (project.LastBuildDate != value.LastBuildDate) changes.Add("LastBuildDate");
            if (project.LastBuildLabel != value.LastBuildLabel) changes.Add("LastBuildLabel");
            if (project.LastSuccessfulBuildLabel != value.LastSuccessfulBuildLabel) changes.Add("LastSuccessfulBuildLabel");
            if (project.NextBuildTime != value.NextBuildTime) changes.Add("NextBuildTime");
            if (project.Queue != value.Queue) changes.Add("Queue");
            if (project.QueuePriority != value.QueuePriority) changes.Add("QueuePriority");
            if (project.Status != value.Status) changes.Add("Status");
            if (project.WebURL != value.WebURL) changes.Add("WebURL");
            if (project.Messages.Length != value.Messages.Length)
            {
                changes.Add("Messages");
            }
            else
            {
                var messageChanged = false;
                for (var loop = 0; loop < project.Messages.Length; loop++)
                {
                    messageChanged = (project.Messages[loop].Text != value.Messages[loop].Text);
                    if (messageChanged) break;
                }
                if (messageChanged) changes.Add("Messages");
            }

            // Make the actual change
            project = value;

            // Fire any change notifications
            foreach (var change in changes)
            {
                FirePropertyChanged(change);
            }
        }
        #endregion

        #region ForceBuild()
        /// <summary>
        /// Sends a force build request to the remote server.
        /// </summary>
        public void ForceBuild()
        {
            client.ForceBuild(InnerProject.Name);
        }

        /// <summary>
        /// Sends a force build request to the remote server.
        /// </summary>
        /// <param name="parameters">The parameters for the build.</param>
        public void ForceBuild(List<NameValuePair> parameters)
        {
            client.ForceBuild(InnerProject.Name, parameters);
        }
        #endregion

        #region AbortBuild()
        /// <summary>
        /// Sends an abort build request to the remote server.
        /// </summary>
        public void AbortBuild()
        {
            client.AbortBuild(InnerProject.Name);
        }
        #endregion

        #region Start()
        /// <summary>
        /// Sends a start project request to the remote server.
        /// </summary>
        public void Start()
        {
            client.StartProject(InnerProject.Name);
        }
        #endregion

        #region Stop()
        /// <summary>
        /// Sends a stop project request to the remote server.
        /// </summary>
        public void Stop()
        {
            client.StopProject(InnerProject.Name);
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
        #region InnerProject
        /// <summary>
        /// The underlying project status.
        /// </summary>
        protected ProjectStatus InnerProject
        {
            get { return project; }
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
