using System;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class DefaultBuildRetrieverForRequestTest : Assertion
	{
		private DynamicMock buildRetrieverMock;
		private DynamicMock buildNameRetrieverMock;
		private DynamicMock requestWrapperMock;
		private DefaultBuildRetrieverForRequest buildRetrieverForRequest;

		private string serverName;
		private string projectName;
		private string logContent;
		private string buildName;
		private Build build;

		[SetUp]
		public void Setup()
		{
			buildRetrieverMock = new DynamicMock(typeof(IBuildRetriever));
			buildNameRetrieverMock = new DynamicMock(typeof(IBuildNameRetriever));
			requestWrapperMock = new DynamicMock(typeof(IRequestWrapper));

			buildRetrieverForRequest = new DefaultBuildRetrieverForRequest((IBuildRetriever) buildRetrieverMock.MockInstance, (IBuildNameRetriever) buildNameRetrieverMock.MockInstance);

			serverName = "my Server";
			projectName = "my Project";
			logContent = "log Content";
			buildName = "myLogfile.xml";
			build = new Build(buildName, logContent, serverName, projectName);
		}

		[Test]
		public void ReturnsLatestBuildIfNoneSpecified()
		{
			requestWrapperMock.ExpectAndReturn("GetServerName", serverName);
			requestWrapperMock.ExpectAndReturn("GetProjectName", projectName);
			requestWrapperMock.ExpectAndReturn("GetBuildSpecifier", new NoBuildSpecified());

			buildNameRetrieverMock.ExpectAndReturn("GetLatestBuildName", buildName, serverName, projectName);
			buildRetrieverMock.ExpectAndReturn("GetBuild", build, serverName, projectName, buildName);

			Build returnedBuild = buildRetrieverForRequest.GetBuild((IRequestWrapper) requestWrapperMock.MockInstance);

			AssertEquals(buildName, returnedBuild.Name);
			AssertEquals(logContent, returnedBuild.Log);

			VerifyAll();
		}

		[Test]
		public void ReturnsSpecifiedBuildIfOneSpecified()
		{
			requestWrapperMock.ExpectAndReturn("GetServerName", serverName);
			requestWrapperMock.ExpectAndReturn("GetProjectName", projectName);
			requestWrapperMock.ExpectAndReturn("GetBuildSpecifier", new NamedBuildSpecifier(buildName));

			buildNameRetrieverMock.ExpectNoCall("GetLatestBuildName", typeof(string), typeof(string));
			buildRetrieverMock.ExpectAndReturn("GetBuild", build, serverName, projectName, buildName);

			Build returnedBuild = buildRetrieverForRequest.GetBuild((IRequestWrapper) requestWrapperMock.MockInstance);

			AssertEquals(buildName, returnedBuild.Name);
			AssertEquals(logContent, returnedBuild.Log);

			VerifyAll();
		}

		private void VerifyAll()
		{
			buildNameRetrieverMock.Verify();
			buildRetrieverMock.Verify();
			requestWrapperMock.Verify();
		}
	}
}
