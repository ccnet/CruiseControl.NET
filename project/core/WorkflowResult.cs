using System.Collections;

namespace ThoughtWorks.CruiseControl.Core
{
	public class WorkflowResult : IntegrationResult
	{
		private ArrayList modifications = new ArrayList();

		public override Modification[] Modifications
		{
			get { return (Modification[])modifications.ToArray(typeof(Modification)); }
			set { modifications.AddRange(value); }
		}
	}
}
