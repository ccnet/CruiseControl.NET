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
		public void ShouldBuildProjectrUrl()
		{
			// Setup
			pathMapperMock.ExpectAndReturn("GetAbsoluteURLForRelativePath", "http://local/foo.htm", "foo.htm");

			// Execute
			string url = urlBuilder.BuildProjectrUrl("foo.htm", "myserver", "myproject");

			// Verify
			AssertEquals("http://local/foo.htm?server=myserver&amp;project=myproject", url);
			VerifyAll();
		}
	}
}
