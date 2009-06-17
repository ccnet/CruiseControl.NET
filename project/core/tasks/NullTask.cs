using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	[ReflectorType("nullTask")]
	public class NullTask : ITask
	{

        [ReflectorProperty("simulateFailure", Required = false)]
        public bool SimulateFailure = false;


		public void Run(IIntegrationResult result)
		{
            if (SimulateFailure)
            {
                result.AddTaskResult("Simulating Failure");
                throw new System.Exception("Simulating a failure");
            }
            else
            {
                result.AddTaskResult(string.Empty);
            }			            
		}
	}
}