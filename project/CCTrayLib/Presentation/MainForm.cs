using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;
using Message=System.Windows.Forms.Message;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class MainForm : Form
	{
		private static int WM_QUERYENDSESSION = 0x0011;

		private MenuItem menuFile;
		private MenuItem menuFileExit;
		private ListView lvProjects;
		private ColumnHeader colProject;
		private ImageList iconList;
		private MainMenu mainMenu;
		private TrayIcon trayIcon;
		private ContextMenu projectContextMenu;
		private MenuItem mnuForce;
		private MenuItem mnuWebPage;
		private MenuItem mnuViewIcons;
		private MenuItem mnuViewList;
		private MenuItem mnuViewDetails;
		private ImageList largeIconList;
		private Button btnForceBuild;
		private ColumnHeader colLastBuildLabel;
		private ColumnHeader colActivity;
		private IContainer components;
		private MenuItem mnuFilePreferences;
		private MenuItem menuItem3;
		private ColumnHeader colDetail;
		private MainFormController controller;
		private MenuItem mnuView;
		private ContextMenu mnuTrayContextMenu;
		private MenuItem mnuTraySettings;
		private MenuItem mnuShow;
		private MenuItem mnuTrayExit;
		private MenuItem menuItem5;
		private ICCTrayMultiConfiguration configuration;
		private ColumnHeader colLastBuildTime;
		private bool systemShutdownInProgress;
		private MenuItem mnuFixBuild;
		private System.Windows.Forms.MenuItem mnuCancelPending;
		private System.Windows.Forms.Splitter splitterQueueView;
		private System.Windows.Forms.Button btnToggleQueueView;
		private System.Windows.Forms.Panel pnlButtons;
		private System.Windows.Forms.Panel pnlViewQueues;
		private System.Windows.Forms.TreeView queueTreeView;
		private System.Windows.Forms.ContextMenu queueContextMenu;
		private System.Windows.Forms.ImageList queueIconList;
		private System.Windows.Forms.MenuItem mnuQueueCancelPending;
		private PersistWindowState windowState;
		private ICache httpCache;

		public MainForm(ICCTrayMultiConfiguration configuration, ICache httpCache)
		{
			this.configuration = configuration;
			this.httpCache = httpCache;

			InitializeComponent();
			HookPersistentWindowState();
		}

		private void HookPersistentWindowState()
		{
			windowState = new PersistWindowState();
			windowState.Parent = this;
			// set registry path in HKEY_CURRENT_USER
			windowState.RegistryPath = @"Software\ThoughtWorks\CCTray";
			windowState.LoadState += new WindowStateEventHandler(OnLoadState);
			windowState.SaveState += new WindowStateEventHandler(OnSaveState);
		}

		private void OnLoadState(object sender, WindowStateEventArgs e)
		{
			// get additional state information from registry
			colProject.Width = (int) e.Key.GetValue("ProjectColumnWidth", 160);
			colActivity.Width = (int) e.Key.GetValue("ActivityColumnWidth", 132);
			colDetail.Width = (int) e.Key.GetValue("DetailColumnWidth", 250);
			colLastBuildLabel.Width = (int) e.Key.GetValue("LastBuildLabelColumnWidth", 120);
			colLastBuildTime.Width = (int) e.Key.GetValue("LastBuildTimeColumnWidth", 130);
			bool isQueueViewPanelVisible = bool.Parse(e.Key.GetValue("QueueViewPanelVisible", bool.FalseString).ToString());
			splitterQueueView.Visible = isQueueViewPanelVisible;
			pnlViewQueues.Visible = isQueueViewPanelVisible;
			UpdateViewQueuesButtonLabel();
			splitterQueueView.SplitPosition = (int) e.Key.GetValue("QueueViewSplitterPosition", 80);
		}

		private void OnSaveState(object sender, WindowStateEventArgs e)
		{
			// save additional state information to registry
			e.Key.SetValue("ProjectColumnWidth", colProject.Width);
			e.Key.SetValue("ActivityColumnWidth", colActivity.Width);
			e.Key.SetValue("DetailColumnWidth", colDetail.Width);
			e.Key.SetValue("LastBuildLabelColumnWidth", colLastBuildLabel.Width);
			e.Key.SetValue("LastBuildTimeColumnWidth", colLastBuildTime.Width);
			e.Key.SetValue("QueueViewPanelVisible", queueTreeView.Visible);
			e.Key.SetValue("QueueViewSplitterPosition", splitterQueueView.SplitPosition);
		}

		private void CreateController(ICache httpCache)
		{
			controller = new MainFormController(configuration, this, httpCache);

			DataBindings.Add("Icon", controller.ProjectStateIconAdaptor, "Icon");

			controller.PopulateImageList(iconList);
			controller.PopulateImageList(largeIconList);
			controller.BindToTrayIcon(trayIcon);
			controller.BindToListView(lvProjects);
			controller.PopulateQueueImageList(queueIconList);
			if (queueTreeView.Visible)
			{
				controller.BindToQueueTreeView(queueTreeView);
			}

			btnForceBuild.DataBindings.Add("Enabled", controller, "IsProjectSelected");
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);
			if (!this.DesignMode)
			{
				// We can only populate the TreeView in the OnLoad event or else you get a horizontal scrollbar
				// appearing - a known bug in .Net 1.1 TreeView (control must be visible when you populate it). 
				// To keep related code together in CreateController() have moved here from constructor.
				CreateController(httpCache);

				controller.StartProjectMonitoring();
				StartServerMonitoringIfQueuesDisplayed();
			}
		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MainForm));
			this.lvProjects = new System.Windows.Forms.ListView();
			this.colProject = new System.Windows.Forms.ColumnHeader();
			this.colActivity = new System.Windows.Forms.ColumnHeader();
			this.colDetail = new System.Windows.Forms.ColumnHeader();
			this.colLastBuildLabel = new System.Windows.Forms.ColumnHeader();
			this.colLastBuildTime = new System.Windows.Forms.ColumnHeader();
			this.projectContextMenu = new System.Windows.Forms.ContextMenu();
			this.mnuForce = new System.Windows.Forms.MenuItem();
			this.mnuWebPage = new System.Windows.Forms.MenuItem();
			this.mnuCancelPending = new System.Windows.Forms.MenuItem();
			this.mnuFixBuild = new System.Windows.Forms.MenuItem();
			this.largeIconList = new System.Windows.Forms.ImageList(this.components);
			this.iconList = new System.Windows.Forms.ImageList(this.components);
			this.mainMenu = new System.Windows.Forms.MainMenu();
			this.menuFile = new System.Windows.Forms.MenuItem();
			this.mnuFilePreferences = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuFileExit = new System.Windows.Forms.MenuItem();
			this.mnuView = new System.Windows.Forms.MenuItem();
			this.mnuViewIcons = new System.Windows.Forms.MenuItem();
			this.mnuViewList = new System.Windows.Forms.MenuItem();
			this.mnuViewDetails = new System.Windows.Forms.MenuItem();
			this.trayIcon = new ThoughtWorks.CruiseControl.CCTrayLib.Presentation.TrayIcon();
			this.mnuTrayContextMenu = new System.Windows.Forms.ContextMenu();
			this.mnuTraySettings = new System.Windows.Forms.MenuItem();
			this.mnuShow = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.mnuTrayExit = new System.Windows.Forms.MenuItem();
			this.pnlButtons = new System.Windows.Forms.Panel();
			this.btnToggleQueueView = new System.Windows.Forms.Button();
			this.btnForceBuild = new System.Windows.Forms.Button();
			this.splitterQueueView = new System.Windows.Forms.Splitter();
			this.pnlViewQueues = new System.Windows.Forms.Panel();
			this.queueTreeView = new System.Windows.Forms.TreeView();
			this.queueIconList = new System.Windows.Forms.ImageList(this.components);
			this.queueContextMenu = new System.Windows.Forms.ContextMenu();
			this.mnuQueueCancelPending = new System.Windows.Forms.MenuItem();
			this.pnlButtons.SuspendLayout();
			this.pnlViewQueues.SuspendLayout();
			this.SuspendLayout();
			// 
			// lvProjects
			// 
			this.lvProjects.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						 this.colProject,
																						 this.colActivity,
																						 this.colDetail,
																						 this.colLastBuildLabel,
																						 this.colLastBuildTime});
			this.lvProjects.ContextMenu = this.projectContextMenu;
			this.lvProjects.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvProjects.FullRowSelect = true;
			this.lvProjects.HideSelection = false;
			this.lvProjects.LargeImageList = this.largeIconList;
			this.lvProjects.Location = new System.Drawing.Point(203, 0);
			this.lvProjects.MultiSelect = false;
			this.lvProjects.Name = "lvProjects";
			this.lvProjects.Size = new System.Drawing.Size(689, 260);
			this.lvProjects.SmallImageList = this.iconList;
			this.lvProjects.TabIndex = 0;
			this.lvProjects.View = System.Windows.Forms.View.Details;
			this.lvProjects.DoubleClick += new System.EventHandler(this.lvProjects_DoubleClick);
			this.lvProjects.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvProjects_ColumnClick);
			this.lvProjects.SelectedIndexChanged += new System.EventHandler(this.lvProjects_SelectedIndexChanged);
			// 
			// colProject
			// 
			this.colProject.Text = "Project";
			this.colProject.Width = 160;
			// 
			// colActivity
			// 
			this.colActivity.Text = "Activity";
			this.colActivity.Width = 132;
			// 
			// colDetail
			// 
			this.colDetail.Text = "Detail";
			this.colDetail.Width = 282;
			// 
			// colLastBuildLabel
			// 
			this.colLastBuildLabel.Text = "Last Build Label";
			this.colLastBuildLabel.Width = 192;
			// 
			// colLastBuildTime
			// 
			this.colLastBuildTime.Text = "Last Build Time";
			this.colLastBuildTime.Width = 112;
			// 
			// projectContextMenu
			// 
			this.projectContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							   this.mnuForce,
																							   this.mnuWebPage,
																							   this.mnuCancelPending,
																							   this.mnuFixBuild});
			this.projectContextMenu.Popup += new System.EventHandler(this.projectContextMenu_Popup);
			// 
			// mnuForce
			// 
			this.mnuForce.Index = 0;
			this.mnuForce.Text = "&Force Build";
			this.mnuForce.Click += new System.EventHandler(this.mnuForce_Click);
			// 
			// mnuWebPage
			// 
			this.mnuWebPage.Index = 1;
			this.mnuWebPage.Text = "Display &Web Page";
			this.mnuWebPage.Click += new System.EventHandler(this.mnuWebPage_Click);
			// 
			// mnuCancelPending
			// 
			this.mnuCancelPending.Index = 2;
			this.mnuCancelPending.Text = "&Cancel Pending";
			this.mnuCancelPending.Click += new System.EventHandler(this.mnuCancelPending_Click);
			// 
			// mnuFixBuild
			// 
			this.mnuFixBuild.Index = 3;
			this.mnuFixBuild.Text = "&Volunteer to Fix Build";
			this.mnuFixBuild.Click += new System.EventHandler(this.mnuFixBuild_Click);
			// 
			// largeIconList
			// 
			this.largeIconList.ImageSize = new System.Drawing.Size(32, 32);
			this.largeIconList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// iconList
			// 
			this.iconList.ImageSize = new System.Drawing.Size(16, 16);
			this.iconList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuFile,
																					 this.mnuView});
			// 
			// menuFile
			// 
			this.menuFile.Index = 0;
			this.menuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.mnuFilePreferences,
																					 this.menuItem3,
																					 this.menuFileExit});
			this.menuFile.Text = "&File";
			// 
			// mnuFilePreferences
			// 
			this.mnuFilePreferences.Index = 0;
			this.mnuFilePreferences.Text = "&Settings...";
			this.mnuFilePreferences.Click += new System.EventHandler(this.mnuFilePreferences_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 1;
			this.menuItem3.Text = "-";
			// 
			// menuFileExit
			// 
			this.menuFileExit.Index = 2;
			this.menuFileExit.Text = "E&xit";
			this.menuFileExit.Click += new System.EventHandler(this.menuFileExit_Click);
			// 
			// mnuView
			// 
			this.mnuView.Index = 1;
			this.mnuView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.mnuViewIcons,
																					this.mnuViewList,
																					this.mnuViewDetails});
			this.mnuView.Text = "&View";
			this.mnuView.Popup += new System.EventHandler(this.mnuView_Popup);
			// 
			// mnuViewIcons
			// 
			this.mnuViewIcons.Index = 0;
			this.mnuViewIcons.Text = "&Icons";
			this.mnuViewIcons.Click += new System.EventHandler(this.mnuViewIcons_Click);
			// 
			// mnuViewList
			// 
			this.mnuViewList.Index = 1;
			this.mnuViewList.Text = "&List";
			this.mnuViewList.Click += new System.EventHandler(this.mnuViewList_Click);
			// 
			// mnuViewDetails
			// 
			this.mnuViewDetails.Index = 2;
			this.mnuViewDetails.Text = "&Details";
			this.mnuViewDetails.Click += new System.EventHandler(this.mnuViewDetails_Click);
			// 
			// trayIcon
			// 
			this.trayIcon.ContextMenu = this.mnuTrayContextMenu;
			this.trayIcon.Icon = null;
			this.trayIcon.Text = "CruiseControl.NET";
			this.trayIcon.Visible = true;
			this.trayIcon.DoubleClick += new System.EventHandler(this.trayIcon_DoubleClick);
			this.trayIcon.Click += new System.EventHandler(this.trayIcon_Click);
			// 
			// mnuTrayContextMenu
			// 
			this.mnuTrayContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							   this.mnuTraySettings,
																							   this.mnuShow,
																							   this.menuItem5,
																							   this.mnuTrayExit});
			// 
			// mnuTraySettings
			// 
			this.mnuTraySettings.Index = 0;
			this.mnuTraySettings.Text = "&Settings...";
			this.mnuTraySettings.Click += new System.EventHandler(this.mnuFilePreferences_Click);
			// 
			// mnuShow
			// 
			this.mnuShow.DefaultItem = true;
			this.mnuShow.Index = 1;
			this.mnuShow.Text = "Show Status &Window";
			this.mnuShow.Click += new System.EventHandler(this.mnuShow_Click);
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 2;
			this.menuItem5.Text = "-";
			// 
			// mnuTrayExit
			// 
			this.mnuTrayExit.Index = 3;
			this.mnuTrayExit.Text = "&Exit";
			this.mnuTrayExit.Click += new System.EventHandler(this.menuFileExit_Click);
			// 
			// pnlButtons
			// 
			this.pnlButtons.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pnlButtons.Controls.Add(this.btnToggleQueueView);
			this.pnlButtons.Controls.Add(this.btnForceBuild);
			this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlButtons.Location = new System.Drawing.Point(0, 260);
			this.pnlButtons.Name = "pnlButtons";
			this.pnlButtons.Size = new System.Drawing.Size(892, 45);
			this.pnlButtons.TabIndex = 1;
			// 
			// btnToggleQueueView
			// 
			this.btnToggleQueueView.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnToggleQueueView.Location = new System.Drawing.Point(105, 10);
			this.btnToggleQueueView.Name = "btnToggleQueueView";
			this.btnToggleQueueView.Size = new System.Drawing.Size(85, 23);
			this.btnToggleQueueView.TabIndex = 1;
			this.btnToggleQueueView.Text = "View &Queues";
			this.btnToggleQueueView.Click += new System.EventHandler(this.btnToggleQueueView_Click);
			// 
			// btnForceBuild
			// 
			this.btnForceBuild.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnForceBuild.Location = new System.Drawing.Point(10, 10);
			this.btnForceBuild.Name = "btnForceBuild";
			this.btnForceBuild.Size = new System.Drawing.Size(85, 23);
			this.btnForceBuild.TabIndex = 0;
			this.btnForceBuild.Text = "Force &Build";
			this.btnForceBuild.Click += new System.EventHandler(this.btnForceBuild_Click);
			// 
			// splitterQueueView
			// 
			this.splitterQueueView.Location = new System.Drawing.Point(200, 0);
			this.splitterQueueView.Name = "splitterQueueView";
			this.splitterQueueView.Size = new System.Drawing.Size(3, 260);
			this.splitterQueueView.TabIndex = 3;
			this.splitterQueueView.TabStop = false;
			this.splitterQueueView.Visible = false;
			// 
			// pnlViewQueues
			// 
			this.pnlViewQueues.Controls.Add(this.queueTreeView);
			this.pnlViewQueues.Dock = System.Windows.Forms.DockStyle.Left;
			this.pnlViewQueues.Location = new System.Drawing.Point(0, 0);
			this.pnlViewQueues.Name = "pnlViewQueues";
			this.pnlViewQueues.Size = new System.Drawing.Size(200, 260);
			this.pnlViewQueues.TabIndex = 4;
			this.pnlViewQueues.Visible = false;
			// 
			// queueTreeView
			// 
			this.queueTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.queueTreeView.ImageList = this.queueIconList;
			this.queueTreeView.Location = new System.Drawing.Point(0, 0);
			this.queueTreeView.Name = "queueTreeView";
			this.queueTreeView.Size = new System.Drawing.Size(200, 260);
			this.queueTreeView.TabIndex = 2;
			this.queueTreeView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.queueTreeView_MouseUp);
			// 
			// queueIconList
			// 
			this.queueIconList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.queueIconList.ImageSize = new System.Drawing.Size(16, 16);
			this.queueIconList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// queueContextMenu
			// 
			this.queueContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							 this.mnuQueueCancelPending});
			// 
			// mnuQueueCancelPending
			// 
			this.mnuQueueCancelPending.Index = 0;
			this.mnuQueueCancelPending.Text = "&Cancel Pending";
			this.mnuQueueCancelPending.Click += new System.EventHandler(this.mnuQueueCancelPending_Click);
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(892, 305);
			this.Controls.Add(this.lvProjects);
			this.Controls.Add(this.splitterQueueView);
			this.Controls.Add(this.pnlViewQueues);
			this.Controls.Add(this.pnlButtons);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Menu = this.mainMenu;
			this.MinimizeBox = false;
			this.Name = "MainForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "CCTray ";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
			this.pnlButtons.ResumeLayout(false);
			this.pnlViewQueues.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private void StartServerMonitoringIfQueuesDisplayed()
		{
			if (queueTreeView.Visible)
			{
				controller.StartServerMonitoring();
			}
		}

		private void menuFileExit_Click(object sender, EventArgs e)
		{
			// If Application.Exit is called, OnClosing won't get raised, which PersistWindowState 
			// relies on. Instead, we just close the window. The application will exit anyway, because
			// the window is the application's main form. We only have to ensure that MainForm_Closing
			// won't intercept it.

			//Application.Exit();
			systemShutdownInProgress = true;
			Close();
		}

		private void lvProjects_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lvProjects.SelectedItems.Count == 0)
				controller.SelectedProject = null;
			else
				controller.SelectedProject = (IProjectMonitor) lvProjects.SelectedItems[0].Tag;
		}

		private void mnuForce_Click(object sender, EventArgs e)
		{
			controller.ForceBuild();
		}

		private void mnuCancelPending_Click(object sender, EventArgs e)
		{
			controller.CancelPending();
		}

		private void btnForceBuild_Click(object sender, EventArgs e)
		{
			controller.ForceBuild();
		}

		private void mnuWebPage_Click(object sender, EventArgs e)
		{
			controller.DisplayWebPage();
		}

		private void lvProjects_DoubleClick(object sender, EventArgs e)
		{
			controller.DisplayWebPage();
		}

		private void mnuViewIcons_Click(object sender, EventArgs e)
		{
			lvProjects.View = View.LargeIcon;
		}

		private void mnuViewList_Click(object sender, EventArgs e)
		{
			lvProjects.View = View.List;
		}

		private void mnuViewDetails_Click(object sender, EventArgs e)
		{
			lvProjects.View = View.Details;
		}

		private void projectContextMenu_Popup(object sender, EventArgs e)
		{
			mnuForce.Enabled = controller.IsProjectSelected;
			mnuWebPage.Enabled = controller.IsProjectSelected;
			mnuCancelPending.Visible = controller.CanCancelPending();
			mnuFixBuild.Visible = controller.CanFixBuild();
		}
		
		private void mnuFilePreferences_Click(object sender, EventArgs e)
		{
			controller.StopProjectMonitoring();
			controller.StopServerMonitoring();

			try
			{
				if (new CCTrayMultiSettingsForm(configuration).ShowDialog() == DialogResult.OK)
				{
					configuration.Reload();
					lvProjects.Items.Clear();
					DataBindings.Clear();
					btnForceBuild.DataBindings.Clear();
					controller.UnbindToQueueTreeView(queueTreeView);
					CreateController(httpCache);
				}
			}
			finally
			{
				controller.StartProjectMonitoring();
				StartServerMonitoringIfQueuesDisplayed();
			}
		}

		private void mnuView_Popup(object sender, EventArgs e)
		{
			mnuViewIcons.Checked = (lvProjects.View == View.LargeIcon);
			mnuViewList.Checked = (lvProjects.View == View.List);
			mnuViewDetails.Checked = (lvProjects.View == View.Details);
		}

		private void mnuShow_Click(object sender, EventArgs e)
		{
			ShowStatusWindow();
		}

		private void trayIcon_Click(object sender, EventArgs e)
		{
			if (Visible)
				NativeMethods.SetForegroundWindow(Handle);
		}

		private void trayIcon_DoubleClick(object sender, EventArgs e)
		{
			if (!controller.OnDoubleClick())
			{
				ShowStatusWindow();
			}
		}

		private void ShowStatusWindow()
		{
			WindowState = FormWindowState.Normal;
			Show();
			NativeMethods.SetForegroundWindow(Handle);
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == WM_QUERYENDSESSION)
			{
				systemShutdownInProgress = true;
			}
			base.WndProc(ref m);
		}

		private void MainForm_Closing(object sender, CancelEventArgs e)
		{
			// Do not close this form, just minimize instead
			// Except when systemShutdown
			if (systemShutdownInProgress)
			{
				systemShutdownInProgress = false;
				e.Cancel = false;
			}
			else
			{
				e.Cancel = true;
				Hide();
			}
		}

		private void lvProjects_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			// Set the ListViewItemSorter as appropriate.
			ListViewItemComparer compare = lvProjects.ListViewItemSorter as ListViewItemComparer;

			if (compare == null)
			{
				lvProjects.ListViewItemSorter = new ListViewItemComparer(e.Column, true);
			}
			else
			{
				if (compare.SortColumn == e.Column)
				{
					// Sort on same column, just the opposite direction.
					compare.SortAscending = !compare.SortAscending;
				}
				else
				{
					compare.SortAscending = false;
					compare.SortColumn = e.Column;
				}
			}

			lvProjects.Sort();
		}

		private void mnuFixBuild_Click(object sender, EventArgs e)
		{
			controller.VolunteerToFixBuild();
		}

		private void btnToggleQueueView_Click(object sender, System.EventArgs e)
		{
			bool isQueueViewVisible = !pnlViewQueues.Visible;
			splitterQueueView.Visible = isQueueViewVisible;
			pnlViewQueues.Visible = isQueueViewVisible;
			UpdateViewQueuesButtonLabel();

			if (isQueueViewVisible)
			{
				controller.BindToQueueTreeView(queueTreeView);
				controller.StartServerMonitoring();
			}
			else
			{
				controller.UnbindToQueueTreeView(queueTreeView);
				controller.StopServerMonitoring();
			}
		}

		private void queueTreeView_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				Point clickPoint = new Point( e.X,e.Y );
				TreeNode clickNode = queueTreeView.GetNodeAt( clickPoint );
				if (clickNode != null)
				{
					queueTreeView.SelectedNode = clickNode;

					IntegrationQueueTreeNodeTag tag = clickNode.Tag as IntegrationQueueTreeNodeTag;
					if (tag.IsQueuedItemNode)
					{
						mnuQueueCancelPending.Enabled = !tag.IsFirstItemOnQueue;
						queueContextMenu.Show(queueTreeView, clickPoint);
					}
				}
			}
		}

		private void mnuQueueCancelPending_Click(object sender, System.EventArgs e)
		{
			if (queueTreeView.SelectedNode == null) return;
			IntegrationQueueTreeNodeTag tag = queueTreeView.SelectedNode.Tag as IntegrationQueueTreeNodeTag;
			if (tag.QueuedItemSnapshot == null) return;
			controller.CancelPendingProjectByName(tag.QueuedItemSnapshot.ProjectName);
		}

		private void UpdateViewQueuesButtonLabel()
		{
			btnToggleQueueView.Text = (pnlViewQueues.Visible) ? "&Hide Queues" : "&Show Queues" ;
		}

		// Implements the manual sorting of items by columns.
		private class ListViewItemComparer : IComparer
		{
			private int col;
			private bool ascendingOrder;

			public int SortColumn
			{
				get { return col; }
				set { col = value; }
			}

			public bool SortAscending
			{
				get { return ascendingOrder; }
				set { ascendingOrder = value; }
			}

			public ListViewItemComparer() : this(0, true)
			{
			}

			public ListViewItemComparer(int column) : this(column, true)
			{
			}

			public ListViewItemComparer(int column, bool ascending)
			{
				SortColumn = column;
				SortAscending = ascending;
			}

			public int Compare(object x, object y)
			{
				int compare =
					String.Compare(((ListViewItem) x).SubItems[SortColumn].Text, ((ListViewItem) y).SubItems[SortColumn].Text);
				if (!ascendingOrder)
				{
					compare = -compare;
				}

				return compare;
			}
		}
	}
}
