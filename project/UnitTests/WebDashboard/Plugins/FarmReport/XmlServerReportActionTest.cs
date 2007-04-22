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

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.FarmReport
{
	[TestFixture]
	public class XmlServerReportActionTest
	{
		private DynamicMock mockFarmService;
        private XmlServerReportAction reportAction;

		private readonly DateTime LastBuildTime = new DateTime(2005, 7, 1, 12, 12, 12);
		private readonly DateTime NextBuildTime = new DateTime(2005, 7, 2, 13, 13, 13);

		[SetUp]
		public void SetUp()
		{
			mockFarmService = new DynamicMock(typeof (IFarmService));
            reportAction = new XmlServerReportAction((IFarmService)mockFarmService.MockInstance);
		}

		[Test]
		public void ReturnsAnXmlResponse()
		{
            mockFarmService.ExpectAndReturn("GetCruiseServerSnapshotListAndExceptions",
			                                new CruiseServerSnapshotListAndExceptions(
			                                	new CruiseServerSnapshotOnServer[0], new CruiseServerException[0]));

			IResponse response = reportAction.Execute(null);
			Assert.IsNotNull(response);
			Assert.AreEqual(typeof (XmlFragmentResponse), response.GetType());

			mockFarmService.Verify();
		}

		[Test]
		public void WhenNoCruiseServerSnapshotEntriesAreReturnedByTheFarmServiceTheXmlContainsJustRootNodes()
		{
            mockFarmService.ExpectAndReturn("GetCruiseServerSnapshotListAndExceptions",
			                                new CruiseServerSnapshotListAndExceptions(
			                                	new CruiseServerSnapshotOnServer[0], new CruiseServerException[0]));
			XmlFragmentResponse response = (XmlFragmentResponse) reportAction.Execute(null);
			string xml = response.ResponseFragment;

            Assert.AreEqual("<CruiseControl><Projects /><Queues /></CruiseControl>", xml);

			mockFarmService.Verify();
		}

		[Test]
		public void WhenOneCruiseServerSnapshotIsReturnedThisIsContainedInTheReturnedXml()
		{
			CruiseServerSnapshot cruiseServerSnapshot = CreateCruiseServerSnapshot();

			CruiseServerSnapshotOnServer cruiseServerSnapshotOnServer = new CruiseServerSnapshotOnServer(cruiseServerSnapshot, null);
            mockFarmService.ExpectAndReturn("GetCruiseServerSnapshotListAndExceptions",
			                                new CruiseServerSnapshotListAndExceptions(
			                                	new CruiseServerSnapshotOnServer[] {cruiseServerSnapshotOnServer}, new CruiseServerException[0]));

			XmlFragmentResponse response = (XmlFragmentResponse) reportAction.Execute(null);
			string xml = response.ResponseFragment;

			// cannot just compare the xml string, since we correctly expect the string to vary based on the
			// timezone in which this code is executing
			XmlDocument doc = LoadAsDocument(xml);

			AssertXPathMatches(doc, "/CruiseControl/Projects/Project/@name", "HelloWorld");
            AssertXPathMatches(doc, "/CruiseControl/Projects/Project/@activity", "Sleeping");
            AssertXPathMatches(doc, "/CruiseControl/Projects/Project/@lastBuildStatus", "Success");
            AssertXPathMatches(doc, "/CruiseControl/Projects/Project/@lastBuildLabel", "build_7");
            AssertXPathMatches(doc, "/CruiseControl/Projects/Project/@lastBuildTime", XmlConvert.ToString(LastBuildTime, XmlDateTimeSerializationMode.Local));
            AssertXPathMatches(doc, "/CruiseControl/Projects/Project/@nextBuildTime", XmlConvert.ToString(NextBuildTime, XmlDateTimeSerializationMode.Local));
            AssertXPathMatches(doc, "/CruiseControl/Projects/Project/@webUrl", "http://blah");
            AssertXPathMatches(doc, "/CruiseControl/Projects/Project/@category", "category");

            AssertXPathMatches(doc, "/CruiseControl/Queues/Queue/@name", "Queue1");
            AssertXPathMatches(doc, "/CruiseControl/Queues/Queue/Request/@projectName", "HelloWorld");
            AssertXPathMatches(doc, "/CruiseControl/Queues/Queue/Request/@activity", "CheckingModifications");

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
			CruiseServerSnapshot cruiseServerSnapshot = CreateCruiseServerSnapshot();

			CruiseServerSnapshotOnServer cruiseServerSnapshotOnServer = new CruiseServerSnapshotOnServer(cruiseServerSnapshot, null);
            mockFarmService.ExpectAndReturn("GetCruiseServerSnapshotListAndExceptions",
			                                new CruiseServerSnapshotListAndExceptions(
			                                	new CruiseServerSnapshotOnServer[] {cruiseServerSnapshotOnServer}, new CruiseServerException[0]));

			XmlFragmentResponse response = (XmlFragmentResponse) reportAction.Execute(null);
			string xml = response.ResponseFragment;

            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.Schemas.Add(ReadSchemaFromResources("XmlServerReportActionSchema.xsd"));
            XmlReader rdr = XmlReader.Create(new StringReader(xml), xmlReaderSettings);
			while (rdr.Read())
			{
			}

			mockFarmService.Verify();
		}

		private CruiseServerSnapshot CreateCruiseServerSnapshot()
		{
		    ProjectStatus[] projectStatuses = new ProjectStatus[]
		        {
		            new ProjectStatus("HelloWorld", "category", ProjectActivity.Sleeping, IntegrationStatus.Success,
		                              ProjectIntegratorState.Running,
		                              "http://blah", LastBuildTime, "build_8", "build_7",
		                              NextBuildTime)
		        };
            QueueSetSnapshot snapshot = new QueueSetSnapshot();
            snapshot.Queues.Add(new QueueSnapshot("Queue1"));
            snapshot.Queues[0].Requests.Add(new QueuedRequestSnapshot("HelloWorld", ProjectActivity.CheckingModifications));

            return new CruiseServerSnapshot(projectStatuses, snapshot);
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