using System;
using System.ComponentModel;
using ThoughtWorks.CruiseControl.Core.Label;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using Exortech.NetReflector;

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
	///		<schedule type="schedule" sleepSeconds="300" />
	/// </project>
	/// ]]>
	/// </code>
	/// </remarks>
	[ReflectorType("project")]
	public class Project : ProjectBase, IProject
	{
		/// <summary>
		/// Raised whenever an integration is completed.
		/// </summary>
		public event IntegrationCompletedEventHandler IntegrationCompleted;

		public const string DEFAULT_WEB_URL = "http://localhost/CruiseControl.NET/";

		private string _webURL = DEFAULT_WEB_URL;
		private ISourceControl _sourceControl;
		private IBuilder _builder;
		private ILabeller _labeller = new DefaultLabeller();
		private IIntegrationCompletedEventHandler[] _publishers;
		private IntegrationResult _lastIntegrationResult = null;
		private ProjectActivity _currentActivity = ProjectActivity.Unknown;
		private int _modificationDelaySeconds = 0;
		private IStateManager _state;

		public Project()
		{
			_state = new ProjectStateManager(this, new IntegrationStateManager());
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

		[ReflectorProperty("sourcecontrol", InstanceTypeKey="type")]
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
		public ITask[] Tasks = new ITask[0];

		[ReflectorProperty("publishExceptions", Required=false)]
		public bool PublishExceptions = false;

		public ProjectActivity CurrentActivity
		{
			get { return _currentActivity; }
		}

		public IntegrationResult LastIntegrationResult
		{
			get
			{
				if (_lastIntegrationResult == null)
					_lastIntegrationResult = LoadLastIntegration();

				return _lastIntegrationResult;
			}
		}

		public IntegrationStatus LatestBuildStatus
		{
			get { return LastIntegrationResult.Status; }
		}

		public IntegrationResult RunIntegration(BuildCondition buildCondition)
		{
			IntegrationResult result = CreateNewIntegrationResult(buildCondition);
			AttemptToRunIntegration(result);
			PostBuild(result);
			return result;
		}

		private IntegrationResult CreateNewIntegrationResult(BuildCondition buildCondition)
		{
			IntegrationResult result = new IntegrationResult(Name);
			if (LastIntegrationResult.IsInitial())
			{
				result.BuildCondition = BuildCondition.ForceBuild;
			}
			else
			{
				result.BuildCondition = buildCondition;
				result.LastIntegrationStatus = LastIntegrationResult.Status;				
			}
			result.Label = Labeller.Generate(LastIntegrationResult);
			return result;
		}

		private void AttemptToRunIntegration(IntegrationResult result)
		{
			result.MarkStartTime();
			try
			{
				result.Modifications = GetSourceModifications(result);
				if (ShouldRunBuild(result))
				{
					CreateTemporaryLabelIfNeeded();
					_sourceControl.GetSource(result);
					RunBuild(result);
					RunTasks(result);
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex);
				result.ExceptionResult = ex;
				result.Status = IntegrationStatus.Exception;
			}
			result.MarkEndTime();
		}

		private Modification[] GetSourceModifications(IntegrationResult results)
		{
			_currentActivity = ProjectActivity.CheckingModifications;
			Modification[] modifications = SourceControl.GetModifications(LastIntegrationResult.StartTime, results.StartTime);
			Log.Info(GetModificationsDetectedMessage(modifications));
			return modifications;
		}

		private string GetModificationsDetectedMessage(Modification[] modifications)
		{
			switch (modifications.Length)
			{
				case 0:
					return "No modifications detected.";
				case 1:
					return "1 modification detected.";
				default:
					return string.Format("{0} modifications detected.", modifications.Length);
			}
		}

		private void RunBuild(IntegrationResult result)
		{
			_currentActivity = ProjectActivity.Building;

			if (result.BuildCondition == BuildCondition.ForceBuild)
				Log.Info("Build forced");

			Log.Info("Building");

			Builder.Run(result);

			Log.Info("Build complete: " + result.Status);
		}

		private void RunTasks(IntegrationResult result)
		{
			foreach (ITask task in Tasks)
			{
				task.Run(result);
			}
		}

		internal void PostBuild(IntegrationResult result)
		{
			if (ShouldPublishException(result))
			{
				Log.Info("Integration complete: " + result.EndTime);

				AttemptToSaveState(result);

				HandleProjectLabelling(result);

				// raise event (publishers do their thing in response)
				OnIntegrationCompleted(new IntegrationCompletedEventArgs(result));

				// update reference to the most recent result
				_lastIntegrationResult = result;
			}
			_currentActivity = ProjectActivity.Sleeping;
		}

		private bool ShouldPublishException(IntegrationResult result)
		{
			if (result.Status == IntegrationStatus.Exception)
			{
				return PublishExceptions;
			}
			else
			{
				return result.Status != IntegrationStatus.Unknown;
			}
		}

		private void AttemptToSaveState(IntegrationResult results)
		{
			try
			{
				StateManager.SaveState(results);
			}
			catch (CruiseControlException ex)
			{
				Log.Error(ex);

				if (results.ExceptionResult == null)
					results.ExceptionResult = ex;
			}
		}

		private IntegrationResult LoadLastIntegration()
		{
			if (StateManager.StateFileExists())
			{
				return StateManager.LoadState();
			}
			else
			{
				// no integration result is on record
				return IntegrationResult.CreateInitialIntegrationResult(Name);
			}
		}

		/// <summary>
		/// Determines whether a build should run.  A build should run if there
		/// are modifications, and none have occurred within the modification
		/// delay.
		/// </summary>
		internal bool ShouldRunBuild(IntegrationResult results)
		{
			if (results.BuildCondition == BuildCondition.ForceBuild)
				return true;

			if (results.HasModifications())
				return ! DoModificationsExistWithinModificationDelay(results);

			return false;
		}

		/// <summary>
		/// Checks whether modifications occurred within the modification delay.  If the
		/// modification delay is not set (has a value of zero or less), this method
		/// will always return false.
		/// </summary>
		private bool DoModificationsExistWithinModificationDelay(IntegrationResult results)
		{
			if (ModificationDelaySeconds <= 0)
				return false;

			//TODO: can the last mod date (which is the time on the SCM) be compared with now (which is the time on the build machine)?
			TimeSpan diff = DateTime.Now - results.LastModificationDate;
			if (diff.TotalSeconds < ModificationDelaySeconds)
			{
				Log.Info("Changes found within the modification delay");
				return true;
			}

			return false;
		}

		/// <summary>
		/// Raises the IntegrationCompleted event.
		/// </summary>
		/// <param name="e">Arguments to pass with the raised event.</param>
		private void OnIntegrationCompleted(IntegrationCompletedEventArgs e)
		{
			if (IntegrationCompleted != null)
				IntegrationCompleted(this, e);
		}

		/// <summary>
		/// Labels the project, if the build was successful.
		/// </summary>
		internal void HandleProjectLabelling(IntegrationResult result)
		{
			if (result.Succeeded)
				SourceControl.LabelSourceControl(result.Label, result.StartTime);
			else
				DeleteTemporaryLabelIfNeeded();
		}

		internal void CreateTemporaryLabelIfNeeded()
		{
			if (SourceControl is ITemporaryLabeller)
			{
				((ITemporaryLabeller) SourceControl).CreateTemporaryLabel();
			}
		}

		internal void DeleteTemporaryLabelIfNeeded()
		{
			if (SourceControl is ITemporaryLabeller)
			{
				((ITemporaryLabeller) SourceControl).DeleteTemporaryLabel();
			}
		}

	}
}