using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class ServerQueryingBuildRetrieverTest
	{
		private DynamicMock cruiseManagerWrapperMock;
		private ServerQueryingBuildRetriever serverQueryingBuildRetriever;
		private string logContent;
		private DefaultBuildSpecifier buildSpecifier;

		[SetUp]
		public void Setup()
		{
			cruiseManagerWrapperMock = new DynamicMock(typeof(ICruiseManagerWrapper));

			serverQueryingBuildRetriever = new ServerQueryingBuildRetriever(((ICruiseManagerWrapper) cruiseManagerWrapperMock.MockInstance));

			buildSpecifier = new DefaultBuildSpecifier(new DefaultProjectSpecifier(new DefaultServerSpecifier("s"), "p"), "myBuild");
			logContent = "log Content";
		}

		private void VerifyAll()
		{
			cruiseManagerWrapperMock.Verify();
		}

		[Test]
		public void ReturnsBuildUsingLogFromServer()
		{
			cruiseManagerWrapperMock.ExpectAndReturn("GetLog", logContent, buildSpecifier);

			Build returnedBuild = serverQueryingBuildRetriever.GetBuild(buildSpecifier);

			Assert.AreEqual(buildSpecifier, returnedBuild.BuildSpecifier);
			Assert.AreEqual(logContent, returnedBuild.Log);

			VerifyAll();
		}
	}
}
