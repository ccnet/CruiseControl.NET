
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.Remoting;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Reflection;
using tw.ccnet.remote;

namespace CCTray
{
	public class CCTray : System.Windows.Forms.Form
	{
		public System.Windows.Forms.ContextMenu contextMenu;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.NotifyIcon trayIcon;
		private Hashtable table;
		private System.Timers.Timer CheckCC;

		private string remoteServerUrl;
		private int pollingInterval;
		
		// Was going to display this in the context popup, but it seems to be limited to 64 chars.
		private string warnings = "";

		public CCTray()
		{
			getConfigValues();
			//
			// Required for Windows Form Designer support
			//
			this.SuspendLayout();
			InitializeComponent();

			// Setup Icon 
			initializeImageTable();
			trayIcon.Icon = getIcon(IntegrationStatus.Unknown);

			// Setup Context Menu
			initializeContextMenu();
			contextMenu.Popup += new System.EventHandler(ContextMenuPopup);

			// Setup Timer
			CheckCC.Interval = pollingInterval;
			CheckCC.Elapsed += new System.Timers.ElapsedEventHandler(CheckCC_Elapsed);
			CheckCC.Enabled = true;

			updateStatus();

			this.ResumeLayout(false);
			MakeToolWindow(this.Handle);
		}

		#region Event Handlers
		private void ContextMenuPopup(object sender, EventArgs args)
		{
			updateStatus();
		}

		public void CheckCC_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			updateStatus();
		}
		#endregion

		private void getConfigValues()
		{
			remoteServerUrl = ConfigurationSettings.AppSettings["cc.net.url"];
			if (remoteServerUrl == null || remoteServerUrl.Length == 0)
			{
				remoteServerUrl = "tcp://location:1234/CruiseManager.rem";
				warnings += "Using default server location ";
			}

			string pollingIntervalString = ConfigurationSettings.AppSettings["check.interval"];
			if (pollingIntervalString == null || pollingIntervalString.Length == 0 || Convert.ToInt32(pollingIntervalString) == 0)
			{
				pollingInterval = 15000;
				warnings += "Using default interval (15 sec)";
			}
			else
			{
				pollingInterval = Convert.ToInt32(pollingIntervalString);
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.contextMenu = new System.Windows.Forms.ContextMenu();
			this.CheckCC = new System.Timers.Timer();
			((System.ComponentModel.ISupportInitialize)(this.CheckCC)).BeginInit();
			// 
			// trayIcon
			// 
			this.trayIcon.ContextMenu = this.contextMenu;
			this.trayIcon.Text = "No Connection";
			this.trayIcon.Visible = true;
			// 
			// CheckCC
			// 
			this.CheckCC.Enabled = true;
			this.CheckCC.SynchronizingObject = this;
			this.CheckCC.Elapsed += new System.Timers.ElapsedEventHandler(this.CheckCC_Elapsed);
			// 
			// CCTray
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Name = "CCTray";
			this.ShowInTaskbar = false;
			this.Text = "CCTray";
			this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
			((System.ComponentModel.ISupportInitialize)(this.CheckCC)).EndInit();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new CCTray());
		}

		public void exit(object sender, System.EventArgs e) 
		{
			this.Close();
			Application.Exit();
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		private void initializeImageTable() 
		{
			table = new Hashtable(3);
			table[IntegrationStatus.Failure] = getIcon("CCTray.Red.ico");
			table[IntegrationStatus.Success] = getIcon("CCTray.Green.ico");
			table[IntegrationStatus.Unknown] = getIcon("CCTray.Gray.ico");
		}

		private Icon getIcon(IntegrationStatus status) 
		{
			return (Icon)table[status];
		}

		private Icon getIcon(string name) 
		{
			return Icon.FromHandle(((Bitmap)Image.FromStream(getIconStream(name))).GetHicon());
		}

		private System.IO.Stream getIconStream(string name) 
		{
			return Assembly.GetCallingAssembly().GetManifestResourceStream(name);
		}

		private void initializeContextMenu() 
		{
			MenuItem exitItem = new MenuItem("exit", new System.EventHandler(exit));
			contextMenu.MenuItems.Add(exitItem);
		}

		private void updateStatus() 
		{
			try 
			{
				ProjectStatus projectStatus = getProjectStatus();
				trayIcon.Text = calculateTrayText(projectStatus);
				trayIcon.Icon = getIcon(projectStatus.BuildStatus);
			} 
			catch (Exception e) 
			{
				trayIcon.Text = failedConnectionTrayText(e);
				trayIcon.Icon = getIcon(IntegrationStatus.Unknown);
			}
		}

		private string calculateTrayText(ProjectStatus projectStatus)
		{
			return String.Format("Server Status: {0}\nActivity: {1}\nLast Build: {2}", 
				projectStatus.Status, 
				(projectStatus.Status == CruiseControlStatus.Stopped) ? ProjectActivity.Unknown : projectStatus.Activity, 
				projectStatus.BuildStatus);
		}

		private string failedConnectionTrayText(Exception e)
		{
			return "No Connection";
		}

		private ProjectStatus getProjectStatus() 
		{
			ICruiseManager remoteCC = (ICruiseManager) RemotingServices.Connect(typeof(ICruiseManager), remoteServerUrl);
			return remoteCC.GetProjectStatus();
		}

		#region Required to only display a Systray Icon
		const int GWL_EXSTYLE = -20;
		const int WS_EX_TOOLWINDOW = 0x00000080;

		private void MakeToolWindow(IntPtr window)
		{
			int windowStyle = GetWindowLong(window, GWL_EXSTYLE);
			SetWindowLong(window, GWL_EXSTYLE, windowStyle | WS_EX_TOOLWINDOW);
		}

		[DllImport("user32.dll")]
		static extern int SetWindowLong(
			IntPtr window,
			int index,
			int value);

		[DllImport("user32.dll")]
		static extern int GetWindowLong(
			IntPtr window,
			int index);
		#endregion
	}
}
