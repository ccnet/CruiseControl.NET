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
			Assert.AreEqual("http://local/foo.htm?_action_myAction=true&amp;myparam=myvalue", url);
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
			Assert.AreEqual("http://local/foo.htm?server=myserver", url);
			VerifyAll();
		}
		
		[Test]
		public void ShouldBuildServerUrlAddingCorrectlyFormattedAction()
		{
			// Setup
			pathMapperMock.ExpectAndReturn("GetAbsoluteURLForRelativePath", "http://local/foo.htm", DefaultUrlBuilder.CONTROLLER_RELATIVE_URL);
			
			// Execute
			string url = urlBuilder.BuildServerUrl(new ActionSpecifierWithName("myAction"), "myserver");

			// Verify
			Assert.AreEqual("http://local/foo.htm?_action_myAction=true&amp;server=myserver", url);
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
			Assert.AreEqual("http://local/foo.htm?server=myserver&amp;project=myproject", url);
			VerifyAll();
		}

		[Test]
		public void ShouldBuildProjectUrlAddingCorrectlyFormattedAction()
		{
			// Setup
			pathMapperMock.ExpectAndReturn("GetAbsoluteURLForRelativePath", "http://local/foo.htm", DefaultUrlBuilder.CONTROLLER_RELATIVE_URL);

			// Execute
			string url = urlBuilder.BuildProjectUrl(new ActionSpecifierWithName("myAction"),"myserver", "myproject");

			// Verify
			Assert.AreEqual("http://local/foo.htm?_action_myAction=true&amp;server=myserver&amp;project=myproject", url);
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
			Assert.AreEqual("http://local/foo.htm?server=myserver&amp;project=myproject&amp;build=mybuild", url);
			VerifyAll();
		}

		[Test]
		public void ShouldBuildBuildUrlAddingCorrectlyFormattedAction()
		{
			// Setup
			pathMapperMock.ExpectAndReturn("GetAbsoluteURLForRelativePath", "http://local/foo.htm", DefaultUrlBuilder.CONTROLLER_RELATIVE_URL);

			// Execute
			string url = urlBuilder.BuildBuildUrl(new ActionSpecifierWithName("myAction"),"myserver", "myproject", "mybuild");

			// Verify
			Assert.AreEqual("http://local/foo.htm?_action_myAction=true&amp;server=myserver&amp;project=myproject&amp;build=mybuild", url);
			VerifyAll();
		}
	}
}
