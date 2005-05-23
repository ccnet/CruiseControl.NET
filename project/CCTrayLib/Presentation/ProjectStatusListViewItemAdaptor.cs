using System;
using System.Diagnostics;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class ProjectStatusListViewItemAdaptor
	{
		readonly ListViewItem item = new ListViewItem();

		public ListViewItem Create( IProjectMonitor projectMonitor )
		{
			projectMonitor.Polled += new PolledEventHandler(Monitor_Polled);

			item.Text = projectMonitor.ProjectName;

			DisplayProjectStateInListViewItem(projectMonitor.ProjectStatus);

			return item;
		}

		private void Monitor_Polled( object sauce, PolledEventArgs e )
		{
			DisplayProjectStateInListViewItem(e.ProjectStatus);
		}

		private void DisplayProjectStateInListViewItem( ProjectStatus status )
		{
			// OK: this is rubbish.  These magic numbers rely on the icons added to the 
			// imagelist on MainForm (and the order in which they were added...)
			// To be replaced!!
			if (status == null)
			{
				item.ImageIndex = 0;
				return;
			}

			if (status.BuildStatus == IntegrationStatus.Success)
			{
				item.ImageIndex = 1;
			}

			if (status.BuildStatus == IntegrationStatus.Failure)
			{
				item.ImageIndex = 2;
			}

			if (status.Activity == ProjectActivity.Building)
			{
				item.ImageIndex = 3;
			}

			Debug.WriteLine("Image index set to " + item.ImageIndex);
		}
	}
}
