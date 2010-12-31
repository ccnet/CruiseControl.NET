namespace CruiseControl.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Threading;
    using System.Windows.Markup;
    using CruiseControl.Core.Interfaces;
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
        #endregion

        #region Public methods
        #region Trigger()
        /// <summary>
        /// Triggers an integration.
        /// </summary>
        public virtual void Trigger()
        {
            throw new NotImplementedException();
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

            logger.Info("Stopped project '{0}'", this.Name);
            this.State = ProjectState.Stopping;
        }
        #endregion

        #region Integrate()
        /// <summary>
        /// Performs an integration.
        /// </summary>
        public virtual void Integrate()
        {
            this.InitialiseForIntegration();
            var context = new TaskExecutionContext();
            this.RunTasks(context, this.Tasks);
            this.CleanUpAfterIntegration();
        }
        #endregion

        #region Validate()
        /// <summary>
        /// Validates this project after it has been loaded.
        /// </summary>
        public override void Validate(IValidationLog validationLog)
        {
            base.Validate(validationLog);
            foreach (var task in this.Tasks)
            {
                task.Validate(validationLog);
            }
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
            if (tasks == null)
            {
                return;
            }

            foreach (var task in tasks)
            {
                if (task.CanRun(context))
                {
                    this.RunTasks(context, task.Run(context));
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
        protected virtual void InitialiseForIntegration()
        {
            foreach (var task in this.Tasks)
            {
                task.Initialise();
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
                task.CleanUp();
            }
        }
        #endregion
        #endregion

        #region Private methods
        #region Main()
        /// <summary>
        /// The main loop for the project.
        /// </summary>
        private void Main()
        {
            this.State = ProjectState.Running;
            this.OnStarted();
            logger.Debug("Project '{0}' has started", this.Name);
            try
            {
                while (this.State == ProjectState.Running)
                {
                    // Sleep a while so we don't overwork the server - assuming our minimum accuracy
                    // is one second
                    Thread.Sleep(500);

                    // TODO: Check for any requests

                    // TODO: Integrate
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
        #endregion
    }
}
