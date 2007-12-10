using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	public class RemoteCruiseServer : ICruiseServer
	{
		public const string URI = "CruiseManager.rem";
		public const string DefaultUri = "tcp://localhost:21234/" + URI;

		private ICruiseServer server;
		private bool disposed;
		private IExecutionEnvironment environment = new ExecutionEnvironment();

		public RemoteCruiseServer(ICruiseServer server, string remotingConfigurationFile)
		{
			this.server = server;
			RemotingConfiguration.Configure(remotingConfigurationFile, false);
			RegisterForRemoting();
		}

		public void Start()
		{
			server.Start();
		}

		public void Stop()
		{
			server.Stop();
		}

		public void Abort()
		{
			server.Abort();
		}

		public void WaitForExit()
		{
			server.WaitForExit();
		}

		public void Start(string project)
		{
			server.Start(project);
		}

		public void Stop(string project)
		{
			server.Stop(project);
		}

		public ICruiseManager CruiseManager
		{
			get { return server.CruiseManager; }
		}

		public ProjectStatus[] GetProjectStatus()
		{
			return server.GetProjectStatus();
		}

		public void ForceBuild(string projectName, string enforcerName)
		{
			server.ForceBuild(projectName, enforcerName);
		}

		public string AbortBuild(string projectName, string enforcerName)
		{
			return server.AbortBuild(projectName, enforcerName);
		}
		
		public void Request(string projectName, IntegrationRequest request)
		{
			server.Request(projectName, request);
		}

		public void WaitForExit(string projectName)
		{
			server.WaitForExit(projectName);
		}

		public void CancelPendingRequest(string projectName)
		{
			server.CancelPendingRequest(projectName);
		}
		
        public CruiseServerSnapshot GetCruiseServerSnapshot()
		{
			return server.GetCruiseServerSnapshot();
		}

		public string GetLatestBuildName(string projectName)
		{
			return server.GetLatestBuildName(projectName);
		}

		public string[] GetBuildNames(string projectName)
		{
			return server.GetBuildNames(projectName);
		}

		public string GetVersion()
		{
			return server.GetVersion();
		}

		public string[] GetMostRecentBuildNames(string projectName, int buildCount)
		{
			return server.GetMostRecentBuildNames(projectName, buildCount);
		}

		public string GetLog(string projectName, string buildName)
		{
			return server.GetLog(projectName, buildName);
		}

		public string GetServerLog()
		{
			return server.GetServerLog();
		}

		public string GetServerLog(string projectName)
		{
			return server.GetServerLog(projectName);
		}

		public void AddProject(string serializedProject)
		{
			server.AddProject(serializedProject);
		}

		public void DeleteProject(string projectName, bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment)
		{
			server.DeleteProject(projectName, purgeWorkingDirectory, purgeArtifactDirectory, purgeSourceControlEnvironment);
		}

		public string GetProject(string name)
		{
			return server.GetProject(name);
		}

		public void UpdateProject(string projectName, string serializedProject)
		{
			server.UpdateProject(projectName, serializedProject);
		}

		public ExternalLink[] GetExternalLinks(string projectName)
		{
			return server.GetExternalLinks(projectName);
		}

		public void SendMessage(string projectName, Message message)
		{
			server.SendMessage(projectName, message);
		}

		public string GetArtifactDirectory(string projectName)
		{
			return server.GetArtifactDirectory(projectName);
		}

		public string GetStatisticsDocument(string projectName)
		{
            return server.GetStatisticsDocument(projectName);
		}

        public string GetModificationHistoryDocument(string projectName)
        {
            return server.GetModificationHistoryDocument(projectName);
        }

        public string GetRSSFeed(string projectName)
        {
            return server.GetRSSFeed(projectName);
        }

		private void RegisterForRemoting()
		{
			MarshalByRefObject marshalByRef = (MarshalByRefObject) server.CruiseManager;
			RemotingServices.Marshal(marshalByRef, URI);

			foreach (IChannel channel in ChannelServices.RegisteredChannels)
			{
				Log.Info("Registered channel: " + channel.ChannelName);

				// GetUrlsForUri is not support on cross-AppDomain channels on mono (as of 1.1.8.3)
				if (environment.IsRunningOnWindows)
				{
					if (channel is IChannelReceiver)
					{
						foreach (string url in ((IChannelReceiver) channel).GetUrlsForUri(URI))
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
				if (disposed) return;
				disposed = true;
			}
			Log.Info("Disconnecting remote server: ");
			RemotingServices.Disconnect((MarshalByRefObject) server.CruiseManager);
			foreach (IChannel channel in ChannelServices.RegisteredChannels)
			{
				Log.Info("Unregistering channel: " + channel.ChannelName);
				ChannelServices.UnregisterChannel(channel);
			}
			server.Dispose();
		}
	}
}
