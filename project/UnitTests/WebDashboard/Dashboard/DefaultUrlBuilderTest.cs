using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class DefaultUrlBuilderTest : Assertion
	{
		private DynamicMock pathMapperMock;
		private DefaultUrlBuilder urlBuilder;

		[SetUp]
		public void Setup()
		{
			pathMapperMock = new DynamicMock(typeof(IPathMapper));
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
			AssertEquals("http://local/foo.htm", url);
			VerifyAll();
		}

		[Test]
		public void ShouldBuildUrlAddingCorrectlyFormattedAction()
		{
			// Setup
			pathMapperMock.ExpectAndReturn("GetAbsoluteURLForRelativePath", "http://local/foo.htm", "foo.htm");
			
			// Execute
			string url = urlBuilder.BuildUrl("foo.htm", new ActionSpecifierWithName("myAction"));

			// Verify
			AssertEquals("http://local/foo.htm?_action_myAction=true", url);
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
			AssertEquals("http://local/foo.htm?myparam=myvalue", url);
			VerifyAll();
		}

		[Test]
		public void ShouldBuildUrlWithActionAndQueryString()
		{
			// Setup
			pathMapperMock.ExpectAndReturn("GetAbsoluteURLForRelativePath", "http://local/foo.htm", "foo.htm");
			
			// Execute
			string url = urlBuilder.BuildUrl("foo.htm", new ActionSpecifierWithName("myAction"), "myparam=myvalue");

			// Verify
			AssertEquals("http://local/foo.htm?_action_myAction=true&amp;myparam=myvalue", url);
			VerifyAll();
		}

		[Test]
		public void ShouldBuildServerUrl()
		{
			// Setup
			pathMapperMock.ExpectAndReturn("GetAbsoluteURLForRelativePath", "http://local/foo.htm", "foo.htm");

			// Execute
			string url = urlBuilder.BuildServerUrl("foo.htm", "myserver");

			// Verify
			AssertEquals("http://local/foo.htm?server=myserver", url);
			VerifyAll();
		}
		
		[Test]
		public void ShouldBuildServerUrlAddingCorrectlyFormattedAction()
		{
			// Setup
			pathMapperMock.ExpectAndReturn("GetAbsoluteURLForRelativePath", "http://local/foo.htm", "foo.htm");
			
			// Execute
			string url = urlBuilder.BuildServerUrl("foo.htm", new ActionSpecifierWithName("myAction"), "myserver");

			// Verify
			AssertEquals("http://local/foo.htm?_action_myAction=true&amp;server=myserver", url);
			VerifyAll();
		}

		[Test]
		public void ShouldBuildProjectUrl()
		{
			// Setup
			pathMapperMock.ExpectAndReturn("GetAbsoluteURLForRelativePath", "http://local/foo.htm", "foo.htm");

			// Execute
			string url = urlBuilder.BuildProjectUrl("foo.htm", "myserver", "myproject");

			// Verify
			AssertEquals("http://local/foo.htm?server=myserver&amp;project=myproject", url);
			VerifyAll();
		}

		[Test]
		public void ShouldBuildBuildUrl()
		{
			// Setup
			pathMapperMock.ExpectAndReturn("GetAbsoluteURLForRelativePath", "http://local/foo.htm", "foo.htm");

			// Execute
			string url = urlBuilder.BuildBuildUrl("foo.htm", "myserver", "myproject", "mybuild");

			// Verify
			AssertEquals("http://local/foo.htm?server=myserver&amp;project=myproject&amp;build=mybuild", url);
			VerifyAll();
		}
	}
}
