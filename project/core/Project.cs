using System.ComponentModel;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Label;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.State;
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

		public const string DEFAULT_WEB_URL = "http://localhost/CruiseControl.NET/";

		private string _webURL = DEFAULT_WEB_URL;
		private ISourceControl _sourceControl = new NullSourceControl();
		private IBuilder _builder;
		private ILabeller _labeller = new DefaultLabeller();
		private ITask[] _tasks = new ITask[0];		
		private IIntegrationCompletedEventHandler[] _publishers = new IIntegrationCompletedEventHandler[0];
		private ProjectActivity _currentActivity = ProjectActivity.Sleeping;
		private int _modificationDelaySeconds = 0;
		private IStateManager _state;
		private IIntegrationResultManager _integrationResultManager;
		private bool _publishExceptions = true;
		private IIntegratable integratable;

		public Project()
		{
			_state = new ProjectStateManager(this, new IntegrationStateManager());
			_integrationResultManager = new IntegrationResultManager(this);
			this.integratable = new IntegrationRunner(_integrationResultManager, this);
		}

		// This is nasty - test constructors and real constructors should be linked, but we have circular references here that need
		// to be sorted out
		public Project(IIntegratable integratable) : this ()
		{ 
			this.integratable = integratable;
		}

		[ReflectorProperty("state", InstanceTypeKey="type", Required=false)]
		[Description("State")]
		public virtual IStateManager StateManager
		{
			get { return _state; }
			set { _state = value; }
		}

		[ReflectorProperty("webURL", Required=false)]
		public string WebURL
		{
			get { return _webURL; }
			set { _webURL = value; }
		}

		[ReflectorProperty("build", InstanceTypeKey="type")]
		public IBuilder Builder
		{
			get { return _builder; }
			set { _builder = value; }
		}

		[ReflectorProperty("sourcecontrol", InstanceTypeKey="type", Required=false)]
		public ISourceControl SourceControl
		{
			get { return _sourceControl; }
			set { _sourceControl = value; }
		}

		/// <summary>
		/// The list of build-completed publishers used by this project.  This property is
		/// intended to be set via Xml configuration.
		/// </summary>
		[ReflectorArray("publishers", Required=false)]
		public IIntegrationCompletedEventHandler[] Publishers
		{
			get { return _publishers; }
			set
			{
				_publishers = value;

				// register each of these event handlers
				foreach (IIntegrationCompletedEventHandler handler in _publishers)
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
			get { return _modificationDelaySeconds; }
			set { _modificationDelaySeconds = value; }
		}

		[ReflectorProperty("labeller", InstanceTypeKey="type", Required=false)]
		public ILabeller Labeller
		{
			get { return _labeller; }
			set { _labeller = value; }
		}

		[ReflectorArray("tasks", Required=false)]
		public ITask[] Tasks
		{
			get { return _tasks; }
			set { _tasks = value; }
		}

		[ReflectorProperty("publishExceptions", Required=false)]
		public bool PublishExceptions
		{
			get { return _publishExceptions; }
			set { _publishExceptions = value; }
		}

		// Move this ideally
		public ProjectActivity Activity
		{
			set { _currentActivity = value; }
		}

		public ProjectActivity CurrentActivity
		{
			get { return _currentActivity; }
		}

		public IIntegrationResult LastIntegrationResult
		{
			get { return _integrationResultManager.LastIntegrationResult; }
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
			Builder.Run(result);
			foreach (ITask task in _tasks)
			{
				task.Run(result);
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
	}
}