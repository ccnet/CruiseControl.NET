using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	[ReflectorType("nullTask")]
	public class NullTask : ITask
	{
		public void Run(IIntegrationResult result)
		{
			return;
		}
	}
}
