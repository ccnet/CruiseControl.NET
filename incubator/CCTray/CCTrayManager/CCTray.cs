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

namespace CCManager
{
	public class CCTray : System.Windows.Forms.Form
	{
		public System.Windows.Forms.ContextMenu contextMenu;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.NotifyIcon trayIcon;
		private Hashtable table;
		private System.Timers.Timer CheckCC;
		private CruiseControlStatus ccStatus;


		public CCTray()
		{

			//
			// Required for Windows Form Designer support
			//
			this.SuspendLayout();
			InitializeComponent();
			initializeImageTable();
			initializeContextMenu();
			trayIcon.Icon = getIcon(IntegrationStatus.Unknown);
			CheckCC_Elapsed(null, null);
			CheckCC.Interval = Convert.ToInt32(ConfigurationSettings.AppSettings["check.interval"]);
			CheckCC.Enabled = true;
			
			contextMenu.Popup += new System.EventHandler(ContextMenuPopup);

			string machineName = ConfigurationSettings.AppSettings["service.machine"];
			string serviceName = ConfigurationSettings.AppSettings["service.name"];

			this.ResumeLayout(false);
			MakeToolWindow(this.Handle);
		}

		private Icon getIcon(IntegrationStatus status) 
		{
			return (Icon)table[status];
		}

		private void initializeImageTable() 
		{
			table = new Hashtable(3);
			table[IntegrationStatus.Failure] = getIcon("CCTrayManager.Red.ico");
			table[IntegrationStatus.Success] = getIcon("CCTrayManager.Green.ico");
			table[IntegrationStatus.Unknown] = getIcon("CCTrayManager.Gray.ico");
			
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
			MenuItem startItem = new MenuItem("start CC", new EventHandler(startCC));
			startItem.Enabled = false;
			contextMenu.MenuItems.Add(startItem);
			MenuItem stopItem = new MenuItem("stop CC", new EventHandler(stopCC));
			stopItem.Enabled = false;
			contextMenu.MenuItems.Add(stopItem);
		}

//		private ServiceController getServiceController(string machineName) 
//		{
//			ServiceController[] services = ServiceController.GetServices();
//			foreach (ServiceController service in services) 
//			{
//				if (service.ServiceName == "CCService")
//					return service;
//			}
//
//			return null;
//		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
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

		private ICruiseManager getManager ()
		{
			return (ICruiseManager) RemotingServices.Connect(typeof(ICruiseManager), ConfigurationSettings.AppSettings["cc.net.url"]);
		}

		private ProjectStatus getProjectStatus() 
		{
			return getManager().GetProjectStatus();
		}

		private void startCC(object sender, EventArgs args) 
		{
			try 
			{
				getManager().StartCruiseControl();
				CheckCC_Elapsed(this, null);
			} 
			catch (Exception) 
			{
			}
		}

		private void stopCC(object sender, EventArgs args) 
		{
			try 
			{
				getManager().StopCruiseControl();
				CheckCC_Elapsed(this, null);
			} 
			catch (Exception) 
			{
			}
		}

		public void exit(object sender, System.EventArgs e) 
		{
			this.Close();
			Application.Exit();
		}

		private void ContextMenuPopup(object sender, EventArgs args)
		{
			updateStatus();
			contextMenu.MenuItems[1].Enabled = (ccStatus == CruiseControlStatus.Stopped);
			contextMenu.MenuItems[2].Enabled = (ccStatus == CruiseControlStatus.Running);
		}

		public void CheckCC_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			updateStatus();
		}

		private void updateStatus() 
		{
			try 
			{
				ProjectStatus status = getProjectStatus();

				ccStatus = status.Status;
				ProjectActivity activity = status.Activity;
				if (ccStatus == CruiseControlStatus.Stopped)
					activity = ProjectActivity.Unknown;
				IntegrationStatus buildStatus = status.BuildStatus;
				
				trayIcon.Text = string.Format("Status: {0}\nActivity: {1}\nLast Build: {2}", ccStatus, activity, buildStatus.ToString());

				trayIcon.Icon = getIcon(buildStatus);
			} 
			catch (Exception) 
			{
				trayIcon.Text = "No Connection";
				trayIcon.Icon = getIcon(IntegrationStatus.Unknown);
				ccStatus = CruiseControlStatus.Unknown;
			}
		}

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


	}
}
