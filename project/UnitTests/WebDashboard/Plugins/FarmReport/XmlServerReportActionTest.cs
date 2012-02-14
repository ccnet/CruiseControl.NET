using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Parameters;
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
			                                new CruiseServerSnapshotListAndExceptions(new CruiseServerSnapshotOnServer[0], new CruiseServerException[0]),
                                            (string)null);

			IResponse response = reportAction.Execute(null);
			Assert.IsNotNull(response);
			Assert.AreEqual(typeof (XmlFragmentResponse), response.GetType());

			mockFarmService.Verify();
		}

		[Test]
		public void WhenNoCruiseServerSnapshotEntriesAreReturnedByTheFarmServiceTheXmlContainsJustRootNodes()
		{
            mockFarmService.ExpectAndReturn("GetCruiseServerSnapshotListAndExceptions",
			                                new CruiseServerSnapshotListAndExceptions(new CruiseServerSnapshotOnServer[0], new CruiseServerException[0]),
                                            (string)null);
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
			                                new CruiseServerSnapshotListAndExceptions(new CruiseServerSnapshotOnServer[] {cruiseServerSnapshotOnServer}, new CruiseServerException[0]),
                                            (string)null);

			XmlFragmentResponse response = (XmlFragmentResponse) reportAction.Execute(null);
			string xml = response.ResponseFragment;

            // cannot just compare the xml string, since we correctly expect the string to vary based on the
			// timezone in which this code is executing
			XmlDocument doc = XPathAssert.LoadAsDocument(xml);
            XPathAssert.Matches(doc, "/CruiseControl/Projects/Project/@name", "HelloWorld");
            XPathAssert.Matches(doc, "/CruiseControl/Projects/Project/@activity", "Sleeping");
            XPathAssert.Matches(doc, "/CruiseControl/Projects/Project/@lastBuildStatus", "Success");
            XPathAssert.Matches(doc, "/CruiseControl/Projects/Project/@lastBuildLabel", "build_7");
            XPathAssert.Matches(doc, "/CruiseControl/Projects/Project/@lastBuildTime", LastBuildTime);
            XPathAssert.Matches(doc, "/CruiseControl/Projects/Project/@nextBuildTime", NextBuildTime);
            XPathAssert.Matches(doc, "/CruiseControl/Projects/Project/@webUrl", "http://blah");
            XPathAssert.Matches(doc, "/CruiseControl/Projects/Project/@category", "category");
            XPathAssert.Matches(doc, "/CruiseControl/Projects/Project/@serverName", Environment.MachineName);

            XPathAssert.Matches(doc, "/CruiseControl/Queues/Queue/@name", "Queue1");
            XPathAssert.Matches(doc, "/CruiseControl/Queues/Queue/Request/@projectName", "HelloWorld");
            XPathAssert.Matches(doc, "/CruiseControl/Queues/Queue/Request/@activity", "CheckingModifications");

			mockFarmService.Verify();
		}
        
		[Test]
		public void ReturnedXmlValidatesAgainstSchema()
		{
			CruiseServerSnapshot cruiseServerSnapshot = CreateCruiseServerSnapshot();

			CruiseServerSnapshotOnServer cruiseServerSnapshotOnServer = new CruiseServerSnapshotOnServer(cruiseServerSnapshot, null);
            mockFarmService.ExpectAndReturn("GetCruiseServerSnapshotListAndExceptions",
			                                new CruiseServerSnapshotListAndExceptions(new CruiseServerSnapshotOnServer[] {cruiseServerSnapshotOnServer}, new CruiseServerException[0]),
                                            (string)null);

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
		                              NextBuildTime,"", "", 0, new ParameterBase[0])
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