using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core
{
	public class RemoteCruiseServer : ICruiseServer
	{
		public const string URI = "CruiseManager.rem";

		private ICruiseServer _server;

		public RemoteCruiseServer(ICruiseServer server)
		{
			_server = server;
			RegisterForRemoting();
		}

		public void Start()
		{
			_server.Start();
		}

		public void Stop()
		{
			_server.Stop();
		}

		public void Abort()
		{
			_server.Abort();
		}

		public void ForceBuild(string projectName)
		{
			_server.ForceBuild(projectName);
		}

		public void RegisterForRemoting()
		{
			CruiseManager manager = new CruiseManager((ICruiseControl)_server);

			string configFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

			RemotingConfiguration.Configure(configFile);
			RemotingServices.Marshal(manager, URI);
 
			VerifyCruiseManagerIsRemoted();
		}

		private void VerifyCruiseManagerIsRemoted()
		{
			IChannelReceiver channel = (IChannelReceiver)ChannelServices.RegisteredChannels[0];
			string url = channel.GetUrlsForUri(URI)[0];
			ICruiseManager marshalledObject = (ICruiseManager) RemotingServices.Connect(typeof(ICruiseManager), url);
			marshalledObject.GetStatus(); // this will throw an exception if it didn't connect

			Log.Info("CruiseManager: Listening on " + url);
		}
	}
}
