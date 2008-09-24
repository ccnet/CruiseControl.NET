using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.X10;
using ThoughtWorks.CruiseControl.CCTrayLib.Speech;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class MainFormController
	{
		private IProjectMonitor selectedProject;
		private readonly ICCTrayMultiConfiguration configuration;
		private Poller serverPoller;
		private Poller projectPoller;
		private readonly IServerMonitor aggregatedServerMonitor;
		private readonly IProjectMonitor aggregatedProjectMonitor;
		private readonly ISingleServerMonitor[] serverMonitors;
		private readonly IProjectMonitor[] projectMonitors;
		private readonly ProjectStateIconAdaptor projectStateIconAdaptor;
		private readonly IProjectStateIconProvider projectStateIconProvider;
		private readonly IIntegrationQueueIconProvider integrationQueueIconProvider;
		private BuildTransitionSoundPlayer soundPlayer;
        private X10Controller x10Controller;
        private SpeakingProjectMonitor speakerForTheDead;

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
			soundPlayer = new BuildTransitionSoundPlayer(aggregatedProjectMonitor, new AudioPlayer(), configuration.Audio);
			LampController lampController = new LampController(configuration.X10,null);
			x10Controller = new X10Controller(aggregatedProjectMonitor,new DateTimeProvider(),configuration.X10,lampController);

			IBalloonMessageProvider balloonMessageProvider = new ConfigurableBalloonMessageProvider(configuration.BalloonMessages);
			speakerForTheDead = new SpeakingProjectMonitor(aggregatedProjectMonitor, balloonMessageProvider, configuration.Speech);
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
		
		public bool IsProjectRunning
		{
			get {
				if (!IsProjectSelected) return false;
				if (!selectedProject.IsConnected) return false;
				else
				{
					bool isProjectRunning = selectedProject.ProjectIntegratorState.Equals(Remote.ProjectIntegratorState.Running.ToString());
					return isProjectRunning;
				}
			}
		}
		
		public event EventHandler IsProjectSelectedChanged;

        public void CopyBuildLabel()
        {
            if (IsProjectSelected)
            {
                // sometimes you get an error on setting test to the clipboard
                // only catch these errors
                try
                {
                    Clipboard.SetText(SelectedProject.Detail.LastBuildLabel);
                }
                catch (System.Runtime.InteropServices.ExternalException )
                    {
                    
                    }
            }
        }

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
            trayIcon.BindToProjectMonitor(aggregatedProjectMonitor,
                configuration.ShouldShowBalloonOnBuildTransition,
                configuration.MinimumNotificationLevel);
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
		
		public void StopProject()
		{
			selectedProject.StopProject();
		}
		
		public void StartProject()
		{
			selectedProject.StartProject();
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

		private static void DisplayWebPageForProject(ISingleProjectDetail project)
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

		public void SetFormShowInTaskbar(Form form)
		{
			form.ShowInTaskbar = configuration.ShowInTaskbar;
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
