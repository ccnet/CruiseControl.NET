using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

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
		public void ShouldBuildUrlAddingCorrectlyFormattedAction()
		{
			// Setup
			pathMapperMock.ExpectAndReturn("GetAbsoluteURLForRelativePath", "http://local/foo.htm", DefaultUrlBuilder.CONTROLLER_RELATIVE_URL);
			
			// Execute
			string url = urlBuilder.BuildUrl("myAction");

			// Verify
			Assert.AreEqual("http://local/foo.htm?_action_myAction=true", url);
			VerifyAll();
		}

		[Test]
		public void ShouldBuildUrlWithActionAndQueryString()
		{
			// Setup
			pathMapperMock.ExpectAndReturn("GetAbsoluteURLForRelativePath", "http://local/foo.htm", DefaultUrlBuilder.CONTROLLER_RELATIVE_URL);
			
			// Execute
			string url = urlBuilder.BuildUrl("myAction", "myparam=myvalue");

			// Verify
			Assert.AreEqual("http://local/foo.htm?_action_myAction=true&myparam=myvalue", url);
			VerifyAll();
		}

		[Test]
		public void ShouldBuildUrlWithAlternativeBaseRelativeUrlIfGiven()
		{
			// Setup
			pathMapperMock.ExpectAndReturn("GetAbsoluteURLForRelativePath", "http://local/foo.htm", "myBaseUrl");
			
			// Execute
			string url = urlBuilder.BuildUrl("myAction", "myparam=myvalue", "myBaseUrl");

			// Verify
			Assert.AreEqual("http://local/foo.htm?_action_myAction=true&myparam=myvalue", url);
			VerifyAll();
		}

		[Test]
		public void ShouldBuildFormNameBasedOnActionName()
		{
			// Execute
			string formName = urlBuilder.BuildFormName("myAction");

			// Verify
			Assert.AreEqual("_action_myAction", formName);
			VerifyAll();
		}
	}
}
