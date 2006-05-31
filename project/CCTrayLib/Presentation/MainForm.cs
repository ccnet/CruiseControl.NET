using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

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
		private Panel panel1;
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
		private PersistWindowState windowState;

		public MainForm(ICCTrayMultiConfiguration configuration)
		{
			this.configuration = configuration;

			InitializeComponent();
			HookPersistentWindowState();
			CreateController();

			controller.StartMonitoring();
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
		}

		private void OnSaveState(object sender, WindowStateEventArgs e)
		{
			// save additional state information to registry
			e.Key.SetValue("ProjectColumnWidth", colProject.Width);
			e.Key.SetValue("ActivityColumnWidth", colActivity.Width);
			e.Key.SetValue("DetailColumnWidth", colDetail.Width);
			e.Key.SetValue("LastBuildLabelColumnWidth", colLastBuildLabel.Width);
			e.Key.SetValue("LastBuildTimeColumnWidth", colLastBuildTime.Width);
		}

		private void CreateController()
		{
			controller = new MainFormController(configuration, this);

			DataBindings.Add("Icon", controller.ProjectStateIconAdaptor, "Icon");

			controller.PopulateImageList(iconList);
			controller.PopulateImageList(largeIconList);
			controller.BindToTrayIcon(trayIcon);
			controller.BindToListView(lvProjects);

			controller.IsProjectSelectedChanged += new EventHandler(Controller_IsProjectSelectedChanged);
			btnForceBuild.DataBindings.Add("Enabled", controller, "IsProjectSelected");
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.btnForceBuild = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
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
			this.lvProjects.Location = new System.Drawing.Point(0, 0);
			this.lvProjects.MultiSelect = false;
			this.lvProjects.Name = "lvProjects";
			this.lvProjects.Size = new System.Drawing.Size(892, 260);
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
																							   this.mnuFixBuild});
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
			// mnuFixBuild
			// 
			this.mnuFixBuild.Index = 2;
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
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Controls.Add(this.btnForceBuild);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 260);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(892, 45);
			this.panel1.TabIndex = 1;
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
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(892, 305);
			this.Controls.Add(this.lvProjects);
			this.Controls.Add(this.panel1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Menu = this.mainMenu;
			this.MinimizeBox = false;
			this.Name = "MainForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "CCTray ";
			this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
			this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private void menuFileExit_Click(object sender, EventArgs e)
		{
			Application.Exit();
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

		private void Controller_IsProjectSelectedChanged(object sender, EventArgs e)
		{
			// unfortunately menu items don't support data binding, so we have to do this manually
			mnuForce.Enabled = controller.IsProjectSelected;
			mnuWebPage.Enabled = controller.IsProjectSelected;
			mnuFixBuild.Visible = controller.CanFixBuild();
		}

		private void mnuFilePreferences_Click(object sender, EventArgs e)
		{
			controller.StopMonitoring();

			try
			{
				if (new CCTrayMultiSettingsForm(configuration).ShowDialog() == DialogResult.OK)
				{
					configuration.Reload();
					lvProjects.Items.Clear();
					DataBindings.Clear();
					btnForceBuild.DataBindings.Clear();
					CreateController();
				}
			}
			finally
			{
				controller.StartMonitoring();
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