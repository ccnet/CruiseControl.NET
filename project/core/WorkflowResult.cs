using System;
using System.Collections;
using System.Text;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	public class WorkflowResult : IntegrationResult
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
