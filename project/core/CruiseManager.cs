using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Threading;

using tw.ccnet.core;
using tw.ccnet.core.configuration;
using tw.ccnet.remote;

namespace tw.ccnet.core 
{
	/// <summary>
	/// Manages an instance of the CruiseControl.NET main process, an exposes
	/// this interface via remoting.  The CCTray is one such example of an
	/// application that may make use of this remote interface.
	/// </summary>
	public class CruiseManager : MarshalByRefObject, ICruiseManager 
	{
		public const int TCP_PORT = 1234;

		ICruiseControl _cruiseControl; 
		ConfigurationLoader _loader; 
		Thread _cruiseControlThread;

		#region Constructors

		public CruiseManager(string configFileName)
		{
			_loader = new ConfigurationLoader(configFileName);
			_cruiseControl = new CruiseControl(_loader);
		}

		/// <summary>
		/// This constructor is intended for testing purposes, and doesn't create
		/// or set a configuration loader.  Calls to <see cref="Configuration"/>
		/// properties will fail.
		/// </summary>
		/// <param name="cruiseControl"></param>
		internal CruiseManager(ICruiseControl cruiseControl)
		{
			_cruiseControl = cruiseControl;
			_loader = null;
		}


		#endregion

		void InitializeThread()
		{
			_cruiseControlThread = new Thread(new ThreadStart(_cruiseControl.Start));
			_cruiseControlThread.Start();
			_cruiseControlThread.Name = "CruiseControl.NET";
		}
		

		#region Starting and stopping CruiseControl.NET

		public void StartCruiseControl()
		{
			if (_cruiseControlThread==null || !_cruiseControlThread.IsAlive)
			{
				InitializeThread();
			}
			_cruiseControl.Start();
		}
		
		public void StopCruiseControl()
		{
			Trace.WriteLine("CruiseControl is stopping");
			_cruiseControl.Stop();
		}
		
		public void StopCruiseControlNow()
		{
			Trace.WriteLine("CruiseControl stopped");
			_cruiseControlThread.Abort();
			_cruiseControlThread = null;
		}
		

		#endregion

		#region Getting status of server and projects

		public CruiseControlStatus GetStatus()
		{
			if(_cruiseControlThread == null)	
				return CruiseControlStatus.Stopped;
			else
			{
				if(_cruiseControl.Stopped)
				{
					if(_cruiseControlThread.IsAlive)
					{
						return CruiseControlStatus.WillBeStopped;
					}
					else
					{
						return CruiseControlStatus.Stopped;
					}
				}
				else
				{
					return CruiseControlStatus.Running;
				}
			}
		}				

		public ProjectStatus GetProjectStatus()
		{
			IEnumerator e =_cruiseControl.Projects.GetEnumerator();
			e.MoveNext();
			Project p = (Project)e.Current;
			return new ProjectStatus(GetStatus(), p.GetLatestBuildStatus(), p.CurrentActivity, p.Name, p.WebURL, p.LastIntegrationResult.StartTime, p.LastIntegrationResult.Label); 
		}


		#endregion

		#region Forcing a build

		public void ForceBuild(string projectName)
		{
			IProject project = _cruiseControl.GetProject(projectName);

			// tell the project's schedule that we want a build forced
			project.Schedule.ForceBuild();
		}

		#endregion

		#region Properties

		public string Configuration
		{
			get 
			{ 
				StreamReader stream = new StreamReader(_loader.ConfigFile);
				try 
				{
					return stream.ReadToEnd();
				} 
				finally 
				{
					stream.Close();
				}            
			}
			set
			{ 
				StreamWriter stream = new StreamWriter(_loader.ConfigFile);
				try 
				{
					stream.Write(value);
				} 
				finally 
				{
					stream.Close();
				}            
			}
		}


		#endregion

		public override object InitializeLifetimeService()
		{
			return null;
		}

		public void RegisterForRemoting()
		{
			string configFile = System.Reflection.Assembly.GetEntryAssembly().Location + ".config";
			string uri = "CruiseManager.rem";

			RemotingConfiguration.Configure(configFile);
			RemotingServices.Marshal(this, uri);
 
			string url = uri;
			try 
			{
				IChannelReceiver channel = (IChannelReceiver)ChannelServices.RegisteredChannels[0];
				url = channel.GetUrlsForUri(uri)[0];

				ICruiseManager marshalledObject = (ICruiseManager) RemotingServices.Connect(typeof(ICruiseManager), url);
				marshalledObject.GetStatus(); // this will throw an exception if it didn't connect
				Console.WriteLine("Listening on " + url);
			} 
			catch 
			{
				throw new Exception("Couldn't listen on " + url);
			}
		}
	}
}