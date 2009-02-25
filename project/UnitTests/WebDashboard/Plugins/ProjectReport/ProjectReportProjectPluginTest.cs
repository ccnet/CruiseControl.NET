using System.Collections;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.Statistics;
using System;
using ThoughtWorks.CruiseControl.WebDashboard.Configuration; 


namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.ProjectReport
{
	[TestFixture]
	public class ProjectReportProjectPluginTest
	{
		private DynamicMock farmServiceMock;
		private DynamicMock viewGeneratorMock;
		private DynamicMock linkFactoryMock;
		private ProjectReportProjectPlugin plugin;
		private DynamicMock cruiseRequestMock;
		private ICruiseRequest cruiseRequest;
        private NetReflectorRemoteServicesConfiguration configuration = new NetReflectorRemoteServicesConfiguration();

		[SetUp]
		public void Setup()
		{
			farmServiceMock = new DynamicMock(typeof(IFarmService));
			viewGeneratorMock = new DynamicMock(typeof(IVelocityViewGenerator));
			linkFactoryMock = new DynamicMock(typeof(ILinkFactory));
            ServerLocation serverConfig = new ServerLocation();
            serverConfig.ServerName = "myServer";
            configuration.Servers = new ServerLocation[] {
                serverConfig
            };

			plugin = new ProjectReportProjectPlugin((IFarmService) farmServiceMock.MockInstance,
				(IVelocityViewGenerator) viewGeneratorMock.MockInstance,
				(ILinkFactory) linkFactoryMock.MockInstance,
                configuration);

			cruiseRequestMock = new DynamicMock(typeof(ICruiseRequest));
			cruiseRequest = (ICruiseRequest ) cruiseRequestMock.MockInstance;

		}

		private void VerifyAll()
		{
			farmServiceMock.Verify();
			viewGeneratorMock.Verify();
			linkFactoryMock.Verify();
		}

		[Test]
		public void ShouldGetProjectDetailsAndUseCorrectTemplate()
		{
			// Setup
			ExternalLink[] links = new ExternalLink[] { new ExternalLink("foo", "bar") };
			IProjectSpecifier projectSpecifier = new DefaultProjectSpecifier(new DefaultServerSpecifier("myServer"), "myProject");
			//IBuildSpecifier buildSpecifier = new DefaultBuildSpecifier(projectSpecifier, "myBuild");
            IBuildSpecifier buildSpecifier = new DefaultBuildSpecifier(projectSpecifier, "log20050818103522Lbuild.0.0.0.9.xml");

			Hashtable expectedContext = new Hashtable();

            DynamicMock requestStub = new DynamicMock(typeof(IRequest));
            IRequest request = (IRequest)requestStub.MockInstance;

            cruiseRequestMock.SetupResult("Request", request);
            requestStub.SetupResult("ApplicationPath", "myAppPath");
            
            farmServiceMock.ExpectAndReturn("GetRSSFeed", "", projectSpecifier);
            
			expectedContext["projectName"] = "myProject";
			expectedContext["externalLinks"] = links;
			expectedContext["noLogsAvailable"] = false;
			expectedContext["mostRecentBuildUrl"] = "buildUrl";

            expectedContext["applicationPath"] = "myAppPath";
            expectedContext["rssDataPresent"] = false;
            expectedContext["rss"] = new GeneralAbsoluteLink("RSS", @"http://localhost/myServer");


			IResponse response = new HtmlFragmentResponse("myView");

			cruiseRequestMock.ExpectAndReturn("ProjectSpecifier", projectSpecifier);
			farmServiceMock.ExpectAndReturn("GetMostRecentBuildSpecifiers", new IBuildSpecifier[] { buildSpecifier }, projectSpecifier, 1);
			farmServiceMock.ExpectAndReturn("GetExternalLinks", links, projectSpecifier);
			linkFactoryMock.ExpectAndReturn("CreateProjectLink", new GeneralAbsoluteLink("foo", "buildUrl"), projectSpecifier, LatestBuildReportProjectPlugin.ACTION_NAME);

            linkFactoryMock.ExpectAndReturn("CreateProjectLink", new GeneralAbsoluteLink("RSS", @"myServer"), projectSpecifier, ThoughtWorks.CruiseControl.WebDashboard.Plugins.RSS.RSSFeed.ACTION_NAME);


            
            farmServiceMock.ExpectAndReturn("GetMostRecentBuildSpecifiers", new IBuildSpecifier[] { buildSpecifier }, projectSpecifier, ProjectReportProjectPlugin.AmountOfBuildsToRetrieve);            
            expectedContext["graphDayInfo"] = new NMock.Constraints.IsTypeOf(typeof(ArrayList));
            
            expectedContext["highestAmountPerDay"] = 1;
            expectedContext["dateMultiPlier"] = 1;

            expectedContext["OKPercent"] = 100;
            expectedContext["NOKPercent"] = 0;
                        



			viewGeneratorMock.ExpectAndReturn("GenerateView", response, @"ProjectReport.vm", new HashtableConstraint(expectedContext));


			// Execute
			plugin.DashPlugins = null;
			IResponse returnedResponse = plugin.Execute(cruiseRequest);

			// Verify
			Assert.AreEqual(response, returnedResponse);
			VerifyAll();
		}

		[Test]
		public void ShouldMarkNoBuildsAvailableIfNoBuildSpecifiersReturnedByRemoteServer()
		{
			// Setup
			ExternalLink[] links = new ExternalLink[] { new ExternalLink("foo", "bar") };
			Hashtable expectedContext = new Hashtable();

            DynamicMock requestStub = new DynamicMock(typeof(IRequest));
            IRequest request = (IRequest)requestStub.MockInstance;
            
            expectedContext["projectName"] = "myProject";
			expectedContext["externalLinks"] = links;
			expectedContext["noLogsAvailable"] = true;

            expectedContext["applicationPath"] = "myAppPath";
            expectedContext["rssDataPresent"] = false;
            expectedContext["rss"] = new GeneralAbsoluteLink("RSS", @"http://localhost/myServer");
        
            cruiseRequestMock.SetupResult("Request", request);
            requestStub.SetupResult("ApplicationPath", "myAppPath");

            IResponse response = new HtmlFragmentResponse("myView");


            
            IProjectSpecifier projectSpecifier = new DefaultProjectSpecifier(new DefaultServerSpecifier("myServer"), "myProject");
			cruiseRequestMock.ExpectAndReturn("ProjectSpecifier", projectSpecifier);
			farmServiceMock.ExpectAndReturn("GetMostRecentBuildSpecifiers", new IBuildSpecifier[0], projectSpecifier, 1);
			farmServiceMock.ExpectAndReturn("GetExternalLinks", links, projectSpecifier);

            farmServiceMock.ExpectAndReturn("GetRSSFeed", "", projectSpecifier);
            linkFactoryMock.ExpectAndReturn("CreateProjectLink", new GeneralAbsoluteLink("RSS", @"myServer"), projectSpecifier, ThoughtWorks.CruiseControl.WebDashboard.Plugins.RSS.RSSFeed.ACTION_NAME);


            farmServiceMock.ExpectAndReturn("GetMostRecentBuildSpecifiers", new IBuildSpecifier[0], projectSpecifier, ProjectReportProjectPlugin.AmountOfBuildsToRetrieve);
            expectedContext["graphDayInfo"] = new NMock.Constraints.IsTypeOf(typeof(ArrayList));

            expectedContext["highestAmountPerDay"] = 1;
            expectedContext["dateMultiPlier"] = 1;

            expectedContext["OKPercent"] = 100;
            expectedContext["NOKPercent"] = 0;

            viewGeneratorMock.ExpectAndReturn("GenerateView", response, @"ProjectReport.vm", new HashtableConstraint(expectedContext));

            
            // Execute
			IResponse returnedResponse = plugin.Execute(cruiseRequest);

			// Verify
			Assert.AreEqual(response, returnedResponse);
			VerifyAll();
		}

		[Test]
		public void ShouldGetProjectDetailsAndUseCorrectTemplateWithSubReportPlugin()
		{
			//Note this 
			// Setup
			ExternalLink[] links = new ExternalLink[] { new ExternalLink("foo", "bar") };
			IProjectSpecifier projectSpecifier = new DefaultProjectSpecifier(new DefaultServerSpecifier("myServer"), "myProject");
            //IBuildSpecifier buildSpecifier = new DefaultBuildSpecifier(projectSpecifier, "myBuild");
            IBuildSpecifier buildSpecifier = new DefaultBuildSpecifier(projectSpecifier, "log20050818103522Lbuild.0.0.0.9.xml");

            Hashtable expectedContext = new Hashtable();

            DynamicMock requestStub = new DynamicMock(typeof(IRequest));
            IRequest request = (IRequest)requestStub.MockInstance;


			expectedContext["projectName"] = "myProject";
			expectedContext["externalLinks"] = links;
			expectedContext["noLogsAvailable"] = false;
			expectedContext["mostRecentBuildUrl"] = "buildUrl";
			expectedContext["pluginInfo"] = "test";

            expectedContext["applicationPath"] = "myAppPath";
            expectedContext["rssDataPresent"] = false;
            expectedContext["rss"] = new GeneralAbsoluteLink("RSS", @"http://localhost/myServer");
            cruiseRequestMock.SetupResult("Request", request);
            requestStub.SetupResult("ApplicationPath", "myAppPath");



			IResponse response = new HtmlFragmentResponse("myView");

			cruiseRequestMock.ExpectAndReturn("ProjectSpecifier", projectSpecifier);
			farmServiceMock.ExpectAndReturn("GetMostRecentBuildSpecifiers", new IBuildSpecifier[] { buildSpecifier }, projectSpecifier, 1);
			farmServiceMock.ExpectAndReturn("GetExternalLinks", links, projectSpecifier);
			linkFactoryMock.ExpectAndReturn("CreateProjectLink", new GeneralAbsoluteLink("foo", "buildUrl"), projectSpecifier, LatestBuildReportProjectPlugin.ACTION_NAME);

            farmServiceMock.ExpectAndReturn("GetRSSFeed", "", projectSpecifier);
            linkFactoryMock.ExpectAndReturn("CreateProjectLink", new GeneralAbsoluteLink("RSS", @"myServer"), projectSpecifier, ThoughtWorks.CruiseControl.WebDashboard.Plugins.RSS.RSSFeed.ACTION_NAME);


            farmServiceMock.ExpectAndReturn("GetMostRecentBuildSpecifiers", new IBuildSpecifier[] { buildSpecifier }, projectSpecifier, ProjectReportProjectPlugin.AmountOfBuildsToRetrieve);
            expectedContext["graphDayInfo"] = new NMock.Constraints.IsTypeOf(typeof(ArrayList));

            expectedContext["highestAmountPerDay"] = 1;
            expectedContext["dateMultiPlier"] = 1;

            expectedContext["OKPercent"] = 100;
            expectedContext["NOKPercent"] = 0;


            viewGeneratorMock.ExpectAndReturn("GenerateView", response, @"ProjectReport.vm", new HashtableConstraint(expectedContext));
                       
            
            // Execute
			plugin.DashPlugins = new IBuildPlugin[1] {new TestPlugin()};
			Assert.IsNotNull(plugin.DashPlugins, "DashPlugins are null");
			Assert.IsInstanceOfType(typeof(IBuildPlugin[]), plugin.DashPlugins);
			IResponse returnedResponse = plugin.Execute(cruiseRequest);

			// Verify
			Assert.AreEqual(response, returnedResponse);
			VerifyAll();
		}
		[Test]
		public void TestMockPluginResponse()
		{
			IBuildPlugin plugIn = new TestPlugin();
			IResponse response =  plugIn.NamedActions[0].Action.Execute(cruiseRequest);
			
			Assert.IsNotNull(response, "Response is null");
			Assert.IsInstanceOfType(typeof(HtmlFragmentResponse), response, "Response is not HTML");

			Assert.AreEqual(new HtmlFragmentResponse("test").ResponseFragment, 
							((HtmlFragmentResponse)response).ResponseFragment, "Responses are not equal");
		}
	}

	
	internal class TestPlugin:IBuildPlugin
	{
		#region IBuildPlugin Members

		public bool IsDisplayedForProject(IProjectSpecifier project)
		{
			return true;
		}

		#endregion

		#region IPlugin Members

		public string LinkDescription
		{
			get
			{
				return "Test Plugin";
			}
		}

		public INamedAction[] NamedActions
		{
			get
			{
				ConfigurableNamedAction act = new ConfigurableNamedAction();
				act.Action = new TestNamedAction();
				return new INamedAction[1] {act};
			}
		}

		#endregion

	}

	internal class TestNamedAction:ICruiseAction
	{
		public IResponse Execute(ICruiseRequest cruiseRequest)
		{
			return new HtmlFragmentResponse("test");
		}
	}

}
