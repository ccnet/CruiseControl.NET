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
		private IConfiguration _config;
		private ICruiseManager _manager;
		private ManualResetEvent _monitor = new ManualResetEvent(true);

		public CruiseServer(IConfiguration config)
		{
			_config = config;
			_manager = new CruiseManager(config);
		}

		public CruiseServer(IConfigurationLoader configLoader) : this(configLoader.Load())
		{
			configLoader.AddConfigurationChangedHandler(new ConfigurationChangedHandler(ResetConfiguration));
		}

		public void Start()
		{
			Log.Info("Starting CruiseControl.NET Server");
			_monitor.Reset();
			StartIntegrators();
		}

		private void StartIntegrators()
		{
			foreach (IProjectIntegrator integrator in _config.Integrators)
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

		public void ResetConfiguration(IConfiguration configuration)
		{
			Log.Info("Configuration changed: Restarting CruiseControl.NET Server ");

			// lock
			StopIntegrators();
			_config = configuration;
			StartIntegrators();
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
