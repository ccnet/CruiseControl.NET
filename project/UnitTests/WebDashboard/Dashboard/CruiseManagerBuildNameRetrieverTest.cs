using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class CruiseManagerBuildNameRetrieverTest
	{
		private DynamicMock cruiseManagerWrapperMock;
		private CruiseManagerBuildNameRetriever nameBuildRetriever;
		private string serverName;
		private string projectName;
		private string buildName;
		private IBuildSpecifier[] buildSpecifiers;
		private DefaultBuildSpecifier buildSpecifier;
		private DefaultProjectSpecifier projectSpecifier;

		[SetUp]
		public void Setup()
		{
			cruiseManagerWrapperMock = new DynamicMock(typeof(ICruiseManagerWrapper));

			nameBuildRetriever = new CruiseManagerBuildNameRetriever((ICruiseManagerWrapper) cruiseManagerWrapperMock.MockInstance);

			serverName = "my Server";
			projectName = "my Project";
			buildName = "myLogfile.xml";
			projectSpecifier = new DefaultProjectSpecifier(new DefaultServerSpecifier(serverName), projectName);
			buildSpecifier = new DefaultBuildSpecifier(this.projectSpecifier, buildName);
			buildSpecifiers = new IBuildSpecifier[] {CreateBuildSpecifier("log3"), CreateBuildSpecifier("log2"), CreateBuildSpecifier("log1")};
		}

		private IBuildSpecifier CreateBuildSpecifier(string buildName)
		{
			return new DefaultBuildSpecifier(projectSpecifier, buildName);
		}

		private void VerifyAll()
		{
			cruiseManagerWrapperMock.Verify();
		}

		[Test]
		public void ReturnsNameOfLatestLog()
		{
			cruiseManagerWrapperMock.ExpectAndReturn("GetLatestBuildSpecifier", buildSpecifier, projectSpecifier);

			Assert.AreEqual(buildSpecifier, nameBuildRetriever.GetLatestBuildSpecifier(projectSpecifier));

			VerifyAll();
		}

		[Test]
		public void NextBuildIsRequestedBuildIfNoneNewer()
		{
			cruiseManagerWrapperMock.ExpectAndReturn("GetBuildSpecifiers", buildSpecifiers, projectSpecifier);

			Assert.AreEqual("log3", nameBuildRetriever.GetNextBuildSpecifier(new DefaultBuildSpecifier(projectSpecifier, "log3")).BuildName);

			VerifyAll();
		}

		[Test]
		public void NextBuildIsNextMostRecentBuildIfOneExists()
		{
			cruiseManagerWrapperMock.ExpectAndReturn("GetBuildSpecifiers", buildSpecifiers, projectSpecifier);

			Assert.AreEqual("log2", nameBuildRetriever.GetNextBuildSpecifier(new DefaultBuildSpecifier(projectSpecifier, "log1")).BuildName);
			VerifyAll();
		}

		[Test]
		public void ThrowsAnExceptionForNextBuildIfBuildIsUnknown()
		{
			cruiseManagerWrapperMock.ExpectAndReturn("GetBuildSpecifiers", buildSpecifiers, projectSpecifier);
			try
			{
				nameBuildRetriever.GetNextBuildSpecifier(new DefaultBuildSpecifier(projectSpecifier, "not a real build"));
				Assert.Fail("Should throw the right exception");
			}
			catch (UnknownBuildException) { }
			VerifyAll();
		}

		[Test]
		public void PreviousBuildIsRequestedBuildIfNoneOlder()
		{
			cruiseManagerWrapperMock.ExpectAndReturn("GetBuildSpecifiers", buildSpecifiers, projectSpecifier);

			Assert.AreEqual("log1", nameBuildRetriever.GetPreviousBuildSpecifier(new DefaultBuildSpecifier(projectSpecifier, "log1")).BuildName);

			VerifyAll();
		}

		[Test]
		public void PreviousBuildIsNextOldestIfOneExists()
		{
			cruiseManagerWrapperMock.ExpectAndReturn("GetBuildSpecifiers", buildSpecifiers, projectSpecifier);

			Assert.AreEqual("log2", nameBuildRetriever.GetPreviousBuildSpecifier(new DefaultBuildSpecifier(projectSpecifier, "log3")).BuildName);

			VerifyAll();
		}

		[Test]
		public void ThrowsAnExceptionForPreviousBuildIfBuildIsUnknown()
		{
			cruiseManagerWrapperMock.ExpectAndReturn("GetBuildSpecifiers", buildSpecifiers, projectSpecifier);
			try
			{
				nameBuildRetriever.GetPreviousBuildSpecifier(new DefaultBuildSpecifier(projectSpecifier, "not a real build"));
				Assert.Fail("Should throw the right exception");
			}
			catch (UnknownBuildException) { }
			VerifyAll();
		}
	}
}