using System.Globalization;
namespace ThoughtWorks.CruiseControl.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Config;
    using ThoughtWorks.CruiseControl.Core.Label;
    using ThoughtWorks.CruiseControl.Core.Publishers;
    using ThoughtWorks.CruiseControl.Core.Publishers.Statistics;
    using ThoughtWorks.CruiseControl.Core.Security;
    using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
    using ThoughtWorks.CruiseControl.Core.State;
    using ThoughtWorks.CruiseControl.Core.Tasks;
    using ThoughtWorks.CruiseControl.Core.Triggers;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Remote.Messages;
    using ThoughtWorks.CruiseControl.Remote.Parameters;

    /// <summary>
    /// A &lt;project&gt; block defines all the configuration for one project running in a CruiseControl.NET server.
    /// </summary>
    /// <title>Project Configuration Block</title>
    /// <version>1.0</version>
    /// <remarks>
    /// <heading>Setting the WebURL</heading>
    /// <para>
    /// The current format of the url for a project, as specified in the &lt;webURL&gt; element is:
    /// </para>
    /// <code type="None">http://{dashboardserver}/{vdir}/server/{ccnetserver}/project/{projectname}/ViewLatestBuildReport.aspx</code>
    /// <para>
    /// For example, if the dashboard was deployed on the server <b>webserver</b> to virtual directory 
    /// <b>ccnet</b>, and if the project to monitor is called <b>test</b> on server cruise, the URL would be: 
    /// </para>
    /// <code type="None">http://webserver/ccnet/server/cruise/project/test/ViewLatestBuildReport.aspx</code>
    /// </remarks>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;project name="Project 1" /&gt;
    /// </code>
    /// <code title="Full Example">
    /// &lt;project name="Project 1" queue="Q1" queuePriority="1"&gt;
    /// &lt;workingDirectory&gt;yourWorkingDirectory&lt;/workingDirectory&gt;
    /// &lt;artifactDirectory&gt;yourArtifactDirectory&lt;/artifactDirectory&gt;
    /// &lt;category&gt;Category 1&lt;/category&gt;
    /// &lt;webURL&gt;http://server1/ccnet/server/local/project/testProject/ViewLatestBuildReport.aspx&lt;/webURL&gt;
    /// &lt;modificationDelaySeconds&gt;2&lt;/modificationDelaySeconds&gt;
    /// &lt;maxSourceControlRetries&gt;5&lt;/maxSourceControlRetries&gt;
    /// &lt;initialState&gt;Stopped&lt;/initialState&gt;
    /// &lt;startupMode&gt;UseInitialState&lt;/startupMode&gt;
    /// &lt;triggers&gt;
    /// &lt;!--yourFirstTriggerType .. --&gt;
    /// &lt;!--yourOtherTriggerType .. --&gt;
    /// &lt;/triggers&gt;
    /// &lt;!-- state type="yourStateManagerType" .. --&gt;
    /// &lt;!-- sourcecontrol type="yourSourceControlType" .. --&gt;
    /// &lt;!-- labeller type="yourLabellerType" .. --&gt;
    /// &lt;prebuild&gt;
    /// &lt;!-- yourFirstPrebuildTask .. --&gt;
    /// &lt;!-- yourOtherPrebuildTask .. --&gt;
    /// &lt;/prebuild&gt;
    /// &lt;tasks&gt;
    /// &lt;!-- yourFirstTask .. --&gt;
    /// &lt;!-- yourOtherTask .. --&gt;
    /// &lt;/tasks&gt;
    /// &lt;publishers&gt;
    /// &lt;!-- yourFirstPublisherTask .. --&gt;
    /// &lt;!-- yourOtherPublisherTask .. --&gt;
    /// &lt;/publishers&gt;
    /// &lt;externalLinks&gt;
    /// &lt;externalLink name="My First Link" url="http://somewhere/" /&gt;
    /// &lt;externalLink name="My Other Link" url="http://somewhere.else/" /&gt;
    /// &lt;/externalLinks&gt;
    /// &lt;parameters&gt;
    /// &lt;textParameter name="Build Name" default="Unknown" /&gt;
    /// &lt;/parameters&gt;
    /// &lt;linkedSites&gt;&lt;namedValue name="ohloh" value="5623"/&gt;&lt;/linkedSites&gt;
    /// &lt;/project&gt;
    /// </code>
    /// </example>
    [ReflectorType("project")]
    public class Project : ProjectBase, IProject, IIntegrationRunnerTarget, IIntegrationRepository,
        IConfigurationValidation, IStatusSnapshotGenerator, IParamatisedProject
    {
        private string webUrl = DefaultUrl();
        private string queueName = string.Empty;
        private int queuePriority;
        private ISourceControl sourceControl = new NullSourceControl();
        private ILabeller labeller = new DefaultLabeller();
        private ITask[] tasks = new ITask[] { new NullTask() };
        private ITask[] publishers = new ITask[] { new XmlLogPublisher() };
        private ProjectActivity currentActivity = ProjectActivity.Sleeping;
        private IStateManager state = new FileStateManager();
        private IIntegrationResultManager integrationResultManager;
        private IIntegratable integratable;
        private QuietPeriod quietPeriod = new QuietPeriod(new DateTimeProvider());
        private List<Message> messages = new List<Message>();
        private int maxSourceControlRetries = 5;
        private IProjectAuthorisation security = new InheritedProjectAuthorisation();
        private ParameterBase[] parameters = new ParameterBase[0];
        private ProjectInitialState initialState = ProjectInitialState.Started;
        private ProjectStartupMode startupMode = ProjectStartupMode.UseLastState;
        private Sourcecontrol.Common.SourceControlErrorHandlingPolicy sourceControlErrorHandling = Common.SourceControlErrorHandlingPolicy.ReportEveryFailure;
        private ProjectStatusSnapshot currentProjectStatus;
        private Dictionary<ITask, ItemStatus> currentProjectItems = new Dictionary<ITask, ItemStatus>();
        private Dictionary<SourceControlOperation, ItemStatus> sourceControlOperations = new Dictionary<SourceControlOperation, ItemStatus>();
        private IConfiguration configuration;
        private IExecutionEnvironment currentExecutionEnvironment;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class.
        /// </summary>
        public Project()
        {
            integrationResultManager = new IntegrationResultManager(this);
            integratable = new IntegrationRunner(integrationResultManager, this, quietPeriod);
            this.PrebuildTasks = new ITask[0];
            this.CryptoFunctions = new DefaultCryptoFunctions();
            this.ShowForceBuildButton = true;
            this.ShowStartStopButton = true;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class.
        /// </summary>
        /// <param name="integratable">The integratable.</param>
        public Project(IIntegratable integratable)
            : this()
        {
            this.integratable = integratable;
        }
        #endregion

        /// <summary>
        /// A set of Tasks to run before the build starts and before the source is updated. A failed task will fail the build and any
        /// subsequent tasks will not run. Tasks are run sequentially, in the order they appear in the configuration. 
        /// </summary>
        /// <version>1.1</version>
        /// <default>None</default>
        [ReflectorProperty("prebuild", Required = false)]
        public ITask[] PrebuildTasks { get; set; }

        /// <summary>
        /// Any security for the project.
        /// </summary>
        /// <version>1.5</version>
        /// <default><link>Inherited Project Security</link></default>
        [ReflectorProperty("security", InstanceTypeKey = "type", Required = false)]
        public IProjectAuthorisation Security
        {
            get { return security; }
            set { security = value; }
        }

        /// <summary>
        /// Dynamic build parameters - these are parameters that are set at build time instead of being hard-coded within the
        /// configuration file 
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("parameters", Required = false)]
        public ParameterBase[] Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        #region Links
        /// <summary>
        /// Links for this project to other sites. 
        /// <br/>Example for the Ohloh Project plugin<br/>
        /// &lt;linkedSites&gt;&lt;namedValue name="ohloh" value="5623"/&gt;&lt;/linkedSites&gt;
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("linkedSites", Required = false)]
        public NameValuePair[] LinkedSites { get; set; }
        #endregion

        /// <summary>
        /// A state manager for the project.
        /// </summary>
        /// <version>1.0</version>
        /// <default><link>File State Manager</link></default>
        [ReflectorProperty("state", InstanceTypeKey = "type", Required = false), Description("State")]
        public IStateManager StateManager
        {
            get { return state; }
            set { state = value; }
        }

        /// <summary>
        /// A reporting URL for this project. This is used by CCTray and the Email Publisher. Typically you should navigate to the
        /// Project Report on the Dashboard, and use its URL.
        /// </summary>
        /// <remarks>
        /// The default URL contains the machine name of the server.
        /// </remarks>
        /// <version>1.0</version>
        /// <default>http://machineName/ccnet</default>
        [ReflectorProperty("webURL", Required = false)]
        public string WebURL
        {
            get { return webUrl; }
            set { webUrl = value; }
        }

        /// <summary>
        /// An optional impersonation account.
        /// </summary>
        /// <remarks>
        /// This is only available on Windows OSs.
        /// </remarks>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("impersonation", InstanceType = typeof(ImpersonationDetails), Required = false)]
        public ImpersonationDetails Impersonation { get; set; }

        /// <summary>
        /// The maximum amount of source control exceptions in a row that may occur, before the project goes to the stopped state(when
        /// StopProjectOnReachingMaxSourceControlRetries is set to true).
        /// </summary>
        /// <version>1.4</version>
        /// <default>5</default>
        [ReflectorProperty("maxSourceControlRetries", Required = false)]
        public int MaxSourceControlRetries
        {
            get { return maxSourceControlRetries; }
            set { maxSourceControlRetries = value < 0 ? 0 : value; }
        }

        /// <summary>
        /// Stops the project on reaching maxSourceControlRetries or not. When set to true, the project will be stopped when the amount of
        /// consecutive source control errors is equal to maxSourceControlRetries.
        /// </summary>
        /// <version>1.4</version>
        /// <default>false</default>
        [ReflectorProperty("stopProjectOnReachingMaxSourceControlRetries", Required = false)]
        public bool StopProjectOnReachingMaxSourceControlRetries {get; set;}
        
        /// <summary>
        /// (Should) show or hide the ForceBuildButton in UI programs. This is an extra setting on top of security.
        /// This is setting is mainly meant to disable the possibility to force a project via a UI. (dashboard and cctray)
        /// Forcing a build via other tools : ccmd, ... will work if you have the rights to do so.
        /// For example there is a project GatherErrorsDuringWeekend which is scheduled to run at Saturday and Sunday.
        /// Forcing this project on a weekday could mess up statistics or so. 
        /// </summary>
        /// <version>1.6</version>
        /// <default>True</default>
        [ReflectorProperty("showForceBuildButton", Required = false)]
        public  bool ShowForceBuildButton {get; set;}
        

        /// <summary>
        /// (Should) show or hide the Start - Stop Button in UI programs. This is an extra setting on top of security.
        /// This is setting is mainly meant to disable the possibility to start or stop a project via a UI. (dashboard and cctray) 
        /// Starting - Stopping a build via other tools : ccmd, ... will work if you have the rights to do so.
        /// For example there is a project GatherErrorsDuringWeekend which is scheduled to run at Saturday and Sunday.
        /// Stopping this project on a weekday could mess up statistics or so. 
        /// </summary>
        /// <version>1.6</version>
        /// <default>True</default>
        [ReflectorProperty("showStartStopButton", Required = false)]
        public bool ShowStartStopButton { get; set; }
        

        /// <summary>
        /// What action to take when a source control error occurs (during GetModifications).
        /// </summary>
        /// <remarks>
        /// These are the possible values :
        /// <list type="1">
        /// <item>
        /// ReportEveryFailure : runs the publisher section whenever there is an error.
        /// </item>
        /// <item>
        /// ReportOnRetryAmount : only runs the publisher section when maxSourceControlRetries has been reached, <br/> 
        /// the publisher section will only be run once.
        /// </item>
        /// <item>
        /// ReportOnEveryRetryAmount : runs the publisher section whenever the maxSourceControlRetries has been reached. <br/>
        /// When maxSourceControlRetries has been reached and the publisher section has ran, the counter is set back to 0.
        /// </item>
        /// </list>
        /// </remarks>
        /// <version>1.4</version>
        /// <default>ReportEveryFailure</default>
        [ReflectorProperty("sourceControlErrorHandling", Required = false)]
        public Common.SourceControlErrorHandlingPolicy SourceControlErrorHandling
        {
            get { return sourceControlErrorHandling; }
            set { sourceControlErrorHandling = value; }
        }

        /// <summary>
        /// The name of the integration queue that this project will use. By default, each project runs in its own queue.
        /// </summary>
        /// <version>1.3</version>
        /// <default>Project name</default>
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

        /// <summary>
        /// The priority of this project within the integration queue. If multiple projects have pending requests in the specified queue then
        /// these requests will be executed according to their priority. Lower priority numbers indicate that integration requests for this
        /// project will execute before other projects in the same queue, however projects with priority 0 are always executed after projects
        /// with non-zero priorities in the same queue.
        /// </summary>
        /// <version>1.3</version>
        /// <default>0</default>
        [ReflectorProperty("queuePriority", Required = false)]
        public int QueuePriority
        {
            get { return queuePriority; }
            set { queuePriority = value; }
        }

        /// <summary>
        /// The source control block to use.
        /// </summary>
        /// <version>1.0</version>
        /// <default><link>Null_Source_Control</link></default>
        [ReflectorProperty("sourcecontrol", InstanceTypeKey = "type", Required = false)]
        public ISourceControl SourceControl
        {
            get { return sourceControl; }
            set { sourceControl = value; }
        }

        /// <summary>
        /// The list of build-completed publishers used by this project. 
        /// </summary>
        /// <default>None</default>
        /// <version><link>Xml Log Publisher</link></version>
        [ReflectorProperty("publishers", Required = false)]
        public ITask[] Publishers
        {
            get { return publishers; }
            set { publishers = value; }
        }

        /// <summary>
        /// The minimum number of seconds allowed between the last check in and the start of a valid build. 
        /// </summary>
        /// <remarks>
        /// If any modifications are found within this interval the system will sleep long enough so the last checkin is just outside this
        /// interval. For example if the modification delay is set to 10 seconds and the last checkin was 7 seconds ago the system will sleep
        /// for 3 seconds and check again. This process will repeat until no modifications have been found within the modification delay
        /// window.
        /// This feature is in CruiseControl.NET for Source Control systems, like CVS, that do not support atomic checkins since starting a
        /// build half way through someone checking in their work could result in invalid 'logical' passes or failures. The property is
        /// optional though so if you are using a source control system with atomic checkins, leave it out (and it will default to '0').
        /// </remarks>
        /// <version>1.0</version>
        /// <default>0</default>
        [ReflectorProperty("modificationDelaySeconds", Required = false)]
        public double ModificationDelaySeconds
        {
            get { return quietPeriod.ModificationDelaySeconds; }
            set { quietPeriod.ModificationDelaySeconds = value; }
        }

        /// <summary>
        /// Labellers are used to generate the label that CCNet uses to identify the specific build. The label generated by CCNet can be used
        /// to version your assemblies or label your version control system with each build.
        /// </summary>
        /// <version>1.0</version>
        /// <default><link>Default Labeller</link></default>
        [ReflectorProperty("labeller", InstanceTypeKey = "type", Required = false)]
        public ILabeller Labeller
        {
            get { return labeller; }
            set { labeller = value; }
        }

        /// <summary>
        /// A set of Tasks to run as part of the build. A failed task will fail the build and any subsequent tasks will not run. Tasks are run
        /// sequentially, in the order they appear in the configuration.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        [ReflectorProperty("tasks", Required = false)]
        public ITask[] Tasks
        {
            get { return tasks; }
            set { tasks = value; }
        }

        /// <summary>
        /// The remote machines that this project can build on.
        /// </summary>
        /// <version>1.6</version>
        /// <default>none</default>
        [ReflectorProperty("remoteTargets", Required = false)]
        public string[] RemoteMachines { get; set; }

        /// <summary>
        /// A data store for project information.
        /// </summary>
        /// <version>1.6</version>
        /// <default>none</default>
        [ReflectorProperty("data", InstanceTypeKey = "type", Required = false)]
        public IDataStore DataStore { get; set; }

        // Move this ideally
        /// <summary>
        /// Gets or sets the activity.	
        /// </summary>
        /// <value>The activity.</value>
        /// <remarks></remarks>
        public ProjectActivity Activity
        {
            get { return currentActivity; }
            set { currentActivity = value; }

        }

        /// <summary>
        /// Gets the current activity.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public ProjectActivity CurrentActivity
        {
            get { return currentActivity; }
        }

        /// <summary>
        /// Gets the current result.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public IIntegrationResult CurrentResult
        {
            get { return integrationResultManager.CurrentIntegration; }
        }

        /// <summary>
        /// Gets or sets the current execution environment.
        /// </summary>
        /// <value>The current execution environment.</value>
        public IExecutionEnvironment CurrentExecutionEnvironment
        {
            get
            {
                if (this.currentExecutionEnvironment == null)
                {
                    this.currentExecutionEnvironment = new ExecutionEnvironment();
                }

                return this.currentExecutionEnvironment;
            }
            set { this.currentExecutionEnvironment = value; }
        }

        /// <summary>
        /// Starts a new integration result.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// The new <see cref="IIntegrationResult"/>.
        /// </returns>
        public IIntegrationResult StartNewIntegration(IntegrationRequest request)
        {
            return this.integratable.StartNewIntegration(request);
        }

        /// <summary>
        /// Integrates the specified request.	
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public IIntegrationResult Integrate(IntegrationRequest request)
        {
            Log.Trace("Integrating {0}", Name);
            IIntegrationResult result = null;

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
            IDisposable impersonation = null;
            var hasError = false;
            var timer = new Stopwatch();
            timer.Start();
            var summary = new BuildSummary
                              {
                                  StartTime = DateTime.Now
                              };
            var logDirectory = this.GetLogDirectory();
            var fileSystem = new SystemIoFileSystem();
            var serialiser = new XmlSerializer(typeof(BuildSummary));

            try
            {
                if (Impersonation != null) impersonation = Impersonation.Impersonate();
                var dynamicSourceControl = sourceControl as IParamatisedItem;
                if (dynamicSourceControl != null)
                {
                    dynamicSourceControl.ApplyParameters(request.BuildValues, parameters);
                }
                result = integratable.Integrate(request);
                summary.Label = result.Label;
                writeSummary(result, logDirectory, summary, timer, fileSystem, serialiser);
            }
            catch (Exception error)
            {
                writeSummary(result, logDirectory, summary, timer, fileSystem, serialiser);
                Log.Error(error);
                hasError = true;
                throw;
            }
            finally
            {
                if (impersonation != null) impersonation.Dispose();

                // Tidy up the status
                lock (currentProjectStatus)
                {
                    CancelAllOutstandingItems(currentProjectStatus);
                    currentProjectStatus.TimeCompleted = DateTime.Now;
                    IntegrationStatus resultStatus = result == null ?
                        (hasError ? IntegrationStatus.Exception : IntegrationStatus.Unknown) :
                        result.Status;
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



            // Store the project status so it can be used by other parts of the system
            if (this.DataStore != null)
            {
                this.DataStore.StoreProjectSnapshot(result, this.GenerateSnapshot());
            }

            // Finally, return the actual result
            return result;
        }

        private void writeSummary(IIntegrationResult r, string logDir, BuildSummary summary, Stopwatch timer, SystemIoFileSystem fileSystem, XmlSerializer serialiser)
        {
            if (r == null) return;

            var path = Path.ChangeExtension(Path.Combine(logDir, new LogFile(r).Filename), "summary");
            timer.Stop();
            summary.Duration = timer.ElapsedMilliseconds;
            summary.Status = r.Status;
            summary.LogName = new LogFile(r).Filename;
            fileSystem.EnsureFolderExists(path);
            using (var output = fileSystem.OpenOutputStream(path))
            {
                serialiser.Serialize(output, summary);
            }
        }


        /// <summary>
        /// Retrieves some summaries for the project.
        /// </summary>
        /// <param name="start">The first summary to retrieve. This starts from the last summary generated.</param>
        /// <param name="count">The number of summaries to retrieve.</param>
        /// <returns>
        /// The summaries.
        /// </returns>
        public IList<BuildSummary> GetSummaries(int start, int count)
        {
            var logDirectory = this.GetLogDirectory();
            if (string.IsNullOrEmpty(logDirectory))
            {
                return null;
            }

            try
            {
                count = count > 50 ? 50 : count;
                var summaries = new List<BuildSummary>();
                var fileSystem = new SystemIoFileSystem();
                var serialiser = new XmlSerializer(typeof(BuildSummary));
                Func<string, BuildSummary> loadSummary = f =>
                                                             {
                                                                 using (var stream = fileSystem.OpenInputStream(f))
                                                                 {
                                                                     var summary = serialiser.Deserialize(stream);
                                                                     return summary as BuildSummary;
                                                                 }
                                                             };
                var files = fileSystem.GetFilesInDirectory(logDirectory, "*.summary", SearchOption.TopDirectoryOnly);
                summaries.AddRange(
                    files.OrderByDescending(f => f).Skip(start).Take(count).Select(loadSummary));
                return summaries;
            }
            catch (DirectoryNotFoundException error)
            {
                throw new CruiseControlException("Log directory does not exist. This normally occurs because there are no builds for this project.", error);
            }
        }

        /// <summary>
        /// Clears the message array of the messages of the specified kind
        /// </summary>
        /// <param name="kind"></param>
        private void ClearMessages(Message.MessageKind kind)
        {
            for (Int32 i = messages.Count - 1; i >= 0; i--)
            {
                Message m = (Message)messages[i];
                if (m.Kind == kind)
                {
                    messages.RemoveAt(i);
                }
            }
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

            if (this.sourceControl is IStatusItem)
            {
                var item = this.sourceControl as IStatusItem;
                item.InitialiseStatus();
                sourceControlStatus = item.GenerateSnapshot();
            }
            else if (SourceControl is IStatusSnapshotGenerator)
            {
                sourceControlStatus = (SourceControl as IStatusSnapshotGenerator).GenerateSnapshot();
            }
            else
            {
                sourceControlStatus = new ItemStatus(
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0}: {1}",
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

                var tbase = task as TaskBase;
                if (tbase != null)
                {
                    tbase.InitialiseStatus();
                }

                var item = task as IStatusItem;
                if (item != null)
                {
                    item.InitialiseStatus();
                    taskItem = item.GenerateSnapshot();
                }


                var dummyStatusSnapshotGenerator = task as IStatusSnapshotGenerator;
                if (dummyStatusSnapshotGenerator != null)
                {
                    taskItem = dummyStatusSnapshotGenerator.GenerateSnapshot();
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

        /// <summary>
        /// Notifies the state of the pending.	
        /// </summary>
        /// <remarks></remarks>
        public void NotifyPendingState()
        {
            currentActivity = ProjectActivity.Pending;
        }

        /// <summary>
        /// Notifies the state of the sleeping.	
        /// </summary>
        /// <remarks></remarks>
        public void NotifySleepingState()
        {
            currentActivity = ProjectActivity.Sleeping;
        }

        /// <summary>
        /// Prebuilds the specified result.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
        public void Prebuild(IIntegrationResult result)
        {
            var parameters = new Dictionary<string, string>();
            if (result.Parameters != null) parameters = NameValuePair.ToDictionary(result.Parameters);
            Prebuild(result, parameters);
        }

        /// <summary>
        /// Prebuilds the specified result.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <remarks></remarks>
        public void Prebuild(IIntegrationResult result, Dictionary<string, string> parameterValues)
        {
            RunTasks(result, PrebuildTasks, parameterValues);
        }

        /// <summary>
        /// Validates the parameters.	
        /// </summary>
        /// <param name="parameterValues">The parameter values.</param>
        /// <remarks></remarks>
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

        /// <summary>
        /// Runs the specified result.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
        public void Run(IIntegrationResult result)
        {
            var parameters = new Dictionary<string, string>();
            if (result.Parameters != null) parameters = NameValuePair.ToDictionary(result.Parameters);
            Run(result, parameters);
        }

        /// <summary>
        /// Runs the specified result.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <remarks></remarks>
        public void Run(IIntegrationResult result, Dictionary<string, string> parameterValues)
        {
            RunTasks(result, tasks, parameterValues);
        }

        private void RunTasks(IIntegrationResult result, IList tasksToRun, Dictionary<string, string> parameterValues)
        {
            foreach (ITask task in tasksToRun)
            {
                var dummy = task as IParamatisedItem;
                if (dummy != null)
                {
                    dummy.ApplyParameters(parameterValues, parameters);
                }

                RunTask(task, result, false);
                if (result.Failed) break;
            }
            CancelTasks(tasksToRun);
        }

        /// <summary>
        /// Aborts the running build.	
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <remarks></remarks>
        public void AbortRunningBuild(string userName)
        {
            AddMessage(new Message(userName, Message.MessageKind.BuildAbortedBy));
            ProcessExecutor.KillProcessCurrentlyRunningForProject(Name);
        }

        /// <summary>
        /// Publishes the results.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
        public void PublishResults(IIntegrationResult result)
        {
            var parameters = new Dictionary<string, string>();
            if (result.Parameters != null) parameters = NameValuePair.ToDictionary(result.Parameters);
            PublishResults(result, parameters);
        }

        /// <summary>
        /// Publishes the results.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <remarks></remarks>
        public void PublishResults(IIntegrationResult result, Dictionary<string, string> parameterValues)
        {
            // Make sure all the tasks have been cancelled
            CancelTasks(PrebuildTasks);
            CancelTasks(Tasks);

            var merged = false;
            var mergeFailed = false;
            foreach (ITask publisher in publishers)
            {
                var isMergeTask = publisher is IMergeTask;
                try
                {
                    merged |= isMergeTask;
                    var dummy = publisher as IParamatisedItem;
                    if (dummy != null)
                    {
                        dummy.ApplyParameters(parameterValues, parameters);
                    }

                    RunTask(publisher, result, true);
                }
                catch (Exception e)
                {
                    mergeFailed |= isMergeTask;
                    Log.Error("Publisher threw exception: " + e);
                }
            }
            if (result.Succeeded)
            {
                messages.Clear();
                //result.FailureTasks.Clear();
            }
            else
            {
                AddBreakersToMessages(result);
                AddFailedTaskToMessages();

                var failedTasks = new List<string>();
                FindFailedTasks(currentProjectStatus, failedTasks);

                //result.FailureTasks.Clear();
                result.FailureTasks.AddRange(failedTasks);
            }

            this.ClearMessages(Message.MessageKind.BuildStatus);
            if (merged && !mergeFailed)
            {
                // Clean up any temporary results
                foreach (var taskResult in result.TaskResults)
                {
                    var temporaryResult = taskResult as ITemporaryResult;
                    if (temporaryResult != null)
                    {
                        temporaryResult.CleanUp();
                    }
                }
            }
            else
            {
                // Display a warning message if there are temporary files that were not deleted
                var hasTemporaryFiles = false;
                foreach (var taskResult in result.TaskResults)
                {
                    var temporaryResult = taskResult as ITemporaryResult;
                    if (temporaryResult != null)
                    {
                        hasTemporaryFiles = true;
                        break;
                    }
                }

                // Display the appropriate message
                // TODO: replace the hard-coded Log with an ILogger - cannot unit test otherwise
                if (hasTemporaryFiles)
                {
                    if (mergeFailed)
                    {
                        Log.Warning("One or more merge publishers failed - temporary files have not been deleted!");
                    }
                    else
                    {
                        Log.Warning("There are no merge publishers - temporary files have not been deleted!");
                    }
                }
            }
        }

        /// <summary>
        /// Runs a specific task and updates the status for the task.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="result"></param>
        /// <param name="isPublisher"></param>
        private void RunTask(ITask task, IIntegrationResult result, bool isPublisher)
        {
            // Load the status details
            ItemStatus status = null;
            if (currentProjectItems.ContainsKey(task)) status = currentProjectItems[task];

            // If there is a status, update it
            var baseTask = task as TaskBase;
            if (status != null)
            {
                if (baseTask != null)
                {
                    status.TimeOfEstimatedCompletion = baseTask.CalculateEstimatedTime();
                    isPublisher = false;
                }

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
                // publishers do not get the overall status, as they are also ran for failed builds
                // a pulisher must have the failed status if itself failed, not if a build failed
                task.Run(result);
                if (status != null && !isPublisher)
                {
                    var wasSuccessful = !result.Failed;
                    if (baseTask != null)
                    {
                        wasSuccessful = baseTask.WasSuccessful;
                    }

                    // Only need to update the status if it is not already set
                    switch (status.Status)
                    {
                        case ItemBuildStatus.Pending:
                        case ItemBuildStatus.Running:
                        case ItemBuildStatus.Unknown:
                            if (wasSuccessful)
                            {
                                status.Status = ItemBuildStatus.CompletedSuccess;
                            }
                            else
                            {
                                status.Status = ItemBuildStatus.CompletedFailed;
                            }
                            break;
                    }
                }
            }
            catch
            {
                // An exception was thrown, so we will assume that the task failed
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
            List<string> breakers = new List<string>();
            string breakingusers = string.Empty;

            foreach (Modification mod in result.Modifications)
            {
                if (!breakers.Contains(mod.UserName))
                {
                    breakers.Add(mod.UserName);
                }
            }


            foreach (string userName in result.FailureUsers)
            {
                if (!breakers.Contains(userName))
                {
                    breakers.Add(userName);
                }
            }


            if (breakers.Count > 0)
            {
                breakingusers = string.Empty;

                foreach (string user in breakers)
                {
                    breakingusers += user + ", ";
                }

                breakingusers = breakingusers.Remove(breakingusers.Length - 2, 2); // remove the last comma and space
            }

            AddMessage(new Message(breakingusers, Message.MessageKind.Breakers));
        }

        private void AddFailedTaskToMessages()
        {
            // Find all the items that failed (there can be multiple)
            var failedTasks = new List<string>();
            FindFailedTasks(currentProjectStatus, failedTasks);

            if (failedTasks.Count > 0)
            {
                // Add a message containing the failed tasks
                AddMessage(
                    new Message(
                         string.Join(
                            ", ",
                            failedTasks.ToArray()), Message.MessageKind.FailingTasks));
            }
        }

        private void FindFailedTasks(ItemStatus item, List<string> failedTasks)
        {
            if (item.ChildItems.Count > 0)
            {
                foreach (var childItem in item.ChildItems)
                {
                    FindFailedTasks(childItem, failedTasks);
                }
            }
            else
            {
                if (item.Status == ItemBuildStatus.CompletedFailed)
                {
                    if (string.IsNullOrEmpty(item.Description))
                    { failedTasks.Add(item.Name); }
                    else
                    { failedTasks.Add(item.Description); }
                }
            }
        }

        /// <summary>
        /// Initializes this instance.	
        /// </summary>
        /// <remarks></remarks>
        public void Initialize()
        {
            Log.Info(string.Format(System.Globalization.CultureInfo.CurrentCulture, "Initializing Project [{0}]", Name));
            SourceControl.Initialize(this);
        }

        /// <summary>
        /// Purges the specified purge working directory.	
        /// </summary>
        /// <param name="purgeWorkingDirectory">The purge working directory.</param>
        /// <param name="purgeArtifactDirectory">The purge artifact directory.</param>
        /// <param name="purgeSourceControlEnvironment">The purge source control environment.</param>
        /// <remarks></remarks>
        public void Purge(bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment)
        {
            Log.Info(string.Format(System.Globalization.CultureInfo.CurrentCulture, "Purging Project [{0}]", Name));
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

        /// <summary>
        /// Gets the statistics.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string Statistics
        {
            get { return StatisticsPublisher.LoadStatistics(ArtifactDirectory); }
        }

        /// <summary>
        /// Gets the modification history.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string ModificationHistory
        {
            get { return ModificationHistoryPublisher.LoadHistory(ArtifactDirectory); }
        }


        /// <summary>
        /// Gets the RSS feed.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string RSSFeed
        {
            get
            {
                if (this.RssFeedLoader != null)
                {
                    return this.RssFeedLoader();
                }

                return RssPublisher.LoadRSSDataDocument(ArtifactDirectory);
            }
        }

        public Func<string> RssFeedLoader { get; set; }


        /// <summary>
        /// Gets the integration repository.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public IIntegrationRepository IntegrationRepository
        {
            get { return this; }
        }

        /// <summary>
        /// Defaults the URL.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string DefaultUrl()
        {
            return string.Format(System.Globalization.CultureInfo.CurrentCulture, "http://{0}/ccnet", Environment.MachineName);
        }

        /// <summary>
        /// Creates the project status.	
        /// </summary>
        /// <param name="integrator">The integrator.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public ProjectStatus CreateProjectStatus(IProjectIntegrator integrator)
        {
            var lastIntegration = this.LastIntegration;
            ProjectStatus status = new ProjectStatus(
                this.Name,
                this.Category,
                this.CurrentActivity,
                lastIntegration.Status,
                integrator.State,
                this.WebURL,
                lastIntegration.StartTime,
                lastIntegration.Label,
                lastIntegration.LastSuccessfulIntegrationLabel,
                this.Triggers.NextBuild,
                this.CurrentBuildStage(),
                this.QueueName,
                this.QueuePriority);
            status.Description = this.Description;
            status.Messages = this.messages.ToArray();
            status.ShowForceBuildButton = this.ShowForceBuildButton;
            status.ShowStartStopButton = this.ShowStartStopButton;
            return status;
        }

        private string CurrentBuildStage()
        {
            if (CurrentActivity == ProjectActivity.Building ||
                CurrentActivity == ProjectActivity.CheckingModifications)
                return integrationResultManager.CurrentIntegration.BuildProgressInformation.GetBuildProgressInformation();

            else
                return string.Empty;
        }

        private IntegrationSummary LastIntegration
        {
            get { return integrationResultManager.LastIntegration; }
        }

        /// <summary>
        /// Adds the message.	
        /// </summary>
        /// <param name="message">The message.</param>
        /// <remarks></remarks>
        public void AddMessage(Message message)
        {
            if (message.Kind == Message.MessageKind.Fixer)
            {
                // only show the last fixer
                var existingFixerMessage = (from m in messages where m.Kind == Message.MessageKind.Fixer select m).SingleOrDefault();
                if (existingFixerMessage != null)
                {
                    messages.Remove(existingFixerMessage);
                }
            }

            messages.Add(message);
        }

        /// <summary>
        /// Gets the build log.	
        /// </summary>
        /// <param name="buildName">Name of the build.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetBuildLog(string buildName)
        {
            var logDirectory = GetLogDirectory();
            if (string.IsNullOrEmpty(logDirectory))
            {
                return string.Empty;
            }

            var filepath = Path.Combine(logDirectory, buildName);

            // Only run this check in a windows environment as MemoryFailPoint has not been implemented in Mono
            if (this.CurrentExecutionEnvironment.IsRunningOnWindows)
            {
                // Check that there is sufficient memory to load the log into memory
                var fileInfo = new FileInfo(filepath);
                if (fileInfo.Length > 1048576)
                {
                    // Since the file is over one Mb, check if there is enough free memory to load the data
                    // Note: We are actually checking to see if there is twice the amount of memory required, this is because often the 
                    // data will need to be copied somewhere else, which means the string will exist in memory at least twice (hopefully
                    // GC will clean up if it is needed more than twice)
                    var fileSizeInMB = Convert.ToInt32(fileInfo.Length / 524288);
                    try
                    {
                        using (new MemoryFailPoint(fileSizeInMB))
                        {
                        }
                    }
                    catch (InsufficientMemoryException error)
                    {
                        // Much nicer to handle an InsufficientMemoryException exception than an OutOfMemoryException - OOM tends to kill
                        // things!
                        throw new CruiseControlException("Insufficient memory to retrieve log: " + error.Message, error);
                    }
                }
            }

            using (var sr = new StreamReader(filepath))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// Gets the build names.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string[] GetBuildNames()
        {
            string logDirectory = GetLogDirectory();
            if (string.IsNullOrEmpty(logDirectory)) return new string[0];
            try
            {
                string[] logFileNames = LogFileUtil.GetLogFileNames(logDirectory);
                Array.Sort(logFileNames);       // Sort the list of filenames since not all *nix systems return a sorted list
                Array.Reverse(logFileNames);
                return logFileNames;
            }
            catch (DirectoryNotFoundException error)
            {
                throw new CruiseControlException("Log directory does not exist. This normally occurs because there are no builds for this project.", error);
            }
        }

        /// <summary>
        /// Gets the most recent build names.	
        /// </summary>
        /// <param name="buildCount">The build count.</param>
        /// <returns></returns>
        /// <remarks></remarks>
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

        /// <summary>
        /// Gets the name of the latest build.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
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
                var xmlPublisher = publisher as XmlLogPublisher;
                if (xmlPublisher != null)
                {
                    return xmlPublisher;
                }
            }
            throw new CruiseControlException("Unable to find Log Publisher for project so can't find log file");
        }

        /// <summary>
        /// Creates the label.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <remarks></remarks>
        public void CreateLabel(IIntegrationResult result)
        {
            if (Labeller is IParamatisedItem)
            {
                (Labeller as IParamatisedItem).ApplyParameters(result.IntegrationRequest.BuildValues,
                    parameters);
            }
            result.Label = Labeller.Generate(result);
        }

        /// <summary>
        /// Sets the state of the project when CCNet service/Console starts. Stopped can be handy when you are adding a lot of projects which
        /// are depending on other projects (via the project trigger) and these may not be build right away. This value is only used when
        /// startupMode is set to UseInitialState.
        /// </summary>
        /// <version>1.5</version>
        /// <default>Started</default>
        [ReflectorProperty("initialState", Required = false)]
        public ProjectInitialState InitialState
        {
            get { return initialState; }
            set { initialState = value; }
        }

        /// <summary>
        /// The start-up mode for this project.
        /// </summary>
        /// <version>1.5</version>
        /// <default>UseLastState</default>
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
        /// <param name="errorProcesser"></param>
        public virtual void Validate(IConfiguration configuration, ConfigurationTrace parent, IConfigurationErrorProcesser errorProcesser)
        {
            if (security.RequiresServerSecurity &&
                (configuration.SecurityManager is NullSecurityManager))
            {
                errorProcesser.ProcessError(
                    new ConfigurationException(
                        string.Format(System.Globalization.CultureInfo.CurrentCulture, "Security is defined for project '{0}', but not defined at the server", this.Name)));
            }

            this.ValidateProject(errorProcesser);
            this.ValidateItem(sourceControl, configuration, parent, errorProcesser);
            this.ValidateItem(labeller, configuration, parent, errorProcesser);
            this.ValidateItems(PrebuildTasks, configuration, parent, errorProcesser);
            this.ValidateItems(tasks, configuration, parent, errorProcesser);
            this.ValidateItems(publishers, configuration, parent, errorProcesser);
            this.ValidateItem(state, configuration, parent, errorProcesser);
            this.ValidateItem(security, configuration, parent, errorProcesser);

            var mt = (MultipleTrigger)this.Triggers;
            if (mt != null)
            {
                this.ValidateItems(mt.Triggers, configuration, parent, errorProcesser);
            }

            this.configuration = configuration;
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
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "Project name '{0}' contains some chars that could cause problems, better use only numbers and letters",
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
        /// <param name="item">The item.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="errorProcesser">The error processer.</param>
        private void ValidateItem(object item, IConfiguration configuration, ConfigurationTrace parent, IConfigurationErrorProcesser errorProcesser)
        {
            if (item == null) return;

            var dummy = item as IConfigurationValidation;
            if (dummy != null)
            {
                dummy.Validate(configuration, parent.Wrap(this), errorProcesser);
            }
        }

        /// <summary>
        /// Validates the configuration of an enumerable.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="errorProcesser">The error processer.</param>
        private void ValidateItems(IEnumerable items, IConfiguration configuration, ConfigurationTrace parent, IConfigurationErrorProcesser errorProcesser)
        {
            if (items != null)
            {
                foreach (object item in items)
                {
                    this.ValidateItem(item, configuration, parent, errorProcesser);
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
            List<PackageDetails> packages = new List<PackageDetails>();
            foreach (string packgeInfo in Directory.GetFiles(ArtifactDirectory, "*-packages.xml", SearchOption.AllDirectories))
            {
                packages.AddRange(LoadPackageList(packgeInfo));
            }
            return packages;
        }

        /// <summary>
        /// Retrieves the list of packages for a build.
        /// </summary>
        /// <param name="buildLabel"></param>
        /// <returns></returns>
        public virtual List<PackageDetails> RetrievePackageList(string buildLabel)
        {
            var listFile = Path.Combine(buildLabel, Name + "-packages.xml");
            listFile = Path.Combine(ArtifactDirectory, listFile);
            if (File.Exists(listFile))
            {
                var packages = LoadPackageList(listFile);
                return packages;
            }
            else
            {
                return new List<PackageDetails>();
            }
        }
        #endregion

        #region RetrieveBuildFinalStatus()
        /// <summary>
        /// Retrieves the final status of a build.
        /// </summary>
        /// <param name="buildName">Name of the build.</param>
        /// <returns>The final status if found; <c>null</c> otherwise.</returns>
        public ItemStatus RetrieveBuildFinalStatus(string buildName)
        {
            if (this.DataStore == null)
            {
                return null;
            }

            Log.Debug("Retrieving final status for build [" + buildName + "]");
            var snapshot = this.DataStore.LoadProjectSnapshot(this, buildName);
            return snapshot;
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
                    details.DateTime = DateTime.Parse(packageElement.GetAttribute("time"), CultureInfo.CurrentCulture);
                    details.NumberOfFiles = Convert.ToInt32(packageElement.GetAttribute("files"), CultureInfo.CurrentCulture);
                    details.Size = Convert.ToInt64(packageElement.GetAttribute("size"), CultureInfo.CurrentCulture);
                    packages.Add(details);
                }
            }
            return packages;
        }
        #endregion

        #region ConfigurationXml
        /// <summary>
        /// Gets or sets the configuration XML.
        /// </summary>
        /// <value>The configuration XML.</value>
        public string ConfigurationXml { get; private set; }
        #endregion

        #region CryptoFunctions
        /// <summary>
        /// Gets or sets the crypto functions.
        /// </summary>
        /// <value>The crypto functions.</value>
        public ICryptoFunctions CryptoFunctions { get; set; }
        #endregion

        #region PreprocessConfiguration()
        /// <summary>
        /// Preprocesses a node prior to loading it via NetReflector.
        /// </summary>
        /// <param name="typeTable">The type table.</param>
        /// <param name="inputNode">The input node.</param>
        /// <returns>
        /// The original node.
        /// </returns>
        /// <remarks>
        /// This method just stores a copy of the original configuraiton. This allows the configuration to
        /// be propogated to remote agents.
        /// </remarks>
        [ReflectionPreprocessor]
        public virtual XmlNode PreprocessConfiguration(NetReflectorTypeTable typeTable, XmlNode inputNode)
        {
            this.ConfigurationXml = inputNode.OuterXml;
            return inputNode;
        }
        #endregion

        /// <summary>
        /// Lists the parameters for the project.
        /// </summary>
        /// <returns></returns>
        public virtual List<ParameterBase> ListBuildParameters()
        {
            var parameterList = new List<ParameterBase>();

            // Add the force build reason
            if (AskForForceBuildReason != DisplayLevel.None)
            {
                var reasonParameter = new TextParameter
                {
                    Description = "What is the reason for this force build?",
                    DisplayName = "Reason",
                    Name = "CCNetForceBuildReason",
                    IsRequired = (AskForForceBuildReason == DisplayLevel.Required)
                };
                parameterList.Add(reasonParameter);
            }

            // Add the user defined parameters
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    parameter.GenerateClientDefault();
                }
                parameterList.AddRange(parameters);
            }
            return parameterList;
        }

        /// <summary>
        /// Clears the not needed messages.	
        /// </summary>
        /// <remarks></remarks>
        public void ClearNotNeededMessages()
        {
            ClearMessages(Message.MessageKind.Breakers);
            ClearMessages(Message.MessageKind.FailingTasks);
            ClearMessages(Message.MessageKind.BuildAbortedBy);

        }

        /// <summary>
        /// Initialises the target for a build.
        /// </summary>
        /// <param name="request">The request.</param>
        public void InitialiseForBuild(IntegrationRequest request)
        {
            this.ClearMessages(Message.MessageKind.BuildStatus);
            this.AddMessage(new Message(request.ToString(), Message.MessageKind.BuildStatus));
        }
    }
}