using System;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Schedules.Test
{
	/// <summary>
	/// Mock implementation of ISchedule for testing purposes.
	/// </summary>
	public class MockSchedule : ITrigger
	{
		public MockSchedule()
		{
		}

		public int ForceBuild_CallCount = 0;
		public void ForceBuild()
		{
			ForceBuild_CallCount++;
		}

		public void IntegrationCompleted()
		{
		}

		public BuildCondition ShouldRunIntegration()
		{
			return BuildCondition.NoBuild;
		}

		public bool ShouldStopIntegration()
		{
			return false;
		}
	}
}
