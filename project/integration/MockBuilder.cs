using Exortech.NetReflector;
using System;
using ThoughtWorks.CruiseControl.Core;

namespace integration
{
	[ReflectorType("mockbuilder")]
	public class MockBuilder : IBuilder
	{
		public void Run(IntegrationResult result)
		{
			// do nothing
		}

		public bool ShouldRun(IntegrationResult result)
		{
			return true;
		}
	}
}
