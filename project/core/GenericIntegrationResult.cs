using System;
using System.Collections;
using System.Text;
using tw.ccnet.remote;

namespace tw.ccnet.core
{
	public class GenericIntegrationResult : IntegrationResult
	{
		private StringBuilder buffer = new StringBuilder();
		private ArrayList modifications = new ArrayList();

		public override string Output
		{
			get { return buffer.ToString(); }
			set { buffer.Append(value);	}
		}

		public override Modification[] Modifications
		{
			get { return (Modification[])modifications.ToArray(typeof(Modification)); }
			set { modifications.AddRange(value); }
		}
	}
}
