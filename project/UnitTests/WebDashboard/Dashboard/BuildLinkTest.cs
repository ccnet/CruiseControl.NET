using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
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
		private IBuildSpecifier buildSpecifier;
		private string description = "my description";
		private BuildLink buildLink;
		private string action = "my action";

		[SetUp]
		public void Setup()
		{
			buildSpecifier = new DefaultBuildSpecifier(new DefaultProjectSpecifier(new DefaultServerSpecifier(serverName), projectName), buildName);
			urlBuilderMock = new DynamicMock(typeof(ICruiseUrlBuilder));
			buildLink = new BuildLink((ICruiseUrlBuilder) urlBuilderMock.MockInstance, buildSpecifier, description, action);
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
			urlBuilderMock.ExpectAndReturn("BuildBuildUrl", "my absolute url", action, buildSpecifier);
			Assert.AreEqual("my absolute url", buildLink.Url);
			VerifyAll();
		}
	}
}
