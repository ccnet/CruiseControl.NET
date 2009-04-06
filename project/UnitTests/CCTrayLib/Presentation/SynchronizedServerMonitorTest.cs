using System.ComponentModel;
using NMock;
using NMock.Constraints;
using NUnit.Framework;

using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Presentation
{
	[TestFixture]
	public class SynchronizedServerMonitorTest
	{
		[Test]
		public void MethodsAndPropertiesDoSimpleDelagationOntoInjectedMonitor()
		{
			DynamicMock mockServerMonitor = new DynamicMock(typeof (ISingleServerMonitor));

			SynchronizedServerMonitor monitor = new SynchronizedServerMonitor(
				(ISingleServerMonitor) mockServerMonitor.MockInstance, null);

			mockServerMonitor.ExpectAndReturn("ServerUrl", @"tcp://blah/");
			Assert.AreEqual(@"tcp://blah/", monitor.ServerUrl);

			mockServerMonitor.ExpectAndReturn("IsConnected", true);
			Assert.AreEqual(true, monitor.IsConnected);

			mockServerMonitor.Expect("Poll");
			monitor.Poll();

			mockServerMonitor.Verify();
		}

		[Test]
		public void WhenPolledIsFiredTheDelegateIsInvokedThroughISynchronisedInvoke()
		{
			DynamicMock mockSynchronizeInvoke = new DynamicMock(typeof (ISynchronizeInvoke));
			StubServerMonitor containedMonitor = new StubServerMonitor(@"tcp://blah/");

			SynchronizedServerMonitor monitor = new SynchronizedServerMonitor(
				containedMonitor,
				(ISynchronizeInvoke) mockSynchronizeInvoke.MockInstance);

			MonitorServerPolledEventHandler delegateToPolledMethod = new MonitorServerPolledEventHandler(Monitor_Polled);
			monitor.Polled += delegateToPolledMethod;

			mockSynchronizeInvoke.Expect("BeginInvoke", delegateToPolledMethod, new IsTypeOf(typeof (object[])));
			containedMonitor.OnPolled(new MonitorServerPolledEventArgs(containedMonitor));

			mockSynchronizeInvoke.Verify();
		}

		[Test]
		public void WhenBuildOccurredIsFiredTheDelegateIsInvokedThroughISynchronisedInvoke()
		{
			DynamicMock mockSynchronizeInvoke = new DynamicMock(typeof (ISynchronizeInvoke));
			StubServerMonitor containedMonitor = new StubServerMonitor(@"tcp://blah/");

			SynchronizedServerMonitor monitor = new SynchronizedServerMonitor(
				containedMonitor,
				(ISynchronizeInvoke) mockSynchronizeInvoke.MockInstance);

			MonitorServerQueueChangedEventHandler delegateToQueueChanged = new MonitorServerQueueChangedEventHandler(Monitor_QueueChanged);
			monitor.QueueChanged += delegateToQueueChanged;

			mockSynchronizeInvoke.Expect("BeginInvoke", delegateToQueueChanged, new IsTypeOf(typeof (object[])));
			containedMonitor.OnQueueChanged(new MonitorServerQueueChangedEventArgs(null));

			mockSynchronizeInvoke.Verify();
		}

		private void Monitor_Polled(object sender, MonitorServerPolledEventArgs args)
		{
			Assert.Fail("Do not expect this method to actually get called as using mocked synchronised invoke");
		}

		private void Monitor_QueueChanged(object sender, MonitorServerQueueChangedEventArgs e)
		{
			Assert.Fail("Do not expect this method to actually get called as using mocked synchronised invoke");
		}
	}
}
