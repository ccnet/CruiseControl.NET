using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Label;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Core.Publishers.Statistics;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

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
	public class Project : ProjectBase, IProject, IIntegrationRunnerTarget, IIntegrationRepository
	{
		private string webUrl = DefaultUrl();
		private string queueName = string.Empty;
		private int queuePriority = 0;
		private ISourceControl sourceControl = new NullSourceControl();
		private ILabeller labeller = new DefaultLabeller();
		private ITask[] tasks = new ITask[] {new NullTask()};
		private ITask[] publishers = new ITask[] {new XmlLogPublisher()};
		private ProjectActivity currentActivity = ProjectActivity.Sleeping;
		private IStateManager state = new FileStateManager(new SystemIoFileSystem());
		private IIntegrationResultManager integrationResultManager;
		private IIntegratable integratable;
		private QuietPeriod quietPeriod = new QuietPeriod(new DateTimeProvider());
		private ArrayList messages = new ArrayList();

		[ReflectorProperty("prebuild", Required=false)]
		public ITask[] PrebuildTasks = new ITask[0];

		public Project()
		{
			integrationResultManager = new IntegrationResultManager(this);
			integratable = new IntegrationRunner(integrationResultManager, this, quietPeriod);
		}

		public Project(IIntegratable integratable) : this()
		{
			this.integratable = integratable;
		}

		[ReflectorProperty("state", InstanceTypeKey="type", Required=false), Description("State")]
		public IStateManager StateManager
		{
			get { return state; }
			set { state = value; }
		}

		[ReflectorProperty("webURL", Required=false)]
		public string WebURL
		{
			get { return webUrl; }
			set { webUrl = value; }
		}

		[ReflectorProperty("queue", Required=false)]
		public string QueueName
		{
			get 
			{
				if (queueName == null | queueName.Length == 0)
				{
					return Name;
				}
				return queueName; 
			}
			set { queueName = value; }
		}

		[ReflectorProperty("queuePriority", Required=false)]
		public int QueuePriority
		{
			get {return queuePriority; }
			set { queuePriority = value; }
		}

		[ReflectorProperty("sourcecontrol", InstanceTypeKey="type", Required=false)]
		public ISourceControl SourceControl
		{
			get { return sourceControl; }
			set { sourceControl = value; }
		}

		/// <summary>
		/// The list of build-completed publishers used by this project.  This property is
		/// intended to be set via Xml configuration.
		/// </summary>
		[ReflectorArray("publishers", Required=false)]
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
		[ReflectorProperty("modificationDelaySeconds", Required=false)]
		public double ModificationDelaySeconds
		{
			get { return quietPeriod.ModificationDelaySeconds; }
			set { quietPeriod.ModificationDelaySeconds = value; }
		}

		[ReflectorProperty("labeller", InstanceTypeKey="type", Required=false)]
		public ILabeller Labeller
		{
			get { return labeller; }
			set { labeller = value; }
		}

		[ReflectorArray("tasks", Required=false)]
		public ITask[] Tasks
		{
			get { return tasks; }
			set { tasks = value; }
		}

		// Move this ideally
		public ProjectActivity Activity
		{
			set { currentActivity = value; }
		}

		public ProjectActivity CurrentActivity
		{
			get { return currentActivity; }
		}
		
		public ProcessMonitor ProcessMonitor
		{
			get { return ProcessMonitor.GetProcessMonitorByProject(this.Name); }
		}
		
		public IIntegrationResult CurrentResult
		{
			get { return integrationResultManager.CurrentIntegration; }
		}

		public IIntegrationResult Integrate(IntegrationRequest request)
		{
			return integratable.Integrate(request);
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
			result.Label = Labeller.Generate(result);
			RunTasks(result, PrebuildTasks);
		}

		public void Run(IIntegrationResult result)
		{
			RunTasks(result, tasks);
		}
		
		private static void RunTasks(IIntegrationResult result, IList tasksToRun)
		{
			foreach (ITask task in tasksToRun)
			{
				task.Run(result);
				if (result.Failed) break;
			}
		}
		
		public string AbortRunningBuild()
		{
			return this.ProcessMonitor.KillProcess();
		}
		
		public void PublishResults(IIntegrationResult result)
		{
			foreach (ITask publisher in publishers)
			{
				try
				{
					publisher.Run(result);
				}
				catch (Exception e)
				{
					Log.Error("Publisher threw exception: " + e);
				}
			}
			if (result.Succeeded) messages.Clear();
		}

		public void Initialize()
		{
			Log.Info(string.Format("Initiatizing Project [{0}]", Name));
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

		public XmlDocument Statistics
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
                                  Triggers.NextBuild, CurrentBuildStage);
			status.Messages = (Message[]) messages.ToArray(typeof (Message));
			return status;
		}

        private string CurrentBuildStage
        {
            get
            {
                string BuildStageFile = integrationResultManager.CurrentIntegration.ListenerFile;
                string data = "";

                if (!File.Exists(BuildStageFile))
                { return ""; }
                else
                {
                    try
                    {
                        StreamReader FileReader = new StreamReader(BuildStageFile);
                        if (FileReader.BaseStream.CanRead)
                        {
                            data = FileReader.ReadToEnd();
                        }

                        FileReader.Close();
                        return data;
                    }
                    catch
                    { return ""; }
                }
            }
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
			if (StringUtil.IsBlank(logDirectory)) return "";
			using (StreamReader sr = new StreamReader(Path.Combine(logDirectory, buildName)))
			{
				return sr.ReadToEnd();
			}
		}

		public string[] GetBuildNames()
		{
			string logDirectory = GetLogDirectory();
			if (StringUtil.IsBlank(logDirectory)) return new string[0];
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
			return (string[]) buildNamesToReturn.ToArray(typeof (string));
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
			if (! Directory.Exists(logDirectory))
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
	}
}
