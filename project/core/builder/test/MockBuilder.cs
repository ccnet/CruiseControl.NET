using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Builder.test
{
	[ReflectorType("mockbuildrunner")]
	public class MockBuilder : IBuilder
	{
		public bool HasRun = false;

		public bool ShouldRun(IntegrationResult result)
		{
			return true;
		}

		public void Run(IntegrationResult result)
		{
			result.Status = IntegrationStatus.Success;
			result.Output = "success";
			HasRun = true;
		}
	}
}
