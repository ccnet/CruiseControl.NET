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
			result.Modifications = GetModifications(result.LastModificationDate, DateTime.Now);
		}
	}
}
