using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Publishers;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
	[TestFixture]
	public class PublisherTest
	{
		[Test]
		public void PublishersShouldBeTasks()
		{
			IMock mock = new DynamicMock(typeof (PublisherBase));
			IntegrationResult result = new IntegrationResult();
			mock.Expect("PublishIntegrationResults", result);

			ITask publisher = (ITask) mock.MockInstance;
			publisher.Run(result);
		}
	}
}