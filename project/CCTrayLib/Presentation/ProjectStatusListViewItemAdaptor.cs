using System;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
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
		private readonly ListViewItem.ListViewSubItem lastBuildTime;
		private readonly ListViewItem.ListViewSubItem projectStatus;
		private readonly ListViewItem.ListViewSubItem serverName;
		private readonly ICCTrayMultiConfiguration config = null;
		
		public ProjectStatusListViewItemAdaptor(IDetailStringProvider detailStringProvider, ICCTrayMultiConfiguration config) : this(detailStringProvider)
		{
			this.config = config;	
		}

		public ProjectStatusListViewItemAdaptor(IDetailStringProvider detailStringProvider)
		{
			this.detailStringProvider = detailStringProvider;
			serverName = new ListViewItem.ListViewSubItem();
			item.SubItems.Add(serverName);
			activity = new ListViewItem.ListViewSubItem(item, "");
			item.SubItems.Add(activity);
			detail = new ListViewItem.ListViewSubItem(item, "");
			item.SubItems.Add(detail);
			lastBuildLabel = new ListViewItem.ListViewSubItem(item, "");
			item.SubItems.Add(lastBuildLabel);
			lastBuildTime = new ListViewItem.ListViewSubItem(item, "");
			item.SubItems.Add(lastBuildTime);
			projectStatus = new ListViewItem.ListViewSubItem(item, "");
			item.SubItems.Add(projectStatus);
		}

		public ListViewItem Create(IProjectMonitor projectMonitor)
		{
			projectMonitor.Polled += new MonitorPolledEventHandler(Monitor_Polled);

			item.Text = projectMonitor.Detail.ProjectName;

			DisplayProjectStateInListViewItem(projectMonitor);

			return item;
		}

		private void Monitor_Polled(object sauce, MonitorPolledEventArgs args)
		{
			DisplayProjectStateInListViewItem(args.ProjectMonitor);
		}

		private void DisplayProjectStateInListViewItem(IProjectMonitor monitor)
		{
			item.ImageIndex = monitor.ProjectState.ImageIndex;

            if (monitor.Detail.IsConnected)
            {
                serverName.Text = monitor.Detail.ServerName;
                if (string.IsNullOrEmpty(serverName.Text)) serverName.Text = monitor.Detail.Configuration.ServerUrl;
                lastBuildLabel.Text = monitor.Detail.LastBuildLabel;
                lastBuildTime.Text = monitor.Detail.LastBuildTime.ToString();
                projectStatus.Text = monitor.ProjectIntegratorState;
                activity.Text = monitor.Detail.Activity.ToString();
            }
            else
            {
                activity.Text = lastBuildLabel.Text = "";
            }

			detail.Text = detailStringProvider.FormatDetailString(monitor.Detail);
		}
	}
}
