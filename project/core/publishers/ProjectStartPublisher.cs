using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using Exortech.NetReflector;
using tw.ccnet.remote;
using tw.ccnet.core.schedule;

namespace tw.ccnet.core.publishers
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
//				manager.Run(Project, new Schedule(0, 1));
			}
			catch (Exception ex)
			{
				// log exception
				Console.WriteLine(ex.ToString());
				_ex = ex;
			}
		}

		protected virtual ICruiseManager GetRemoteCruiseManager()
		{
			return (ICruiseManager) RemotingServices.Connect(typeof(ICruiseManager), Url);
		}

		private void FinishedCallback(IAsyncResult ar)
		{
			Console.WriteLine("3" + _result.IsCompleted);			
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
