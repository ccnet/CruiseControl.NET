using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Label;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	/// <summary>
	/// Information about a project and how to integrate it.  As multiple projects
	/// per CruiseControl.NET server are supported, all project-specific information
	/// must be captured here.
	/// </summary>
	/// <remarks>
	/// A project is the combination of the source control location,
	/// build command, publishers, and state elements.
	/// <code>
	/// <![CDATA[
	/// <project name="foo">
	///		<sourcecontrol type="cvs"></sourcecontrol>
	///		<build type="nant"></build>
	///		<state type="state"></state>
	///		<publishers></publishers>
	/// </project>
	/// ]]>
	/// </code>
	/// </remarks>
	[ReflectorType("project")]
	public class Project : ProjectBase, IProject, IIntegrationRunnerTarget
	{
		/// <summary>
		/// Raised whenever an integration is completed.
		/// </summary>
		public event IntegrationCompletedEventHandler IntegrationCompleted;

		private string webUrl = DefaultUrl();
		private ISourceControl sourceControl = new NullSourceControl();
		private ITask builder = new NullTask();
		private ILabeller labeller = new DefaultLabeller();
		private ITask[] tasks = new ITask[0];
		private IIntegrationCompletedEventHandler[] publishers = new IIntegrationCompletedEventHandler[0];
		private ProjectActivity currentActivity = ProjectActivity.Sleeping;
		private int modificationDelaySeconds = 0;
		private IStateManager state;
		private IIntegrationResultManager integrationResultManager;
		private bool publishExceptions = true;
		private IIntegratable integratable;

		public Project()
		{
			state = new ProjectStateManager(this, new IntegrationStateManager());
			integrationResultManager = new IntegrationResultManager(this);
			integratable = new IntegrationRunner(integrationResultManager, this);
		}

		// This is nasty - test constructors and real constructors should be linked, but we have circular references here that need
		// to be sorted out
		public Project(IIntegratable integratable) : this()
		{
			this.integratable = integratable;
		}

		[ReflectorProperty("state", InstanceTypeKey="type", Required=false)]
		[Description("State")]
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
		public IIntegrationCompletedEventHandler[] Publishers
		{
			get { return publishers; }
			set
			{
				publishers = value;

				// register each of these event handlers
				foreach (IIntegrationCompletedEventHandler handler in publishers)
					IntegrationCompleted += handler.IntegrationCompletedEventHandler;
			}
		}

		/// <summary>
		/// A period of time, in seconds.  When modifications are found within this period,
		/// a build (which would otherwise occur) is delayed until this many seconds have
		/// passed.  The intention is to allow a developer to complete a multi-stage
		/// checkin.
		/// </summary>
		[ReflectorProperty("modificationDelaySeconds", Required=false)]
		public int ModificationDelaySeconds
		{
			get { return modificationDelaySeconds; }
			set { modificationDelaySeconds = value; }
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

		public void OnIntegrationCompleted(IIntegrationResult result)
		{
			if (IntegrationCompleted != null)
			{
				IntegrationCompletedEventArgs e = new IntegrationCompletedEventArgs(result);
				IntegrationCompleted(this, e);
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