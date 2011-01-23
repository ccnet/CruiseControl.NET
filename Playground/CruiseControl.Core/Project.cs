namespace CruiseControl.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows.Markup;
    using System.Xaml;
    using CruiseControl.Common.Messages;
    using CruiseControl.Core.Interfaces;
    using Ninject;
    using NLog;

    /// <summary>
    /// A project.
    /// </summary>
    [ContentProperty("Tasks")]
    public class Project
        : ServerItem
    {
        #region Private fields
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private Thread mainThread;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class.
        /// </summary>
        public Project()
        {
            this.InitialiseProperties(new Task[0]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="tasks">The tasks.</param>
        public Project(string name, params Task[] tasks)
            : base(name)
        {
            this.InitialiseProperties(tasks);
        }
        #endregion

        #region Public properties
        #region State
        /// <summary>
        /// Gets the project state.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ProjectState State { get; private set; }
        #endregion

        #region Tasks
        /// <summary>
        /// Gets or sets the tasks.
        /// </summary>
        /// <value>The tasks.</value>
        public IList<Task> Tasks { get; private set; }
        #endregion

        #region SourceControl
        /// <summary>
        /// Gets the source control blocks to use.
        /// </summary>
        public IList<SourceControlBlock> SourceControl { get; private set; }
        #endregion

        #region Triggers
        /// <summary>
        /// Gets the triggers.
        /// </summary>
        public IList<Trigger> Triggers { get; private set; }
        #endregion

        #region UniversalName
        /// <summary>
        /// Gets the universal name of this item.
        /// </summary>
        /// <value>
        /// The universal name.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string UniversalName
        {
            get
            {
                var name = (this.Server == null ? "urn:ccnet:" : this.Server.UniversalName) +
                           ":" + this.Name ?? string.Empty;
                return name;
            }
        }
        #endregion

        #region MainThreadException
        /// <summary>
        /// Gets any breaking exception on the main thread.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Exception MainThreadException { get; private set; }
        #endregion

        #region TaskExecutionFactory
        /// <summary>
        /// Gets or sets the task context factory.
        /// </summary>
        /// <value>
        /// The task context factory.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Inject]
        public ITaskExecutionFactory TaskExecutionFactory { get; set; }
        #endregion

        #region FileSystem
        /// <summary>
        /// Gets or sets the file system.
        /// </summary>
        /// <value>
        /// The file system.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Inject]
        public IFileSystem FileSystem { get; set; }
        #endregion

        #region Clock
        /// <summary>
        /// Gets or sets the clock.
        /// </summary>
        /// <value>
        /// The clock.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Inject]
        public IClock Clock { get; set; }
        #endregion

        #region PersistedState
        /// <summary>
        /// Gets the state that is persisted after every integration.
        /// </summary>
        /// <value>
        /// The persisted state.
        /// </value>
        public PersistedProjectState PersistedState { get; private set; }
        #endregion
        #endregion

        #region Public methods
        #region Locate()
        /// <summary>
        /// Locates an item by its universal name.
        /// </summary>
        /// <param name="name">The universal name of the item.</param>
        /// <returns>
        /// The item if found; <c>null</c> otherwise.
        /// </returns>
        public override object Locate(string name)
        {
            // To complicate things there are two valid universal names for a project - the project as 
            // root item name and project within a structure path name - therefore we need to check 
            // both of them
            var thisName = this.UniversalName;
            var fullName = this.Host == null
                               ? string.Empty
                               : this.Host.UniversalName + ":" + this.Name ?? string.Empty;

            // If the server part does not match then this is for a different server
            if (!name.StartsWith(thisName, StringComparison.CurrentCultureIgnoreCase) &&
                !name.StartsWith(fullName, StringComparison.CurrentCultureIgnoreCase))
            {
                return null;
            }

            // If the lengths are the same then matching the server itself
            if ((name.Length == thisName.Length) || (name.Length == fullName.Length))
            {
                return this;
            }

            // Check all the children
            var item = LocateWithinCollection(null, this.Tasks, name);
            item = LocateWithinCollection(item, this.SourceControl, name);
            item = LocateWithinCollection(item, this.Triggers, name);
            return item;
        }
        #endregion

        #region CanStart()
        /// <summary>
        /// Determines whether this instance can start.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance can start; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CanStart()
        {
            // TODO: Add the logic for checking if the project should be started
            return this.State != ProjectState.Running &&
                this.State != ProjectState.Starting;
        }
        #endregion

        #region CanStop()
        /// <summary>
        /// Determines whether this instance can stop.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance can stop; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CanStop()
        {
            return this.State != ProjectState.Stopped &&
                   this.State != ProjectState.Stopping;
        }
        #endregion

        #region AskToIntegrate()
        /// <summary>
        /// Asks if an item can integrate.
        /// </summary>
        /// <param name="context">The context to use.</param>
        public override void AskToIntegrate(IntegrationContext context)
        {
            if (this.Host != null)
            {
                logger.Debug("Asking host if {0} can integrate", this.Name);
                this.Host.AskToIntegrate(context);
            }
        }
        #endregion

        #region Start()
        /// <summary>
        /// Starts this project.
        /// </summary>
        public void Start()
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                var message = "Cannot start a project without a name";
                logger.Error(message);
                // TODO: Use a custom exception
                throw new InvalidOperationException(message);
            }

            if (this.State != ProjectState.Stopped)
            {
                var message = "The project '" +
                    this.Name +
                    "' must be in a stopped state before it can be started";
                logger.Warn(message);
                // TODO: Use a custom exception
                throw new InvalidOperationException(message);
            }

            // Make sure the state has been loaded
            if (this.PersistedState == null)
            {
                this.LoadPersistedState();
            }

            // Start up the project thread
            logger.Info("Starting project '{0}'", this.Name);
            this.State = ProjectState.Starting;
            this.mainThread = new Thread(this.Main)
                                  {
                                      Name = this.Name,
                                      IsBackground = true
                                  };
            this.mainThread.Start();
        }
        #endregion

        #region Stop()
        /// <summary>
        /// Stops this project.
        /// </summary>
        public void Stop()
        {
            if (this.State != ProjectState.Running)
            {
                var message = "The project '" +
                    (this.Name ?? string.Empty) +
                    "'must be in a running state before it can be stopped";
                // TODO: Use a custom exception
                throw new InvalidOperationException(message);
            }

            logger.Info("Stopping project '{0}'", this.Name);
            this.State = ProjectState.Stopping;
        }
        #endregion

        #region Integrate()
        /// <summary>
        /// Performs an integration.
        /// </summary>
        /// <param name="request">The request.</param>
        public virtual IntegrationStatus Integrate(IntegrationRequest request)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var status = IntegrationStatus.Unknown;
            logger.Debug("Initialising integration for '{0}'", this.Name);
            if (this.InitialiseForIntegration())
            {
                logger.Debug("Running tasks for '{0}'", this.Name);
                var context = this.TaskExecutionFactory.StartNew(this, request);
                try
                {
                    this.RunTasks(context, this.Tasks);
                }
                finally
                {
                    context.Complete();
                    status = context.CurrentStatus;
                }
            }

            logger.Debug("Cleaning up after integration for '{0}'", this.Name);
            this.CleanUpAfterIntegration();

            stopwatch.Stop();
            logger.Debug("Total duration for integration for '{0}' was {1:#,##0.000}s",
                this.Name,
                (double)stopwatch.ElapsedMilliseconds / 1000);
            return status;
        }
        #endregion

        #region Validate()
        /// <summary>
        /// Validates this project after it has been loaded.
        /// </summary>
        public override void Validate(IValidationLog validationLog)
        {
            logger.Debug("Validating project '{0}'", this.Name ?? string.Empty);
            base.Validate(validationLog);
            foreach (var task in this.Tasks)
            {
                task.Validate(validationLog);
            }

            foreach (var trigger in this.Triggers)
            {
                trigger.Validate(validationLog);
            }

            foreach (var sourceControl in this.SourceControl)
            {
                sourceControl.Validate(validationLog);
            }
        }
        #endregion

        #region ListProjects()
        /// <summary>
        /// Lists the projects within this item.
        /// </summary>
        /// <returns>The projects within this item.</returns>
        public override IEnumerable<Project> ListProjects()
        {
            var projects = new[] { this };
            return projects;
        }
        #endregion

        #region LoadPersistedState()
        /// <summary>
        /// Loads the persisted state for this project.
        /// </summary>
        public virtual void LoadPersistedState()
        {
            var configFile = this.GenerateProjectPath("project.state");
            if (this.FileSystem.CheckIfFileExists(configFile))
            {
                logger.Debug("Loading project state for '{0}'", this.Name);
                using (var stream = this.FileSystem.OpenFileForRead(configFile))
                {
                    this.PersistedState = XamlServices.Load(stream) as PersistedProjectState;
                }
            }

            if (this.PersistedState == null)
            {
                logger.Debug("Starting new project state for '{0}'", this.Name);
                this.PersistedState = new PersistedProjectState();
            }
        }
        #endregion

        #region SavePersistedState()
        /// <summary>
        /// Saves the persisted state for this project.
        /// </summary>
        public virtual void SavePersistedState()
        {
            if (this.PersistedState != null)
            {
                logger.Debug("Saving project state for '{0}'", this.Name);
                var configFile = this.GenerateProjectPath("project.state");
                using (var stream = this.FileSystem.OpenFileForWrite(configFile))
                {
                    this.PersistedState.Save(stream);
                }
            }
        }
        #endregion
        #endregion

        #region Actions
        #region Start()
        /// <summary>
        /// Starts the project if it is no already started.
        /// </summary>
        /// <param name="request">The request containing the details.</param>
        /// <returns>A message containing the response details.</returns>
        [RemoteAction]
        [Description("Starts a project if it is not already started.")]
        public virtual ProjectMessage Start(ProjectMessage request)
        {
            this.Start();
            var response = new ProjectMessage
            {
                ProjectName = this.Name
            };
            return response;
        }
        #endregion

        #region Stop()
        /// <summary>
        /// Stops the project if it is not already stopped.
        /// </summary>
        /// <param name="request">The request containing the details.</param>
        /// <returns>A message containing the response details.</returns>
        [RemoteAction]
        [Description("Stops a project if it is not already stopped.")]
        public virtual ProjectMessage Stop(ProjectMessage request)
        {
            this.Stop();
            var response = new ProjectMessage
                               {
                                   ProjectName = this.Name
                               };
            return response;
        }
        #endregion
        #endregion

        #region Protected methods
        #region OnStarted()
        /// <summary>
        /// Extension point for when the project has started.
        /// </summary>
        protected virtual void OnStarted()
        {
        }
        #endregion

        #region OnStopped()
        /// <summary>
        /// Extension point for when the project has stopped.
        /// </summary>
        protected virtual void OnStopped()
        {
        }
        #endregion

        #region RunTasks()
        /// <summary>
        /// Runs a set of tasks.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="tasks">The tasks.</param>
        protected virtual void RunTasks(TaskExecutionContext context, IEnumerable<Task> tasks)
        {
            var enumerator = tasks.GetEnumerator();
            while (true)
            {
                try
                {
                    var hasTask = enumerator.MoveNext();
                    if (!hasTask)
                    {
                        break;
                    }
                }
                catch (Exception error)
                {
                    logger.WarnException("Unhandled exception caught in task", error);
                    break;
                }

                var task = enumerator.Current;
                if (task.CanRun(context))
                {
                    var taskContext = context.StartChild(task);
                    try
                    {
                        this.RunTasks(context, task.Run(taskContext));
                    }
                    finally
                    {
                        taskContext.Complete();
                    }
                }
                else
                {
                    task.Skip(context);
                }
            }
        }
        #endregion

        #region InitialiseForIntegration()
        /// <summary>
        /// Initialises this project for an integration.
        /// </summary>
        protected virtual bool InitialiseForIntegration()
        {
            try
            {
                foreach (var sourceControl in this.SourceControl)
                {
                    sourceControl.Initialise();
                }

                foreach (var task in this.Tasks)
                {
                    task.Initialise();
                }

                return true;
            }
            catch (Exception error)
            {
                logger.Error("An unhandled error occurred during initialisation", error);
                return false;
            }
        }
        #endregion

        #region CleanUpAfterIntegration()
        /// <summary>
        /// Cleans up after an integration.
        /// </summary>
        protected virtual void CleanUpAfterIntegration()
        {
            foreach (var task in this.Tasks)
            {
                try
                {
                    task.CleanUp();
                }
                catch (Exception error)
                {
                    logger.ErrorException("Unhandled error during task clean up", error);
                }
            }

            foreach (var sourceControl in this.SourceControl)
            {
                try
                {
                    sourceControl.CleanUp();
                }
                catch (Exception error)
                {
                    logger.ErrorException("Unhandled error during source control clean up", error);
                }
            }

            foreach (var trigger in this.Triggers)
            {
                try
                {
                    trigger.Reset();
                }
                catch (Exception error)
                {
                    logger.ErrorException("Unhandled error during trigger clean up", error);
                }
            }
        }
        #endregion
        #endregion

        #region Private methods
        #region LocateWithinCollection()
        /// <summary>
        /// Locates the within collection.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="name">The name.</param>
        /// <returns>
        /// The item if found; <c>null</c> otherwise.
        /// </returns>
        private static object LocateWithinCollection(
            object item,
            IEnumerable<ProjectItem> collection,
            string name)
        {
            if (item != null)
            {
                return item;
            }

            foreach (var child in collection)
            {
                item = child.Locate(name);
                if (item != null)
                {
                    break;
                }
            }

            return item;
        }
        #endregion

        #region Main()
        /// <summary>
        /// The main loop for the project.
        /// </summary>
        private void Main()
        {
            // Initialise
            this.MainThreadException = null;
            this.State = ProjectState.Running;
            foreach (var trigger in this.Triggers)
            {
                trigger.Initialise();
            }

            this.OnStarted();
            logger.Debug("Project '{0}' has started", this.Name);
            try
            {
                try
                {
                    while (this.State == ProjectState.Running)
                    {
                        // Sleep a while so we don't overwork the server - assuming our minimum accuracy
                        // is one second hence firing at 2Hz
                        Thread.Sleep(500);
                        CheckForIntegration();
                    }
                }
                catch (Exception error)
                {
                    logger.ErrorException("An error has occurred in project '" + this.Name + "'", error);
                    this.MainThreadException = error;
                }

                // Clean up
                foreach (var trigger in this.Triggers)
                {
                    trigger.CleanUp();
                }

                this.OnStopped();
            }
            finally
            {
                // Make sure this project is marked as stopped no matter how it is stopped
                this.State = ProjectState.Stopped;
                logger.Debug("Project '{0}' has stopped", this.Name);
            }
        }
        #endregion

        #region CheckForIntegration()
        /// <summary>
        /// Checks if an integration can proceed.
        /// </summary>
        private void CheckForIntegration()
        {
            // Get the first trigger that has been tripped
            var request = this.Triggers
                .Select(t => t.Check())
                .FirstOrDefault(r => r != null);
            if (request != null)
            {
                logger.Info(
                    "Received integration request from '{0}' for '{1}'",
                    request.SourceTrigger,
                    this.Name);

                // Check if we can integrate
                var context = new IntegrationContext(this);
                this.AskToIntegrate(context);
                // TODO: make the time out configurable
                if (context.Wait(TimeSpan.FromDays(7)))
                {
                    // Make sure there is always a state object
                    if (this.PersistedState == null)
                    {
                        this.PersistedState = new PersistedProjectState();
                    }

                    // Perform the actual integration
                    logger.Info("Starting integration for '{0}'", this.Name);
                    var startTime = this.Clock.Now;
                    var status = IntegrationStatus.Unknown;
                    try
                    {
                        status = this.Integrate(request);
                    }
                    catch (Exception error)
                    {
                        logger.ErrorException("An unexpected error crashed the integration", error);
                        status = IntegrationStatus.Error;
                    }
                    finally
                    {
                        this.PersistedState.LastIntegration = new IntegrationSummary
                                                                  {
                                                                      StartTime = startTime,
                                                                      FinishTime = this.Clock.Now,
                                                                      Status = status
                                                                  };
                        this.SavePersistedState();
                    }
                    logger.Info("Completed integration for '{0}'", this.Name);
                }
                else
                {
                    logger.Info("Cancelling integration for '{0}'", this.Name);
                }
            }
        }
        #endregion

        #region InitialiseProperties()
        /// <summary>
        /// Initialises this project.
        /// </summary>
        /// <param name="tasks">The tasks.</param>
        private void InitialiseProperties(IEnumerable<Task> tasks)
        {
            this.State = ProjectState.Stopped;
            this.Tasks = this.InitialiseCollection(tasks);
            this.SourceControl = this.InitialiseCollection<SourceControlBlock>(null);
            this.Triggers = this.InitialiseCollection<Trigger>(null);
        }
        #endregion

        #region InitialiseCollection()
        /// <summary>
        /// Initialises a collection.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="items">The items.</param>
        /// <returns>The initialised collection.</returns>
        private ObservableCollection<TItem> InitialiseCollection<TItem>(IEnumerable<TItem> items)
            where TItem : ProjectItem
        {
            var actualItems = items ?? new TItem[0];
            var collection = new ObservableCollection<TItem>(actualItems);
            foreach (var item in actualItems)
            {
                item.Project = this;
            }

            collection.CollectionChanged += this.UpdateProjectItem;
            return collection;
        }
        #endregion

        #region UpdateProjectItem()
        /// <summary>
        /// Updates the project item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void UpdateProjectItem(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (ProjectItem child in e.OldItems ?? new ProjectItem[0])
            {
                child.Project = null;
            }

            foreach (ProjectItem child in e.NewItems ?? new ProjectItem[0])
            {
                child.Project = this;
            }
        }
        #endregion

        #region GenerateProjectPath()
        /// <summary>
        /// Generates a path to a relative file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>
        /// The path for the file.
        /// </returns>
        private string GenerateProjectPath(string file)
        {
            var path = Path.Combine(
                Environment.CurrentDirectory,
                this.Name,
                file);
            return path;
        }
        #endregion
        #endregion
    }
}
