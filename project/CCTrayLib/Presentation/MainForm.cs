using System;
using System.ComponentModel;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class MainForm : Form
	{
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
		private MenuItem menuItem1;
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

		public MainForm()
		{
			InitializeComponent();
		}


		public void AttachController(MainFormController controller)
		{
			this.controller = controller;

			DataBindings.Add("Icon", controller.ProjectStateIconAdaptor, "Icon");
			trayIcon.BindToIconProvider(controller.ProjectStateIconAdaptor);

			controller.PotentiallyHookUpBuildOccurredEvents(trayIcon);
			controller.BindToListView(lvProjects);

			ApplyDataBinding();

			controller.StartMonitoring();

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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof (MainForm));
			this.lvProjects = new System.Windows.Forms.ListView();
			this.colProject = new System.Windows.Forms.ColumnHeader();
			this.colActivity = new System.Windows.Forms.ColumnHeader();
			this.colLastBuildLabel = new System.Windows.Forms.ColumnHeader();
			this.projectContextMenu = new System.Windows.Forms.ContextMenu();
			this.mnuForce = new System.Windows.Forms.MenuItem();
			this.mnuWebPage = new System.Windows.Forms.MenuItem();
			this.largeIconList = new System.Windows.Forms.ImageList(this.components);
			this.iconList = new System.Windows.Forms.ImageList(this.components);
			this.mainMenu = new System.Windows.Forms.MainMenu();
			this.menuFile = new System.Windows.Forms.MenuItem();
			this.mnuFilePreferences = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuFileExit = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.mnuViewIcons = new System.Windows.Forms.MenuItem();
			this.mnuViewList = new System.Windows.Forms.MenuItem();
			this.mnuViewDetails = new System.Windows.Forms.MenuItem();
			this.trayIcon = new ThoughtWorks.CruiseControl.CCTrayLib.Presentation.TrayIcon();
			this.panel1 = new System.Windows.Forms.Panel();
			this.btnForceBuild = new System.Windows.Forms.Button();
			this.colDetail = new System.Windows.Forms.ColumnHeader();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// lvProjects
			// 
			this.lvProjects.Columns.AddRange(new System.Windows.Forms.ColumnHeader[]
				{
					this.colProject,
					this.colActivity,
					this.colDetail,
					this.colLastBuildLabel
				});
			this.lvProjects.ContextMenu = this.projectContextMenu;
			this.lvProjects.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvProjects.LargeImageList = this.largeIconList;
			this.lvProjects.Location = new System.Drawing.Point(0, 0);
			this.lvProjects.MultiSelect = false;
			this.lvProjects.Name = "lvProjects";
			this.lvProjects.Size = new System.Drawing.Size(892, 305);
			this.lvProjects.SmallImageList = this.iconList;
			this.lvProjects.TabIndex = 0;
			this.lvProjects.View = System.Windows.Forms.View.Details;
			this.lvProjects.DoubleClick += new System.EventHandler(this.lvProjects_DoubleClick);
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
			// colLastBuildLabel
			// 
			this.colLastBuildLabel.Text = "Last Build Label";
			this.colLastBuildLabel.Width = 192;
			// 
			// projectContextMenu
			// 
			this.projectContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[]
				{
					this.mnuForce,
					this.mnuWebPage
				});
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
			// largeIconList
			// 
			this.largeIconList.ImageSize = new System.Drawing.Size(32, 32);
			this.largeIconList.ImageStream = ((System.Windows.Forms.ImageListStreamer) (resources.GetObject("largeIconList.ImageStream")));
			this.largeIconList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// iconList
			// 
			this.iconList.ImageSize = new System.Drawing.Size(16, 16);
			this.iconList.ImageStream = ((System.Windows.Forms.ImageListStreamer) (resources.GetObject("iconList.ImageStream")));
			this.iconList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[]
				{
					this.menuFile,
					this.menuItem1
				});
			// 
			// menuFile
			// 
			this.menuFile.Index = 0;
			this.menuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[]
				{
					this.mnuFilePreferences,
					this.menuItem3,
					this.menuFileExit
				});
			this.menuFile.Text = "&File";
			// 
			// mnuFilePreferences
			// 
			this.mnuFilePreferences.Index = 0;
			this.mnuFilePreferences.Text = "&Preferences";
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
			// menuItem1
			// 
			this.menuItem1.Index = 1;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[]
				{
					this.mnuViewIcons,
					this.mnuViewList,
					this.mnuViewDetails
				});
			this.menuItem1.Text = "&View";
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
			this.trayIcon.ContextMenu = null;
			this.trayIcon.Icon = null;
			this.trayIcon.Text = "CruiseControl.NET\n(This tooltip information still to be implemented)";
			this.trayIcon.Visible = true;
			this.trayIcon.Click += new System.EventHandler(this.trayIcon_Click);
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
			this.btnForceBuild.Text = "Force Build";
			this.btnForceBuild.Click += new System.EventHandler(this.btnForceBuild_Click);
			// 
			// colDetail
			// 
			this.colDetail.Text = "Detail";
			this.colDetail.Width = 282;
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(892, 305);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.lvProjects);
			this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
			this.Menu = this.mainMenu;
			this.Name = "MainForm";
			this.Text = "CruiseControl.NET";
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


		private void trayIcon_Click(object sender, EventArgs e)
		{
			WindowState = FormWindowState.Normal;
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			ShowInTaskbar = WindowState != FormWindowState.Minimized;
		}

		private void ApplyDataBinding()
		{
			controller.IsProjectSelectedChanged += new EventHandler(Controller_IsProjectSelectedChanged);
			btnForceBuild.DataBindings.Add("Enabled", controller, "IsProjectSelected");
		}

		private void Controller_IsProjectSelectedChanged(object sender, EventArgs e)
		{
			// unfortunately menu items don't support data binding, so we have to do this manually
			mnuForce.Enabled = controller.IsProjectSelected;
			mnuWebPage.Enabled = controller.IsProjectSelected;
		}

		private void mnuFilePreferences_Click(object sender, EventArgs e)
		{
			controller.ShowPreferencesDialog();
		}
	}
}