using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class BuildLinkTest
	{
		private DynamicMock urlBuilderMock;
		private string serverName = "my server";
		private string projectName = "my project";
		private string buildName = "my build";
		private string description = "my description";
		private BuildLink buildLink;
		private ActionSpecifierWithName actionSpecifier = new ActionSpecifierWithName("my action");

		[SetUp]
		public void Setup()
		{
			urlBuilderMock = new DynamicMock(typeof(IUrlBuilder));
			buildLink = new BuildLink((IUrlBuilder) urlBuilderMock.MockInstance, serverName, projectName, buildName, description, this.actionSpecifier);
		}

		private void VerifyAll()
		{
			urlBuilderMock.Verify();
		}

		[Test]
		public void ShouldReturnGivenDescription()
		{
			Assert.AreEqual(description, buildLink.Description);
			VerifyAll();
		}

		[Test]
		public void ShouldReturnCalculatedAbsoluteUrl()
		{
			urlBuilderMock.ExpectAndReturn("BuildBuildUrl", "my absolute url", actionSpecifier, serverName, projectName, buildName);
			Assert.AreEqual("my absolute url", buildLink.AbsoluteURL);
			VerifyAll();
		}
	}
}
