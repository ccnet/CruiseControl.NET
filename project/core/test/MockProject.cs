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

		string _name;
		ISchedule _schedule;
		ProjectActivity _projectActivity = ProjectActivity.Unknown; // default

		public MockProject(string name, ISchedule schedule)
		{
			_name = name;
			_schedule = schedule;
		}

		public string Name 
		{ 
			get { return _name; } 
		}

		public int MinimumSleepTimeMillis 
		{
			get { return 0; }
		}

		public ISchedule Schedule 
		{ 
			get { return _schedule; } 
			set { _schedule = value; } 
		}

		public ArrayList Publishers
		{
			get { return new ArrayList(); }
		}

		public int RunIntegration_CallCount = 0;
		public IntegrationResult RunIntegration_ReturnValue = null;
		public BuildCondition RunIntegration_buildCondition = (BuildCondition)(-1); // default
		public virtual IntegrationResult RunIntegration(BuildCondition buildCondition)
		{
			RunIntegration_CallCount++;
			RunIntegration_buildCondition = buildCondition;
			return RunIntegration_ReturnValue;
		}

		public IntegrationStatus GetLatestBuildStatus() 
		{
			return IntegrationStatus.Unknown;
		}

		public ProjectActivity CurrentActivity 
		{
			get { return _projectActivity; }
			set { _projectActivity = value; }
		}
	}

	public class ExceptionMockProject : MockProject
	{
		public const string EXCEPTION_MESSAGE = "Intentional exception";
		public ExceptionMockProject(string name, Schedule schedule) : base(name, schedule) {}

		public override IntegrationResult RunIntegration(BuildCondition buildCondition)
		{
			Schedule.IntegrationCompleted();
			throw new Exception(EXCEPTION_MESSAGE);
		}
	}
}
