using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;
using ThoughtWorks.CruiseControl.CCTrayLib.ServerConnection;

namespace CCTrayMulti
{
	public class MainForm : Form
	{
		private MainMenu mainMenu1;
		private MenuItem menuFile;
		private MenuItem menuFileExit;
		private MenuItem menuItem1;
		private MenuItem menuItem2;
		private ListView lvProjects;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ColumnHeader colProject;
		private System.Windows.Forms.ColumnHeader colProjectStatus;
		private System.ComponentModel.IContainer components;


		public MainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			ApplyTemporaryHackToGetSomethingUpAndRunning();

		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
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
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuFile = new System.Windows.Forms.MenuItem();
			this.menuFileExit = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.colProject = new System.Windows.Forms.ColumnHeader();
			this.colProjectStatus = new System.Windows.Forms.ColumnHeader();
			this.SuspendLayout();
			// 
			// lvProjects
			// 
			this.lvProjects.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						 this.colProject,
																						 this.colProjectStatus});
			this.lvProjects.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvProjects.Location = new System.Drawing.Point(0, 0);
			this.lvProjects.Name = "lvProjects";
			this.lvProjects.Size = new System.Drawing.Size(292, 266);
			this.lvProjects.SmallImageList = this.imageList1;
			this.lvProjects.TabIndex = 0;
			this.lvProjects.View = System.Windows.Forms.View.Details;
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuFile,
																					  this.menuItem1});
			// 
			// menuFile
			// 
			this.menuFile.Index = 0;
			this.menuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuFileExit});
			this.menuFile.Text = "&File";
			// 
			// menuFileExit
			// 
			this.menuFileExit.Index = 0;
			this.menuFileExit.Text = "&Exit";
			this.menuFileExit.Click += new System.EventHandler(this.menuFileExit_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 1;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem2});
			this.menuItem1.Text = "Test";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 0;
			this.menuItem2.Text = "Add a build";
			this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
			// 
			// imageList1
			// 
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// colProject
			// 
			this.colProject.Text = "Project";
			this.colProject.Width = 160;
			// 
			// colProjectStatus
			// 
			this.colProjectStatus.Text = "Status";
			this.colProjectStatus.Width = 100;
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.Add(this.lvProjects);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Menu = this.mainMenu1;
			this.Name = "MainForm";
			this.Text = "CruiseControl.NET";
			this.ResumeLayout(false);

		}

		#endregion

		private void menuFileExit_Click( object sender, EventArgs e )
		{
			Application.Exit();
		}

		private void menuItem2_Click( object sender, EventArgs e )
		{
			lvProjects.Items.Add( new ListViewItem( "test", 1 ) );
		}

		private Poller poller;

		private void ApplyTemporaryHackToGetSomethingUpAndRunning()
		{
			RemoteCruiseManagerFactory remoteCruiseManagerFactory = new RemoteCruiseManagerFactory();
			ICruiseProjectManagerFactory factory = new CruiseProjectManagerFactory( remoteCruiseManagerFactory );

			CCTrayMultiConfiguration configuration = new CCTrayMultiConfiguration(factory, "settings.xml");

			IProjectMonitor[] monitors = configuration.GetProjectStatusMonitors();

			foreach (IProjectMonitor monitor in monitors)
			{
				lvProjects.Items.Add(new ProjectStatusListViewItemAdaptor().Create(monitor));
			}

			poller = new Poller( 5000, monitors );
			poller.Start();

			Debug.WriteLine( "started" );
		}

	}
}