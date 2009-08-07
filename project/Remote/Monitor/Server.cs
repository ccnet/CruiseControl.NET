using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.Remote.Monitor
{
    /// <summary>
    /// A monitor to watch a remote server.
    /// </summary>
    public class Server
        : IDisposable, INotifyPropertyChanged, IEquatable<Server>
    {
        #region Private fields
        private Dictionary<string, Project> projects = new Dictionary<string, Project>();
        private Dictionary<string, BuildQueue> buildQueues = new Dictionary<string, BuildQueue>();
        private ReaderWriterLock syncLock = new ReaderWriterLock();
        private IServerWatcher watcher;
        private CruiseServerClientBase client;
        private Exception exception;
        private Version version;
        private DataBag data = new DataBag();
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="Server"/> with the default watcher.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        public Server(string address)
        {
            if (string.IsNullOrEmpty(address)) throw new ArgumentNullException("address");
            var factory = new CruiseServerClientFactory();
            var client = factory.GenerateClient(address);
            InitialiseServer(client, new ManualServerWatcher(client));
        }

        /// <summary>
        /// Initialise a new <see cref="Server"/> with the default watcher.
        /// </summary>
        /// <param name="address">The address of the server.</param>
        /// <param name="settings">The start-up settings to use.</param>
        public Server(string address, ClientStartUpSettings settings)
        {
            if (string.IsNullOrEmpty(address)) throw new ArgumentNullException("address");
            var factory = new CruiseServerClientFactory();
            var client = factory.GenerateClient(address, settings);
            InitialiseServer(client, new ManualServerWatcher(client));
        }

        /// <summary>
        /// Initialise a new <see cref="Server"/> with the default watcher.
        /// </summary>
        /// <param name="client">The underlying client.</param>
        public Server(CruiseServerClientBase client)
        {
            if (client == null) throw new ArgumentNullException("client");
            InitialiseServer(client, new ManualServerWatcher(client));
        }

        /// <summary>
        /// Initialise a new <see cref="Server"/> with a watcher and a client.
        /// </summary>
        /// <param name="client">The underlying client.</param>
        /// <param name="watcher">The watcher to use.</param>
        public Server(CruiseServerClientBase client, IServerWatcher watcher)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (watcher == null) throw new ArgumentNullException("watcher");
            InitialiseServer(client, watcher);
        }
        #endregion

        #region Public properties
        #region Name
        /// <summary>
        /// The name of the server.
        /// </summary>
        public string Name
        {
            get { return client.TargetServer; }
        }
        #endregion

        #region Projects
        /// <summary>
        /// The projects for the server.
        /// </summary>
        public IEnumerable<Project> Projects
        {
            get
            {
                syncLock.AcquireReaderLock(5000);
                try
                {
                    return projects.Values;
                }
                finally
                {
                    syncLock.ReleaseReaderLock();
                }
            }
        }
        #endregion

        #region BuildQueues
        /// <summary>
        /// The build queues for the server.
        /// </summary>
        public IEnumerable<BuildQueue> BuildQueues
        {
            get
            {
                syncLock.AcquireReaderLock(5000);
                try
                {
                    return buildQueues.Values;
                }
                finally
                {
                    syncLock.ReleaseReaderLock();
                }
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
                    foreach (var project in projects.Values)
                    {
                        project.Exception = value;
                    }
                }
            }
        }
        #endregion

        #region Client
        /// <summary>
        /// The underlying server client.
        /// </summary>
        public CruiseServerClientBase Client
        {
            get { return client; }
        }
        #endregion

        #region Watcher
        /// <summary>
        /// The underlying watcher.
        /// </summary>
        public IServerWatcher Watcher
        {
            get { return watcher; }
        }
        #endregion

        #region Version
        /// <summary>
        /// The current version of the server.
        /// </summary>
        public Version Version
        {
            get { return version; }
        }
        #endregion

        #region IsLoggedIn
        /// <summary>
        /// Is there a user logged in (i.e. does the client has a valid session.)
        /// </summary>
        public bool IsLoggedIn
        {
            get { return !string.IsNullOrEmpty(client.SessionToken); }
        }
        #endregion

        #region Data
        /// <summary>
        /// Gets the data bag.
        /// </summary>
        public DataBag Data
        {
            get { return Data; }
        }
        #endregion
        #endregion

        #region Public methods
        #region Refresh()
        /// <summary>
        /// Force a refresh of the server status.
        /// </summary>
        public virtual void Refresh()
        {
            watcher.Refresh();
        }
        #endregion

        #region Dispose()
        /// <summary>
        /// Cleans up when this server is no longer needed.
        /// </summary>
        public void Dispose()
        {
            var disposableWatcher = watcher as IDisposable;
            if (disposableWatcher != null)
            {
                disposableWatcher.Dispose();
            }
        }
        #endregion

        #region FindProject()
        /// <summary>
        /// Attempt to find a project by its name.
        /// </summary>
        /// <param name="name">The name of the project to find.</param>
        /// <returns>The project if found, null otherwise.</returns>
        public Project FindProject(string name)
        {
            if (projects.ContainsKey(name)) return projects[name];
            return null;
        }
        #endregion

        #region FindBuildQueue()
        /// <summary>
        /// Attempt to find a build queue by its name.
        /// </summary>
        /// <param name="name">The name of the build queue to find.</param>
        /// <returns>The build queue if found, null otherwise.</returns>
        public BuildQueue FindBuildQueue(string name)
        {
            if (buildQueues.ContainsKey(name)) return buildQueues[name];
            return null;
        }
        #endregion

        #region Login()
        /// <summary>
        /// Attempt to login a user.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Login(string userName, string password)
        {
            // Generate the credentials
            var credentials = new List<NameValuePair>();
            if (!string.IsNullOrEmpty(userName))
            {
                credentials.Add(
                    new NameValuePair(
                        LoginRequest.UserNameCredential,
                        userName));
            }
            if (!string.IsNullOrEmpty(password))
            {
                credentials.Add(
                    new NameValuePair(
                        LoginRequest.PasswordCredential,
                        password));
            }

            // Send the login request
            try
            {
                var result = client.Login(credentials);
                if (result) FireLoginChanged();
                return result;
            }
            catch (Exception error)
            {
                throw new CommunicationsException("An unexpected error has occurred", error);
            }
        }
        #endregion

        #region Logout
        /// <summary>
        /// Logout the current user.
        /// </summary>
        public void Logout()
        {
            if (IsLoggedIn)
            {
                Client.Logout();
                FireLoginChanged();
            }
        }
        #endregion

        #region Equals()
        /// <summary>
        /// Compare if two servers are the same.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Server);
        }

        /// <summary>
        /// Compare if two servers are the same.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool Equals(Server obj)
        {
            if (obj == null) return false;
            return (obj.Client.Address == Client.Address) &&
                (obj.Client.TargetServer == Client.TargetServer);
        }
        #endregion

        #region GetHashCode()
        /// <summary>
        /// Return the hash code for this server.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Client.TargetServer.GetHashCode() + 
                Client.Address.GetHashCode();
        }
        #endregion
        #endregion

        #region Public events
        #region ProjectAdded
        /// <summary>
        /// A new project has been added.
        /// </summary>
        public event EventHandler<ProjectChangedArgs> ProjectAdded;
        #endregion

        #region ProjectRemoved
        /// <summary>
        /// An existing project has been removed.
        /// </summary>
        public event EventHandler<ProjectChangedArgs> ProjectRemoved;
        #endregion

        #region BuildQueueAdded
        /// <summary>
        /// A new build queue has been added.
        /// </summary>
        public event EventHandler<BuildQueueChangedArgs> BuildQueueAdded;
        #endregion

        #region BuildQueueRemoved
        /// <summary>
        /// An existing build queue has been removed.
        /// </summary>
        public event EventHandler<BuildQueueChangedArgs> BuildQueueRemoved;
        #endregion
        
        #region PropertyChanged
        /// <summary>
        /// A property has been changed on this project.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region LoginChanged
        /// <summary>
        /// The login status for the underlying client has changed.
        /// </summary>
        public event EventHandler LoginChanged;
        #endregion
        #endregion

        #region Protected methods
        #region FireProjectAdded()
        /// <summary>
        /// Fires the <see cref="ProjectAdded"/> event.
        /// </summary>
        /// <param name="project">The project that was added.</param>
        protected void FireProjectAdded(Project project)
        {
            if (ProjectAdded != null)
            {
                var args = new ProjectChangedArgs(project);
                ProjectAdded(this, args);
            }
        }
        #endregion

        #region FireProjectRemoved()
        /// <summary>
        /// Fires the <see cref="ProjectRemoved"/> event.
        /// </summary>
        /// <param name="project">The project that was added.</param>
        protected void FireProjectRemoved(Project project)
        {
            if (ProjectRemoved != null)
            {
                var args = new ProjectChangedArgs(project);
                ProjectRemoved(this, args);
            }
        }
        #endregion

        #region FireBuildQueueAdded()
        /// <summary>
        /// Fires the <see cref="BuildQueueAdded"/> event.
        /// </summary>
        /// <param name="buildQueue">The build queue that was added.</param>
        protected void FireBuildQueueAdded(BuildQueue buildQueue)
        {
            if (BuildQueueAdded != null)
            {
                var args = new BuildQueueChangedArgs(buildQueue);
                BuildQueueAdded(this, args);
            }
        }
        #endregion

        #region FireBuildQueueRemoved()
        /// <summary>
        /// Fires the <see cref="BuildQueueRemoved"/> event.
        /// </summary>
        /// <param name="buildQueue">The build queue that was added.</param>
        protected void FireBuildQueueRemoved(BuildQueue buildQueue)
        {
            if (BuildQueueRemoved != null)
            {
                var args = new BuildQueueChangedArgs(buildQueue);
                BuildQueueRemoved(this, args);
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

        #region FireLoginChanged()
        /// <summary>
        /// Fire the <see cref="LoginChanged"/> event.
        /// </summary>
        protected void FireLoginChanged()
        {
            if (LoginChanged != null)
            {
                LoginChanged(this, EventArgs.Empty);
            }
        }
        #endregion

        #region OnWatcherUpdate()
        /// <summary>
        /// Update the status based on the latest snapshot.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnWatcherUpdate(object sender, ServerUpdateArgs e)
        {
            var newProjects = new List<Project>();
            var oldProjects = new List<Project>();
            var newQueues = new List<BuildQueue>();
            var oldQueues = new List<BuildQueue>();
            var projectValues = new Dictionary<Project, ProjectStatus>();
            var queueValues = new Dictionary<BuildQueue, QueueSnapshot>();

            syncLock.AcquireWriterLock(10000);
            try
            {
                Exception = e.Exception;
                if (e.Exception == null)
                {
                    // Check for any project differences
                    var oldProjectNames = new List<string>(projects.Keys);
                    foreach (var project in e.Snapshot.ProjectStatuses)
                    {
                        // Check if this project has already been loaded
                        var projectName = project.Name;
                        if (oldProjectNames.Contains(projectName))
                        {
                            projectValues.Add(projects[projectName], project);
                            oldProjectNames.Remove(projectName);
                        }
                        else
                        {
                            // Otherwise this is a new project
                            var newProject = new Project(client, this, project);
                            newProjects.Add(newProject);
                        }
                    }

                    // Check for any queue differences
                    var oldQueueNames = new List<string>(buildQueues.Keys);
                    foreach (var queue in e.Snapshot.QueueSetSnapshot.Queues)
                    {
                        // Check if this queue has already been loaded
                        var queueName = queue.QueueName;
                        if (oldQueueNames.Contains(queueName))
                        {
                            queueValues.Add(buildQueues[queueName], queue);
                            oldQueueNames.Remove(queueName);
                        }
                        else
                        {
                            // Otherwise this is a new queue
                            var newQueue = new BuildQueue(client, this, queue);
                            newQueues.Add(newQueue);
                        }
                    }

                    // Store the old projects and queues
                    foreach (var project in oldProjectNames)
                    {
                        oldProjects.Add(projects[project]);
                    }
                    foreach (var queue in oldQueueNames)
                    {
                        oldQueues.Add(buildQueues[queue]);
                    }

                    // Perform the actual update
                    foreach (var project in oldProjectNames)
                    {
                        projects.Remove(project);
                    }
                    foreach (var queue in oldQueueNames)
                    {
                        buildQueues.Remove(queue);
                    }
                    foreach (var project in newProjects)
                    {
                        projects.Add(project.Name, project);
                    }
                    foreach (var queue in newQueues)
                    {
                        buildQueues.Add(queue.Name, queue);
                    }
                }
            }
            finally
            {
                syncLock.ReleaseWriterLock();
            }

            // Update all the projects and queues
            foreach (var value in projectValues)
            {
                value.Key.Update(value.Value);
            }
            foreach (var value in queueValues)
            {
                value.Key.Update(value.Value);
            }

            // Tell any listeners about any changes
            foreach (var project in newProjects)
            {
                FireProjectAdded(project);
            }
            foreach (var queue in newQueues)
            {
                FireBuildQueueAdded(queue);
            }
            foreach (var project in oldProjects)
            {
                FireProjectRemoved(project);
            }
            foreach (var queue in oldQueues)
            {
                FireBuildQueueRemoved(queue);
            }
        }
        #endregion
        #endregion

        #region Private methods
        #region InitialiseServer()
        /// <summary>
        /// Initialise the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="watcher"></param>
        private void InitialiseServer(CruiseServerClientBase client, IServerWatcher watcher)
        {
            this.watcher = watcher;
            this.watcher.Update += OnWatcherUpdate;
            this.client = client;

            try
            {
                client.ProcessSingleAction(s =>
                {
                    version = new Version(client.GetServerVersion());
                }, client);
            }
            catch
            {
                // This means there will be no version for the server
            }
        }
        #endregion
        #endregion
    }
}
