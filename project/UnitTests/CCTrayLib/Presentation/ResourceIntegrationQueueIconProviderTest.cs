using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Presentation
{
	[TestFixture]
	public class ResourceIntegrationQueueIconProviderTest
	{
		[Test]
		public void CanRetriveIconsForNodeType()
		{
			ResourceIntegrationQueueIconProvider iconProvider = new ResourceIntegrationQueueIconProvider();

			Assert.AreEqual(ResourceIntegrationQueueIconProvider.REMOTING_SERVER, iconProvider.GetStatusIconForNodeType(IntegrationQueueNodeType.RemotingServer));
			Assert.AreEqual(ResourceIntegrationQueueIconProvider.HTTP_SERVER, iconProvider.GetStatusIconForNodeType(IntegrationQueueNodeType.HttpServer));
            Assert.AreEqual(ResourceIntegrationQueueIconProvider.QUEUE_EMPTY, iconProvider.GetStatusIconForNodeType(IntegrationQueueNodeType.QueueEmpty));
            Assert.AreEqual(ResourceIntegrationQueueIconProvider.QUEUE_POPULATED, iconProvider.GetStatusIconForNodeType(IntegrationQueueNodeType.QueuePopulated));
            Assert.AreEqual(ResourceIntegrationQueueIconProvider.CHECKING_MODIFICATIONS, iconProvider.GetStatusIconForNodeType(IntegrationQueueNodeType.CheckingModifications));
            Assert.AreEqual(ResourceIntegrationQueueIconProvider.BUILDING, iconProvider.GetStatusIconForNodeType(IntegrationQueueNodeType.Building));
			Assert.AreEqual(ResourceIntegrationQueueIconProvider.PENDING, iconProvider.GetStatusIconForNodeType(IntegrationQueueNodeType.PendingInQueue));
		}
	}

}
