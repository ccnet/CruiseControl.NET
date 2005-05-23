using System;
using System.Windows.Forms;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Presentation
{
	public class TestingProjectMonitor : IProjectMonitor
	{
		ProjectStatus projectStatus;
		string projectName;

		public TestingProjectMonitor( string projectName )
		{
			this.projectName = projectName;
		}

		public string ProjectName
		{
			get { return projectName; }
		}
		public ProjectStatus ProjectStatus
		{
			get { return projectStatus; }
			set { projectStatus = value; }
		}

		public void OnBuildOccurred(BuildOccurredEventArgs args)
		{
			if (BuildOccurred != null)
				BuildOccurred(this, args);
		}

		public void OnPolled(PolledEventArgs args)
		{
			if (Polled != null)
				Polled(this, args); 
		}

		public event BuildOccurredEventHandler BuildOccurred;
		public event PolledEventHandler Polled;

		public void Poll()
		{
			throw new NotImplementedException();
		}
	}

	[TestFixture]
	public class ProjectStatusListViewItemAdaptorTest
	{
		[Test]
		public void CanCreateListViewItem()
		{
			TestingProjectMonitor projectMonitor = new TestingProjectMonitor("projectName");

			ProjectStatusListViewItemAdaptor adaptor = new ProjectStatusListViewItemAdaptor();
			ListViewItem item = adaptor.Create( projectMonitor );

			Assert.AreEqual("projectName", item.Text);
			Assert.AreEqual(0, item.ImageIndex);
		}


		[Test]
		public void WhenTheStateOfTheProjectChangesTheIconIsUpdated()
		{
			TestingProjectMonitor projectMonitor = new TestingProjectMonitor("projectName");
			ProjectStatusListViewItemAdaptor adaptor = new ProjectStatusListViewItemAdaptor();
			ListViewItem item = adaptor.Create(projectMonitor);

			Assert.AreEqual("projectName", item.Text);
			Assert.AreEqual(0, item.ImageIndex);

			projectMonitor.ProjectStatus = new ProjectStatus(ProjectIntegratorState.Running, 
				IntegrationStatus.Success, 
				ProjectActivity.Sleeping, 
				"a", "b", new DateTime(2004, 1, 1), "x", new DateTime(2004, 1, 2) );

			projectMonitor.OnPolled(new PolledEventArgs(projectMonitor.ProjectStatus));

			Assert.AreEqual("projectName", item.Text);
			Assert.AreEqual(1, item.ImageIndex);

		}

	}
}