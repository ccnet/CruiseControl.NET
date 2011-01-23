using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
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
		private string action = "my action";

		[SetUp]
		public void Setup()
		{
			serverSpecifier = new DefaultServerSpecifier(serverName);
			urlBuilderMock = new DynamicMock(typeof(ICruiseUrlBuilder));
			serverLink = new ServerLink((ICruiseUrlBuilder) urlBuilderMock.MockInstance, serverSpecifier, description, this.action);
		}

		private void VerifyAll()
		{
			urlBuilderMock.Verify();
		}

		[Test]
		public void ShouldReturnGivenDescription()
		{
			Assert.AreEqual(description, serverLink.Text);
			VerifyAll();
		}

		[Test]
		public void ShouldReturnCalculatedAbsoluteUrl()
		{
			urlBuilderMock.ExpectAndReturn("BuildServerUrl", "my absolute url", action, serverSpecifier);
			Assert.AreEqual("my absolute url", serverLink.Url);
			VerifyAll();
		}
	}
}
