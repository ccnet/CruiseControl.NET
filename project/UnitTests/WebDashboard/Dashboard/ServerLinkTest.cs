using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class ServerLinkTest
	{
		private DynamicMock urlBuilderMock;
		private string serverName = "my server";
		private IServerSpecifier serverSpecifier;
		private string description = "my description";
		private ServerLink serverLink;
		private ActionSpecifierWithName actionSpecifier = new ActionSpecifierWithName("my action");

		[SetUp]
		public void Setup()
		{
			serverSpecifier = new DefaultServerSpecifier(serverName);
			urlBuilderMock = new DynamicMock(typeof(IUrlBuilder));
			serverLink = new ServerLink((IUrlBuilder) urlBuilderMock.MockInstance, serverSpecifier, description, this.actionSpecifier);
		}

		private void VerifyAll()
		{
			urlBuilderMock.Verify();
		}

		[Test]
		public void ShouldReturnGivenDescription()
		{
			Assert.AreEqual(description, serverLink.Description);
			VerifyAll();
		}

		[Test]
		public void ShouldReturnCalculatedAbsoluteUrl()
		{
			urlBuilderMock.ExpectAndReturn("BuildServerUrl", "my absolute url", actionSpecifier, serverSpecifier);
			Assert.AreEqual("my absolute url", serverLink.AbsoluteURL);
			VerifyAll();
		}
	}
}
