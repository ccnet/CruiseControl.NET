using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Label;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Core.Publishers.Statistics;
using ThoughtWorks.CruiseControl.Core.Security;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote.Parameters;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// A project manages the workflow for 
    /// A project is the combination of source control providers,
    /// build tasks, publisher tasks, labellers, and state managers.
    /// </summary>
    /// <remarks>
    /// <code>
    /// <![CDATA[
    /// <project name="foo">
    ///		<state type="state"></state>
    ///		<sourcecontrol type="cvs" />
    ///		<tasks><nant /></tasks>
    ///		<publishers><xmllogger /></publishers>
    /// </project>
    /// ]]>
    /// </code>
    /// </remarks>
    [ReflectorType("project")]
    public class Project : ProjectBase, IProject, IIntegrationRunnerTarget, IIntegrationRepository,
        IConfigurationValidation, IStatusSnapshotGenerator, IParamatisedProject
    {
        private string webUrl = DefaultUrl();
        private string queueName = string.Empty;
        private int queuePriority = 0;
        private ISourceControl sourceControl = new NullSourceControl();
        private ILabeller labeller = new DefaultLabeller();
        private ITask[] tasks = new ITask[] { new NullTask() };
        private ITask[] publishers = new ITask[] { new XmlLogPublisher() };
        private ProjectActivity currentActivity = ProjectActivity.Sleeping;
        private IStateManager state = new FileStateManager(new SystemIoFileSystem());
        private IIntegrationResultManager integrationResultManager;
        private IIntegratable integratable;
        private QuietPeriod quietPeriod = new QuietPeriod(new DateTimeProvider());
        private ArrayList messages = new ArrayList();
        private int maxSourceControlRetries = 5;
        private IProjectAuthorisation security = new InheritedProjectAuthorisation();
        private ParameterBase[] parameters = new ParameterBase[0];
        private ProjectInitialState initialState = ProjectInitialState.Started;
        private ProjectStartupMode startupMode = ProjectStartupMode.UseLastState;
        private bool StopProjectOnReachingMaxSourceControlRetries = false;
        private Sourcecontrol.Common.SourceControlErrorHandlingPolicy sourceControlErrorHandling = Common.SourceControlErrorHandlingPolicy.ReportEveryFailure;
        private ProjectStatusSnapshot currentProjectStatus;
        private Dictionary<ITask, ItemStatus> currentProjectItems = new Dictionary<ITask, ItemStatus>();
        private Dictionary<SourceControlOperation, ItemStatus> sourceControlOperations = new Dictionary<SourceControlOperation, ItemStatus>();

        [ReflectorProperty("prebuild", Required = false)]
        public ITask[] PrebuildTasks = new ITask[0];

        public Project()
        {
            integrationResultManager = new IntegrationResultManager(this);
            integratable = new IntegrationRunner(integrationResultManager, this, quietPeriod);

            // Generates the initial snapshot
            currentProjectStatus = new ProjectStatusSnapshot();
            PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                switch (e.PropertyName)
                {
                    case "Name":
                        lock (currentProjectStatus)
                        {
                            currentProjectStatus.Name = Name;
                        }
                        break;
                    case "Description":
                        lock (currentProjectStatus)
                        {
                            currentProjectStatus.Description = Description;
                        }
                        break;
                }
            };
        }

        public Project(IIntegratable integratable)
            : this()
        {
            this.integratable = integratable;
        }

        [ReflectorProperty("security", InstanceTypeKey = "type", Required = false)]
        public IProjectAuthorisation Security
        {
            get { return security; }
            set { security = value; }
        }

        [ReflectorProperty("parameters", Required=false)]
        public ParameterBase[] Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        #region Links
        /// <summary>
        /// Link this project to other sites.
        /// </summary>
        [ReflectorProperty("linkedSites", Required = false)]
        public NameValuePair[] LinkedSites { get; set; }
        #endregion

        [ReflectorProperty("state", InstanceTypeKey = "type", Required = false), Description("State")]
        public IStateManager StateManager
        {
            get { return state; }
            set { state = value; }
        }

        [ReflectorProperty("webURL", Required = false)]
        public string WebURL
        {
            get { return webUrl; }
            set { webUrl = value; }
        }


        [ReflectorProperty("maxSourceControlRetries", Required = false)]
        public int MaxSourceControlRetries
        {
            get { return maxSourceControlRetries; }
            set { maxSourceControlRetries = value < 0 ? 0 : value; }
        }


        [ReflectorProperty("stopProjectOnReachingMaxSourceControlRetries", Required = false)]
        public bool stopProjectOnReachingMaxSourceControlRetries
        {
            get { return StopProjectOnReachingMaxSourceControlRetries; }
            set { StopProjectOnReachingMaxSourceControlRetries = value; }
        }


        [ReflectorProperty("sourceControlErrorHandling", Required = false)]
        public Sourcecontrol.Common.SourceControlErrorHandlingPolicy SourceControlErrorHandling
        {
            get { return sourceControlErrorHandling; }
            set { sourceControlErrorHandling = value; }
        }


        [ReflectorProperty("queue", Required = false)]
        public string QueueName
        {
            get
            {
                if (string.IsNullOrEmpty(queueName)) return Name;
                return queueName;
            }
            set { queueName = value.Trim(); }
        }

        [ReflectorProperty("queuePriority", Required = false)]
        public int QueuePriority
        {
            get { return queuePriority; }
            set { queuePriority = value; }
        }

        [ReflectorProperty("sourcecontrol", InstanceTypeKey = "type", Required = false)]
        public ISourceControl SourceControl
        {
            get { return sourceControl; }
            set { sourceControl = value; }
        }

        /// <summary>
        /// The list of build-completed publishers used by this project.  This property is
        /// intended to be set via Xml configuration.
        /// </summary>
        [ReflectorArray("publishers", Required = false)]
        public ITask[] Publishers
        {
            get { return publishers; }
            set { publishers = value; }
        }

        /// <summary>
        /// A period of time, in seconds.  When modifications are found within this period,
        /// a build (which would otherwise occur) is delayed until this many seconds have
        /// passed.  The intention is to allow a developer to complete a multi-stage
        /// checkin.
        /// </summary>
        [ReflectorProperty("modificationDelaySeconds", Required = false)]
        public double ModificationDelaySeconds
        {
            get { return quietPeriod.ModificationDelaySeconds; }
            set { quietPeriod.ModificationDelaySeconds = value; }
        }

        [ReflectorProperty("labeller", InstanceTypeKey = "type", Required = false)]
        public ILabeller Labeller
        {
            get { return labeller; }
            set { labeller = value; }
        }

        [ReflectorArray("tasks", Required = false)]
        public ITask[] Tasks
        {
            get { return tasks; }
            set { tasks = value; }
        }

        // Move this ideally
        public ProjectActivity Activity
        {
            get { return currentActivity; }
            set { currentActivity = value; }

        }

        public ProjectActivity CurrentActivity
        {
            get { return currentActivity; }
        }

        public IIntegrationResult CurrentResult
        {
            get { return integrationResultManager.CurrentIntegration; }
        }

        public IIntegrationResult Integrate(IntegrationRequest request)
        {
            lock (currentProjectStatus)
            {
                // Build up all the child items
                // Note: this will only build up the direct children, it doesn't handle below the initial layer
                currentProjectItems.Clear();
                sourceControlOperations.Clear();
                currentProjectStatus.Status = ItemBuildStatus.Running;
                currentProjectStatus.ChildItems.Clear();
                currentProjectStatus.TimeCompleted = null;
                currentProjectStatus.TimeOfEstimatedCompletion = null;
                currentProjectStatus.TimeStarted = DateTime.Now;
                GenerateSourceControlOperation(SourceControlOperation.CheckForModifications);
                GenerateTaskStatuses("Pre-build tasks", PrebuildTasks);
                GenerateSourceControlOperation(SourceControlOperation.GetSource);
                GenerateTaskStatuses("Build tasks", Tasks);
                GenerateTaskStatuses("Publisher tasks", Publishers);
            }

            // Start the integration
            IIntegrationResult result = null;
            try 
            {
                result = integratable.Integrate(request);
            }
            finally
            {
                // Tidy up the status
                lock (currentProjectStatus)
                {
                    CancelAllOutstandingItems(currentProjectStatus);
                    currentProjectStatus.TimeCompleted = DateTime.Now;
                    IntegrationStatus resultStatus = result == null ? IntegrationStatus.Unknown : result.Status;
                    switch (resultStatus)
                    {
                        case IntegrationStatus.Success:
                            currentProjectStatus.Status = ItemBuildStatus.CompletedSuccess;
                            break;
                        case IntegrationStatus.Unknown:
                            // This probably means the build was cancelled (i.e. no changes detected)
                            currentProjectStatus.Status = ItemBuildStatus.Cancelled;
                            break;
                        default:
                            currentProjectStatus.Status = ItemBuildStatus.CompletedFailed;
                            break;
                    }
                }
            }

            // Finally, return the actual result
            return result;
        }

        /// <summary>
        /// Cancels all outstanding items on a status item.
        /// </summary>
        private void CancelAllOutstandingItems(ItemStatus value)
        {
            if ((value.Status == ItemBuildStatus.Running) ||
                (value.Status == ItemBuildStatus.Pending))
            {
                ItemBuildStatus status = ItemBuildStatus.Cancelled;
                foreach (ItemStatus item in value.ChildItems)
                {
                    CancelAllOutstandingItems(item);
                    if (item.Status == ItemBuildStatus.CompletedFailed)
                    {
                        status = ItemBuildStatus.CompletedFailed;
                    }
                    else if ((item.Status == ItemBuildStatus.CompletedSuccess) &&
                        (status == ItemBuildStatus.Cancelled))
                    {
                        status = ItemBuildStatus.CompletedSuccess;
                    }
                }
                value.Status = status;
            }
        }

        private void GenerateSourceControlOperation(SourceControlOperation operation)
        {
            ItemStatus sourceControlStatus = null;

            if (SourceControl is IStatusSnapshotGenerator)
            {
                sourceControlStatus = (SourceControl as IStatusSnapshotGenerator).GenerateSnapshot();
            }
            else
            {
                sourceControlStatus = new ItemStatus(
                    string.Format("{0}: {1}", 
                        SourceControl.GetType().Name,
                        operation));
                sourceControlStatus.Status = ItemBuildStatus.Pending;
            }

            // Only add the item if it has been initialised
            if (sourceControlStatus != null)
            {
                currentProjectStatus.AddChild(sourceControlStatus);
                sourceControlOperations.Add(operation, sourceControlStatus);
            }
        }

        private void GenerateTaskStatuses(string name, IList tasks)
        {
            // Generate the group status
            ItemStatus groupItem = new ItemStatus(name);
            groupItem.Status = ItemBuildStatus.Pending;

            // Add each status
            foreach (ITask task in tasks)
            {
                ItemStatus taskItem = null;
                if (task is IStatusSnapshotGenerator)
                {
                    taskItem = (task as IStatusSnapshotGenerator).GenerateSnapshot();
                }
                else
                {
                    taskItem = new ItemStatus(task.GetType().Name);
                    taskItem.Status = ItemBuildStatus.Pending;
                }

                // Only add the item if it has been initialised
                if (taskItem != null)
                {
                    groupItem.AddChild(taskItem);
                    currentProjectItems.Add(task, taskItem);
                }
            }

            // Only add the group item if it contains children
            if (groupItem.ChildItems.Count > 0) currentProjectStatus.AddChild(groupItem);
        }

        public void NotifyPendingState()
        {
            currentActivity = ProjectActivity.Pending;
        }

        public void NotifySleepingState()
        {
            currentActivity = ProjectActivity.Sleeping;
        }

        public void Prebuild(IIntegrationResult result)
        {
            Prebuild(result, new Dictionary<string, string>());
        }

        public void Prebuild(IIntegrationResult result, Dictionary<string, string> parameterValues)
		{
            RunTasks(result, PrebuildTasks, parameterValues);
        }

        public virtual void ValidateParameters(Dictionary<string, string> parameterValues)
        {
            Log.Debug("Validating parameters");
            if (parameters != null)
            {
                List<Exception> results = new List<Exception>();
                foreach (ParameterBase parameter in parameters)
                {
                    string value = null;
                    if (parameterValues.ContainsKey(parameter.Name)) value = parameterValues[parameter.Name];
                    results.AddRange(parameter.Validate(value));
                }
                if (results.Count > 0)
                {
                    var error = new StringBuilder();
                    error.Append("The following errors were found in the parameters:");
                    foreach (Exception err in results)
                    {
                        error.Append(Environment.NewLine + err.Message);
                    }
                    Exception exception = new Exception(error.ToString());
                    Log.Warning(exception);
                    throw exception;
                }
            }
        }

        public void Run(IIntegrationResult result)
        {
            var parameters = new Dictionary<string, string>();
            if (result.Parameters != null) parameters = NameValuePair.ToDictionary(result.Parameters);
            Run(result, parameters);
        }

        public void Run(IIntegrationResult result, Dictionary<string, string> parameterValues)
		{
            RunTasks(result, tasks, parameterValues);
        }

        private void RunTasks(IIntegrationResult result, IList tasksToRun, Dictionary<string, string> parameterValues)
        {
            foreach (ITask task in tasksToRun)
            {
                if (task is IParamatisedTask)
                {
                    (task as IParamatisedTask).ApplyParameters(parameterValues, parameters);
                }

                RunTask(task, result);
                if (result.Failed) break;
            }
            CancelTasks(tasksToRun);
        }

        public void AbortRunningBuild()
        {
            ProcessExecutor.KillProcessCurrentlyRunningForProject(Name);
        }

        public void PublishResults(IIntegrationResult result)
        {
            PublishResults(result, new Dictionary<string, string>());
        }

		public void PublishResults(IIntegrationResult result, Dictionary<string, string> parameterValues)
		{
            // Make sure all the tasks have been cancelled
            CancelTasks(PrebuildTasks);
            CancelTasks(Tasks);

            foreach (ITask publisher in publishers)
            {
                try
                {
                    if (publisher is IParamatisedTask)
                    {
                        (publisher as IParamatisedTask).ApplyParameters(parameterValues, parameters);
                    }

                    RunTask(publisher, result);
                }
                catch (Exception e)
                {
                    Log.Error("Publisher threw exception: " + e);
                }
            }
            if (result.Succeeded)
            {
                messages.Clear();
            }
            else
            {
                AddBreakersToMessages(result);
            }
        }

        /// <summary>
        /// Runs a specific task and updates the status for the task.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="result"></param>
        private void RunTask(ITask task, IIntegrationResult result)
        {
            // Load the status details
            ItemStatus status = null;
            if (currentProjectItems.ContainsKey(task)) status = currentProjectItems[task];

            // If there is a status, update it
            if (status != null)
            {
                status.TimeStarted = DateTime.Now;
                status.Status = ItemBuildStatus.Running;
                if ((status.Parent != null) && (status.Parent.Status == ItemBuildStatus.Pending))
                {
                    status.Parent.TimeStarted = status.TimeStarted;
                    status.Parent.Status = ItemBuildStatus.Running;
                }
            }

            try
            {
                // Run the actual task
                task.Run(result);
                if (status != null)
                {
                    if (result.Failed)
                    {
                        status.Status = ItemBuildStatus.CompletedFailed;
                    }
                    else
                    {
                        status.Status = ItemBuildStatus.CompletedSuccess;
                    }
                }
            }
            catch
            {
                if (status != null) status.Status = ItemBuildStatus.CompletedFailed;
                throw;
            }
            finally
            {
                if (status != null) status.TimeCompleted = DateTime.Now;
            }
        }

        /// <summary>
        /// Cancels any tasks that have not been run.
        /// </summary>
        /// <param name="tasks"></param>
        private void CancelTasks(IList tasks)
        {
            ItemBuildStatus overallStatus = ItemBuildStatus.Cancelled;
            List<ItemStatus> statuses = new List<ItemStatus>();
            foreach (ITask task in tasks)
            {
                if (currentProjectItems.ContainsKey(task))
                {
                    ItemStatus status = currentProjectItems[task];
                    if ((status.Parent != null) && !statuses.Contains(status.Parent))
                    {
                        statuses.Add(status.Parent);
                    }

                    // Check the status and change it if it is still pending
                    if (status.Status == ItemBuildStatus.Pending) status.Status = ItemBuildStatus.Cancelled;

                    // Next, check for the overall status - by default it is cancelled, but completed statuses
                    // will override it
                    if (status.Status == ItemBuildStatus.CompletedFailed)
                    {
                        overallStatus = ItemBuildStatus.CompletedFailed;
                    }
                    else if ((status.Status == ItemBuildStatus.CompletedSuccess) && (overallStatus == ItemBuildStatus.Cancelled))
                    {
                        overallStatus = ItemBuildStatus.CompletedSuccess;
                    }
                }
            }

            // Update any parent items
            foreach (ItemStatus status in statuses)
            {
                status.Status = overallStatus;
                if ((overallStatus == ItemBuildStatus.CompletedFailed) ||
                    (overallStatus == ItemBuildStatus.CompletedSuccess))
                {
                    status.TimeCompleted = DateTime.Now;
                }
            }
        }

        private void AddBreakersToMessages(IIntegrationResult result)
        {
            System.Collections.Generic.List<string> breakers = new System.Collections.Generic.List<string>();
            string breakingusers = "";

            foreach (Modification mod in result.Modifications)
            {
                if (!breakers.Contains(mod.UserName))
                {
                    breakers.Add(mod.UserName);
                }
            }


            foreach (string UserName in result.FailureUsers)
            {
                if (!breakers.Contains(UserName))
                {
                    breakers.Add(UserName);
                }
            }


            if (breakers.Count > 0)
            {
                breakingusers = "Breakers : ";

                foreach (string user in breakers)
                {
                    breakingusers += user + ", ";
                }

                breakingusers = breakingusers.Remove(breakingusers.Length - 2, 2); // remove the last comma and space
            }

            AddMessage(new Message(breakingusers));
        }

        public void Initialize()
        {
            Log.Info(string.Format("Initializing Project [{0}]", Name));
            SourceControl.Initialize(this);
        }

        public void Purge(bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment)
        {
            Log.Info(string.Format("Purging Project [{0}]", Name));
            if (purgeSourceControlEnvironment)
            {
                SourceControl.Purge(this);
            }
            if (purgeWorkingDirectory && Directory.Exists(WorkingDirectory))
            {
                new IoService().DeleteIncludingReadOnlyObjects(WorkingDirectory);
            }
            if (purgeArtifactDirectory && Directory.Exists(ArtifactDirectory))
            {
                new IoService().DeleteIncludingReadOnlyObjects(ArtifactDirectory);
            }
        }

        public string Statistics
        {
            get { return StatisticsPublisher.LoadStatistics(ArtifactDirectory); }
        }

        public string ModificationHistory
        {
            get { return ModificationHistoryPublisher.LoadHistory(ArtifactDirectory); }
        }


        public string RSSFeed
        {
            get { return RssPublisher.LoadRSSDataDocument(ArtifactDirectory); }
        }


        public IIntegrationRepository IntegrationRepository
        {
            get { return this; }
        }

        public static string DefaultUrl()
        {
            return string.Format("http://{0}/ccnet", Environment.MachineName);
        }

        public ProjectStatus CreateProjectStatus(IProjectIntegrator integrator)
        {
            ProjectStatus status =
                new ProjectStatus(Name, Category, CurrentActivity, LastIntegration.Status, integrator.State, WebURL,
                                  LastIntegration.StartTime, LastIntegration.Label,
                                  LastIntegration.LastSuccessfulIntegrationLabel,
                                  Triggers.NextBuild, CurrentBuildStage(), QueueName, QueuePriority);
            status.Description = Description;
            status.Messages = (Message[])messages.ToArray(typeof(Message));
            return status;
        }

        private string CurrentBuildStage()
        {
            if (CurrentActivity != ProjectActivity.Building)
                return "";
            else
                return integrationResultManager.CurrentIntegration.BuildProgressInformation.GetBuildProgressInformation();
        }

        private IntegrationSummary LastIntegration
        {
            get { return integrationResultManager.LastIntegration; }
        }

        public void AddMessage(Message message)
        {
            messages.Add(message);
        }

        public string GetBuildLog(string buildName)
        {
            string logDirectory = GetLogDirectory();
            if (string.IsNullOrEmpty(logDirectory)) return "";
            using (StreamReader sr = new StreamReader(Path.Combine(logDirectory, buildName)))
            {
                return sr.ReadToEnd();
            }
        }

        public string[] GetBuildNames()
        {
            string logDirectory = GetLogDirectory();
            if (string.IsNullOrEmpty(logDirectory)) return new string[0];
            string[] logFileNames = LogFileUtil.GetLogFileNames(logDirectory);
            Array.Reverse(logFileNames);
            return logFileNames;
        }

        public string[] GetMostRecentBuildNames(int buildCount)
        {
            string[] buildNames = GetBuildNames();
            ArrayList buildNamesToReturn = new ArrayList();
            for (int i = 0; i < ((buildCount < buildNames.Length) ? buildCount : buildNames.Length); i++)
            {
                buildNamesToReturn.Add(buildNames[i]);
            }
            return (string[])buildNamesToReturn.ToArray(typeof(string));
        }

        public string GetLatestBuildName()
        {
            string[] buildNames = GetBuildNames();
            if (buildNames.Length > 0)
            {
                return buildNames[0];
            }
            else
            {
                return string.Empty;
            }
        }

        private string GetLogDirectory()
        {
            XmlLogPublisher publisher = GetLogPublisher();
            string logDirectory = publisher.LogDirectory(ArtifactDirectory);
            if (!Directory.Exists(logDirectory))
            {
                Log.Warning("Log Directory [ " + logDirectory + " ] does not exist. Are you sure any builds have completed?");
            }
            return logDirectory;
        }

        private XmlLogPublisher GetLogPublisher()
        {
            foreach (ITask publisher in Publishers)
            {
                if (publisher is XmlLogPublisher)
                {
                    return (XmlLogPublisher)publisher;
                }
            }
            throw new CruiseControlException("Unable to find Log Publisher for project so can't find log file");
        }
        
        public void CreateLabel(IIntegrationResult result)
        {
            result.Label = Labeller.Generate(result);
        }

        /// <summary>
        /// The initial start-up state to set.
        /// </summary>
        [ReflectorProperty("initialState", Required = false)]
        public ProjectInitialState InitialState
        {
            get { return initialState; }
            set { initialState = value; }
        }

        /// <summary>
        /// The start-up mode for this project.
        /// </summary>
        [ReflectorProperty("startupMode", Required = false)]
        public ProjectStartupMode StartupMode
        {
            get { return startupMode; }
            set { startupMode = value; }
        }

        /// <summary>
        /// Checks the internal validation of the item.
        /// </summary>
        /// <param name="configuration">The entire configuration.</param>
        /// <param name="parent">The parent item for the item being validated.</param>
        public virtual void Validate(IConfiguration configuration, object parent, IConfigurationErrorProcesser errorProcesser)
        {
            if (security.RequiresServerSecurity &&
                (configuration.SecurityManager is NullSecurityManager))
            {
                errorProcesser.ProcessError(
                    new ConfigurationException(
                        string.Format("Security is defined for project '{0}', but not defined at the server", this.Name)));
            }

            ValidateProject(errorProcesser);
            ValidateItem(sourceControl, configuration, errorProcesser);
            ValidateItem(labeller, configuration, errorProcesser);
            ValidateItems(PrebuildTasks, configuration, errorProcesser);
            ValidateItems(tasks, configuration, errorProcesser);
            ValidateItems(publishers, configuration, errorProcesser);
            ValidateItem(state, configuration, errorProcesser);
            ValidateItem(security, configuration, errorProcesser);
        }

        /// <summary>
        /// Validate the project details.
        /// </summary>
        /// <remarks>
        /// Currently the only check is the project name does not contain any invalid characters.
        /// </remarks>
        private void ValidateProject(IConfigurationErrorProcesser errorProcesser)
        {
            if (ContainsInvalidChars(this.Name))
            {
                errorProcesser.ProcessWarning(
                    string.Format("Project name '{0}' contains some chars that could cause problems, better use only numbers and letters",
                        Name));
            }
        }

        /// <summary>
        /// Check each character to make sure it is valid.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>False if the item contains no invalid characters, true otherwise.</returns>
        private bool ContainsInvalidChars(string item)
        {
            bool result = false;

            for (Int32 i = 0; i < item.Length; i++)
            {
                if (!char.IsLetterOrDigit(item, i) &&
                    item[i] != '.' &&
                    item[i] != ' ' &&
                    item[i] != '-' &&
                    item[i] != '_')
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Validates the configuration of an item.
        /// </summary>
        /// <param name="item"></param>
        private void ValidateItem(object item, IConfiguration configuration, IConfigurationErrorProcesser errorProcesser)
        {
            if ((item != null) && (item is IConfigurationValidation))
            {
                (item as IConfigurationValidation).Validate(configuration, this, errorProcesser);
            }
        }

        /// <summary>
        /// Validates the configuration of an enumerable.
        /// </summary>
        /// <param name="item"></param>
        private void ValidateItems(IEnumerable items, IConfiguration configuration, IConfigurationErrorProcesser errorProcesser)
        {
            if (items != null)
            {
                foreach (object item in items)
                {
                    ValidateItem(item, configuration, errorProcesser);
                }
            }
        }

        #region GenerateSnapshot()
        /// <summary>
        /// Generates a snapshot of the current status.
        /// </summary>
        /// <returns></returns>
        public virtual ItemStatus GenerateSnapshot()
        {
            lock (currentProjectStatus)
            {
                // Update the status of the snapshot
                if (currentProjectStatus.Status == ItemBuildStatus.Unknown)
                {
                    switch (LastIntegration.Status)
                    {
                        case IntegrationStatus.Success:
                            currentProjectStatus.Status = ItemBuildStatus.CompletedSuccess;
                            break;
                        case IntegrationStatus.Failure:
                        case IntegrationStatus.Exception:
                            currentProjectStatus.Status = ItemBuildStatus.CompletedFailed;
                            break;
                    }
                }

                return currentProjectStatus.Clone();
            }
        }
        #endregion

        #region RecordSourceControlOperation()
        /// <summary>
        /// Records a source control operation.
        /// </summary>
        /// <param name="operation">The operation to record.</param>
        /// <param name="status">The status of the operation.</param>
        public virtual void RecordSourceControlOperation(SourceControlOperation operation, ItemBuildStatus status)
        {
            if (sourceControlOperations.ContainsKey(operation))
            {
                sourceControlOperations[operation].Status = status;
                switch (status)
                {
                    case ItemBuildStatus.Running:
                        sourceControlOperations[operation].TimeStarted = DateTime.Now;
                        break;
                    case ItemBuildStatus.CompletedFailed:
                    case ItemBuildStatus.CompletedSuccess:
                        sourceControlOperations[operation].TimeCompleted = DateTime.Now;
                        break;
                }
            }
        }
        #endregion

        #region RetrievePackageList()
        /// <summary>
        /// Retrieves the latest list of packages.
        /// </summary>
        /// <returns></returns>
        public virtual List<PackageDetails> RetrievePackageList()
        {
            string listFile = Name + "-packages.xml";
            listFile = Path.Combine(ArtifactDirectory, listFile);
            List<PackageDetails> packages = LoadPackageList(listFile);
            return packages;
        }

        /// <summary>
        /// Retrieves the list of packages for a build.
        /// </summary>
        /// <param name="buildLabel"></param>
        /// <returns></returns>
        public virtual List<PackageDetails> RetrievePackageList(string buildLabel)
        {
            string listFile = Path.Combine(buildLabel, Name + "-packages.xml");
            listFile = Path.Combine(ArtifactDirectory, listFile);
            if (File.Exists(listFile))
            {
                List<PackageDetails> packages = LoadPackageList(listFile);
                return packages;
            }
            else
            {
                List<PackageDetails> packages = RetrievePackageList();
                for (int loop = 0; loop < packages.Count; )
                {
                    if (!string.Equals(packages[loop].BuildLabel, buildLabel, StringComparison.InvariantCultureIgnoreCase))
                    {
                        packages.RemoveAt(loop);
                    }
                    else
                    {
                        loop++;
                    }
                }
                return packages;
            }
        }
        #endregion

        #region LoadPackageList()
        private List<PackageDetails> LoadPackageList(string fileName)
        {
            List<PackageDetails> packages = new List<PackageDetails>();
            if (File.Exists(fileName))
            {
                XmlDocument packageList = new XmlDocument();
                packageList.Load(fileName);
                foreach (XmlElement packageElement in packageList.SelectNodes("/packages/package"))
                {
                    string packageFileName = packageElement.GetAttribute("file");
                    packageFileName = packageFileName.Replace(ArtifactDirectory, string.Empty);
                    if (packageFileName.StartsWith("\\")) packageFileName = packageFileName.Substring(1);
                    PackageDetails details = new PackageDetails(packageFileName);
                    details.Name = packageElement.GetAttribute("name");
                    details.BuildLabel = packageElement.GetAttribute("label");
                    details.DateTime = DateTime.Parse(packageElement.GetAttribute("time"));
                    details.NumberOfFiles = Convert.ToInt32(packageElement.GetAttribute("files"));
                    details.Size = Convert.ToInt64(packageElement.GetAttribute("size"));
                    packages.Add(details);
                }
            }
            return packages;
        }
        #endregion

        /// <summary>
        /// Lists the parameters for the project.
        /// </summary>
        /// <returns></returns>
        public virtual List<ParameterBase> ListBuildParameters()
        {
            if (parameters == null)
            {
                return new List<ParameterBase>();
            }
            else
            {
                foreach (var parameter in parameters)
                {
                    parameter.GenerateClientDefault();
                }
                return new List<ParameterBase>(parameters);
            }
        }
    }
}
