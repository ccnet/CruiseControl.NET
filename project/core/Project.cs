using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using Exortech.NetReflector;
using tw.ccnet.core.history;
using tw.ccnet.core.label;
using tw.ccnet.core.publishers;
using tw.ccnet.core;
using tw.ccnet.remote;
using tw.ccnet.core.schedule;

namespace tw.ccnet.core
{
	/// <remarks>
	/// A project is the combination of the source control location, build command, publishers, and history elements.
	/// <code>
	/// <![CDATA[
	/// <project name="foo">
	///		<sourcecontrol type="cvs"></sourcecontrol>
	///		<build type="nant"></build>
	///		<history type="xml"></history>
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
		private IBuildHistory _history = new XmlBuildHistory();
		private int _timeout = 60000;
		private bool _stopped = false;
		private IntegrationResult _lastIntegration;
		private IntegrationResult _currentIntegration;
		private event IntegrationCompletedEventHandler _integrationCompletedEventHandler;
		private event IntegrationExceptionEventHandler _integrationExceptionEventHandler;
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

		[ReflectorProperty("history", InstanceTypeKey="type", Required=false)]
		public IBuildHistory BuildHistory
		{
			get { return _history; }
			set { _history = value; }
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
					_lastIntegration = (BuildHistory.Exists()) ? BuildHistory.Load() : new IntegrationResult();
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
			// lock
			if (NotStopped)
			{
				try
				{
					RunIntegration();
				}
				catch (CruiseControlException ex)
				{
					RaiseIntegrationExceptionEvent(ex); 
				}
			}
		}

		public void RunIntegration()
		{
			PreBuild();
			DateTime modStart = DateTime.Now;
			GetSourceModifications();
			if (ShouldRunIntegration())
			{
				RunBuild();
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
			RaiseIntegrationCompeletedEvent(CurrentIntegration);
			BuildHistory.Save(CurrentIntegration);
			LastIntegration = CurrentIntegration;
		}

		public void Sleep() 
		{
			Sleep(sleepTime);
		}

		protected void Sleep(int sleepTime)
		{	
			TimeSpan span = new TimeSpan(0, 0, 0, 0, sleepTime);
			Log(String.Format("Sleeping for {0} hours {1} minutes {2} seconds", span.Hours, span.Minutes, span.Seconds));
			currentActivity = "sleeping";
			Thread.Sleep(sleepTime);
		}

		private IntegrationResult LoadLastIntegration()
		{
			return (BuildHistory.Exists()) ? BuildHistory.Load() : new IntegrationResult();
		}

		private bool NotStopped
		{
			get { return ! _stopped; }
		}

		private void Log(string message)
		{
			Trace.WriteLine(String.Format("[project: {0}] {1}", Name, message)); 
		}

		public void AddPublisher(object publisher)
		{
			_publishers.Add(publisher);
			AddEventHandler(publisher);
		}

		internal bool ShouldRunIntegration() 
		{
			if (CurrentIntegration.ShouldRunIntegration()) 
			{
				DateTime lastModified = CurrentIntegration.LastModificationDate;
				if (ModificationDelay > 0) 
				{
					TimeSpan diff = DateTime.Now - lastModified;
					if (diff.TotalMilliseconds < ModificationDelay) 
					{
						sleepTime = ModificationDelay - (int)diff.TotalMilliseconds;
						Log(string.Format("Changes found within the modification delay"));
						return false;
					}

					return true;
				} 
				else 
				{
					return true;
				}
			}
			return false;
		}

		public IntegrationStatus GetLastBuildStatus() 
		{
			if (LastIntegration != null)
				return LastIntegration.Status;
			return IntegrationStatus.Unknown;
		}

		public string CurrentActivity 
		{
			get { return currentActivity; }
		}

		#region Event Handling
		private void AddEventHandlers(IList list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				AddEventHandler(list[i]);
			}
		}

		private void AddEventHandler(object handler)
		{
			if (handler is IIntegrationCompletedEventHandler)
			{
				AddIntegrationCompletedEventHandler(((IIntegrationCompletedEventHandler)handler).IntegrationCompletedEventHandler);
			}
			else if (handler is IIntegrationExceptionEventHandler)
			{
				AddIntegrationExceptionEventHandler(((IIntegrationExceptionEventHandler)handler).IntegrationExceptionEventHandler);
			}
		}

		public void AddIntegrationCompletedEventHandler(IntegrationCompletedEventHandler handler)
		{
			_integrationCompletedEventHandler += handler;
		}

		private void RaiseIntegrationCompeletedEvent(IntegrationResult result)
		{
			if (_integrationCompletedEventHandler != null)
			{
				_integrationCompletedEventHandler(this, result);
			}
		}

		public void AddIntegrationExceptionEventHandler(IntegrationExceptionEventHandler handler)
		{
			_integrationExceptionEventHandler += handler;
		}

		private void RaiseIntegrationExceptionEvent(CruiseControlException ex)
		{
			if (_integrationExceptionEventHandler != null)
			{
				_integrationExceptionEventHandler(this, ex);
			}
		}
		#endregion Event Handling
	}
}
