using System;
using System.Collections;
using Exortech.NetReflector;
using tw.ccnet.remote;

namespace tw.ccnet.core
{
	[ReflectorType("generic")]
	public class GenericProject : IProject
	{
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

		public void Run(bool forceBuild)
		{
		}

		public void AddIntegrationEventHandler(IntegrationEventHandler handler)
		{
		}
		
		public IntegrationStatus GetLastBuildStatus()
		{
			return IntegrationStatus.Unknown;
		}

		public int MinimumSleepTime 
		{ 
			get { return 0; }
		}
	}
}
