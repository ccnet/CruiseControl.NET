using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
	[TestFixture]
	public class PublisherTest
	{
		[Test]
		public void PublishersShouldBeTasks()
		{
			IMock mock = new DynamicMock(typeof (ITask));
			IntegrationResult result = new IntegrationResult();
			mock.Expect("Run", result);

			ITask publisher = (ITask) mock.MockInstance;
			publisher.Run(result);
		}
	}
}