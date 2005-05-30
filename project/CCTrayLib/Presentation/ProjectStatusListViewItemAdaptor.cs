using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class ProjectStatusListViewItemAdaptor
	{
		private readonly ListViewItem item = new ListViewItem();

		public ListViewItem Create( IProjectMonitor projectMonitor )
		{
			projectMonitor.Polled += new MonitorPolledEventHandler( Monitor_Polled );

			item.Text = projectMonitor.ProjectName;

			DisplayProjectStateInListViewItem( projectMonitor.ProjectState );

			return item;
		}

		private void Monitor_Polled( object sauce, MonitorPolledEventArgs args )
		{
			DisplayProjectStateInListViewItem( args.ProjectMonitor.ProjectState);
		}

		private void DisplayProjectStateInListViewItem( ProjectState state )
		{
			item.ImageIndex = state.ImageIndex;
		}
	}
}