using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class CachingCruiseServerManagerTest
	{
		private DynamicMock wrappedManagerMock;
		private ICruiseServerManager cachingManager;

		[SetUp]
		public void SetUp()
		{
			wrappedManagerMock = new DynamicMock(typeof(ICruiseServerManager));
			cachingManager = new CachingCruiseServerManager((ICruiseServerManager) wrappedManagerMock.MockInstance);
		}

		[Test]
		public void ShouldDelegateMostMethodsToWrappedInstance()
		{
			wrappedManagerMock.ExpectAndReturn("ServerUrl", "testUrl");
			wrappedManagerMock.ExpectAndReturn("DisplayName", "testDisplayName");
			wrappedManagerMock.ExpectAndReturn("Transport", BuildServerTransport.HTTP);
			wrappedManagerMock.Expect("CancelPendingRequest", "testProjectName");


			Assert.AreEqual("testUrl", cachingManager.ServerUrl);
			Assert.AreEqual("testDisplayName", cachingManager.DisplayName);
			Assert.AreEqual(BuildServerTransport.HTTP, cachingManager.Transport);
			cachingManager.CancelPendingRequest("testProjectName");

		
			wrappedManagerMock.Verify();
		}

		[Test]
		public void ShouldDelegateFirstSnapshotGet()
		{
			CruiseServerSnapshot snapshot = new CruiseServerSnapshot();
			wrappedManagerMock.ExpectAndReturn("GetCruiseServerSnapshot", snapshot);

			Assert.AreSame(snapshot, cachingManager.GetCruiseServerSnapshot());

			wrappedManagerMock.Verify();
		}

		[Test]
		public void ShouldReturnSecondSnapshotGetWithoutDelegating()
		{
			CruiseServerSnapshot snapshot = new CruiseServerSnapshot();
			wrappedManagerMock.ExpectAndReturn("GetCruiseServerSnapshot", snapshot);

			Assert.AreSame(snapshot, cachingManager.GetCruiseServerSnapshot());
			Assert.AreSame(snapshot, cachingManager.GetCruiseServerSnapshot());

			wrappedManagerMock.Verify();
		}

		[Test]
		public void ShouldDelegateSnapshotGetAfterCacheCleared()
		{
			CruiseServerSnapshot snapshot1 = new CruiseServerSnapshot();
			wrappedManagerMock.ExpectAndReturn("GetCruiseServerSnapshot", snapshot1);
			CruiseServerSnapshot snapshot2 = new CruiseServerSnapshot();
			wrappedManagerMock.ExpectAndReturn("GetCruiseServerSnapshot", snapshot2);

			Assert.AreSame(snapshot1, cachingManager.GetCruiseServerSnapshot());
			Assert.AreSame(snapshot1, cachingManager.GetCruiseServerSnapshot());
			((ICache) cachingManager).InvalidateCache();
			Assert.AreSame(snapshot2, cachingManager.GetCruiseServerSnapshot());

			wrappedManagerMock.Verify();
		}	
	}
}
