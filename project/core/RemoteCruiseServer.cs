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
		public const string DefaultUri = "tcp://localhost:21234/" + URI;

		private ICruiseServer _server;
		private bool _disposed;

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

		public void Start(string project)
		{
			_server.Start(project);
		}

		public void Stop(string project)
		{
			_server.Stop(project);
		}

		public ICruiseManager CruiseManager
		{
			get { return _server.CruiseManager; }
		}

		public ProjectStatus[] GetProjectStatus()
		{
			return _server.GetProjectStatus();
		}

		public void ForceBuild(string projectName)
		{
			_server.ForceBuild(projectName);
		}

		public void Request(string projectName, IntegrationRequest request)
		{
			_server.Request(projectName, request);
		}

		public void WaitForExit(string projectName)
		{
			_server.WaitForExit(projectName);
		}

		public string GetLatestBuildName(string projectName)
		{
			return _server.GetLatestBuildName(projectName);
		}

		public string[] GetBuildNames(string projectName)
		{
			return _server.GetBuildNames(projectName);
		}

		public string GetVersion()
		{
			return _server.GetVersion();
		}

		public string[] GetMostRecentBuildNames(string projectName, int buildCount)
		{
			return _server.GetMostRecentBuildNames(projectName, buildCount);
		}

		public string GetLog(string projectName, string buildName)
		{
			return _server.GetLog(projectName, buildName);
		}

		public string GetServerLog()
		{
			return _server.GetServerLog();
		}

		public void AddProject(string serializedProject)
		{
			_server.AddProject(serializedProject);
		}

		public void DeleteProject(string projectName, bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment)
		{
			_server.DeleteProject(projectName, purgeWorkingDirectory, purgeArtifactDirectory, purgeSourceControlEnvironment);
		}

		public string GetProject(string name)
		{
			return _server.GetProject(name);
		}

		public void UpdateProject(string projectName, string serializedProject)
		{
			_server.UpdateProject(projectName, serializedProject);
		}

		public ExternalLink[] GetExternalLinks(string projectName)
		{
			return _server.GetExternalLinks(projectName);
		}

		public void SendMessage(string projectName, Message message)
		{
			_server.SendMessage(projectName, message);
		}

		public string GetArtifactDirectory(string projectName)
		{
			return _server.GetArtifactDirectory(projectName);
		}

		public string GetStatisticsDocument(string projectName)
		{
			return _server.GetStatisticsDocument(projectName);
		}

		private void RegisterForRemoting()
		{
			MarshalByRefObject marshalByRef = (MarshalByRefObject)_server.CruiseManager;
			RemotingServices.Marshal(marshalByRef, URI);
 
			foreach (IChannel channel in ChannelServices.RegisteredChannels)
			{
				Log.Info("Registered channel: " + channel.ChannelName);

				// GetUrlsForUri is not support on cross-AppDomain channels on mono (as of 1.1.8.3)
				if (ExecutionEnvironment.IsRunningOnWindows)
				{
					if (channel is IChannelReceiver)
					{
						foreach (string url in ((IChannelReceiver)channel).GetUrlsForUri(URI))
						{
							Log.Info("CruiseManager: Listening on url: " + url);
						}
					}
				}
			}
		}

		void IDisposable.Dispose()
		{
			lock (this)
			{
				if (_disposed) return;		
				_disposed = true;
			}
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