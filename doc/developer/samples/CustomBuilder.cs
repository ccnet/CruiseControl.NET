using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.Sample.Builder
{
	[ReflectorType("myBuilder")]
	public class NAntBuilder : IBuilder
	{
		public bool ShouldRun(IntegrationResult result)
		{
			return result.Working;
		}

		public void Run(IntegrationResult result)
		{
			Console.WriteLine("Hello World!");
		}
	}
}