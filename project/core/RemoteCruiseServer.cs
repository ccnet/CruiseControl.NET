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
		private static readonly string ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

		public RemoteCruiseServer(ICruiseServer server) : this(server, ConfigurationFile)
		{
		}

		public RemoteCruiseServer(ICruiseServer server, string remotingConfigurationFile)
		{
			_server = server;
			RemotingConfiguration.Configure(remotingConfigurationFile);
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

		public void WaitForExit()
		{
			_server.WaitForExit();
		}

		public ICruiseManager CruiseManager
		{
			get { return _server.CruiseManager; }
		}

		private void RegisterForRemoting()
		{
			MarshalByRefObject marshalByRef = (MarshalByRefObject)_server.CruiseManager;
			RemotingServices.Marshal(marshalByRef, URI);
 
			foreach (IChannel channel in ChannelServices.RegisteredChannels)
			{
				Log.Info("Registered channel: " + channel.ChannelName);
				if (channel is IChannelReceiver)
				{
					foreach (string url in ((IChannelReceiver)channel).GetUrlsForUri(URI))
					{
						Log.Info("CruiseManager: Listening on url: " + url);
					}
				}
			}
		}

		void IDisposable.Dispose()
		{
			Log.Info("Disconnecting remote server: ");
			RemotingServices.Disconnect((MarshalByRefObject)_server.CruiseManager);
			foreach (IChannel channel in ChannelServices.RegisteredChannels)
			{
				Log.Info("Unregistering channel: " + channel.ChannelName);
				ChannelServices.UnregisterChannel(channel);
			}
			_server.Dispose();
		}
	}
}
