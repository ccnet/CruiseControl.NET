using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class DefaultCruiseUrlBuilderTest
	{
		private Mock<IUrlBuilder> urlBuilderMock;
		private DefaultCruiseUrlBuilder cruiseUrlBuilder;
		private DefaultServerSpecifier serverSpecifier;
		private IProjectSpecifier projectSpecifier;
		private DefaultBuildSpecifier buildSpecifier;

		[SetUp]
		public void Setup()
		{
			urlBuilderMock = new Mock<IUrlBuilder>();
			serverSpecifier = new DefaultServerSpecifier("myserver");
			projectSpecifier = new DefaultProjectSpecifier(serverSpecifier, "myproject");
			buildSpecifier = new DefaultBuildSpecifier(projectSpecifier, "mybuild");
			cruiseUrlBuilder = new DefaultCruiseUrlBuilder((IUrlBuilder) urlBuilderMock.Object);
		}

		private void VerifyAll()
		{
			urlBuilderMock.Verify();
		}

		[Test]
		public void ShouldBuildServerUrlAddingCorrectlyFormattedAction()
		{
			// Setup
			urlBuilderMock.Setup(builder => builder.BuildUrl("myAction", "", "server/myserver")).Returns("myUrl").Verifiable();

			// Execute
			string url = cruiseUrlBuilder.BuildServerUrl("myAction", serverSpecifier);

			// Verify
			Assert.AreEqual("myUrl", url);
			VerifyAll();
		}

		[Test]
		public void ShouldBuildServerUrlAddingCorrectlyFormattedActionAndQueryString()
		{
			// Setup
			urlBuilderMock.Setup(builder => builder.BuildUrl("myAction", "query1=arg1", "server/myserver")).Returns("myUrl").Verifiable();

			// Execute
			string url = cruiseUrlBuilder.BuildServerUrl("myAction", serverSpecifier, "query1=arg1");

			// Verify
			Assert.AreEqual("myUrl", url);
			VerifyAll();
		}

		[Test]
		public void ShouldBuildProjectUrlAddingCorrectlyFormattedAction()
		{
			// Setup
			urlBuilderMock.Setup(builder => builder.BuildUrl("myAction", "", "server/myserver/project/myproject")).Returns("myUrl").Verifiable();

			// Execute
			string url = cruiseUrlBuilder.BuildProjectUrl("myAction", projectSpecifier);

			// Verify
			Assert.AreEqual("myUrl", url);
			VerifyAll();
		}

		[Test]
		public void ShouldUrlEncodeProject()
		{
			// Setup
			urlBuilderMock.Setup(builder => builder.BuildUrl("myAction", "", "server/myserver/project/myproject%232")).Returns("myUrl").Verifiable();
			projectSpecifier = new DefaultProjectSpecifier(serverSpecifier, "myproject#2");

			// Execute
			string url = cruiseUrlBuilder.BuildProjectUrl("myAction", projectSpecifier);

			// Verify
			Assert.AreEqual("myUrl", url);
			VerifyAll();
		}

		[Test]
		public void ShouldUrlEncodeProjectWithSpaces()
		{
			// Setup
			urlBuilderMock.Setup(builder => builder.BuildUrl("myAction", "", "server/myserver/project/myproject%202")).Returns("myUrl").Verifiable();
			projectSpecifier = new DefaultProjectSpecifier(serverSpecifier, "myproject 2");

			// Execute
			string url = cruiseUrlBuilder.BuildProjectUrl("myAction", projectSpecifier);

			// Verify
			Assert.AreEqual("myUrl", url);
			VerifyAll();
		}

		[Test]
		public void ShouldBuildBuildUrlAddingCorrectlyFormattedAction()
		{
			// Setup
			urlBuilderMock.Setup(builder => builder.BuildUrl("myAction", "", "server/myserver/project/myproject/build/mybuild")).Returns("myUrl").Verifiable();

			// Execute
			string url = cruiseUrlBuilder.BuildBuildUrl("myAction", buildSpecifier);

			// Verify
			Assert.AreEqual("myUrl", url);
			VerifyAll();
		}

		[Test]
		public void ShouldDelegateExtensionToSubBuilder()
		{
			// Setup
			urlBuilderMock.SetupSet(builder => builder.Extension = "foo").Verifiable();

			// Execute
			cruiseUrlBuilder.Extension = "foo";

			// Verify
			VerifyAll();
		}
	}
}