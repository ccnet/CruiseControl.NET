using System;
using System.Diagnostics;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class MainFormController
	{
		private IProjectMonitor selectedProject;
		private ICCTrayMultiConfiguration configuration;
		private Poller poller;
		private IProjectMonitor aggregatedMonitor;
		private IProjectMonitor[] monitors;
		private ProjectStateIconAdaptor projectStateIconAdaptor;

		public MainFormController( ICCTrayMultiConfiguration configuration )
		{
			this.configuration = configuration;
			monitors = configuration.GetProjectStatusMonitors();
			aggregatedMonitor = new AggregatingProjectMonitor( monitors );
			projectStateIconAdaptor = new ProjectStateIconAdaptor( aggregatedMonitor, new ResourceProjectStateIconProvider() );
		}


		public IProjectMonitor SelectedProject
		{
			get { return selectedProject; }
			set
			{
				selectedProject = value;
				if (IsProjectSelectedChanged != null)
					IsProjectSelectedChanged( this, EventArgs.Empty );
			}
		}

		public bool IsProjectSelected
		{
			get { return selectedProject != null; }
		}

		public event EventHandler IsProjectSelectedChanged;

		public void ForceBuild()
		{
			if (IsProjectSelected)
				SelectedProject.ForceBuild();
		}

		public void DisplayWebPage()
		{
			if (IsProjectSelected && SelectedProject.ProjectStatus != null)
			{
				string url = SelectedProject.ProjectStatus.WebURL;
				Process.Start( url );
			}
		}

		public void BindToTrayIcon( TrayIcon trayIcon )
		{
			trayIcon.BindTo( aggregatedMonitor );
		}


		public void BindToListView( ListView listView )
		{
			IDetailStringProvider detailStringProvider = new DetailStringProvider();
			foreach (IProjectMonitor monitor in monitors)
			{
				ListViewItem item = new ProjectStatusListViewItemAdaptor( detailStringProvider ).Create( monitor );
				item.Tag = monitor;
				listView.Items.Add( item );
			}
		}

		public void StartMonitoring()
		{
			poller = new Poller( 5000, aggregatedMonitor );
			poller.Start();
		}

		public ProjectStateIconAdaptor ProjectStateIconAdaptor
		{
			get { return projectStateIconAdaptor; }
		}

		public void ShowPreferencesDialog()
		{
			new CCTrayMultiSettingsForm().ShowDialog();
		}
	}
}