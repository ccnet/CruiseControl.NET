using System;

namespace tw.ccnet.remote
{
	public interface ISchedule
	{
		bool ForceBuild { get; }
		bool ShouldRun();
		TimeSpan CalculateTimeToNextIntegration();
		void Update();
	}
}
