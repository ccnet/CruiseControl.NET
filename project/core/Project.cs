using System;
using System.Collections;

using Exortech.NetReflector;

using ThoughtWorks.CruiseControl.Core.Label;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Core.Schedules;
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

		private string _webURL = "http://localhost/CruiseControl.NET/"; // default value
		private ISourceControl _sourceControl;
		private IBuilder _builder;
		private ILabeller _labeller = new DefaultLabeller();
		private ArrayList _publishers = new ArrayList();
		private IntegrationResult _lastIntegrationResult = null;
		private ProjectActivity _currentActivity = ProjectActivity.Unknown;
		private int _modificationDelaySeconds = 0;

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
		/// intended to be set via Xml configuration.  Additional publishers should be
		/// added via the <see cref="AddPublisher"/> method.
		/// </summary>
		[ReflectorCollection("publishers", InstanceType=typeof(ArrayList), Required=false)]
		public ArrayList Publishers
		{
			get { return _publishers; }
			set 
			{ 
				_publishers = value; 

				// an arraylist is used for netreflector, and this array cast ensures
				// strong typing at runtime
				IIntegrationCompletedEventHandler[] handlers
					= (IIntegrationCompletedEventHandler[])_publishers.ToArray(typeof(IIntegrationCompletedEventHandler));

				// register each of these event handlers
				foreach (IIntegrationCompletedEventHandler handler in handlers)
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

		public ILabeller Labeller
		{
			get { return _labeller; }
			set { _labeller = value; }
		}

		public IntegrationResult LastIntegrationResult
		{
			get 
			{ 
				if (_lastIntegrationResult == null)
					_lastIntegrationResult = LoadLastIntegration();

				return _lastIntegrationResult; 
			}

			set { _lastIntegrationResult = value; }
		}

		public ProjectActivity CurrentActivity 
		{
			get { return _currentActivity; }
		}

		public IntegrationResult RunIntegration(BuildCondition buildCondition)
		{
			if (buildCondition==BuildCondition.ForceBuild)
				Log.Info("Build forced");

			IntegrationResult results = null;
			bool attemptingBuild = false;
			try
			{
				CreateNewIntegrationResult(out results);
				GetSourceModifications(results);
				attemptingBuild = ShouldRunBuild(results, buildCondition);
			
				if (attemptingBuild)
					RunBuild(results);
			}
			catch (CruiseControlException ex)
			{
				// TODO what happens when build causes other types of exceptions?
				Log.Error(ex);

				// store exception
				if (results != null)
					results.ExceptionResult = ex;

				// if an exception occurred, we're going to log it, so flag postbuild to occur
				attemptingBuild = true;
			}
			finally
			{
				if (attemptingBuild)
					PostBuild(results);
			}

			// go to sleep
			_currentActivity = ProjectActivity.Sleeping;

			return results;
		}

		internal void CreateNewIntegrationResult(out IntegrationResult results)
		{
			results = new IntegrationResult();
			results.ProjectName = Name;
			results.LastIntegrationStatus = LastIntegrationResult.Status; // test
			results.Label = Labeller.Generate(LastIntegrationResult);
			results.MarkStartTime();
		}

		internal void GetSourceModifications(IntegrationResult results)
		{
			_currentActivity = ProjectActivity.CheckingModifications;

			results.Modifications = SourceControl.GetModifications(LastIntegrationResult.StartTime,  results.StartTime);

			Log.Info(GetModificationsDetectedMessage(results));
		}

		private string GetModificationsDetectedMessage(IntegrationResult result)
		{
			switch (result.Modifications.Length)
			{
				case 0: 
					return "No modifications detected.";
				case 1:
					return "1 modification detected.";
				default:
					return string.Format("{0} modifications detected.", result.Modifications.Length);
			}
		}

		internal void RunBuild(IntegrationResult results)
		{
			_currentActivity = ProjectActivity.Building;

			Log.Info("Building");
			
			Builder.Run(results);

			Log.Info("Build complete: " + results.Status); 
		}

		internal void PostBuild(IntegrationResult results)
		{
			results.MarkEndTime();

			AttemptToSaveState(results);
			
			HandleProjectLabelling(results);
			
			// raise event (publishers do their thing in response)
			OnIntegrationCompleted(new IntegrationCompletedEventArgs(results));
			
			// update reference to the most recent result
			LastIntegrationResult = results;
			
			Log.Info("Integration complete: " + results.EndTime);
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
				// TODO consider something such as IntegrationResult.Empty, to indicate 'unknown state'
				return new IntegrationResult();
			}
		}

		/// <summary>
		/// Determines whether a build should run.  A build should run if there
		/// are modifications, and none have occurred within the modification
		/// delay.
		/// </summary>
		internal bool ShouldRunBuild(IntegrationResult results, BuildCondition buildCondition)
		{
			if (buildCondition == BuildCondition.ForceBuild)
				return true;

			if (results.HasModifications()) 
				return !DoModificationsExistWithinModificationDelay(results);

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

			TimeSpan diff = DateTime.Now - results.LastModificationDate;
			if (diff.TotalMilliseconds < ModificationDelaySeconds) 
			{
				// The new approach of polling the schedule for when-to-build means that
				// this log message would appear a few times each second.
				//				Log("Changes found within the modification delay");
				return true;
			}

			return false;
		}

		/// <summary>
		/// Raises the IntegrationCompleted event.
		/// </summary>
		/// <param name="e">Arguments to pass with the raised event.</param>
		protected virtual void OnIntegrationCompleted(IntegrationCompletedEventArgs e)
		{
			if (IntegrationCompleted!=null)
				IntegrationCompleted(this, e);
		}

		public IntegrationStatus GetLatestBuildStatus()
		{
			if (LastIntegrationResult != null)
				return LastIntegrationResult.Status;

			return IntegrationStatus.Unknown;
		}

		/// <summary>
		/// Labels the project, if the build was successful.
		/// </summary>
		private void HandleProjectLabelling(IntegrationResult result)
		{
			if (result.Succeeded) 
				SourceControl.LabelSourceControl(result.Label, result.StartTime);
		}
	}
}
