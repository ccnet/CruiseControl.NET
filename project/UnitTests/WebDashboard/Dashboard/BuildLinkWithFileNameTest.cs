using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class BuildLinkWithFileNameTest
	{
		private DynamicMock urlBuilderMock;
		private string serverName = "my server";
		private string projectName = "my project";
		private string buildName = "my build";
		private IBuildSpecifier buildSpecifier;
		private string description = "my description";
		private string fileName = "another.html";
		private BuildLinkWithFileName buildLink;
		private ActionSpecifierWithName actionSpecifier = new ActionSpecifierWithName("my action");

		[SetUp]
		public void Setup()
		{
			buildSpecifier = new DefaultBuildSpecifier(new DefaultProjectSpecifier(new DefaultServerSpecifier(serverName), projectName), buildName);
			urlBuilderMock = new DynamicMock(typeof(IUrlBuilder));
			buildLink = new BuildLinkWithFileName((IUrlBuilder) urlBuilderMock.MockInstance, buildSpecifier, description, this.actionSpecifier, fileName);
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
			urlBuilderMock.ExpectAndReturn("BuildBuildUrl", "my absolute url", actionSpecifier, buildSpecifier, fileName);
			Assert.AreEqual("my absolute url", buildLink.Url);
			VerifyAll();
		}
	}
}
