using System.Windows.Forms;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Presentation
{
	[TestFixture]
	public class ProjectStatusListViewItemAdaptorTest
	{
		[Test]
		public void CanCreateListViewItem()
		{
			TestingProjectMonitor projectMonitor = new TestingProjectMonitor( "projectName" );

			ProjectStatusListViewItemAdaptor adaptor = new ProjectStatusListViewItemAdaptor();
			ListViewItem item = adaptor.Create( projectMonitor );

			Assert.AreEqual( "projectName", item.Text );
			Assert.AreEqual( 0, item.ImageIndex );
		}


		[Test]
		public void WhenTheStateOfTheProjectChangesTheIconIsUpdated()
		{
			TestingProjectMonitor projectMonitor = new TestingProjectMonitor( "projectName" );
			ProjectStatusListViewItemAdaptor adaptor = new ProjectStatusListViewItemAdaptor();
			ListViewItem item = adaptor.Create( projectMonitor );

			Assert.AreEqual( "projectName", item.Text );
			Assert.AreEqual( 0, item.ImageIndex );

			projectMonitor.ProjectState = ProjectState.Building;

			projectMonitor.OnPolled( new MonitorPolledEventArgs( projectMonitor ) );

			Assert.AreEqual( "projectName", item.Text );
			Assert.AreEqual( ProjectState.Building.ImageIndex, item.ImageIndex );

		}

	}
}