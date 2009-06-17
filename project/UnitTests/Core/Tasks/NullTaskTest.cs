using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
	[TestFixture]
	public class NullTasklTest
	{
		private NullTask task;

		[SetUp]
		public void Setup()
		{
			task = new NullTask();
		}

		[Test]
		public void ShouldReturnUnchangedResult()
		{
			IntegrationResult result = new IntegrationResult();
			task.Run(result);
			Assert.IsTrue(result.Succeeded);
		}

        [Test, ExpectedException("System.Exception", "Simulating a failure")]
        public void ShouldThrowExceptionWhenSimulateFailureIsTrue()
        {
            IntegrationResult result = new IntegrationResult();
            task.SimulateFailure = true;
            task.Run(result);
            
        }

    
    }
}