using System;

namespace tw.ccnet.core
{
	public interface ISchedule
	{
		bool ShouldRun();
		TimeSpan CalculateTimeToNextIntegration();
		void Update();
	}
}
