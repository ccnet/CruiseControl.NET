using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.LogViewerPlugin;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.LogViewerPlugin
{
	[TestFixture]
	public class LogViewerTest : Assertion
	{
		private DynamicMock buildRetrieverMock;

		private LogViewer logViewer;

		private string url;
		private Build build;

		[SetUp]
		public void Setup()
		{
			buildRetrieverMock = new DynamicMock(typeof(IBuildRetriever));

			logViewer = new LogViewer((IBuildRetriever) buildRetrieverMock.MockInstance);

			url = "http://foo.bar";
			build  = new Build("mybuild", "", url);
		}

		private void VerifyAll()
		{
			buildRetrieverMock.Verify();
		}

		[Test]
		public void ReturnsURLOfLatestBuild()
		{
			buildRetrieverMock.ExpectAndReturn("GetBuild", build);

			LogViewerResults results = logViewer.Do();
			AssertEquals(url, results.RedirectURL);

			VerifyAll();
		}
	}
}
