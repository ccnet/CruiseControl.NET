using System;
using tw.ccnet.remote;

namespace tw.ccnet.core
{
	public interface IProject
	{
		string Name { get; }
		ISchedule Schedule { get; }
		void Run();
		void RunIntegration();  // temporary until scheduling is fully integrated
		void AddIntegrationCompletedEventHandler(IntegrationCompletedEventHandler handler);
		void AddIntegrationExceptionEventHandler(IntegrationExceptionEventHandler handler);
		IntegrationStatus GetLastBuildStatus();
		void Sleep();
	}
}
