using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;

using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Service
{
	public class CCService : ServiceBase
	{
		static void Main()
		{
			ServiceBase.Run(new ServiceBase[] { new CCService() } );
		}

		private Container components = null;
		private ICruiseServer server;

		public CCService()
		{
			// This call is required by the Windows.Forms Component Designer.
			InitializeComponent();
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
			// Set working directory to service executable's home directory.
			Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

			string configFile = GetConfigFilename();
			FileInfo configFileInfo = (new FileInfo(configFile));

			if (! configFileInfo.Exists)
			{
				EventLog.WriteEntry("CCService", string.Format("Config file {0} does not exist - exiting application", configFileInfo.FullName), EventLogEntryType.Error);
				return;
			}

			server = CruiseServerFactory.Create(UseRemoting(), configFile);
			server.Start();
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
			return (Configuration.Remoting != null && Configuration.Remoting.Trim().ToLower() == "on");
		}
 
		/// <summary>
		/// Stop this service.
		/// </summary>
		protected override void OnStop()
		{
			server.Abort();
		}

		protected override void OnPause()
		{
			server.Stop();
		}

		protected override void OnContinue()
		{
			server.Start();
		}
	}
}
