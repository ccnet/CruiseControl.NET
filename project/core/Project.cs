using System;
using System.Collections;

using Exortech.NetReflector;

using tw.ccnet.core.label;
using tw.ccnet.core.publishers;
using tw.ccnet.core.schedule;
using tw.ccnet.core.state;
using tw.ccnet.core.util;
using tw.ccnet.remote;

namespace tw.ccnet.core
{
	/// <remarks>
	/// A project is the combination of the source control location, build command, publishers, and state elements.
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
	public class Project : IProject
	{
		/// <summary>
		/// Raised whenever an integration is completed.
		/// </summary>
		public event IntegrationCompletedEventHandler IntegrationCompleted;

		string _name;
		string _webURL = "http://localhost/CruiseControl.NET/"; // default value
		ISchedule _schedule;
		ISourceControl _sourceControl;
		IBuilder _builder;
		ILabeller _labeller = new DefaultLabeller();
		ArrayList _publishers = new ArrayList();
		IStateManager _state = new IntegrationStateManager();
		IntegrationResult _lastIntegrationResult = null;
		IntegrationResult _currentIntegrationResult = null;
		ProjectActivity _currentActivity = ProjectActivity.Unknown;
		int _modificationDelay = 0;
		int _sleepTime = 0;
		bool _stopped = false;

		#region Properties set via Xml configuration

		[ReflectorProperty("name")]
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		[ReflectorProperty("webURL", Required=false)]
		public string WebURL
		{
			get { return _webURL; }
			set { _webURL = value; }
		}

		[ReflectorProperty("schedule", InstanceTypeKey="type", Required=false)]
		public ISchedule Schedule
		{
			get 
			{
				// construct a new schedule if none exists
				if (_schedule==null) 
					_schedule = new Schedule();

				return _schedule; 
			}
			set { _schedule = value; }
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

		[ReflectorProperty("state", InstanceTypeKey="type", Required=false)]
		public IStateManager StateManager
		{
			get { return _state; }
			set { _state = value; }
		}

		[ReflectorProperty("modificationDelay", Required=false)]
		public int ModificationDelay 
		{
			get { return _modificationDelay; }
			set { _modificationDelay = value; }
		}

		#endregion

		#region Other properties

		public bool Stopped 
		{
			get { return _stopped; }
			set { _stopped = value; }
		}

		public int MinimumSleepTime 
		{
			get { return _sleepTime; }
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
				{
					_lastIntegrationResult = LoadLastIntegration();
				}
				return _lastIntegrationResult; 
			}

			set { _lastIntegrationResult = value; }
		}

		public IntegrationResult CurrentIntegrationResult
		{
			get { return _currentIntegrationResult; }
			set { _currentIntegrationResult = value; }
		}

		public ProjectActivity CurrentActivity 
		{
			get { return _currentActivity; }
		}

		#endregion

		public void RunIntegrationAndForceBuild() 
		{ 
			RunIntegration(true); 
		}

		public void RunIntegration(bool forceBuild)
		{
			if (Stopped)
				return;

			// lock
			_sleepTime = 0;
			try
			{
				InitialiseCurrentIntegrationResult();
				GetSourceModifications();
				if (forceBuild || ShouldRunBuild())
				{
					RunBuild();
					PostBuild();
				}
			}
			catch (CruiseControlException ex)
			{
				Log("Exception occurred while running integration", ex);
				CurrentIntegrationResult.ExceptionResult = ex;
				PostBuild();
			}

			// go to sleep
			_currentActivity = ProjectActivity.Sleeping;
		}

		internal void InitialiseCurrentIntegrationResult()
		{
			CurrentIntegrationResult = new IntegrationResult();
			CurrentIntegrationResult.ProjectName = Name;
			CurrentIntegrationResult.LastIntegrationStatus = LastIntegrationResult.Status;		// test
			CurrentIntegrationResult.Label = Labeller.Generate(LastIntegrationResult);
			CurrentIntegrationResult.MarkStartTime();
		}

		internal void GetSourceModifications()
		{
			_currentActivity = ProjectActivity.CheckingModifications;
			CurrentIntegrationResult.Modifications = SourceControl.GetModifications(LastIntegrationResult.StartTime,  CurrentIntegrationResult.StartTime);
			Log(String.Format("{0} Modifications detected...", CurrentIntegrationResult.Modifications.Length));
		}

		internal void RunBuild()
		{
			_currentActivity = ProjectActivity.Building;
			Builder.Run(CurrentIntegrationResult);
			Log(String.Format("Build Complete: {0}", CurrentIntegrationResult.Status.ToString())); 
		}

		internal void PostBuild()
		{
			CurrentIntegrationResult.MarkEndTime();
			AttemptToSaveState();
			HandleProjectLabelling(CurrentIntegrationResult);
			// raise event (publishers do their thing in response)
			OnIntegrationCompleted(new IntegrationCompletedEventArgs(CurrentIntegrationResult));
			// update reference to the most recent result
			LastIntegrationResult = CurrentIntegrationResult;
			Log(String.Format("Integration Complete... {0}", CurrentIntegrationResult.EndTime));
		}

		void AttemptToSaveState()
		{
			try
			{
				StateManager.Save(CurrentIntegrationResult);
			}
			catch (CruiseControlException ex)
			{
				Log("Exception when saving integration state", ex);
				if (CurrentIntegrationResult.ExceptionResult == null)
				{
					CurrentIntegrationResult.ExceptionResult = ex;
				}
			}
		}

		IntegrationResult LoadLastIntegration()
		{
			if (StateManager.Exists())
			{
				return StateManager.Load();
			}
			else
			{
				// no integration result is on record
				// TODO consider something such as IntegrationResult.Empty, to indicate 'unknown state'
				return new IntegrationResult();
			}
		}

		#region Logging helper methods

		private void Log(string message)
		{
			LogUtil.Log(this, message);
		}

		private void Log(string message, CruiseControlException ex)
		{
			LogUtil.Log(this, message, ex);
		}

		#endregion

		/// <summary>
		/// Determines whether a build should run.  A build should run if there
		/// are modifications, and none have occurred within the modification
		/// delay.
		/// </summary>
		internal bool ShouldRunBuild() 
		{
			if (CurrentIntegrationResult.HasModifications()) 
				return !DoModificationsExistWithinModificationDelay();

			return false;
		}

		/// <summary>
		/// Checks whether modifications occurred within the modification delay.  If the
		/// modification delay is not set (has a value of zero or less), this method
		/// will always return false.
		/// </summary>
		bool DoModificationsExistWithinModificationDelay()
		{
			if (ModificationDelay <= 0) 
				return false;

			TimeSpan diff = DateTime.Now - CurrentIntegrationResult.LastModificationDate;
			if (diff.TotalMilliseconds < ModificationDelay) 
			{
				_sleepTime = ModificationDelay - (int)diff.TotalMilliseconds;
				Log("Changes found within the modification delay");
				return true;
			}

			return false;
		}

		public IntegrationStatus GetLatestBuildStatus() 
		{
			if (LastIntegrationResult!=null)
				return LastIntegrationResult.Status;

			return IntegrationStatus.Unknown;
		}

		/// <summary>
		/// Labels the project, if the build was successful.
		/// </summary>
		void HandleProjectLabelling(IntegrationResult result) 
		{
			if (result.Succeeded) 
				SourceControl.LabelSourceControl(result.Label, result.StartTime);
		}

		#region Event Handling

		public void AddPublisher(PublisherBase publisher)
		{
			_publishers.Add(publisher);
			IntegrationCompleted += publisher.IntegrationCompletedEventHandler;
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

		#endregion
	}
}
