using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Label;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Label
{
	[TestFixture]
	public class RemoteProjectLabellerTest
	{
		private Mock<ICruiseManager> mockCruiseManager;
		private Mock<IRemotingService> mockRemotingService;
		private RemoteProjectLabeller labeller;

		[SetUp]
		protected void SetUp()
		{
			mockCruiseManager = new Mock<ICruiseManager>();
			mockCruiseManager.Setup(manager => manager.GetProjectStatus()).Returns(new ProjectStatus[1] {NewProjectStatus("foo", "1")}).Verifiable();

			mockRemotingService = new Mock<IRemotingService>();
			mockRemotingService.Setup(service => service.Connect(typeof(ICruiseManager), RemoteCruiseServer.DefaultManagerUri)).Returns(mockCruiseManager.Object).Verifiable();

			labeller = new RemoteProjectLabeller((IRemotingService) mockRemotingService.Object);
		}

		[Test]
		public void ShouldConnectToRemoteServerAndRetrieveLabel()
		{
			labeller.ProjectName = "foo";
			Assert.AreEqual("1", labeller.Generate(IntegrationResultMother.CreateSuccessful()));
		}

		[Test]
		public void ShouldThrowExceptionIfProjectNameIsInvalid()
		{
			labeller.ProjectName = "invalid";
            Assert.That(delegate { labeller.Generate(IntegrationResultMother.CreateSuccessful()); },
                        Throws.TypeOf<NoSuchProjectException>());
		}

		private ProjectStatus NewProjectStatus(string projectName, string label)
		{
			return ProjectStatusFixture.New(projectName, label);
		}
	}
}