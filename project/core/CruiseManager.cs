using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Threading;

using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Configuration;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core 
{
	/// <summary>
	/// Manages an instance of the CruiseControl.NET main process, an exposes
	/// this interface via remoting.  The CCTray is one such example of an
	/// application that may make use of this remote interface.
	/// </summary>
	public class CruiseManager : MarshalByRefObject, ICruiseManager, ICruiseServer
	{
		public const int TCP_PORT = 1234;

		private ICruiseControl _cruiseControl; 

		public CruiseManager(ICruiseControl cruiseControl)
		{
			_cruiseControl = cruiseControl;
		}

		#region Starting and stopping CruiseControl.NET

		public void StartCruiseControl()
		{
			_cruiseControl.Start();
			LogUtil.Log("CruiseManager", "CruiseControl is stopping");
		}
		
		public void StopCruiseControl()
		{
			_cruiseControl.Stop();
			LogUtil.Log("CruiseManager", "CruiseControl is stopping");
		}
		
		public void StopCruiseControlNow()
		{
			Abort();
		}

		public void Abort()
		{
			_cruiseControl.Abort();
			LogUtil.Log("CruiseManager", "CruiseControl stopped");
		}
		
		void ICruiseServer.Start()
		{
			RegisterForRemoting();
			StartCruiseControl();
		}

		void ICruiseServer.Stop()
		{
			StopCruiseControl();
		}

		#endregion

		#region Getting status of server and projects

		public CruiseControlStatus GetStatus()
		{
			return _cruiseControl.Status;
		}				

		public ProjectStatus GetProjectStatus()
		{
			IEnumerator e =_cruiseControl.Projects.GetEnumerator();
			e.MoveNext();
			Project p = (Project)e.Current;
			return new ProjectStatus(GetStatus(), p.GetLatestBuildStatus(), p.CurrentActivity, p.Name, p.WebURL, p.LastIntegrationResult.StartTime, p.LastIntegrationResult.Label); 
		}
		#endregion

		public void ForceBuild(string projectName)
		{
			((ICruiseServer)_cruiseControl).ForceBuild(projectName);
		}

		#region Properties

		public string Configuration
		{
			get { return _cruiseControl.Configuration.ReadXml(); }
			set { _cruiseControl.Configuration.WriteXml(value); }
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
				LogUtil.Log("CruiseManager", "Listening on " + url);
			} 
			catch 
			{
				throw new Exception("Couldn't listen on " + url);
			}
		}
	}
}