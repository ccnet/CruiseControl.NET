using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;

using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;

namespace ThoughtWorks.CruiseControl.Service
{
	public class CCService : ServiceBase
	{
		private Container components = null;
		private CruiseManager manager;

		public CCService()
		{
			// This call is required by the Windows.Forms Component Designer.
			InitializeComponent();
		}

		static void Main()
		{
			ServiceBase.Run(new ServiceBase[] { new CCService() } );
		}

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new Container();
			this.ServiceName = "CCService";
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

		/// <summary>
		/// Set things in motion so your service can do its work.
		/// </summary>
		protected override void OnStart(string[] args)
		{
			string configFile = GetConfigFilename();
			FileInfo configFileInfo = (new FileInfo(configFile));

			if (! configFileInfo.Exists)
			{
				EventLog.WriteEntry("CCService", string.Format("Config file {0} does not exist - exiting application", configFileInfo.FullName), EventLogEntryType.Error);
				return;
			}

			// in a service application the work has to be done in a separate thread so we use CruiseManager no matter what
			// we will register it on a channel if remoting is on
			// TODO: switch to use factory once cruisecontrol operates as a separate thread
			manager = new CruiseManager(new CruiseServer(new ConfigurationLoader(configFile)));
			
			if (UseRemoting()) 
				manager.RegisterForRemoting();

			manager.StartCruiseControl();
		}

		private string GetConfigFilename()
		{
			string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
			string defaultConfigFile = appDirectory + @"\ccnet.config";
			string configFile = Configuration.ConfigFileName;
			if (configFile == null || configFile.Trim().Length == 0)
				configFile = defaultConfigFile;
			return configFile;
		}

		private bool UseRemoting()
		{
			string remote = Configuration.Remoting;
			return (remote != null && remote.Trim().ToLower() == "on");
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
