using System;
using tw.ccnet.remote;

namespace tw.ccnet.core
{
	public interface IProject
	{
		string Name { get; }
		ISchedule Schedule { get; }
		void Run(bool forceBuild);

		void AddIntegrationEventHandler(IntegrationEventHandler handler);
		IntegrationStatus GetLastBuildStatus();
		int MinimumSleepTime { get; }
	}
}
