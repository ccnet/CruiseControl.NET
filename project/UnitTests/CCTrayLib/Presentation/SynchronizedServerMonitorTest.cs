using System.ComponentModel;
using Moq;
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
			var mockServerMonitor = new Mock<ISingleServerMonitor>();

			SynchronizedServerMonitor monitor = new SynchronizedServerMonitor(
				(ISingleServerMonitor) mockServerMonitor.Object, null);

			mockServerMonitor.SetupGet(_monitor => _monitor.ServerUrl).Returns(@"tcp://blah/").Verifiable();
			Assert.AreEqual(@"tcp://blah/", monitor.ServerUrl);

			mockServerMonitor.SetupGet(_monitor => _monitor.IsConnected).Returns(true).Verifiable();
			Assert.AreEqual(true, monitor.IsConnected);

			mockServerMonitor.Setup(_monitor => _monitor.Poll()).Verifiable();
			monitor.Poll();

			mockServerMonitor.Verify();
		}

		[Test]
		public void WhenPolledIsFiredTheDelegateIsInvokedThroughISynchronisedInvoke()
		{
			var mockSynchronizeInvoke = new Mock<ISynchronizeInvoke>();
			StubServerMonitor containedMonitor = new StubServerMonitor(@"tcp://blah/");

			SynchronizedServerMonitor monitor = new SynchronizedServerMonitor(
				containedMonitor,
				(ISynchronizeInvoke) mockSynchronizeInvoke.Object);

			MonitorServerPolledEventHandler delegateToPolledMethod = new MonitorServerPolledEventHandler(Monitor_Polled);
			monitor.Polled += delegateToPolledMethod;

			mockSynchronizeInvoke.Setup(invoke => invoke.BeginInvoke(delegateToPolledMethod, It.IsAny<object[]>())).Verifiable();
			containedMonitor.OnPolled(new MonitorServerPolledEventArgs(containedMonitor));

			mockSynchronizeInvoke.Verify();
		}

		[Test]
		public void WhenBuildOccurredIsFiredTheDelegateIsInvokedThroughISynchronisedInvoke()
		{
			var mockSynchronizeInvoke = new Mock<ISynchronizeInvoke>();
			StubServerMonitor containedMonitor = new StubServerMonitor(@"tcp://blah/");

			SynchronizedServerMonitor monitor = new SynchronizedServerMonitor(
				containedMonitor,
				(ISynchronizeInvoke) mockSynchronizeInvoke.Object);

			MonitorServerQueueChangedEventHandler delegateToQueueChanged = new MonitorServerQueueChangedEventHandler(Monitor_QueueChanged);
			monitor.QueueChanged += delegateToQueueChanged;

			mockSynchronizeInvoke.Setup(invoke => invoke.BeginInvoke(delegateToQueueChanged, It.IsAny<object[]>())).Verifiable();
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
