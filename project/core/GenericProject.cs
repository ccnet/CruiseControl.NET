using System;
using System.Collections;
using Exortech.NetReflector;
using tw.ccnet.remote;

namespace tw.ccnet.core
{
	/// <summary>
	/// TODO explain why this class is here...
	/// </summary>
	[ReflectorType("generic")]
	public class GenericProject : IProject
	{
		public event IntegrationCompletedEventHandler IntegrationCompleted;

		private IList _tasks = new ArrayList();
		public string Name 
		{ 
			get { return null; }
		}

		public ISchedule Schedule 
		{
			get { return null; }
		}

		[ReflectorCollection("tasks", InstanceType=typeof(ArrayList))]
		public IList Tasks
		{
			get { return _tasks; }
			set { _tasks = value; }
		}

		public IntegrationResult RunIntegration(BuildCondition buildCondition)
		{
			return null;
		}
		
		public IntegrationStatus GetLatestBuildStatus()
		{
			return IntegrationStatus.Unknown;
		}

		public int MinimumSleepTimeMillis 
		{ 
			get { return 0; }
		}

		public ArrayList Publishers
		{
			get { return new ArrayList(); }
		}

		public ProjectActivity CurrentActivity 
		{
			get { return ProjectActivity.Unknown; }
		}
	}
}
