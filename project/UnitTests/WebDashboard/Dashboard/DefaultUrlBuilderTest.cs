using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class DefaultUrlBuilderTest
	{
		private DynamicMock pathMapperMock;
		private DefaultUrlBuilder urlBuilder;
		private DefaultServerSpecifier serverSpecifier;
		private IProjectSpecifier projectSpecifier;
		private DefaultBuildSpecifier buildSpecifier;

		[SetUp]
		public void Setup()
		{
			pathMapperMock = new DynamicMock(typeof(IPathMapper));
			serverSpecifier = new DefaultServerSpecifier("myserver");
			projectSpecifier = new DefaultProjectSpecifier(serverSpecifier, "myproject");
			buildSpecifier = new DefaultBuildSpecifier(projectSpecifier, "mybuild");
			urlBuilder = new DefaultUrlBuilder((IPathMapper) pathMapperMock.MockInstance);
		}

		private void VerifyAll()
		{
			pathMapperMock.Verify();
		}

		[Test]
		public void ShouldBuildUrlUsingPathMapperToGenerateAbsoluteUrl()
		{
			// Setup
			pathMapperMock.ExpectAndReturn("GetAbsoluteURLForRelativePath", "http://local/foo.htm", "foo.htm");
			
			// Execute
			string url = urlBuilder.BuildUrl("foo.htm");

			// Verify
			Assert.AreEqual("http://local/foo.htm", url);
			VerifyAll();
		}

		[Test]
		public void ShouldBuildUrlAddingCorrectlyFormattedAction()
		{
			// Setup
			pathMapperMock.ExpectAndReturn("GetAbsoluteURLForRelativePath", "http://local/foo.htm", DefaultUrlBuilder.CONTROLLER_RELATIVE_URL);
			
			// Execute
			string url = urlBuilder.BuildUrl(new ActionSpecifierWithName("myAction"));

			// Verify
			Assert.AreEqual("http://local/foo.htm?_action_myAction=true", url);
			VerifyAll();
		}

		[Test]
		public void IfPartialQueryStringSpecifiedThenAddItToEndOfUrl()
		{
			// Setup
			pathMapperMock.ExpectAndReturn("GetAbsoluteURLForRelativePath", "http://local/foo.htm", "foo.htm");
			
			// Execute
			string url = urlBuilder.BuildUrl("foo.htm", "myparam=myvalue");

			// Verify
			Assert.AreEqual("http://local/foo.htm?myparam=myvalue", url);
			VerifyAll();
		}

		[Test]
		public void ShouldBuildUrlWithActionAndQueryString()
		{
			// Setup
			pathMapperMock.ExpectAndReturn("GetAbsoluteURLForRelativePath", "http://local/foo.htm", DefaultUrlBuilder.CONTROLLER_RELATIVE_URL);
			
			// Execute
			string url = urlBuilder.BuildUrl(new ActionSpecifierWithName("myAction"), "myparam=myvalue");

			// Verify
			Assert.AreEqual("http://local/foo.htm?_action_myAction=true&myparam=myvalue", url);
			VerifyAll();
		}

		[Test]
		public void ShouldBuildServerUrl()
		{
			// Setup
			pathMapperMock.ExpectAndReturn("GetAbsoluteURLForRelativePath", "http://local/foo.htm", "foo.htm");

			// Execute
			string url = urlBuilder.BuildServerUrl("foo.htm", serverSpecifier);

			// Verify
			Assert.AreEqual("http://local/foo.htm?server=myserver", url);
			VerifyAll();
		}
		
		[Test]
		public void ShouldBuildServerUrlAddingCorrectlyFormattedAction()
		{
			// Setup
			pathMapperMock.ExpectAndReturn("GetAbsoluteURLForRelativePath", "http://local/foo.htm", DefaultUrlBuilder.CONTROLLER_RELATIVE_URL);
			
			// Execute
			string url = urlBuilder.BuildServerUrl(new ActionSpecifierWithName("myAction"), serverSpecifier);

			// Verify
			Assert.AreEqual("http://local/foo.htm?_action_myAction=true&server=myserver", url);
			VerifyAll();
		}

		[Test]
		public void ShouldBuildProjectUrl()
		{
			// Setup
			pathMapperMock.ExpectAndReturn("GetAbsoluteURLForRelativePath", "http://local/foo.htm", "foo.htm");

			// Execute
			string url = urlBuilder.BuildProjectUrl("foo.htm", projectSpecifier);

			// Verify
			Assert.AreEqual("http://local/foo.htm?server=myserver&project=myproject", url);
			VerifyAll();
		}

		[Test]
		public void ShouldBuildProjectUrlAddingCorrectlyFormattedAction()
		{
			// Setup
			pathMapperMock.ExpectAndReturn("GetAbsoluteURLForRelativePath", "http://local/foo.htm", DefaultUrlBuilder.CONTROLLER_RELATIVE_URL);

			// Execute
			string url = urlBuilder.BuildProjectUrl(new ActionSpecifierWithName("myAction"), projectSpecifier);

			// Verify
			Assert.AreEqual("http://local/foo.htm?_action_myAction=true&server=myserver&project=myproject", url);
			VerifyAll();
		}

		[Test]
		public void ShouldBuildBuildUrl()
		{
			// Setup
			pathMapperMock.ExpectAndReturn("GetAbsoluteURLForRelativePath", "http://local/foo.htm", "foo.htm");

			// Execute
			string url = urlBuilder.BuildBuildUrl("foo.htm", buildSpecifier);

			// Verify
			Assert.AreEqual("http://local/foo.htm?server=myserver&project=myproject&build=mybuild", url);
			VerifyAll();
		}

		[Test]
		public void ShouldBuildBuildUrlAddingCorrectlyFormattedAction()
		{
			// Setup
			pathMapperMock.ExpectAndReturn("GetAbsoluteURLForRelativePath", "http://local/foo.htm", DefaultUrlBuilder.CONTROLLER_RELATIVE_URL);

			// Execute
			string url = urlBuilder.BuildBuildUrl(new ActionSpecifierWithName("myAction"), buildSpecifier);

			// Verify
			Assert.AreEqual("http://local/foo.htm?_action_myAction=true&server=myserver&project=myproject&build=mybuild", url);
			VerifyAll();
		}

		[Test]
		public void ShouldBuildFormNameBasedOnActionNameWithNoArgs()
		{
			// Execute
			string formName = urlBuilder.BuildFormName(new ActionSpecifierWithName("myAction"));

			// Verify
			Assert.AreEqual("_action_myAction", formName);
			VerifyAll();
		}

		[Test]
		public void ShouldBuildFormNameBasedOnActionNameAndArgs()
		{
			// Execute
			string formName = urlBuilder.BuildFormName(new ActionSpecifierWithName("myAction"), "arg1", "arg2");

			// Verify
			Assert.AreEqual("_action_myAction_arg1_arg2", formName);
			VerifyAll();
		}
	}
}
