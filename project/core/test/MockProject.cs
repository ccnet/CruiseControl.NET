using System;
using System.Collections;

using ThoughtWorks.CruiseControl.Core.Triggers;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Test
{
	public class MockProject : IProject
	{
		string _name;
		ITrigger _schedule;
		ProjectActivity _projectActivity = ProjectActivity.Unknown; // default

		public MockProject(string name, ITrigger Trigger)
		{
			_name = name;
			_schedule = Trigger;
		}

		public string Name 
		{ 
			get { return _name; } 
		}

		public ITrigger[] Triggers
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public int MinimumSleepTimeMillis 
		{
			get { return 0; }
		}

		public ArrayList Publishers
		{
			get { return new ArrayList(); }
		}

		public string WebURL 
		{ 
			get { return ""; }
		}

		public int RunIntegration_CallCount = 0;
		public IntegrationResult RunIntegration_ReturnValue = null;
		public BuildCondition RunIntegration_buildCondition = (BuildCondition)(-1); // default
		public virtual IIntegrationResult RunIntegration(BuildCondition buildCondition)
		{
			RunIntegration_CallCount++;
			RunIntegration_buildCondition = buildCondition;
			return RunIntegration_ReturnValue;
		}

		public IntegrationStatus LatestBuildStatus
		{
			get { return IntegrationStatus.Unknown; }
		}

		public void Purge(bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment)
		{
			return;
		}

		public ProjectActivity CurrentActivity 
		{
			get { return _projectActivity; }
			set { _projectActivity = value; }
		}

		public string WorkingDirectory
		{
			get { throw new NotImplementedException(); }
		}

		public string ArtifactDirectory
		{
			get { throw new NotImplementedException(); }
		}

	}

	public class ExceptionMockProject : MockProject
	{
		public const string EXCEPTION_MESSAGE = "Intentional exception";
		public ExceptionMockProject(string name, PollingIntervalTrigger PollingIntervalTrigger) : base(name, PollingIntervalTrigger) {}

		public override IIntegrationResult RunIntegration(BuildCondition buildCondition)
		{
			foreach (ITrigger trigger in Triggers)
			{
				trigger.IntegrationCompleted();	
			}
			throw new Exception(EXCEPTION_MESSAGE);
		}
	}
}
