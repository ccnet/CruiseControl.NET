using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class DashboardXmlParserTest
	{

		const string PROJECTS_XML = @"<Projects xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xsi:noNamespaceSchemaLocation='C:\SF\ccnet\dashboard.xsd'>
	<Project name='SvnTest' activity='Sleeping' lastBuildStatus='Exception' lastBuildLabel='8' lastBuildTime='2005-09-28T10:30:34.6362160+01:00' nextBuildTime='2005-10-04T14:31:52.4509248+01:00' webUrl='http://mrtickle/ccnet/'/>
	<Project name='projectName' activity='Sleeping' lastBuildStatus='Success' lastBuildLabel='13' lastBuildTime='2005-09-15T17:33:07.6447696+01:00' nextBuildTime='2005-10-04T14:31:51.7799600+01:00' webUrl='http://mrtickle/ccnet/'/>
</Projects>";

        const string CRUISE_SERVER_XML = @"<CruiseControl xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
    <Projects>
	    <Project name='SvnTest' activity='Sleeping' lastBuildStatus='Exception' lastBuildLabel='8' lastBuildTime='2005-09-28T10:30:34.6362160+01:00' nextBuildTime='2005-10-04T14:31:52.4509248+01:00' webUrl='http://mrtickle/ccnet/'/>
	    <Project name='projectName' activity='Sleeping' lastBuildStatus='Success' lastBuildLabel='13' lastBuildTime='2005-09-15T17:33:07.6447696+01:00' nextBuildTime='2005-10-04T14:31:51.7799600+01:00' webUrl='http://mrtickle/ccnet/'/>
    </Projects>
    <Queues>
        <Queue name='Queue1'>
            <Request projectName='projectName' activity='CheckingModifications' />
            <Request projectName='SVNTest' activity='Pending' />
        </Queue>
        <Queue name='Queue2'>
            <Request projectName='Missing' activity='Building' />
        </Queue>
    </Queues>
</CruiseControl>";

		[Test]
		public void ReturnsCorrectProjectDetailsFromProjectsOnlyXml()
		{
			DashboardXmlParser parser = new DashboardXmlParser();
			
			CruiseServerSnapshot snapshot = parser.ExtractAsCruiseServerSnapshot(PROJECTS_XML);
			Assert.IsNotNull(snapshot);

            Assert.AreEqual(2, snapshot.ProjectStatuses.Length);
		    AssertProjectsSerializedCorrectly(snapshot);
		}

	    private static void AssertProjectsSerializedCorrectly(CruiseServerSnapshot snapshot)
	    {
	        ProjectStatus projectStatus1 = snapshot.ProjectStatuses[0];
	        Assert.AreEqual("SvnTest", projectStatus1.Name);
	        Assert.AreEqual(ProjectActivity.Sleeping, projectStatus1.Activity);
	        Assert.AreEqual(IntegrationStatus.Exception, projectStatus1.BuildStatus);
	        Assert.AreEqual("8", projectStatus1.LastBuildLabel);
	        Assert.AreEqual("http://mrtickle/ccnet/", projectStatus1.WebURL);

	        ProjectStatus projectStatus2 = snapshot.ProjectStatuses[1];
	        Assert.AreEqual("projectName", projectStatus2.Name);
	        Assert.AreEqual(ProjectActivity.Sleeping, projectStatus2.Activity);
	        Assert.AreEqual(IntegrationStatus.Success, projectStatus2.BuildStatus);
	        Assert.AreEqual("13", projectStatus2.LastBuildLabel);
	        Assert.AreEqual("http://mrtickle/ccnet/", projectStatus2.WebURL);
	    }

	    [Test]
		public void ReturnsListOfProjectsFromProjectsXml()
		{
			DashboardXmlParser parser = new DashboardXmlParser();

			string[] names = parser.ExtractProjectNames(PROJECTS_XML);
			Assert.AreEqual(2, names.Length);
			Assert.AreEqual("SvnTest", names[0]);
			Assert.AreEqual("projectName", names[1]);
		}

        [Test]
        public void ReturnsListOfProjectsFromProjectsAndQueuesXml()
        {
            DashboardXmlParser parser = new DashboardXmlParser();

            string[] names = parser.ExtractProjectNames(CRUISE_SERVER_XML);
            Assert.AreEqual(2, names.Length);
            Assert.AreEqual("SvnTest", names[0]);
            Assert.AreEqual("projectName", names[1]);
        }

        [Test]
        public void ReturnsCorrectProjectDetailsFromProjectsAndQueuesXml()
        {
            DashboardXmlParser parser = new DashboardXmlParser();

            CruiseServerSnapshot snapshot = parser.ExtractAsCruiseServerSnapshot(CRUISE_SERVER_XML);
            Assert.IsNotNull(snapshot);

            Assert.AreEqual(2, snapshot.ProjectStatuses.Length);
            AssertProjectsSerializedCorrectly(snapshot);

            Assert.AreEqual(2, snapshot.QueueSetSnapshot.Queues.Count);
            QueueSnapshot queueSnapshot1 = snapshot.QueueSetSnapshot.Queues[0];
            Assert.AreEqual("Queue1", queueSnapshot1.QueueName);
            Assert.AreEqual("projectName", queueSnapshot1.Requests[0].ProjectName);
            Assert.AreEqual(ProjectActivity.CheckingModifications, queueSnapshot1.Requests[0].Activity);
            Assert.AreEqual("SVNTest", queueSnapshot1.Requests[1].ProjectName);
            Assert.AreEqual(ProjectActivity.Pending, queueSnapshot1.Requests[1].Activity);

            QueueSnapshot queueSnapshot2 = snapshot.QueueSetSnapshot.Queues[1];
            Assert.AreEqual("Queue2", queueSnapshot2.QueueName);
            Assert.AreEqual("Missing", queueSnapshot2.Requests[0].ProjectName);
            Assert.AreEqual(ProjectActivity.Building, queueSnapshot2.Requests[0].Activity);
        }
    }
}