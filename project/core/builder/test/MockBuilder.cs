using System;
using Exortech.NetReflector;
using tw.ccnet.remote;

namespace tw.ccnet.core.builder.test
{
	[ReflectorType("mockbuildrunner")]
	public class MockBuilder : IBuilder
	{
		public bool HasRun = false;

		public void Build(IntegrationResult result)
		{
			result.Status = IntegrationStatus.Success;
			result.Output = "success";
			HasRun = true;
		}
	}
}
