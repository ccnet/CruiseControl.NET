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
		private int sleep = 0;
		private ISchedule schedule = new Schedule();

		public MockProject(string name)
		{
			this.name = name;
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

		public virtual void Run()
		{
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

		public int SleepTime
		{
			get { return sleep; }
			set { sleep = value; }
		}

		public void AddIntegrationEventHandler(IntegrationEventHandler handler) {}

		public IntegrationStatus GetLastBuildStatus() 
		{
			return IntegrationStatus.Unknown;
		}

		public void Sleep() 
		{
		}
	}

	public class ExceptionMockProject : MockProject
	{
		public const string EXCEPTION_MESSAGE = "Intentional exception";
		public ExceptionMockProject(string name) : base(name) {}

		public override void RunIntegration()
		{
			throw new Exception(EXCEPTION_MESSAGE);
		}
	}
}
