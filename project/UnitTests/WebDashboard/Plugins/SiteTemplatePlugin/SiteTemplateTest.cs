using System.Web.UI.HtmlControls;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Config;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.SiteTemplatePlugin;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.SiteTemplatePlugin
{
	[TestFixture]
	public class SiteTemplateTest : Assertion
	{
		private DynamicMock requestWrapperMock;
		private DynamicMock buildListerMock;
		private DynamicMock buildRetrieverMock;
		private DynamicMock configurationGetterMock;
		private DynamicMock buildNameRetrieverMock;
		private SiteTemplate siteTemplate;
		private Build build;
		private ICruiseRequest request;
		private string server;
		private string project;
		private string buildName;

		[SetUp]
		public void Setup()
		{
			requestWrapperMock = new DynamicMock(typeof(ICruiseRequest));
			configurationGetterMock = new DynamicMock(typeof(IConfigurationGetter));
			buildListerMock = new DynamicMock(typeof(IBuildLister));
			buildRetrieverMock = new DynamicMock(typeof(IBuildRetrieverForRequest));
			buildNameRetrieverMock = new DynamicMock(typeof(IBuildNameRetriever));

			request = (ICruiseRequest) requestWrapperMock.MockInstance;
			siteTemplate = new SiteTemplate(
				request,
				(IConfigurationGetter) configurationGetterMock.MockInstance,
				(IBuildLister) buildListerMock.MockInstance,
				(IBuildRetrieverForRequest) buildRetrieverMock.MockInstance,
				(IBuildNameRetriever) buildNameRetrieverMock.MockInstance);

			server = "myserver";
			project = "myproject";
			buildName = "log20040721095851Lbuild.1.xml";
			build = new Build(buildName, "my content", server, project);
		}

		private void VerifyMocks()
		{
			requestWrapperMock.Verify();
			configurationGetterMock.Verify();
			buildListerMock.Verify();
			buildRetrieverMock.Verify();
			buildNameRetrieverMock.Verify();
		}

		[Test]
		public void DoesntGetBuildListFromListerIfNoProjectSpecified()
		{
			requestWrapperMock.ExpectAndReturn("GetProjectName", "");
			requestWrapperMock.ExpectAndReturn("GetServerName", "server");
			buildListerMock.ExpectNoCall("GetBuildLinks", typeof(string), typeof(string));

			siteTemplate.Do();
			VerifyMocks();
		}

		[Test]
		public void GetsBuildListFromListerIfProjectSpecified()
		{
			HtmlAnchor anchor = new HtmlAnchor();
			requestWrapperMock.ExpectAndReturn("GetProjectName", "myProject");
			requestWrapperMock.ExpectAndReturn("GetServerName", "myServer");
			buildListerMock.ExpectAndReturn("GetBuildLinks", new HtmlAnchor[] { anchor } , "myServer", "myProject");
			buildRetrieverMock.ExpectAndReturn("GetBuild", build, request);

			SiteTemplateResults results = siteTemplate.Do();
			AssertEquals(anchor, results.BuildLinkList[0]);
			VerifyMocks();
		}
	}
}
