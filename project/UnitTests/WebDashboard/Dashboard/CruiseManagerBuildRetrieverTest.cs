using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class CruiseManagerBuildRetrieverTest
	{
		private DynamicMock cruiseManagerMock;
		private CruiseManagerBuildRetriever cruiseManagerBuildRetriever;
		private string serverName;
		private string projectName;
		private string logContent;
		private string buildName;

		[SetUp]
		public void Setup()
		{
			cruiseManagerMock = new DynamicMock(typeof(ICruiseManagerWrapper));

			cruiseManagerBuildRetriever = new CruiseManagerBuildRetriever((ICruiseManagerWrapper) cruiseManagerMock.MockInstance);

			serverName = "my Server";
			projectName = "my Project";
			logContent = "log Content";
			buildName = "myLogfile.xml";
		}

		[Test]
		public void ReturnsSpecifiedLog()
		{
			cruiseManagerMock.ExpectAndReturn("GetLog", logContent, serverName, projectName, buildName);

			Build returnedBuild = cruiseManagerBuildRetriever.GetBuild(serverName, projectName, buildName);
			Assert.AreEqual(buildName, returnedBuild.Name);
			Assert.AreEqual(logContent, returnedBuild.Log);
			Assert.AreEqual(serverName, returnedBuild.ServerName);
			Assert.AreEqual(projectName, returnedBuild.ProjectName);

			VerifyAll();
		}

		private void VerifyAll()
		{
			cruiseManagerMock.Verify();
		}
	}
}
