using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using Exortech.NetReflector;
using tw.ccnet.core.state;
using tw.ccnet.core.label;
using tw.ccnet.core.publishers;
using tw.ccnet.core;
using tw.ccnet.remote;
using tw.ccnet.core.schedule;
using tw.ccnet.core.util;

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
		private string _name;
		private ISchedule _schedule;
		private ISourceControl _sourceControl;
		private IBuilder _builder;
		private ILabeller _labeller = new DefaultLabeller();
		private IList _publishers = new ArrayList();
		private IStateManager _state = new IntegrationStateManager();
		private int _timeout = 60000;
		private bool _stopped = false;
		private IntegrationResult _lastIntegration;
		private IntegrationResult _currentIntegration;
		private event IntegrationEventHandler _integrationEventHandler;
		private string currentActivity;
		private int modificationDelay = 0;
		private int sleepTime = 0;

		#region Project Properties
		[ReflectorProperty("name")]
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		[ReflectorProperty("schedule", InstanceTypeKey="type", Required=false)]
		public ISchedule Schedule
		{
			get 
			{ 
				if (_schedule == null) 
				{
					_schedule = new Schedule();
				}
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

		[ReflectorCollection("publishers", InstanceType=typeof(ArrayList), Required=false)]
		public IList Publishers
		{
			get { return _publishers; }
			set 
			{ 
				_publishers = value; 
				AddEventHandlers(_publishers);
			}
		}

		[ReflectorProperty("sleepTime", Required=false)]
		public int IntegrationTimeout
		{
			get { return _timeout; }
			set { _timeout = value; }
		}

		public bool Stopped 
		{
			get { return _stopped; }
			set { _stopped = value; }
		}

		[ReflectorProperty("state", InstanceTypeKey="type", Required=false)]
		public IStateManager StateManager
		{
			get { return _state; }
			set { _state = value; }
		}

		public ILabeller Labeller
		{
			get { return _labeller; }
			set { _labeller = value; }
		}

		public IntegrationResult LastIntegration
		{
			get 
			{ 
				if (_lastIntegration == null)
				{
					_lastIntegration = LoadLastIntegration();
				}
				return _lastIntegration; 
			}

			set { _lastIntegration = value; }
		}

		public IntegrationResult CurrentIntegration
		{
			get { return _currentIntegration; }
			set { _currentIntegration = value; }
		}

		[ReflectorProperty("modificationDelay", Required=false)]
		public int ModificationDelay 
		{
			get { return modificationDelay; }
			set { modificationDelay = value; }
		}
		#endregion

		public void Run() 
		{ 
			Run(true); 
		}

		public void Run(bool forceBuild)
		{
			if (Stopped) return;

			// lock
			try
			{
				PreBuild();
				DateTime modStart = DateTime.Now;
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
				CurrentIntegration.ExceptionResult = ex;
				PostBuild();
			}
		}

		internal void PreBuild()
		{
			Log("Starting new integration...");
			currentActivity = "building";
			sleepTime = IntegrationTimeout;
			CurrentIntegration = new IntegrationResult();
			CurrentIntegration.ProjectName = Name;
			CurrentIntegration.LastIntegrationStatus = LastIntegration.Status;		// test
			CurrentIntegration.Label = Labeller.Generate(LastIntegration);
			CurrentIntegration.Start();
		}

		internal void GetSourceModifications()
		{
			CurrentIntegration.Modifications = SourceControl.GetModifications(LastIntegration.LastModificationDate,  CurrentIntegration.StartTime);
			Log(String.Format("{0} Modifications detected...", CurrentIntegration.Modifications.Length));
		}

		internal void RunBuild()
		{
			Builder.Build(CurrentIntegration);
			Log(String.Format("Build Complete: {0}", CurrentIntegration.Status.ToString())); 
		}

		internal void PostBuild()
		{
			CurrentIntegration.End();
			try
			{
				StateManager.Save(CurrentIntegration);
			}
			catch (CruiseControlException ex)
			{
				Log("Exception when saving integration state", ex);
				if (CurrentIntegration.ExceptionResult == null)
				{
					CurrentIntegration.ExceptionResult = ex;
				}
			}
			LabelProject(CurrentIntegration);
			RaiseIntegrationEvent(CurrentIntegration);
			LastIntegration = CurrentIntegration;
		}

		private IntegrationResult LoadLastIntegration()
		{
			return (StateManager.Exists()) ? StateManager.Load() : new IntegrationResult();
		}

		private void Log(string message)
		{
			LogUtil.Log(this, message);
		}

		private void Log(string message, CruiseControlException ex)
		{
			LogUtil.Log(this, message, ex);
		}

		public void AddPublisher(IIntegrationEventHandler publisher)
		{
			_publishers.Add(publisher);
			AddIntegrationEventHandler(publisher.IntegrationEventHandler);
		}

		internal bool ShouldRunBuild() 
		{
			if (CurrentIntegration.ShouldRunIntegration()) 
			{
				if (ModificationDelay > 0) 
				{
					return CheckModificationDelay();
				} 
				else 
				{
					return true;
				}
			}
			return false;
		}

		private bool CheckModificationDelay()
		{
			TimeSpan diff = DateTime.Now - CurrentIntegration.LastModificationDate;
			if (diff.TotalMilliseconds < ModificationDelay) 
			{
				sleepTime = ModificationDelay - (int)diff.TotalMilliseconds;
				Log("Changes found within the modification delay");
				return false;
			}
			return true;
		}

		public IntegrationStatus GetLastBuildStatus() 
		{
			if (LastIntegration != null)
			{
				return LastIntegration.Status;
			}
			return IntegrationStatus.Unknown;
		}

		public string CurrentActivity 
		{
			get { return currentActivity; }
		}

		private void LabelProject(IntegrationResult result) 
		{
			if (result.Succeeded) 
			{
				SourceControl.LabelSourceControl(result.Label, result.StartTime);
			}
		}

		#region Event Handling
		private void AddEventHandlers(IList list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				AddIntegrationEventHandler(((IIntegrationEventHandler)list[i]).IntegrationEventHandler);
			}
		}

		public void AddIntegrationEventHandler(IntegrationEventHandler handler)
		{
			_integrationEventHandler += handler;
		}

		private void RaiseIntegrationEvent(IntegrationResult result)
		{
			if (_integrationEventHandler != null)
			{
				_integrationEventHandler(this, result);
			}
		}
		#endregion Event Handling
	}
}
