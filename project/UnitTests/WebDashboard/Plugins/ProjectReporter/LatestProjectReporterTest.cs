using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReporterPlugin;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.ProjectReporter
{
	[TestFixture]
	public class LatestProjectReporterTest : Assertion
	{
		private DynamicMock mockUrlGenerator;

		[SetUp]
		public void Setup()
		{
			mockUrlGenerator = new DynamicMock(typeof(IProjectUrlGenerator));
		}

		private void VerifyAll()
		{
			mockUrlGenerator.Verify();
		}

		[Test]
		public void DescriptionIsCorrect()
		{
			LatestProjectReporter reporter = new LatestProjectReporter();
			AssertEquals("Latest", reporter.Description);
		}

		[Test]
		public void UsesCorrectProjectScopeUrl()
		{
			mockUrlGenerator.ExpectAndReturn("GenerateUrl", "test.htm", "ProjectReport.aspx", "myserver", "myproject");

			LatestProjectReporter reporter = new LatestProjectReporter();
			AssertEquals("test.htm", reporter.CreateURL("myserver", "myproject", (IProjectUrlGenerator) mockUrlGenerator.MockInstance));
			
			VerifyAll();
		}
	}
}
