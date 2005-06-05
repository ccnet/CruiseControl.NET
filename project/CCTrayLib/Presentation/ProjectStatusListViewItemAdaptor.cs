using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class ProjectStatusListViewItemAdaptor
	{
		private readonly IDetailStringProvider detailStringProvider;
		private readonly ListViewItem item = new ListViewItem();
		private readonly ListViewItem.ListViewSubItem activity;
		private readonly ListViewItem.ListViewSubItem detail;
		private readonly ListViewItem.ListViewSubItem lastBuildLabel;

		public ProjectStatusListViewItemAdaptor(IDetailStringProvider detailStringProvider)
		{
			this.detailStringProvider = detailStringProvider;
			activity = new ListViewItem.ListViewSubItem( item, "" );
			item.SubItems.Add( activity );
			detail = new ListViewItem.ListViewSubItem( item, "" );
			item.SubItems.Add( detail );
			lastBuildLabel = new ListViewItem.ListViewSubItem( item, "" );
			item.SubItems.Add( lastBuildLabel );

		}

		public ListViewItem Create( IProjectMonitor projectMonitor )
		{
			projectMonitor.Polled += new MonitorPolledEventHandler( Monitor_Polled );

			item.Text = projectMonitor.ProjectName;

			DisplayProjectStateInListViewItem( projectMonitor );

			return item;
		}

		private void Monitor_Polled( object sauce, MonitorPolledEventArgs args )
		{
			DisplayProjectStateInListViewItem( args.ProjectMonitor );
		}

		private void DisplayProjectStateInListViewItem( IProjectMonitor monitor )
		{
			item.ImageIndex = monitor.ProjectState.ImageIndex;

			if (monitor.ProjectStatus != null)
			{
				lastBuildLabel.Text = monitor.ProjectStatus.LastBuildLabel;
				activity.Text = monitor.ProjectStatus.Activity.ToString();
			}
			else
			{
				activity.Text = lastBuildLabel.Text = "";
			}

			detail.Text = detailStringProvider.FormatDetailString(monitor);
		}
	}
}