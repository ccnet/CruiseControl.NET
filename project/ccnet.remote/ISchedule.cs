using System;

namespace tw.ccnet.remote
{
	public interface ISchedule
	{
		// TODO provide some documentation on this interface... it's a bit cryptic!

		bool ForceBuild { get; }
		bool ShouldRun();
		TimeSpan CalculateTimeToNextIntegration();
		void Update();
	}
}
