using System.Web.UI.HtmlControls;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.config;
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
		private ICruiseRequestWrapper requestWrapper;
		private string server;
		private string project;
		private string buildName;

		[SetUp]
		public void Setup()
		{
			requestWrapperMock = new DynamicMock(typeof(ICruiseRequestWrapper));
			configurationGetterMock = new DynamicMock(typeof(IConfigurationGetter));
			buildListerMock = new DynamicMock(typeof(IBuildLister));
			buildRetrieverMock = new DynamicMock(typeof(IBuildRetrieverForRequest));
			buildNameRetrieverMock = new DynamicMock(typeof(IBuildNameRetriever));

			requestWrapper = (ICruiseRequestWrapper) requestWrapperMock.MockInstance;
			siteTemplate = new SiteTemplate(
				requestWrapper,
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
		public void IfNoProjectSpecifiedThenNotProjectMode()
		{
			requestWrapperMock.ExpectAndReturn("GetProjectName", "");
			requestWrapperMock.ExpectAndReturn("GetServerName", "server");
			SiteTemplateResults results = siteTemplate.Do();
			AssertEquals(false, results.ProjectMode);

			VerifyMocks();
		}

		[Test]
		public void IfNoServerSpecifiedThenNotProjectMode()
		{
			requestWrapperMock.ExpectAndReturn("GetProjectName", "project");
			requestWrapperMock.ExpectAndReturn("GetServerName", "");
			SiteTemplateResults results = siteTemplate.Do();
			AssertEquals(false, results.ProjectMode);

			VerifyMocks();
		}

		[Test]
		public void IfProjectSpecifiedThenProjectMode()
		{
			requestWrapperMock.ExpectAndReturn("GetProjectName", "myProject");
			requestWrapperMock.ExpectAndReturn("GetServerName", "myProject");
			buildRetrieverMock.ExpectAndReturn("GetBuild", build, requestWrapper);

			SiteTemplateResults results = siteTemplate.Do();
			AssertEquals(true, results.ProjectMode);

			VerifyMocks();
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
			buildRetrieverMock.ExpectAndReturn("GetBuild", build, requestWrapper);

			SiteTemplateResults results = siteTemplate.Do();
			AssertEquals(anchor, results.BuildLinkList[0]);
			VerifyMocks();
		}

		[Test]
		public void DoesntCreateListOfBuildPluginsIfNoProjectSpecified()
		{
			requestWrapperMock.ExpectAndReturn("GetProjectName", "");
			requestWrapperMock.ExpectAndReturn("GetServerName", "server");
			SiteTemplateResults results = siteTemplate.Do();
			AssertEquals(0, results.BuildPluginsList.Length);

			VerifyMocks();
		}

		[Test]
		public void CreatesCorrectListOfBuildPluginLinks()
		{
			IPluginSpecification buildPlugin = new AssemblyLoadingPluginSpecification(typeof(TestBuildPlugin).FullName, typeof(TestBuildPlugin).Assembly.CodeBase);
			IPluginSpecification serverPlugin = new AssemblyLoadingPluginSpecification(typeof(TestServerPlugin).FullName, typeof(TestServerPlugin).Assembly.CodeBase);
			IPluginSpecification[] assemblyLoadingPlugins = new IPluginSpecification[] { buildPlugin, serverPlugin };
			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", assemblyLoadingPlugins, PluginsSectionHandler.SectionName);
			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", assemblyLoadingPlugins, PluginsSectionHandler.SectionName);
			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", assemblyLoadingPlugins, PluginsSectionHandler.SectionName);
			requestWrapperMock.ExpectAndReturn("GetProjectName", project);
			requestWrapperMock.ExpectAndReturn("GetServerName", server);
			buildRetrieverMock.ExpectAndReturn("GetBuild", build, requestWrapper);

			SiteTemplateResults results = siteTemplate.Do();

			AssertEquals(1, results.BuildPluginsList.Length);
			AssertEquals("Test Build Plugin", results.BuildPluginsList[0].InnerHtml);
			AssertEquals("testbuild.aspx", results.BuildPluginsList[0].HRef);
		}

		[Test]
		public void DoesntCreateListOfServerPluginsIfNoProjectSpecified()
		{
			requestWrapperMock.ExpectAndReturn("GetProjectName", "");
			requestWrapperMock.ExpectAndReturn("GetServerName", "server");
			SiteTemplateResults results = siteTemplate.Do();
			AssertEquals(0, results.ServerPluginsList.Length);

			VerifyMocks();
		}

		[Test]
		public void CreatesCorrectListOfServerPluginLinks()
		{
			IPluginSpecification buildPlugin = new AssemblyLoadingPluginSpecification(typeof(TestBuildPlugin).FullName, typeof(TestBuildPlugin).Assembly.CodeBase);
			IPluginSpecification serverPlugin = new AssemblyLoadingPluginSpecification(typeof(TestServerPlugin).FullName, typeof(TestServerPlugin).Assembly.CodeBase);
			IPluginSpecification[] assemblyLoadingPlugins = new IPluginSpecification[] { buildPlugin, serverPlugin };
			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", assemblyLoadingPlugins, PluginsSectionHandler.SectionName);
			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", assemblyLoadingPlugins, PluginsSectionHandler.SectionName);
			configurationGetterMock.ExpectAndReturn("GetConfigFromSection", assemblyLoadingPlugins, PluginsSectionHandler.SectionName);
			requestWrapperMock.ExpectAndReturn("GetProjectName", project);
			requestWrapperMock.ExpectAndReturn("GetServerName", server);
			buildRetrieverMock.ExpectAndReturn("GetBuild", build, requestWrapper);

			SiteTemplateResults results = siteTemplate.Do();

			AssertEquals(1, results.ServerPluginsList.Length);
			AssertEquals("Test Server Plugin", results.ServerPluginsList[0].InnerHtml);
			AssertEquals("testserver.aspx",results.ServerPluginsList[0].HRef);
		}
	}

	public class TestBuildPlugin : IBuildPlugin
	{
		string description = "Test Build Plugin";
		string url = "testbuild.aspx";

		public string Description
		{
			get { return description; }
		}

		public string CreateURL (string serverName, string projectName, string buildName, IBuildUrlGenerator urlGenerator)
		{
			return url;
		}
	}

	public class TestServerPlugin : IServerPlugin
	{
		string description = "Test Server Plugin";
		string url = "testserver.aspx";

		public string Description
		{
			get { return description; }
		}

		public string CreateURL (string serverName, IServerUrlGenerator urlGenerator)
		{
			return url;
		}
	}
}
