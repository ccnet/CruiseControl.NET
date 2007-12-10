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
		private Poller serverPoller;
		private Poller projectPoller;
		private IServerMonitor aggregatedServerMonitor;
		private IProjectMonitor aggregatedProjectMonitor;
		private ISingleServerMonitor[] serverMonitors;
		private IProjectMonitor[] projectMonitors;
		private ProjectStateIconAdaptor projectStateIconAdaptor;
		private IProjectStateIconProvider projectStateIconProvider;
		private IIntegrationQueueIconProvider integrationQueueIconProvider;

		public MainFormController(ICCTrayMultiConfiguration configuration, ISynchronizeInvoke owner)
		{
			this.configuration = configuration;

			serverMonitors = configuration.GetServerMonitors();
			for (int i = 0; i < serverMonitors.Length; i++)
			{
				serverMonitors[i] = new SynchronizedServerMonitor(serverMonitors[i], owner);
			}
			aggregatedServerMonitor = new AggregatingServerMonitor(serverMonitors);
			integrationQueueIconProvider = new ResourceIntegrationQueueIconProvider();

			projectMonitors = configuration.GetProjectStatusMonitors(serverMonitors);
			for (int i = 0; i < projectMonitors.Length; i++)
			{
				projectMonitors[i] = new SynchronizedProjectMonitor(projectMonitors[i], owner);
			}			
			aggregatedProjectMonitor = new AggregatingProjectMonitor(projectMonitors);
			projectStateIconProvider = new ConfigurableProjectStateIconProvider(configuration.Icons);
			projectStateIconAdaptor = new ProjectStateIconAdaptor(aggregatedProjectMonitor, projectStateIconProvider);
			new BuildTransitionSoundPlayer(aggregatedProjectMonitor, new AudioPlayer(), configuration.Audio);

			if (configuration.X10 != null && configuration.X10.Enabled)
			{
				new X10Controller(
					aggregatedProjectMonitor,
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
		
		public IProjectMonitor[] Monitors
		{
			get
			{
				return projectMonitors;
			}
		}
		


		public bool IsProjectSelected
		{
			get { return selectedProject != null; }
		}
		
		public bool IsProjectBuilding
		{
			get
			{
				if (SelectedProject != null)
				{
					if ((SelectedProject.ProjectState == ProjectState.Building) ||
						(SelectedProject.ProjectState == ProjectState.BrokenAndBuilding))
					{
						return true;
					}
					else
					{
						return false;
					}
				}
				else
				{
					return false;
				}
			}
		}
		
		public event EventHandler IsProjectSelectedChanged;

		public void ForceBuild()
		{
			if (IsProjectSelected && SelectedProject.ProjectState != ProjectState.NotConnected)
			{
				SelectedProject.ForceBuild();
			}
		}
		
		public void AbortBuild()
		{
			if (IsProjectSelected && SelectedProject.ProjectState != ProjectState.NotConnected)
			{
				SelectedProject.AbortBuild();
			}
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
			trayIcon.BindToProjectMonitor(aggregatedProjectMonitor, configuration.ShouldShowBalloonOnBuildTransition);
		}

		public void BindToListView(ListView listView)
		{
			IDetailStringProvider detailStringProvider = new DetailStringProvider();
			foreach (IProjectMonitor monitor in projectMonitors)
			{
				ListViewItem item = new ProjectStatusListViewItemAdaptor(detailStringProvider, configuration).Create(monitor);
				item.Tag = monitor;
				listView.Items.Add(item);
			}
			if (listView.Items.Count > 0) listView.Items[0].Selected = true;
		}

		public void BindToQueueTreeView(QueueTreeView treeView)
		{
			StartProjectMonitoring();
			treeView.BeginUpdate();
			treeView.Nodes.Clear();
			foreach (ISingleServerMonitor monitor in serverMonitors)
			{
				IntegrationQueueTreeNodeAdaptor adaptor = new IntegrationQueueTreeNodeAdaptor(monitor);
				TreeNode serverTreeNode = adaptor.Create();
				treeView.Nodes.Add(serverTreeNode);
			}
			treeView.EndUpdate();
			if (treeView.Nodes.Count > 0)
			{
				treeView.SelectedNode = treeView.Nodes[0];
			}
		}

		public void UnbindToQueueTreeView(QueueTreeView treeView)
		{
			treeView.BeginUpdate();
			foreach (TreeNode node in treeView.Nodes)
			{
				IntegrationQueueTreeNodeAdaptor adaptor = node.Tag as IntegrationQueueTreeNodeAdaptor;
				if (adaptor != null)
				{
					adaptor.Detach();
				}
			}
			treeView.Nodes.Clear();
			treeView.EndUpdate();
		}

		public void StartServerMonitoring()
		{
			StopServerMonitoring();
			serverPoller = new Poller(configuration.PollPeriodSeconds, aggregatedServerMonitor);
			serverPoller.Start();

		    StartProjectMonitoring();
		}

		public void StopServerMonitoring()
		{
		    StopProjectMonitoring();

			if (serverPoller != null)
			{
				serverPoller.Stop();
				serverPoller = null;
			}
		}

        private void StartProjectMonitoring()
        {
            StopProjectMonitoring();
            projectPoller = new Poller(configuration.PollPeriodSeconds, aggregatedProjectMonitor);
            projectPoller.Start();
        }

        private void StopProjectMonitoring()
        {
            if (projectPoller != null)
            {
                projectPoller.Stop();
                projectPoller = null;
            }
        }

		public ProjectStateIconAdaptor ProjectStateIconAdaptor
		{
			get { return projectStateIconAdaptor; }
		}

		public bool OnDoubleClick()
		{
			if (configuration.TrayIconDoubleClickAction == TrayIconDoubleClickAction.NavigateToWebPageOfFirstProject)
			{
				if (projectMonitors.Length != 0)
				{
					DisplayWebPageForProject(projectMonitors[0].Detail);
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
			imageList.Images.Add(projectStateIconProvider.GetStatusIconForState(ProjectState.NotConnected).Icon);
			imageList.Images.Add(projectStateIconProvider.GetStatusIconForState(ProjectState.Success).Icon);
			imageList.Images.Add(projectStateIconProvider.GetStatusIconForState(ProjectState.Broken).Icon);
			imageList.Images.Add(projectStateIconProvider.GetStatusIconForState(ProjectState.Building).Icon);
			imageList.Images.Add(projectStateIconProvider.GetStatusIconForState(ProjectState.BrokenAndBuilding).Icon);
		}

		public void PopulateQueueImageList(ImageList imageList)
		{
			imageList.Images.Clear();
			imageList.Images.Add(integrationQueueIconProvider.GetStatusIconForNodeType(IntegrationQueueNodeType.RemotingServer).Icon);
			imageList.Images.Add(integrationQueueIconProvider.GetStatusIconForNodeType(IntegrationQueueNodeType.HttpServer).Icon);
            imageList.Images.Add(integrationQueueIconProvider.GetStatusIconForNodeType(IntegrationQueueNodeType.QueueEmpty).Icon);
            imageList.Images.Add(integrationQueueIconProvider.GetStatusIconForNodeType(IntegrationQueueNodeType.QueuePopulated).Icon);
            imageList.Images.Add(integrationQueueIconProvider.GetStatusIconForNodeType(IntegrationQueueNodeType.CheckingModifications).Icon);
            imageList.Images.Add(integrationQueueIconProvider.GetStatusIconForNodeType(IntegrationQueueNodeType.Building).Icon);
			imageList.Images.Add(integrationQueueIconProvider.GetStatusIconForNodeType(IntegrationQueueNodeType.PendingInQueue).Icon);
		}

        public void SetFormTopMost(Form form)
        {
            form.TopMost = configuration.AlwaysOnTop;
        }

		public bool CanFixBuild()
		{
			return IsProjectSelected && 
			       (selectedProject.ProjectState == ProjectState.Broken || selectedProject.ProjectState == ProjectState.BrokenAndBuilding);
		}

		public void VolunteerToFixBuild()
		{
			if (IsProjectSelected) selectedProject.FixBuild(configuration.FixUserName);
		}

		public bool CanCancelPending()
		{
			return IsProjectSelected && selectedProject.IsPending;
		}

		public void CancelPending()
		{
			if (IsProjectSelected) selectedProject.CancelPending();
		}

		public void CancelPendingProjectByName(string projectName)
		{
			foreach (IProjectMonitor projectMonitor in projectMonitors)
			{
				if (projectMonitor.Detail.ProjectName == projectName)
				{
					SelectedProject = projectMonitor;
					CancelPending();
					break;
				}
			}
		}
	}
}
