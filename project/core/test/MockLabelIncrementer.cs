using System;
using Exortech.NetReflector;

namespace tw.ccnet.core.test
{
	[ReflectorType("mockIncrementer")]
	public class MockLabelIncrementer : ILabelIncrementer
	{
		private string label = "1";

		[ReflectorProperty("label", Required=false)]
		public string Label 
		{
			get { return label; }
			set { label = value; }
		}

		public string GetNextLabel() 
		{
			return label;
		}
	}
}
