using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class BuildLinkTest
	{
		private Mock<ICruiseUrlBuilder> urlBuilderMock;
		private string serverName = "my server";
		private string projectName = "my project";
		private string buildName = "my build";
		private IBuildSpecifier buildSpecifier;
		private string description = "my description";
		private BuildLink buildLink;
		private string action = "my action";

		[SetUp]
		public void Setup()
		{
			buildSpecifier = new DefaultBuildSpecifier(new DefaultProjectSpecifier(new DefaultServerSpecifier(serverName), projectName), buildName);
			urlBuilderMock = new Mock<ICruiseUrlBuilder>();
			buildLink = new BuildLink((ICruiseUrlBuilder) urlBuilderMock.Object, buildSpecifier, description, action);
		}

		private void VerifyAll()
		{
			urlBuilderMock.Verify();
		}

		[Test]
		public void ShouldReturnGivenDescription()
		{
			Assert.AreEqual(description, buildLink.Text);
			VerifyAll();
		}

		[Test]
		public void ShouldReturnCalculatedAbsoluteUrl()
		{
			urlBuilderMock.Setup(builder => builder.BuildBuildUrl(action, buildSpecifier)).Returns("my absolute url").Verifiable();
			Assert.AreEqual("my absolute url", buildLink.Url);
			VerifyAll();
		}
	}
}
