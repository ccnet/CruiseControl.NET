using System;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("defaultsourcecontrol")]
	public class DefaultSourceControl : ISourceControl
	{
		public Modification[] GetModifications(DateTime from, DateTime to)
		{
			Modification[] mods = new Modification[1];
			mods[0] = new Modification();
			mods[0].ModifiedTime = DateTime.Now;
			return mods;
		}

		public void LabelSourceControl(string label, DateTime timeStamp) 
		{
		}

		public void GetSource(IntegrationResult result)
		{
			
		}

		public void Initialize(IProject project)
		{
		}

		public bool ShouldRun(IntegrationResult result, IProject project)
		{
			return result.Working;
		}

		public void Run(IntegrationResult result, IProject project)
		{
			result.Modifications = GetModifications(result.LastModificationDate, DateTime.Now);
		}
	}
}
