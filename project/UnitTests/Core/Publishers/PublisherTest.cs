using Moq;
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
			var mock = new Mock<ITask>();
			IntegrationResult result = new IntegrationResult();
			mock.Setup(task => task.Run(result)).Verifiable();

			ITask publisher = (ITask) mock.Object;
			publisher.Run(result);
			mock.Verify();
		}
	}
}