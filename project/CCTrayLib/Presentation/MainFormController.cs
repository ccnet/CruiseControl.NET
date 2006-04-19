using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.X10;

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
		private IProjectStateIconProvider iconProvider;

		public MainFormController(ICCTrayMultiConfiguration configuration, ISynchronizeInvoke owner)
		{
			this.configuration = configuration;
			monitors = configuration.GetProjectStatusMonitors();

			for (int i = 0; i < monitors.Length; i++)
			{
				monitors[i] = new SynchronizedProjectMonitor(monitors[i], owner);
			}

			aggregatedMonitor = new AggregatingProjectMonitor(monitors);
			iconProvider = new ConfigurableProjectStateIconProvider(configuration.Icons);
			projectStateIconAdaptor = new ProjectStateIconAdaptor(aggregatedMonitor, iconProvider);
			new BuildTransitionSoundPlayer(aggregatedMonitor, new AudioPlayer(), configuration.Audio);

			if (configuration.X10 != null && configuration.X10.Enabled)
			{
				new X10Controller(
					aggregatedMonitor,
					new LampController(new X10LowLevelDriver(HouseCode.A, configuration.X10.ComPort)),
					new DateTimeProvider(),
					configuration.X10);
			}
		}


		public IProjectMonitor SelectedProject
		{
			get { return selectedProject; }
			set
			{
				selectedProject = value;
				if (IsProjectSelectedChanged != null)
					IsProjectSelectedChanged(this, EventArgs.Empty);
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
			if (IsProjectSelected)
			{
				DisplayWebPageForProject(SelectedProject.Detail);
			}
		}

		public void BindToTrayIcon(TrayIcon trayIcon)
		{
			trayIcon.IconProvider = ProjectStateIconAdaptor;
			trayIcon.BalloonMessageProvider = new ConfigurableBalloonMessageProvider(configuration.BalloonMessages);
			trayIcon.BindToProjectMonitor(aggregatedMonitor, configuration.ShouldShowBalloonOnBuildTransition);
		}


		public void BindToListView(ListView listView)
		{
			IDetailStringProvider detailStringProvider = new DetailStringProvider();
			foreach (IProjectMonitor monitor in monitors)
			{
				ListViewItem item = new ProjectStatusListViewItemAdaptor(detailStringProvider).Create(monitor);
				item.Tag = monitor;
				listView.Items.Add(item);
			}
			if (listView.Items.Count > 0) listView.Items[0].Selected = true;
		}

		public void StartMonitoring()
		{
			StopMonitoring();
			poller = new Poller(configuration.PollPeriodSeconds*1000, aggregatedMonitor);
			poller.Start();
		}

		public ProjectStateIconAdaptor ProjectStateIconAdaptor
		{
			get { return projectStateIconAdaptor; }
		}

		public void StopMonitoring()
		{
			if (poller != null)
			{
				poller.Stop();
				poller = null;
			}
		}

		public bool OnDoubleClick()
		{
			if (configuration.TrayIconDoubleClickAction == TrayIconDoubleClickAction.NavigateToWebPageOfFirstProject)
			{
				if (monitors.Length != 0)
				{
					DisplayWebPageForProject(monitors[0].Detail);
					return true;
				}
			}

			return false;
		}

		private void DisplayWebPageForProject(ISingleProjectDetail project)
		{
			if (project.IsConnected)
			{
				string url = project.WebURL;
				Process.Start(url);
			}
		}

		public void PopulateImageList(ImageList imageList)
		{
			imageList.Images.Clear();
			imageList.Images.Add(iconProvider.GetStatusIconForState(ProjectState.NotConnected).Icon);
			imageList.Images.Add(iconProvider.GetStatusIconForState(ProjectState.Success).Icon);
			imageList.Images.Add(iconProvider.GetStatusIconForState(ProjectState.Broken).Icon);
			imageList.Images.Add(iconProvider.GetStatusIconForState(ProjectState.Building).Icon);
		}

		public bool CanFixBuild()
		{
			return IsProjectSelected && selectedProject.ProjectState == ProjectState.Broken;
		}

		public void VolunteerToFixBuild()
		{
			if (IsProjectSelected) selectedProject.FixBuild();
		}
	}
}