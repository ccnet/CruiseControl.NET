using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.FarmReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.Configuration;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.BuildReport
{
	[TestFixture]
	public class XmlReportActionTest
	{
		private DynamicMock mockFarmService;
		private XmlReportAction reportAction;

		private readonly DateTime LastBuildTime = new DateTime(2005, 7, 1, 12, 12, 12);
		private readonly DateTime NextBuildTime = new DateTime(2005, 7, 2, 13, 13, 13);

		[SetUp]
		public void SetUp()
		{
			mockFarmService = new DynamicMock(typeof (IFarmService));
			reportAction = new XmlReportAction((IFarmService) mockFarmService.MockInstance);
		}

		[Test]
		public void ReturnsAXmlResponse()
		{
			mockFarmService.ExpectAndReturn("GetProjectStatusListAndCaptureExceptions",
			                                new ProjectStatusListAndExceptions(
			                                	new ProjectStatusOnServer[0], new CruiseServerException[0]));

			IResponse response = reportAction.Execute(null);
			Assert.IsNotNull(response);
			Assert.AreEqual(typeof (XmlFragmentResponse), response.GetType());

			mockFarmService.Verify();
		}

		[Test]
		public void WhenNoProjectStatusEntriesAreReturnedByTheFarmServiceTheXmlContainsJustASingleRootNode()
		{
			mockFarmService.ExpectAndReturn("GetProjectStatusListAndCaptureExceptions",
			                                new ProjectStatusListAndExceptions(
			                                	new ProjectStatusOnServer[0], new CruiseServerException[0]));
			XmlFragmentResponse response = (XmlFragmentResponse) reportAction.Execute(null);
			string xml = response.ResponseFragment;

			Assert.AreEqual("<Projects CCType=\"CCNet\" />", xml);

			mockFarmService.Verify();
		}

		[Test]
		public void WhenOneProjectStatusIsReturnedThisIsContainedInTheReturnedXml()
		{
			ProjectStatus projectStatus = CreateProjectStatus();
            ServerLocation ServerSpecifier = new ServerLocation();
            ServerSpecifier.ServerName = "localhost";

            ProjectStatusOnServer projectStatusOnServer = new ProjectStatusOnServer(projectStatus, ServerSpecifier);
			mockFarmService.ExpectAndReturn("GetProjectStatusListAndCaptureExceptions",
			                                new ProjectStatusListAndExceptions(
			                                	new ProjectStatusOnServer[] {projectStatusOnServer}, new CruiseServerException[0]));

			XmlFragmentResponse response = (XmlFragmentResponse) reportAction.Execute(null);
			string xml = response.ResponseFragment;
			XmlDocument doc = XPathAssert.LoadAsDocument(xml);

            XPathAssert.Matches(doc, "/Projects/Project/@name", "HelloWorld");
            XPathAssert.Matches(doc, "/Projects/Project/@activity", "Sleeping");
            XPathAssert.Matches(doc, "/Projects/Project/@lastBuildStatus", "Success");
            XPathAssert.Matches(doc, "/Projects/Project/@lastBuildLabel", "build_7");
            XPathAssert.Matches(doc, "/Projects/Project/@lastBuildTime", LastBuildTime);
            XPathAssert.Matches(doc, "/Projects/Project/@nextBuildTime", NextBuildTime);
            XPathAssert.Matches(doc, "/Projects/Project/@webUrl", "http://blah");
            XPathAssert.Matches(doc, "/Projects/Project/@category", "category");

			mockFarmService.Verify();
		}

		[Test]
		public void ReturnedXmlValidatesAgainstSchema()
		{
			ProjectStatus projectStatus = CreateProjectStatus();
            ServerLocation ServerSpecifier = new ServerLocation();
            ServerSpecifier.ServerName = "localhost";


            ProjectStatusOnServer projectStatusOnServer = new ProjectStatusOnServer(projectStatus, ServerSpecifier);
			mockFarmService.ExpectAndReturn("GetProjectStatusListAndCaptureExceptions",
			                                new ProjectStatusListAndExceptions(
			                                	new ProjectStatusOnServer[] {projectStatusOnServer}, new CruiseServerException[0]));

			XmlFragmentResponse response = (XmlFragmentResponse) reportAction.Execute(null);
			string xml = response.ResponseFragment;

		    XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
		    xmlReaderSettings.Schemas.Add(ReadSchemaFromResources("XmlReportActionSchema.xsd"));
		    XmlReader rdr = XmlReader.Create(new StringReader(xml), xmlReaderSettings);
			while (rdr.Read())
			{
			}

			mockFarmService.Verify();
		}

		private ProjectStatus CreateProjectStatus()
		{
			return
				new ProjectStatus("HelloWorld", "category", ProjectActivity.Sleeping, IntegrationStatus.Success, ProjectIntegratorState.Running,
				                  "http://blah", LastBuildTime, "build_8", "build_7",
                                  NextBuildTime, "", "", 0);
		}

		private XmlSchema ReadSchemaFromResources(string filename)
		{
			using (Stream s = ResourceUtil.LoadResource(GetType(), filename))
			{
				return XmlSchema.Read(s, null);
			}
		}
	}
}