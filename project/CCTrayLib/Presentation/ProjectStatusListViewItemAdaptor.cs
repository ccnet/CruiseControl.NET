using System;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class ProjectStatusListViewItemAdaptor
	{
		private readonly ListViewItem item = new ListViewItem();
		private readonly ListViewItem.ListViewSubItem activity;
		private readonly ListViewItem.ListViewSubItem lastBuildLabel;

		IProjectMonitor projectMonitor;

		public ProjectStatusListViewItemAdaptor()
		{
			activity = new ListViewItem.ListViewSubItem(item, "");
			item.SubItems.Add(activity);
			lastBuildLabel = new ListViewItem.ListViewSubItem(item, "");
			item.SubItems.Add(lastBuildLabel);

		}

		public ListViewItem Create( IProjectMonitor projectMonitor )
		{
			this.projectMonitor = projectMonitor;
			
			projectMonitor.Polled += new MonitorPolledEventHandler( Monitor_Polled );

			item.Text = projectMonitor.ProjectName;
			
			DisplayProjectStateInListViewItem( projectMonitor);

			return item;
		}

		private void Monitor_Polled( object sauce, MonitorPolledEventArgs args )
		{
			DisplayProjectStateInListViewItem( args.ProjectMonitor);
		}

		private void DisplayProjectStateInListViewItem( IProjectMonitor monitor)
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
		}
	}
}