using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class ProjectLinkTest
	{
		private DynamicMock urlBuilderMock;
		private string serverName = "my server";
		private string projectName = "my project";
		private IProjectSpecifier projectSpecifier;
		private string description = "my description";
		private ProjectLink projectLink;
		private ActionSpecifierWithName actionSpecifier = new ActionSpecifierWithName("my action");

		[SetUp]
		public void Setup()
		{
			projectSpecifier = new DefaultProjectSpecifier(new DefaultServerSpecifier(serverName), projectName);
			urlBuilderMock = new DynamicMock(typeof(IUrlBuilder));
			projectLink = new ProjectLink((IUrlBuilder) urlBuilderMock.MockInstance, projectSpecifier, description, this.actionSpecifier);
		}

		private void VerifyAll()
		{
			urlBuilderMock.Verify();
		}

		[Test]
		public void ShouldReturnGivenDescription()
		{
			Assert.AreEqual(description, projectLink.Description);
			VerifyAll();
		}

		[Test]
		public void ShouldReturnCalculatedAbsoluteUrl()
		{
			urlBuilderMock.ExpectAndReturn("BuildProjectUrl", "my absolute url", actionSpecifier, projectSpecifier);
			Assert.AreEqual("my absolute url", projectLink.AbsoluteURL);
			VerifyAll();
		}
	}
}
