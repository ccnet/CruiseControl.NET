using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	[ReflectorType("mockbuildrunner")]
	public class MockBuilder : ITask
	{
		public const string BUILDER_OUTPUT = "success";
		public bool HasRun = false;

		public void Run(IIntegrationResult result)
		{
			result.AddTaskResult(BUILDER_OUTPUT);
			HasRun = true;
		}
	}
}