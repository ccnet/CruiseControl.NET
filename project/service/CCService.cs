using System;
using System.Configuration;
using System.IO;
using System.ServiceProcess;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Service
{
	public class CCService : ServiceBase
	{
		public const string DefaultServiceName = "CCService";
		private const string DefaultConfigFileName = "ccnet.config";
		private readonly string DefaultDirectory = AppDomain.CurrentDomain.BaseDirectory;

		private ICruiseServer server;

		public CCService()
		{
			ServiceName = LookupServiceName();
		}

		private string ConfigFilename
		{
			get 
			{
				string configFilename = ConfigurationSettings.AppSettings["ccnet.config"];
				return StringUtil.IsBlank(configFilename) ? DefaultConfigFilePath() : configFilename;
			}
		}

		private string Remoting
		{
			get { return ConfigurationSettings.AppSettings["remoting"]; }
		}

		protected override void OnStart(string[] args)
		{
			// Set working directory to service executable's home directory.
			Directory.SetCurrentDirectory(DefaultDirectory);

			VerifyConfigFileExists();
			CreateAndStartCruiseServer();
		}

		private string DefaultConfigFilePath()
		{
			return Path.Combine(DefaultDirectory, DefaultConfigFileName);			
		}

		private void VerifyConfigFileExists()
		{
			FileInfo configFileInfo = new FileInfo(ConfigFilename);
			if (!configFileInfo.Exists)
			{
				throw new Exception(string.Format("CruiseControl.NET configuration file {0} does not exist.", configFileInfo.FullName));
			}			
		}
		
		private void CreateAndStartCruiseServer()
		{
			server = new CruiseServerFactory().Create(UseRemoting(), ConfigFilename);
			server.Start();
		}

		private bool UseRemoting()
		{
			return (Remoting != null && Remoting.Trim().ToLower() == "on");
		}

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

		private string LookupServiceName()
		{
			string serviceName = ConfigurationSettings.AppSettings["service.name"];
			return StringUtil.IsBlank(serviceName) ? DefaultServiceName : serviceName;
		}

		private static void Main()
		{
			ServiceBase.Run(new ServiceBase[] {new CCService()});
		}
	}
}