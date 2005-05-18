using System;
using Exortech.NetReflector;
using NMock;
using NMock.Remoting;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
	[TestFixture]
	public class ForceBuildPublisherTest
	{
		[Test]
		public void PopulateFromConfigurationXml()
		{
			string xml = @"<forcebuild project=""proj"" serverUri=""http://localhost"" integrationStatus=""Failure"" />";
			ForceBuildPublisher publisher = NetReflector.Read(xml) as ForceBuildPublisher;
			Assert.AreEqual("proj", publisher.Project);
			Assert.AreEqual("http://localhost", publisher.ServerUri);
			Assert.AreEqual(IntegrationStatus.Failure, publisher.IntegrationStatus);
		}

		[Test]
		public void PopulateFromMinimalXml()
		{
			string xml = @"<forcebuild project=""proj"" />";
			ForceBuildPublisher publisher = NetReflector.Read(xml) as ForceBuildPublisher;
			Assert.AreEqual("proj", publisher.Project);
			Assert.AreEqual("tcp://localhost:21234/CruiseManager.rem", publisher.ServerUri);
			Assert.AreEqual(IntegrationStatus.Success, publisher.IntegrationStatus);
		}

		[Test]
		public void ShouldReqestForceBuildOnRemoteCruiseServer()
		{
			IMock mockCruiseManager = new RemotingMock(typeof(ICruiseManager));
			mockCruiseManager.Expect("ForceBuild", "project");
			IMock mockRemotingService = new DynamicMock(typeof(IRemotingService));
			mockRemotingService.ExpectAndReturn("Connect", mockCruiseManager.MockInstance, typeof(ICruiseManager), "tcp://localhost:21234/CruiseManager.rem");

			ForceBuildPublisher publisher = new ForceBuildPublisher((IRemotingService)mockRemotingService.MockInstance);
			publisher.Project = "project";
			publisher.ServerUri = "tcp://localhost:21234/CruiseManager.rem";
			publisher.Run(IntegrationResultMother.CreateSuccessful());

			mockCruiseManager.Verify();
			mockRemotingService.Verify();
		}

		[Test]
		public void ShouldOnlyForceBuildIfIntegrationStatusMatches()
		{
			IMock mockRemotingService = new DynamicMock(typeof(IRemotingService));
			mockRemotingService.ExpectNoCall("Connect", typeof(Type), typeof(string));
			
			ForceBuildPublisher publisher = new ForceBuildPublisher((IRemotingService)mockRemotingService.MockInstance);
			publisher.IntegrationStatus = IntegrationStatus.Exception;
			publisher.Run(IntegrationResultMother.CreateFailed());

			mockRemotingService.Verify();
		}
	}
}
