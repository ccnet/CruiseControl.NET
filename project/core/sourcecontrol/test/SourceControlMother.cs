using System;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test
{	
	public class SourceControlMother
	{
		public static ISourceControl Create()
		{
			ISourceControl control = new MockSourceControl();
			return control;
		}
	}

	[ReflectorType("mock")]
	public class MockSourceControl : ISourceControl 
	{
		public readonly static DateTime LastModificationTime = DateTime.Now.AddDays(-0.5);
		private string label;

		public string Label 
		{
			get { return label; }
		}

		public Modification[] GetModifications(DateTime from, DateTime to) 
		{
			if (from < LastModificationTime && LastModificationTime < to) 
			{
				return CreateModifications();
			} 
			else 
			{
				return new Modification[0];
			}
		}

		public bool ShouldRun(IntegrationResult result)
		{
			return true;
		}

		public void Run(IntegrationResult result)
		{
			result.Modifications = GetModifications(result.LastModificationDate, DateTime.Now);
		}
 
		private Modification[] CreateModifications()
		{
			Modification[] modifications = new Modification[3];
			for (int i = 0; i < modifications.Length; i++)
			{
				modifications[i] = new Modification();
				modifications[i].ModifiedTime = LastModificationTime;
			}
			return modifications;
		}

		public void LabelSourceControl(string label, DateTime timeStamp) 
		{
			this.label = label;
		}
	}

}