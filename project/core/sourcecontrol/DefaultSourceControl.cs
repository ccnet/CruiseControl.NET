using System;
using Exortech.NetReflector;

namespace tw.ccnet.core.sourcecontrol
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

		public void Run(IntegrationResult result)
		{
			result.Modifications = GetModifications(result.LastModificationDate, DateTime.Now);
		}
	}
}
