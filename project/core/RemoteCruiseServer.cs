using System;
using tw.ccnet.remote;

namespace tw.ccnet.core
{
	public class RemoteCruiseServer : ICruiseServer
	{
		private ICruiseServer _server;

		public RemoteCruiseServer(ICruiseServer server)
		{
			_server = server;
			CruiseManager manager = new CruiseManager((ICruiseControl)server);
			manager.RegisterForRemoting();
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
	}
}
