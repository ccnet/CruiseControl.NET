using System;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class CruiseManagerBuildNameRetrieverTest : Assertion
	{
		private DynamicMock cruiseManagerMock;
		private CruiseManagerBuildNameRetriever nameBuildRetriever;
		private string serverName;
		private string projectName;
		private string buildName;
		private string[] buildNames;

		[SetUp]
		public void Setup()
		{
			cruiseManagerMock = new DynamicMock(typeof(ICruiseManagerWrapper));

			nameBuildRetriever = new CruiseManagerBuildNameRetriever((ICruiseManagerWrapper) cruiseManagerMock.MockInstance);

			serverName = "my Server";
			projectName = "my Project";
			buildName = "myLogfile.xml";
			buildNames = new string[] {"log3", "log2", "log1"};

		}

		private void VerifyAll()
		{
			cruiseManagerMock.Verify();
		}

		[Test]
		public void ReturnsNameOfLatestLog()
		{
			cruiseManagerMock.ExpectAndReturn("GetLatestBuildName", buildName, serverName, projectName);

			AssertEquals(buildName, nameBuildRetriever.GetLatestBuildName(serverName, projectName));

			VerifyAll();
		}

		[Test]
		public void NextBuildIsRequestedBuildIfNoneNewer()
		{
			cruiseManagerMock.ExpectAndReturn("GetBuildNames", buildNames, serverName, projectName);

			AssertEquals("log3", nameBuildRetriever.GetNextBuildName(new Build("log3", "", serverName, projectName)));

			VerifyAll();
		}

		[Test]
		public void NextBuildIsNextMostRecentBuildIfOneExists()
		{
			cruiseManagerMock.ExpectAndReturn("GetBuildNames", buildNames, serverName, projectName);

			AssertEquals("log2", nameBuildRetriever.GetNextBuildName(new Build("log1", "", serverName, projectName)));

			VerifyAll();
		}

		[Test]
		public void ThrowsAnExceptionForNextBuildIfBuildIsUnknown()
		{
			cruiseManagerMock.ExpectAndReturn("GetBuildNames", buildNames, serverName, projectName);
			try
			{
				nameBuildRetriever.GetNextBuildName(new Build("not a real build", "",serverName, projectName));
				Fail("Should throw the right exception");
			}
			catch (UnknownBuildException) { }
		}

		[Test]
		public void PreviousBuildIsRequestedBuildIfNoneOlder()
		{
			cruiseManagerMock.ExpectAndReturn("GetBuildNames", buildNames, serverName, projectName);

			AssertEquals("log1", nameBuildRetriever.GetPreviousBuildName(new Build("log1", "",serverName, projectName)));

			VerifyAll();
		}

		[Test]
		public void PreviousBuildIsNextOldestIfOneExists()
		{
			cruiseManagerMock.ExpectAndReturn("GetBuildNames", buildNames, serverName, projectName);

			AssertEquals("log2", nameBuildRetriever.GetPreviousBuildName(new Build("log3", "",serverName, projectName)));

			VerifyAll();
		}

		[Test]
		public void ThrowsAnExceptionForPreviousBuildIfBuildIsUnknown()
		{
			cruiseManagerMock.ExpectAndReturn("GetBuildNames", buildNames, serverName, projectName);
			try
			{
				nameBuildRetriever.GetPreviousBuildName(new Build("not a real build", "",serverName, projectName));
				Fail("Should throw the right exception");
			}
			catch (UnknownBuildException) { }
		}
	}
}