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
		private SiteTemplate siteTemplate;
		private Build build;

		[SetUp]
		public void Setup()
		{
			requestWrapperMock = new DynamicMock(typeof(IRequestWrapper));
			configurationGetterMock = new DynamicMock(typeof(IConfigurationGetter));
			buildListerMock = new DynamicMock(typeof(IBuildLister));
			buildRetrieverMock = new DynamicMock(typeof(IBuildRetriever));

			siteTemplate = new SiteTemplate(
				(IRequestWrapper) requestWrapperMock.MockInstance,
				(IConfigurationGetter) configurationGetterMock.MockInstance,
				(IBuildLister) buildListerMock.MockInstance,
				(IBuildRetriever) buildRetrieverMock.MockInstance);

			build = new Build("log20040721095851Lbuild.1.xml", "", "");
		}

		private void VerifyMocks()
		{
			requestWrapperMock.Verify();
			configurationGetterMock.Verify();
			buildListerMock.Verify();
			buildRetrieverMock.Verify();
		}

		[Test]
		public void IfNoProjectSpecifiedThenNotProjectMode()
		{
			requestWrapperMock.ExpectAndReturn("GetProjectName", "");
			SiteTemplateResults results = siteTemplate.Do();
			AssertEquals(false, results.ProjectMode);

			VerifyMocks();
		}

		[Test]
		public void IfNoServerSpecifiedThenNotProjectMode()
		{
			requestWrapperMock.ExpectAndReturn("GetServerName", "");
			SiteTemplateResults results = siteTemplate.Do();
			AssertEquals(false, results.ProjectMode);

			VerifyMocks();
		}

		[Test]
		public void IfProjectSpecifiedThenProjectMode()
		{
			requestWrapperMock.SetupResult("GetProjectName", "myProject");
			buildRetrieverMock.ExpectAndReturn("GetBuild", build);
			buildRetrieverMock.ExpectAndReturn("GetPreviousBuild", build, build);
			buildRetrieverMock.ExpectAndReturn("GetNextBuild", build, build);

			SiteTemplateResults results = siteTemplate.Do();
			AssertEquals(true, results.ProjectMode);

			VerifyMocks();
		}

		[Test]
		public void DoesntGetBuildListFromListerIfNoProjectSpecified()
		{
			requestWrapperMock.SetupResult("GetProjectName", "");
			buildListerMock.ExpectNoCall("GetBuildLinks", typeof(string), typeof(string));

			siteTemplate.Do();
			VerifyMocks();
		}

		[Test]
		public void GetsBuildListFromListerIfProjectSpecified()
		{
			HtmlAnchor anchor = new HtmlAnchor();
			requestWrapperMock.SetupResult("GetProjectName", "myProject");
			requestWrapperMock.SetupResult("GetServerName", "myServer");
			buildListerMock.ExpectAndReturn("GetBuildLinks", new HtmlAnchor[] { anchor } , "myServer", "myProject");
			buildRetrieverMock.ExpectAndReturn("GetBuild", build);
			buildRetrieverMock.ExpectAndReturn("GetPreviousBuild", build, build);
			buildRetrieverMock.ExpectAndReturn("GetNextBuild", build, build);

			SiteTemplateResults results = siteTemplate.Do();
			AssertEquals(anchor, results.BuildLinkList[0]);
			VerifyMocks();
		}
	}
}
