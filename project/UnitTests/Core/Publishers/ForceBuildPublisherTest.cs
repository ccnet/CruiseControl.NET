using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Remote;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
	[TestFixture]
	public class ForceBuildPublisherTest
	{
        private MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
        }

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
            var factory = mocks.StrictMock<ICruiseServerClientFactory>();
            var client = mocks.StrictMock<CruiseServerClientBase>();
            SetupResult.For(factory.GenerateClient("tcp://localhost:21234/CruiseManager.rem"))
                .Return(client);
            Expect.Call(() => client.ForceBuild("project", null))
                .Constraints(Rhino.Mocks.Constraints.Is.Equal("project"),
                    Rhino.Mocks.Constraints.Is.TypeOf<List<NameValuePair>>());
            Expect.Call(client.SessionToken).SetPropertyAndIgnoreArgument();
            mocks.ReplayAll();

			ForceBuildPublisher publisher = new ForceBuildPublisher(factory);
			publisher.Project = "project";
			publisher.ServerUri = "tcp://localhost:21234/CruiseManager.rem";
			publisher.Run(IntegrationResultMother.CreateSuccessful());

            mocks.VerifyAll();
		}

		[Test]
		public void ShouldOnlyForceBuildIfIntegrationStatusMatches()
		{
            var factory = mocks.StrictMock<ICruiseServerClientFactory>();
            mocks.ReplayAll();

			ForceBuildPublisher publisher = new ForceBuildPublisher(factory);
			publisher.IntegrationStatus = IntegrationStatus.Exception;
			publisher.Run(IntegrationResultMother.CreateFailed());

            mocks.VerifyAll();
        }
	}
}