using System.ComponentModel;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Presentation
{
	[TestFixture]
	public class SynchronizedProjectMonitorTest
	{
		[Test]
		public void MethodsAndPropertiesDoSimpleDelagationOntoInjectedMonitor()
		{
			DynamicMock mockProjectMonitor = new DynamicMock(typeof (IProjectMonitor));

			SynchronizedProjectMonitor monitor = new SynchronizedProjectMonitor(
				(IProjectMonitor) mockProjectMonitor.MockInstance, null);

			mockProjectMonitor.ExpectAndReturn("ProjectState", null);
			Assert.IsNull(monitor.ProjectState);

            Dictionary<string, string> parameters = new Dictionary<string, string>();
			mockProjectMonitor.Expect("ForceBuild", parameters);
			monitor.ForceBuild(parameters, null);

			mockProjectMonitor.Expect("Poll");
			monitor.Poll();

			mockProjectMonitor.Verify();
		}

		[Test]
		public void WhenPolledIsFiredTheDelegateIsInvokedThroughISynchronisedInvoke()
		{
			DynamicMock mockSynchronizeInvoke = new DynamicMock(typeof (ISynchronizeInvoke));
			StubProjectMonitor containedMonitor = new StubProjectMonitor("test");

			SynchronizedProjectMonitor monitor = new SynchronizedProjectMonitor(
				containedMonitor,
				(ISynchronizeInvoke) mockSynchronizeInvoke.MockInstance);

			MonitorPolledEventHandler delegateToPolledMethod = new MonitorPolledEventHandler(Monitor_Polled);
			monitor.Polled += delegateToPolledMethod;

			mockSynchronizeInvoke.Expect("BeginInvoke", delegateToPolledMethod, new IsTypeOf(typeof (object[])));
			containedMonitor.OnPolled(new MonitorPolledEventArgs(containedMonitor));

			mockSynchronizeInvoke.Verify();
		}

		[Test]
		public void WhenBuildOccurredIsFiredTheDelegateIsInvokedThroughISynchronisedInvoke()
		{
			DynamicMock mockSynchronizeInvoke = new DynamicMock(typeof (ISynchronizeInvoke));
			StubProjectMonitor containedMonitor = new StubProjectMonitor("test");

			SynchronizedProjectMonitor monitor = new SynchronizedProjectMonitor(
				containedMonitor,
				(ISynchronizeInvoke) mockSynchronizeInvoke.MockInstance);

			MonitorBuildOccurredEventHandler delegateToBuildOccurred = new MonitorBuildOccurredEventHandler(Monitor_BuildOccurred);
			monitor.BuildOccurred += delegateToBuildOccurred;

			mockSynchronizeInvoke.Expect("BeginInvoke", delegateToBuildOccurred, new IsTypeOf(typeof (object[])));
			containedMonitor.OnBuildOccurred(new MonitorBuildOccurredEventArgs(null, BuildTransition.StillFailing));

			mockSynchronizeInvoke.Verify();
		}

		private void Monitor_Polled(object sender, MonitorPolledEventArgs args)
		{
			Assert.Fail("Do not expect this method to actually get called as using mcoked synchronised invoke");
		}

		private void Monitor_BuildOccurred(object sender, MonitorBuildOccurredEventArgs e)
		{
			Assert.Fail("Do not expect this method to actually get called as using mcoked synchronised invoke");
		}
	}
}