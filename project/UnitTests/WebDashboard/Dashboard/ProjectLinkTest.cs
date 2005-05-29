using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
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
		private string action = "my action";

		[SetUp]
		public void Setup()
		{
			projectSpecifier = new DefaultProjectSpecifier(new DefaultServerSpecifier(serverName), projectName);
			urlBuilderMock = new DynamicMock(typeof(ICruiseUrlBuilder));
			projectLink = new ProjectLink((ICruiseUrlBuilder) urlBuilderMock.MockInstance, projectSpecifier, description, this.action);
		}

		private void VerifyAll()
		{
			urlBuilderMock.Verify();
		}

		[Test]
		public void ShouldReturnGivenDescription()
		{
			Assert.AreEqual(description, projectLink.Text);
			VerifyAll();
		}

		[Test]
		public void ShouldReturnCalculatedAbsoluteUrl()
		{
			urlBuilderMock.ExpectAndReturn("BuildProjectUrl", "my absolute url", action, projectSpecifier);
			Assert.AreEqual("my absolute url", projectLink.Url);
			VerifyAll();
		}
	}
}
