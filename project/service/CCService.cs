using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using tw.ccnet.core;
using tw.ccnet.core.configuration;
using System.Runtime.Remoting;
using System.Configuration;

namespace tw.ccnet.service
{
	public class CCService : System.ServiceProcess.ServiceBase
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private CruiseManager manager;

		public CCService()
		{
			// This call is required by the Windows.Forms Component Designer.
			InitializeComponent();
			string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
			string appConfigFile = appDirectory + @"\service.exe.config";
			string defaultConfigFile = appDirectory + "\\ccnet.confg";
			string configFile = ConfigurationSettings.AppSettings["ccnet.config"];
			if (configFile == null || configFile.Length == 0)
				configFile = defaultConfigFile;

			FileInfo configFileInfo = (new FileInfo(configFile));

			if (! configFileInfo.Exists)
			{
				EventLog.WriteEntry("CCService", string.Format("Config file {0} does not exist - exiting application", configFileInfo.FullName), EventLogEntryType.Error);
				return;
			}

			// in a service application the work has to be done in a separate thread so we use CruiseManager no matter what
			// we will register it on a channel if remoting is on
			manager = new CruiseManager();
			manager.InitializeCruiseControl(configFile);
			if (useRemoting()) 
			{
				RemotingConfiguration.Configure(appConfigFile);
				RemotingServices.Marshal(manager, "CruiseManager.rem");
			}
		}

		static void Main()
		{
			System.ServiceProcess.ServiceBase[] ServicesToRun;
	
			ServicesToRun = new System.ServiceProcess.ServiceBase[] { new CCService() };

			System.ServiceProcess.ServiceBase.Run(ServicesToRun);
		}

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			this.ServiceName = "CCService";
		}

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

		/// <summary>
		/// Set things in motion so your service can do its work.
		/// </summary>
		protected override void OnStart(string[] args)
		{
			if (manager != null)
				manager.StartCruiseControl();
		}

		private bool useRemoting() 
		{

			string remote = ConfigurationSettings.AppSettings["remoting"];
			return (remote != null && remote == "on");
		}
 
		/// <summary>
		/// Stop this service.
		/// </summary>
		protected override void OnStop()
		{
			manager.StopCruiseControlNow();
		}

		protected override void OnPause()
		{
			manager.StopCruiseControl();
		}

		protected override void OnContinue()
		{
			manager.StartCruiseControl();
		}

		


	}
}
