using System;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("nullSourceControl")]
	public class NullSourceControl : ISourceControl
	{
		public Modification[] GetModifications(DateTime from, DateTime to)
		{
			return new Modification[0];
		}

		public void LabelSourceControl( string label, IIntegrationResult result ) 
		{
		}

		public void GetSource(IIntegrationResult result)
		{
			
		}

		public void Initialize(IProject project)
		{
		}

		public void Purge(IProject project)
		{
		}

		public void Run(IIntegrationResult result)
		{
		}
	}
}
