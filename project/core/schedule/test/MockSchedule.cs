using System;
using tw.ccnet.remote;

namespace tw.ccnet.core.schedule.test
{
	/// <summary>
	/// Mock implementation of ISchedule for testing purposes.
	/// </summary>
	public class MockSchedule : ISchedule
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
