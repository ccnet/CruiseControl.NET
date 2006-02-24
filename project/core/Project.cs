using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Label;
using ThoughtWorks.CruiseControl.Core.Publishers;
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
	public class Project : ProjectBase, IProject, IIntegrationRunnerTarget
	{
		private string webUrl = DefaultUrl();
		private ISourceControl sourceControl = new NullSourceControl();
		private ITask builder = new NullTask();
		private ILabeller labeller = new DefaultLabeller();
		private ITask[] tasks = new ITask[0];
		private ITask[] publishers = new ITask[] {new XmlLogPublisher()};
		private ProjectActivity currentActivity = ProjectActivity.Sleeping;
		private IStateManager state = new FileStateManager(new SystemIoFileSystem());
		private IIntegrationResultManager integrationResultManager;
		private bool publishExceptions = true;
		private IIntegratable integratable;
		private QuietPeriod quietPeriod = new QuietPeriod(new DateTimeProvider());

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

		[ReflectorProperty("build", InstanceTypeKey="type", Required=false)]
		public ITask Builder
		{
			get { return builder; }
			set { builder = value; }
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

		[ReflectorProperty("publishExceptions", Required=false)]
		public bool PublishExceptions
		{
			get { return publishExceptions; }
			set { publishExceptions = value; }
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

		public IIntegrationResult LastIntegrationResult
		{
			get { return integrationResultManager.LastIntegrationResult; }
		}

		public IntegrationStatus LatestBuildStatus
		{
			get { return LastIntegrationResult.Status; }
		}

		public IIntegrationResult RunIntegration(BuildCondition buildCondition)
		{
			if (buildCondition == BuildCondition.ForceBuild)
				Log.Info("Build forced");

			return integratable.RunIntegration(buildCondition);
		}

		public void Run(IIntegrationResult result)
		{
			IList tasksToRun = new ArrayList(tasks);
			if (Builder != null) tasksToRun.Insert(0, builder);

			foreach (ITask task in tasksToRun)
			{
				task.Run(result);
				if (result.Failed) break;
			}
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

		public static string DefaultUrl()
		{
			return string.Format("http://{0}/ccnet", Environment.MachineName);
		}
	}
}