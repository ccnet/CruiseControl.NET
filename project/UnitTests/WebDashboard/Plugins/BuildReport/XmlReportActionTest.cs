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

			Assert.AreEqual("<Projects />", xml);

			mockFarmService.Verify();
		}

		[Test]
		public void WhenOneProjectStatusIsReturnedThisIsContainedInTheReturnedXml()
		{
			ProjectStatus projectStatus = CreateProjectStatus();

			ProjectStatusOnServer projectStatusOnServer = new ProjectStatusOnServer(projectStatus, null);
			mockFarmService.ExpectAndReturn("GetProjectStatusListAndCaptureExceptions",
			                                new ProjectStatusListAndExceptions(
			                                	new ProjectStatusOnServer[] {projectStatusOnServer}, new CruiseServerException[0]));

			XmlFragmentResponse response = (XmlFragmentResponse) reportAction.Execute(null);
			string xml = response.ResponseFragment;

			// cannot just compare the xml string, since we correctly expect the string to vary based on the
			// timezone in which this code is executing
			XmlDocument doc = LoadAsDocument(xml);

			AssertXPathMatches(doc, "/Projects/Project/@name", "HelloWorld");
			AssertXPathMatches(doc, "/Projects/Project/@activity", "Sleeping");
			AssertXPathMatches(doc, "/Projects/Project/@lastBuildStatus", "Success");
			AssertXPathMatches(doc, "/Projects/Project/@lastBuildLabel", "build_7");
			AssertXPathMatches(doc, "/Projects/Project/@lastBuildTime", XmlConvert.ToString(LastBuildTime));
			AssertXPathMatches(doc, "/Projects/Project/@nextBuildTime", XmlConvert.ToString(NextBuildTime));
			AssertXPathMatches(doc, "/Projects/Project/@webUrl", "http://blah");
			AssertXPathMatches(doc, "/Projects/Project/@category", "category");

			mockFarmService.Verify();
		}

		private void AssertXPathMatches(XmlDocument doc, string xpath, string expectedValue)
		{
			XmlNode node = doc.SelectSingleNode(xpath);
			Assert.IsNotNull(node, "Expected to find match for xpath " + xpath);

			Assert.AreEqual(node.InnerText, expectedValue, "Unexpected value for xpath " + xpath);
		}

		private XmlDocument LoadAsDocument(string xml)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);
			return doc;
		}

		[Test]
		public void ReturnedXmlValidatesAgainstSchema()
		{
			ProjectStatus projectStatus = CreateProjectStatus();

			ProjectStatusOnServer projectStatusOnServer = new ProjectStatusOnServer(projectStatus, null);
			mockFarmService.ExpectAndReturn("GetProjectStatusListAndCaptureExceptions",
			                                new ProjectStatusListAndExceptions(
			                                	new ProjectStatusOnServer[] {projectStatusOnServer}, new CruiseServerException[0]));

			XmlFragmentResponse response = (XmlFragmentResponse) reportAction.Execute(null);
			string xml = response.ResponseFragment;

			XmlValidatingReader rdr = new XmlValidatingReader(xml, XmlNodeType.Document, null);
			rdr.Schemas.Add(ReadSchemaFromResources("XmlReportActionSchema.xsd"));
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
				                  NextBuildTime);
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