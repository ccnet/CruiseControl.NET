using System;
using System.Collections;
using System.Threading;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	public class CruiseServer : ICruiseServer
	{
		private IConfigurationContainer _config;
		private ICruiseManager _manager;
		private ManualResetEvent _monitor = new ManualResetEvent(true);

		public CruiseServer(IConfigurationContainer config)
		{
			_config = config;
			_config.AddConfigurationChangedHandler(new ConfigurationChangedHandler(ResetConfiguration));

			_manager = new CruiseManager(config);
		}

		public void Start()
		{
			Log.Info("Starting CruiseControl.NET Server");
			_monitor.Reset();
			StartIntegrators(_config);
		}

		private void StartIntegrators(IConfiguration configuration)
		{
			foreach (IProjectIntegrator integrator in configuration.Integrators)
			{
				integrator.Start();
			}
		}

		public void Stop()
		{
			Log.Info("Stopping CruiseControl.NET Server");
			StopIntegrators();
			_monitor.Set();
		}

		private void StopIntegrators()
		{
			foreach (IProjectIntegrator integrator in _config.Integrators)
			{
				integrator.Stop();
			}
			WaitForIntegratorsToExit();
		}

		public void Abort()
		{
			Log.Info("Aborting CruiseControl.NET Server");
			foreach (IProjectIntegrator integrator in _config.Integrators)
			{
				integrator.Abort();
			}
			WaitForIntegratorsToExit();
			_monitor.Set();
		}

		public void WaitForExit()
		{
			_monitor.WaitOne();
		}

		public void ResetConfiguration(IConfiguration newConfig)
		{
			Log.Info("Configuration changed: Restarting CruiseControl.NET Server ");

			// lock
			StopIntegrators();
			StartIntegrators(newConfig);
		}

		private void WaitForIntegratorsToExit()
		{
			foreach (IProjectIntegrator integrator in _config.Integrators)
			{
				integrator.WaitForExit();
			}		
		}

		public ICruiseManager CruiseManager
		{
			get { return _manager; }
		}

		public IConfiguration Configuration
		{
			get { return _config; }
		}
	}
}
