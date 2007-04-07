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
			Assert.AreEqual(ResourceIntegrationQueueIconProvider.QUEUE, iconProvider.GetStatusIconForNodeType(IntegrationQueueNodeType.Queue));
			Assert.AreEqual(ResourceIntegrationQueueIconProvider.FIRST_IN_QUEUE, iconProvider.GetStatusIconForNodeType(IntegrationQueueNodeType.FirstInQueue));
			Assert.AreEqual(ResourceIntegrationQueueIconProvider.PENDING, iconProvider.GetStatusIconForNodeType(IntegrationQueueNodeType.PendingInQueue));
		}
	}

}
