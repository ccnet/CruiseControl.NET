using System;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.builder
{
	/// <summary>
	/// Executes and aggregates the results from multiple builders
	/// </summary>
	public class MultiBuilder : IBuilder
	{
		private IBuilder[] builders = new IBuilder[0];

		public void Run(IIntegrationResult result)
		{
			foreach (IBuilder builder in builders)
			{
				builder.Run(result);
			}
		}

		[ReflectorArray("builders")]
		public IBuilder[] Builders
		{
			get { return builders; }
			set { builders = value; }
		}
	}
}
