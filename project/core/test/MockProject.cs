using System;
using System.Collections;
using System.Threading;

using tw.ccnet.core.schedule;
using tw.ccnet.remote;

namespace tw.ccnet.core.test
{
	public class MockProject : IProject
	{
		public event IntegrationCompletedEventHandler IntegrationCompleted;

		private string name;
		private ISchedule schedule;

		public MockProject(string name, ISchedule schedule)
		{
			this.name = name;
			this.schedule = schedule;
		}

		public string Name 
		{ 
			get { return name; } 
		}

		public int MinimumSleepTime 
		{
			get { return 0; }
		}

		public ISchedule Schedule 
		{ 
			get { return schedule; } 
			set { schedule = value; } 
		}

		public ArrayList Publishers
		{
			get { return new ArrayList(); }
		}

		public bool RunIntegration_forceBuild;
		public virtual void RunIntegration(bool forceBuild)
		{
			RunIntegration_forceBuild = forceBuild;
			RunIntegration();
		}

		public int RunIntegration_CallCount = 0;
		public virtual void RunIntegration()
		{
			RunIntegration_CallCount++;
		}

		public IntegrationStatus GetLatestBuildStatus() 
		{
			return IntegrationStatus.Unknown;
		}

		public ProjectActivity CurrentActivity 
		{
			get { return ProjectActivity.Unknown; }
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
