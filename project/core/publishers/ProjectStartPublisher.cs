using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Schedules;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	[ReflectorType("startproject")]
	public class ProjectStartPublisher : PublisherBase
	{
		private delegate void StartProjectDelegate(IntegrationResult result);

		[ReflectorProperty("project")]
		public string Project;
		[ReflectorProperty("url")]
		public string Url;

		private IAsyncResult _result;
		private Exception _ex;

		public override void PublishIntegrationResults(IProject project, IntegrationResult result)
		{
			AsyncCallback cb = new AsyncCallback(FinishedCallback);

			StartProjectDelegate startProjectDelegate = new StartProjectDelegate(StartProject);
			_result = startProjectDelegate.BeginInvoke(result, cb, new object());			
		}

		private void StartProject(IntegrationResult result)
		{
			try
			{
				ICruiseManager manager = GetRemoteCruiseManager();
				manager.ForceBuild(Project);
			}
			catch (Exception ex)
			{
				Log.Error(ex);
				_ex = ex;
			}
		}

		protected virtual ICruiseManager GetRemoteCruiseManager()
		{
			return (ICruiseManager) RemotingServices.Connect(typeof(ICruiseManager), Url);
		}

		private void FinishedCallback(IAsyncResult ar)
		{
		}

		public void WaitForCompletion()
		{
			_result.AsyncWaitHandle.WaitOne();
			if (_ex == null) return;

			if (_ex is SocketException || _ex is WebException || _ex is RemotingException)
			{
				throw new CruiseControlRemotingException("Remote server may not have be started or the connection url may not be correct", Url, _ex);
			}
			else
			{
				throw _ex;
			}
		}
	}
}
