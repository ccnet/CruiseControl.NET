using NUnit.Framework;
using System;
using System.Threading;
using ThoughtWorks.CruiseControl.Core.Config;

namespace ThoughtWorks.CruiseControl.Core.Test
{
	[TestFixture]
	public class CruiseServerTest : Assertion
	{
		private ConfigurationStub configStub;
		private CruiseServer server;
		private ManualResetEvent monitor;

		[SetUp]
		protected void SetUp()
		{
			configStub = new ConfigurationStub(2);
			server = new CruiseServer(configStub);
		}

		[Test]
		public void StartAllProjectsInCruiseServer()
		{
			configStub.GetIntegratorMock(0).Expect("Start");
			configStub.GetIntegratorMock(1).Expect("Start");

			server.Start();

			configStub.Verify();
		}

		[Test]
		public void StopAllProjectsInCruiseServer()
		{
			configStub.GetIntegratorMock(0).Expect("Stop");
			configStub.GetIntegratorMock(1).Expect("Stop");
			configStub.GetIntegratorMock(0).Expect("WaitForExit");
			configStub.GetIntegratorMock(1).Expect("WaitForExit");

			server.Stop();

			configStub.Verify();
		}

		[Test]
		public void AbortAllProjectsInCruiseServer()
		{
			configStub.GetIntegratorMock(0).Expect("Abort");
			configStub.GetIntegratorMock(1).Expect("Abort");
			configStub.GetIntegratorMock(0).Expect("WaitForExit");
			configStub.GetIntegratorMock(1).Expect("WaitForExit");

			server.Abort();

			configStub.Verify();
		}

		[Test]
		public void ReloadConfiguration()
		{
			configStub.GetIntegratorMock(0).Expect("Stop");
			configStub.GetIntegratorMock(1).Expect("Stop");
			configStub.GetIntegratorMock(0).Expect("WaitForExit");
			configStub.GetIntegratorMock(1).Expect("WaitForExit");
			configStub.GetIntegratorMock(0).Expect("Start");
			configStub.GetIntegratorMock(1).Expect("Start");

			server.ResetConfiguration(configStub);

			configStub.Verify();
		}

		[Test]
		public void WaitForExitAfterStop()
		{
			monitor = new ManualResetEvent(false);

			Thread stopThread = new Thread(new ThreadStart(Stop));
			stopThread.Start();

			server.Start();
			monitor.Set();
			server.WaitForExit();
		}

		private void Stop()
		{
			monitor.WaitOne();
			Thread.Sleep(0);
			server.Stop();
		}

		[Test]
		public void WaitForExitAfterAbort()
		{
			monitor = new ManualResetEvent(false);

			Thread abortThread = new Thread(new ThreadStart(Abort));
			abortThread.Start();

			server.Start();
			monitor.Set();
			server.WaitForExit();
		}

		private void Abort()
		{
			monitor.WaitOne();
			Thread.Sleep(0);
			server.Abort();
		}
	}
}
