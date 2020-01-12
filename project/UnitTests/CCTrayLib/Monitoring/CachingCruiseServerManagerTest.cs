using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class CachingCruiseServerManagerTest
	{
		private Mock<ICruiseServerManager> wrappedManagerMock;
		private ICruiseServerManager cachingManager;

		[SetUp]
		public void SetUp()
		{
			wrappedManagerMock = new Mock<ICruiseServerManager>();
			cachingManager = new CachingCruiseServerManager((ICruiseServerManager) wrappedManagerMock.Object);
		}

		[Test]
		public void ShouldDelegateMostMethodsToWrappedInstance()
		{
            MockSequence sequence = new MockSequence();
            wrappedManagerMock.InSequence(sequence).SetupGet(_manager => _manager.Configuration).
                Returns(new BuildServer("tcp://testUrl")).Verifiable();
            wrappedManagerMock.InSequence(sequence).SetupGet(_manager => _manager.Configuration).
                Returns(new BuildServer("tcp://testUrl")).Verifiable();
			wrappedManagerMock.SetupGet(_manager => _manager.DisplayName).Returns("testDisplayName").Verifiable();
			wrappedManagerMock.Setup(_manager => _manager.CancelPendingRequest("testProjectName")).Verifiable();


			Assert.AreEqual("tcp://testUrl", cachingManager.Configuration.Url);
			Assert.AreEqual("testDisplayName", cachingManager.DisplayName);
			Assert.AreEqual(BuildServerTransport.Remoting, cachingManager.Configuration.Transport);
			cachingManager.CancelPendingRequest("testProjectName");

		
			wrappedManagerMock.Verify();
		}

		[Test]
		public void ShouldDelegateFirstSnapshotGet()
		{
			CruiseServerSnapshot snapshot = new CruiseServerSnapshot();
			wrappedManagerMock.Setup(_manager => _manager.GetCruiseServerSnapshot()).Returns(snapshot).Verifiable();

			Assert.AreSame(snapshot, cachingManager.GetCruiseServerSnapshot());

			wrappedManagerMock.Verify();
		}

		[Test]
		public void ShouldReturnSecondSnapshotGetWithoutDelegating()
		{
			CruiseServerSnapshot snapshot = new CruiseServerSnapshot();
			wrappedManagerMock.Setup(_manager => _manager.GetCruiseServerSnapshot()).Returns(snapshot).Verifiable();

			Assert.AreSame(snapshot, cachingManager.GetCruiseServerSnapshot());
			Assert.AreSame(snapshot, cachingManager.GetCruiseServerSnapshot());

			wrappedManagerMock.Verify();
		}

		[Test]
		public void ShouldDelegateSnapshotGetAfterCacheCleared()
		{
			CruiseServerSnapshot snapshot1 = new CruiseServerSnapshot();
			CruiseServerSnapshot snapshot2 = new CruiseServerSnapshot();
			wrappedManagerMock.SetupSequence(_manager => _manager.GetCruiseServerSnapshot())
				.Returns(snapshot1)
				.Returns(snapshot2);

			Assert.AreSame(snapshot1, cachingManager.GetCruiseServerSnapshot());
			Assert.AreSame(snapshot1, cachingManager.GetCruiseServerSnapshot());
			((ICache) cachingManager).InvalidateCache();
			Assert.AreSame(snapshot2, cachingManager.GetCruiseServerSnapshot());

			wrappedManagerMock.Verify(_manager => _manager.GetCruiseServerSnapshot(), Times.Exactly(2));
		}	
	}
}
