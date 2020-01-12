using System;
using System.Collections;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.Configuration;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;


namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.ProjectReport
{
	[TestFixture]
	public class ProjectReportProjectPluginTest
	{
		private Mock<IFarmService> farmServiceMock;
		private Mock<IVelocityViewGenerator> viewGeneratorMock;
		private Mock<ILinkFactory> linkFactoryMock;
		private ProjectReportProjectPlugin plugin;
		private Mock<ICruiseRequest> cruiseRequestMock;
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

			farmServiceMock = new Mock<IFarmService>();
            farmServiceMock.Setup(service => service.GetProjectStatusListAndCaptureExceptions(It.IsAny<IServerSpecifier>(), It.IsAny<string>())).Returns(statusList);
			viewGeneratorMock = new Mock<IVelocityViewGenerator>();
			linkFactoryMock = new Mock<ILinkFactory>();
            ServerLocation serverConfig = new ServerLocation();
            serverConfig.ServerName = "myServer";
            configuration.Servers = new ServerLocation[] {
                serverConfig
            };
            var urlBuilderMock = new Mock<ICruiseUrlBuilder>();
            urlBuilderMock.Setup(builder => builder.BuildProjectUrl(It.IsAny<string>(), It.IsAny <IProjectSpecifier>())).Returns(string.Empty);

			plugin = new ProjectReportProjectPlugin((IFarmService) farmServiceMock.Object,
				(IVelocityViewGenerator) viewGeneratorMock.Object,
				(ILinkFactory) linkFactoryMock.Object,
                configuration,
                (ICruiseUrlBuilder)urlBuilderMock.Object);

			cruiseRequestMock = new Mock<ICruiseRequest>();
			cruiseRequest = (ICruiseRequest ) cruiseRequestMock.Object;

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

            var requestStub = new Mock<IRequest>();
            IRequest request = (IRequest)requestStub.Object;

            cruiseRequestMock.SetupGet(r => r.Request).Returns(request);
            requestStub.SetupGet(r => r.ApplicationPath).Returns("myAppPath");
            
            farmServiceMock.Setup(service => service.GetRSSFeed(projectSpecifier, It.IsAny<string>())).Returns("").Verifiable();
            
			expectedContext["projectName"] = "myProject";
			expectedContext["externalLinks"] = links;
			expectedContext["noLogsAvailable"] = false;
			expectedContext["mostRecentBuildUrl"] = "buildUrl";

            expectedContext["applicationPath"] = "myAppPath";
            expectedContext["rssDataPresent"] = false;
            expectedContext["rss"] = new GeneralAbsoluteLink("RSS", @"http://localhost/myServer");


			HtmlFragmentResponse response = new HtmlFragmentResponse("myView");

            cruiseRequestMock.SetupGet(r => r.ProjectSpecifier).Returns(projectSpecifier).Verifiable();
			farmServiceMock.Setup(service => service.GetMostRecentBuildSpecifiers(projectSpecifier, 1, null)).Returns(new IBuildSpecifier[] { buildSpecifier }).Verifiable();
			farmServiceMock.Setup(service => service.GetExternalLinks(projectSpecifier, null)).Returns(links).Verifiable();
			linkFactoryMock.Setup(factory => factory.CreateProjectLink(projectSpecifier, LatestBuildReportProjectPlugin.ACTION_NAME)).Returns(new GeneralAbsoluteLink("foo", "buildUrl")).Verifiable();
            linkFactoryMock.Setup(factory => factory.CreateProjectLink(projectSpecifier, ThoughtWorks.CruiseControl.WebDashboard.Plugins.RSS.RSSFeed.ACTION_NAME)).Returns(new GeneralAbsoluteLink("RSS", @"myServer")).Verifiable();


            farmServiceMock.Setup(service => service.GetMostRecentBuildSpecifiers(projectSpecifier, ProjectReportProjectPlugin.AmountOfBuildsToRetrieve, null)).Returns(new IBuildSpecifier[] { buildSpecifier }).Verifiable();
            expectedContext["graphDayInfo"] = new ArrayList();
            
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

			viewGeneratorMock.Setup(generator => generator.GenerateView(@"ProjectReport.vm", It.IsAny<Hashtable>())).
				Callback<string, Hashtable>((name, context) => Assert.AreEqual(context, expectedContext)).Returns(response).Verifiable();


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

            var requestStub = new Mock<IRequest>();
            IRequest request = (IRequest)requestStub.Object;
            
            expectedContext["projectName"] = "myProject";
			expectedContext["externalLinks"] = links;
			expectedContext["noLogsAvailable"] = true;

            expectedContext["applicationPath"] = "myAppPath";
            expectedContext["rssDataPresent"] = false;
            expectedContext["rss"] = new GeneralAbsoluteLink("RSS", @"http://localhost/myServer");
        
            cruiseRequestMock.SetupGet(r => r.Request).Returns(request);
            requestStub.SetupGet(r => r.ApplicationPath).Returns("myAppPath");

            HtmlFragmentResponse response = new HtmlFragmentResponse("myView");

            
            IProjectSpecifier projectSpecifier = new DefaultProjectSpecifier(new DefaultServerSpecifier("myServer"), "myProject");
            cruiseRequestMock.SetupGet(r => r.ProjectSpecifier).Returns(projectSpecifier).Verifiable();
			farmServiceMock.Setup(service => service.GetMostRecentBuildSpecifiers(projectSpecifier, 1, null)).Returns(new IBuildSpecifier[0]).Verifiable();
			farmServiceMock.Setup(service => service.GetExternalLinks(projectSpecifier, null)).Returns(links).Verifiable();

            farmServiceMock.Setup(service => service.GetRSSFeed(projectSpecifier, It.IsAny<string>())).Returns("").Verifiable();
            linkFactoryMock.Setup(factory => factory.CreateProjectLink(projectSpecifier, ThoughtWorks.CruiseControl.WebDashboard.Plugins.RSS.RSSFeed.ACTION_NAME)).Returns(new GeneralAbsoluteLink("RSS", @"myServer")).Verifiable();


            farmServiceMock.Setup(service => service.GetMostRecentBuildSpecifiers(projectSpecifier, ProjectReportProjectPlugin.AmountOfBuildsToRetrieve, null)).Returns(new IBuildSpecifier[0]).Verifiable();
            expectedContext["graphDayInfo"] = new ArrayList();

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

            viewGeneratorMock.Setup(generator => generator.GenerateView(@"ProjectReport.vm", It.IsAny<Hashtable>())).
                Callback<string, Hashtable>((name, context) => Assert.AreEqual(context, expectedContext)).Returns(response).Verifiable();

            
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

            var requestStub = new Mock<IRequest>();
            IRequest request = (IRequest)requestStub.Object;


			expectedContext["projectName"] = "myProject";
			expectedContext["externalLinks"] = links;
			expectedContext["noLogsAvailable"] = false;
			expectedContext["mostRecentBuildUrl"] = "buildUrl";
			expectedContext["pluginInfo"] = "test";

            expectedContext["applicationPath"] = "myAppPath";
            expectedContext["rssDataPresent"] = false;
            expectedContext["rss"] = new GeneralAbsoluteLink("RSS", @"http://localhost/myServer");
            cruiseRequestMock.SetupGet(r => r.Request).Returns(request);
            requestStub.SetupGet(r => r.ApplicationPath).Returns("myAppPath");



			HtmlFragmentResponse response = new HtmlFragmentResponse("myView");

            cruiseRequestMock.SetupGet(r => r.ProjectSpecifier).Returns(projectSpecifier).Verifiable();
			farmServiceMock.Setup(service => service.GetMostRecentBuildSpecifiers(projectSpecifier, 1, null)).Returns(new IBuildSpecifier[] { buildSpecifier }).Verifiable();
			farmServiceMock.Setup(service => service.GetExternalLinks(projectSpecifier, null)).Returns(links);
			linkFactoryMock.Setup(factory => factory.CreateProjectLink(projectSpecifier, LatestBuildReportProjectPlugin.ACTION_NAME)).Returns(new GeneralAbsoluteLink("foo", "buildUrl")).Verifiable();

            farmServiceMock.Setup(service => service.GetRSSFeed(projectSpecifier, It.IsAny<string>())).Returns("").Verifiable();
            linkFactoryMock.Setup(factory => factory.CreateProjectLink(projectSpecifier, ThoughtWorks.CruiseControl.WebDashboard.Plugins.RSS.RSSFeed.ACTION_NAME)).Returns(new GeneralAbsoluteLink("RSS", @"myServer")).Verifiable();

            farmServiceMock.Setup(service => service.GetMostRecentBuildSpecifiers(projectSpecifier, ProjectReportProjectPlugin.AmountOfBuildsToRetrieve, null)).Returns(new IBuildSpecifier[] { buildSpecifier }).Verifiable();
            expectedContext["graphDayInfo"] = new ArrayList();

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

			viewGeneratorMock.Setup(generator => generator.GenerateView(@"ProjectReport.vm", It.IsAny<Hashtable>())).
				Callback<string, Hashtable>((name, context) => Assert.AreEqual(context, expectedContext)).Returns(response).Verifiable();
                       
            
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
