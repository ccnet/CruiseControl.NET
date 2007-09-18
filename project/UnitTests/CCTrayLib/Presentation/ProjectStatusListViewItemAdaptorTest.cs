using System.Windows.Forms;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.Core;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Presentation
{
	[TestFixture]
	public class ProjectStatusListViewItemAdaptorTest
	{
		private DynamicMock mockProjectDetailStringFormatter;
		private IDetailStringProvider detailStringFormatter;

		[SetUp]
		public void SetUp()
		{
			mockProjectDetailStringFormatter = new DynamicMock(typeof (IDetailStringProvider));
			detailStringFormatter = (IDetailStringProvider) mockProjectDetailStringFormatter.MockInstance;
		}

		[Test]
		public void CanCreateListViewItem()
		{
			StubProjectMonitor projectMonitor = new StubProjectMonitor("projectName");

			ProjectStatusListViewItemAdaptor adaptor = new ProjectStatusListViewItemAdaptor(detailStringFormatter);
			ListViewItem item = adaptor.Create(projectMonitor);

			Assert.AreEqual("projectName", item.Text);
			Assert.AreEqual(0, item.ImageIndex);
		}


		[Test]
		public void WhenTheStateOfTheProjectChangesTheIconIsUpdated()
		{
			StubProjectMonitor projectMonitor = new StubProjectMonitor("projectName");
			ProjectStatusListViewItemAdaptor adaptor = new ProjectStatusListViewItemAdaptor(detailStringFormatter);
			ListViewItem item = adaptor.Create(projectMonitor);

			Assert.AreEqual("projectName", item.Text);
			Assert.AreEqual(0, item.ImageIndex);

			projectMonitor.ProjectState = ProjectState.Building;

			projectMonitor.OnPolled(new MonitorPolledEventArgs(projectMonitor));

			Assert.AreEqual("projectName", item.Text);
			Assert.AreEqual(ProjectState.Building.ImageIndex, item.ImageIndex);
		}

		[Test]
		public void WhenTheStateOfTheProjectChangesTheStatusEntriesOnTheListViewItemAreUpdated()
		{
			StubProjectMonitor projectMonitor = new StubProjectMonitor("projectName");
			projectMonitor.ProjectState = ProjectState.Building;
			projectMonitor.ProjectStatus = null;

			ProjectStatusListViewItemAdaptor adaptor = new ProjectStatusListViewItemAdaptor(detailStringFormatter);
			ListViewItem item = adaptor.Create(projectMonitor);

			Assert.AreEqual(6, item.SubItems.Count);
			ListViewItem.ListViewSubItem activity = item.SubItems[2];
			ListViewItem.ListViewSubItem label = item.SubItems[4];

			Assert.AreEqual("", activity.Text);
			Assert.AreEqual("", label.Text);

			ProjectStatus status = ProjectStatusFixture.New(ProjectActivity.Sleeping, "lastLabel");
			projectMonitor.ProjectStatus = status;

			projectMonitor.OnPolled(new MonitorPolledEventArgs(projectMonitor));

			Assert.AreEqual("Sleeping", activity.Text);
			Assert.AreEqual("lastLabel", label.Text);

		}

		[Test]
		public void UsesDescriptionBuilderToGenerateDetailCaption()
		{
			StubProjectMonitor projectMonitor = new StubProjectMonitor("projectName");
			mockProjectDetailStringFormatter.Strict = true;

			mockProjectDetailStringFormatter.ExpectAndReturn("FormatDetailString", "test1", projectMonitor);
			ProjectStatusListViewItemAdaptor adaptor = new ProjectStatusListViewItemAdaptor(detailStringFormatter);
			ListViewItem item = adaptor.Create(projectMonitor);

			ListViewItem.ListViewSubItem detail = item.SubItems[3];
			Assert.AreEqual("test1", detail.Text);

			mockProjectDetailStringFormatter.ExpectAndReturn("FormatDetailString", "test2", projectMonitor);
			projectMonitor.OnPolled(new MonitorPolledEventArgs(projectMonitor));

			Assert.AreEqual("test2", detail.Text);

			mockProjectDetailStringFormatter.Verify();
		}

	}
}