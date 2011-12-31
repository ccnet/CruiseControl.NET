using System;
using System.Collections;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;
using ThoughtWorks.CruiseControl.WebDashboard.Configuration;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.Statistics;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;


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
            ProjectStatusOnServer server = new ProjectStatusOnServer(new ProjectStatus("myProject", IntegrationStatus.Success, DateTime.Now),
                new DefaultServerSpecifier("myServer"));
            ProjectStatusListAndExceptions statusList = new ProjectStatusListAndExceptions(
                new ProjectStatusOnServer[] {
                    server
                }, new CruiseServerException[] {
                });

			farmServiceMock = new DynamicMock(typeof(IFarmService));
            farmServiceMock.SetupResult("GetProjectStatusListAndCaptureExceptions", statusList, typeof(IServerSpecifier), typeof(string));
			viewGeneratorMock = new DynamicMock(typeof(IVelocityViewGenerator));
			linkFactoryMock = new DynamicMock(typeof(ILinkFactory));
            ServerLocation serverConfig = new ServerLocation();
            serverConfig.ServerName = "myServer";
            configuration.Servers = new ServerLocation[] {
                serverConfig
            };
            var urlBuilderMock = new DynamicMock(typeof(ICruiseUrlBuilder));
            urlBuilderMock.SetupResult("BuildProjectUrl", string.Empty, typeof(string), typeof(IProjectSpecifier));

			plugin = new ProjectReportProjectPlugin((IFarmService) farmServiceMock.MockInstance,
				(IVelocityViewGenerator) viewGeneratorMock.MockInstance,
				(ILinkFactory) linkFactoryMock.MockInstance,
                configuration,
                (ICruiseUrlBuilder)urlBuilderMock.MockInstance);

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
		[Ignore("Disable until it is fixed on non en_US systems!")]
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
            cruiseRequestMock.ExpectAndReturn("ProjectSpecifier", projectSpecifier);
			farmServiceMock.ExpectAndReturn("GetMostRecentBuildSpecifiers", new IBuildSpecifier[] { buildSpecifier }, projectSpecifier, 1, null);
			farmServiceMock.ExpectAndReturn("GetExternalLinks", links, projectSpecifier, null);
			linkFactoryMock.ExpectAndReturn("CreateProjectLink", new GeneralAbsoluteLink("foo", "buildUrl"), projectSpecifier, LatestBuildReportProjectPlugin.ACTION_NAME);

            linkFactoryMock.ExpectAndReturn("CreateProjectLink", new GeneralAbsoluteLink("RSS", @"myServer"), projectSpecifier, ThoughtWorks.CruiseControl.WebDashboard.Plugins.RSS.RSSFeed.ACTION_NAME);


            farmServiceMock.ExpectAndReturn("GetMostRecentBuildSpecifiers", new IBuildSpecifier[] { buildSpecifier }, projectSpecifier, ProjectReportProjectPlugin.AmountOfBuildsToRetrieve, null);
            expectedContext["graphDayInfo"] = new NMock.Constraints.IsTypeOf(typeof(ArrayList));
            
            expectedContext["highestAmountPerDay"] = 1;
            expectedContext["dateMultiPlier"] = 1;

            expectedContext["OKPercent"] = 100;
            expectedContext["NOKPercent"] = 0;

            expectedContext["server"] = new DefaultServerSpecifier("myServer");
            expectedContext["StatusMessage"] = string.Empty;
            expectedContext["status"] = null;
            expectedContext["StartStopButtonName"] = "StartBuild";
            expectedContext["StartStopButtonValue"] = "Start";
            expectedContext["ForceAbortBuildButtonName"] = "ForceBuild";
            expectedContext["ForceAbortBuildButtonValue"] = "Force";

			viewGeneratorMock.ExpectAndReturn("GenerateView", response, @"ProjectReport.vm", new HashtableConstraint(expectedContext));


			// Execute
			IResponse returnedResponse = plugin.Execute(cruiseRequest);

			// Verify
			Assert.AreEqual(response, returnedResponse);
			VerifyAll();
		}

		[Test]
		[Ignore("Disable until it is fixed on non en_US systems!")]
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
            cruiseRequestMock.ExpectAndReturn("ProjectSpecifier", projectSpecifier);
			farmServiceMock.ExpectAndReturn("GetMostRecentBuildSpecifiers", new IBuildSpecifier[0], projectSpecifier, 1, null);
			farmServiceMock.ExpectAndReturn("GetExternalLinks", links, projectSpecifier, null);

            farmServiceMock.ExpectAndReturn("GetRSSFeed", "", projectSpecifier);
            linkFactoryMock.ExpectAndReturn("CreateProjectLink", new GeneralAbsoluteLink("RSS", @"myServer"), projectSpecifier, ThoughtWorks.CruiseControl.WebDashboard.Plugins.RSS.RSSFeed.ACTION_NAME);


            farmServiceMock.ExpectAndReturn("GetMostRecentBuildSpecifiers", new IBuildSpecifier[0], projectSpecifier, ProjectReportProjectPlugin.AmountOfBuildsToRetrieve, null);
            expectedContext["graphDayInfo"] = new NMock.Constraints.IsTypeOf(typeof(ArrayList));

            expectedContext["highestAmountPerDay"] = 1;
            expectedContext["dateMultiPlier"] = 1;

            expectedContext["OKPercent"] = 100;
            expectedContext["NOKPercent"] = 0;

            expectedContext["server"] = new DefaultServerSpecifier("myServer");
            expectedContext["StatusMessage"] = string.Empty;
            expectedContext["status"] = null;
            expectedContext["StartStopButtonName"] = "StartBuild";
            expectedContext["StartStopButtonValue"] = "Start";
            expectedContext["ForceAbortBuildButtonName"] = "ForceBuild";
            expectedContext["ForceAbortBuildButtonValue"] = "Force";

            viewGeneratorMock.ExpectAndReturn("GenerateView", response, @"ProjectReport.vm", new HashtableConstraint(expectedContext));

            
            // Execute
			IResponse returnedResponse = plugin.Execute(cruiseRequest);

			// Verify
			Assert.AreEqual(response, returnedResponse);
			VerifyAll();
		}

		[Test]
		[Ignore("Disable until it is fixed on non en_US systems!")]
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
            cruiseRequestMock.ExpectAndReturn("ProjectSpecifier", projectSpecifier);
			farmServiceMock.ExpectAndReturn("GetMostRecentBuildSpecifiers", new IBuildSpecifier[] { buildSpecifier }, projectSpecifier, 1, null);
			farmServiceMock.ExpectAndReturn("GetExternalLinks", links, projectSpecifier, null);
			linkFactoryMock.ExpectAndReturn("CreateProjectLink", new GeneralAbsoluteLink("foo", "buildUrl"), projectSpecifier, LatestBuildReportProjectPlugin.ACTION_NAME);

            farmServiceMock.ExpectAndReturn("GetRSSFeed", "", projectSpecifier);
            linkFactoryMock.ExpectAndReturn("CreateProjectLink", new GeneralAbsoluteLink("RSS", @"myServer"), projectSpecifier, ThoughtWorks.CruiseControl.WebDashboard.Plugins.RSS.RSSFeed.ACTION_NAME);

            farmServiceMock.ExpectAndReturn("GetMostRecentBuildSpecifiers", new IBuildSpecifier[] { buildSpecifier }, projectSpecifier, ProjectReportProjectPlugin.AmountOfBuildsToRetrieve, null);
            expectedContext["graphDayInfo"] = new NMock.Constraints.IsTypeOf(typeof(ArrayList));

            expectedContext["highestAmountPerDay"] = 1;
            expectedContext["dateMultiPlier"] = 1;

            expectedContext["OKPercent"] = 100;
            expectedContext["NOKPercent"] = 0;

            expectedContext["server"] = new DefaultServerSpecifier("myServer");
            expectedContext["StatusMessage"] = string.Empty;
            expectedContext["status"] = null;
            expectedContext["StartStopButtonName"] = "StartBuild";
            expectedContext["StartStopButtonValue"] = "Start";
            expectedContext["ForceAbortBuildButtonName"] = "ForceBuild";
            expectedContext["ForceAbortBuildButtonValue"] = "Force";

            viewGeneratorMock.ExpectAndReturn("GenerateView", response, @"ProjectReport.vm", new HashtableConstraint(expectedContext));
                       
            
            // Execute
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
			Assert.That(response, Is.InstanceOf<HtmlFragmentResponse>(), "Response is not HTML");

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
