using System;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core
{
	[ReflectorType("test")]
	public class TestSourceControl : ISourceControl
	{
		public void Run(IntegrationResult result)
		{
			//No op
		}

		public bool ShouldRun(IntegrationResult result)
		{
			return true;
		}

		public Modification[] GetModifications(DateTime from, DateTime to)
		{
			Modification m = new Modification();
			Modification[] arr = new Modification[1];
			arr[0] = m;		
			return arr;
		}

		public void LabelSourceControl(string label, DateTime timeStamp)
		{
			//no op
		}

		public void GetSource(IntegrationResult result)
		{
			// no op
		}

		public void Initialize(string projectName, string workingDirectory)
		{
		}
	}
}
