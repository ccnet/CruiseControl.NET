using System;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTray.Test
{
    [NUnit.Framework.TestFixture]
	public class StatusMonitorTransitionChangeTest : Assertion
	{

		private Mock _remoteProxyMock;
		private StatusMonitor _monitor;
		private bool _isError;
		private bool _isPolled;
		[SetUp]
		public void Init()
		{
			_isError = false;
			_isPolled = false;
			_remoteProxyMock = new DynamicMock(typeof(IRemoteCruiseProxyLoader));
		    _monitor = new StatusMonitor((IRemoteCruiseProxyLoader) _remoteProxyMock.MockInstance);
		}
		[Test]
		public void ShouldNotifyErrorListenersIfExceptionIsThrown()
		{
			_monitor.Error +=new ErrorEventHandler(OnError);
			_remoteProxyMock.ExpectAndThrow("LoadProxy",new Exception("Test"),new IsAnything());
			_monitor.Poll();
			Assert(_isError);
		}

		[Test]
		public void ShouldNotifyPollEventWhenPollingAndNotInvokeError()
		{
			_monitor.Polled +=new PolledEventHandler(OnPolled);
		    ProjectStatus status = new ProjectStatus(CruiseControlStatus.Running, IntegrationStatus.Success, ProjectActivity.Sleeping, "foo", "http://foo.bar", DateTime.Now, "Foo");
			Mock cruiseManagerMock = new DynamicMock(typeof(ICruiseManager));
			ProjectStatus[] statuses = new ProjectStatus[1];
			statuses [0]= status;
			cruiseManagerMock.SetupResult("GetProjectStatus",statuses);
			_monitor.Settings = new Settings();
			_monitor.Settings.ProjectName = "foo";
			_remoteProxyMock.SetupResult("LoadProxy", (ICruiseManager)cruiseManagerMock.MockInstance, typeof(Settings));
			_monitor.Poll();
			Assert(_isPolled);
			Assert(!_isError);
		}

		private void OnError(object sauce, ErrorEventArgs e)
		{
			_isError = true;					
		}

		private void OnPolled(object sauce, PolledEventArgs e)
		{
			_isPolled = true;
		}

	    
	}
}
																				 