using System;
using System.Threading;
using tw.ccnet.core.schedule;
using tw.ccnet.remote;

namespace tw.ccnet.core.test
{
	public class MockProject : IProject
	{
		private string name;
		private int runs = 0;
		private ISchedule schedule;
		public bool ForceBuild;

		public MockProject(string name, ISchedule schedule)
		{
			this.name = name;
			this.schedule = schedule;
		}

		public string Name 
		{ 
			get { return name; } 
		}

		public ISchedule Schedule 
		{ 
			get { return schedule; } 
			set { schedule = value; } 
		}

		public virtual void Run(bool forceBuild)
		{
			ForceBuild = forceBuild;
			RunIntegration();
		}

		public virtual void RunIntegration()
		{
			runs++;
		}

		public int Runs
		{
			get { return runs; }
		}

		public void AddIntegrationEventHandler(IntegrationEventHandler handler) {}

		public IntegrationStatus GetLastBuildStatus() 
		{
			return IntegrationStatus.Unknown;
		}
	}

	public class ExceptionMockProject : MockProject
	{
		public const string EXCEPTION_MESSAGE = "Intentional exception";
		public ExceptionMockProject(string name, Schedule schedule) : base(name, schedule) {}

		public override void RunIntegration()
		{
			throw new Exception(EXCEPTION_MESSAGE);
		}
	}
}
