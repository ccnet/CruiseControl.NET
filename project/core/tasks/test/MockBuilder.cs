using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Tasks.Test
{
	[ReflectorType("mockbuildrunner")]
	public class MockBuilder : ITask
	{
		public const string BUILDER_OUTPUT = "success";
		public bool HasRun = false;

		public void Run(IIntegrationResult result)
		{
			result.Status = IntegrationStatus.Success;
			result.AddTaskResult(BUILDER_OUTPUT);
			HasRun = true;
		}
	}
}
