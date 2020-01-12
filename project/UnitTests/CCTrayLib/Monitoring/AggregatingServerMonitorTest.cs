using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Presentation;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class AggregatingServerMonitorTest
	{
		private Mock<IServerMonitor> monitor1;
		private Mock<IServerMonitor> monitor2;
		private Mock<IServerMonitor> monitor3;
		private IServerMonitor[] monitors;
		private AggregatingServerMonitor aggregator;

		[SetUp]
		public void SetUp()
		{
			monitor1 = new Mock<IServerMonitor>();
			monitor2 = new Mock<IServerMonitor>();
			monitor3 = new Mock<IServerMonitor>();

			monitors = new IServerMonitor[]
				{
					(IServerMonitor) monitor1.Object,
					(IServerMonitor) monitor2.Object,
					(IServerMonitor) monitor3.Object,
				};

			aggregator = new AggregatingServerMonitor(monitors);
		}

		[TearDown]
		public void TearDown()
		{
			monitor1.Verify();
			monitor2.Verify();
			monitor3.Verify();
		}

		[Test]
		public void PollInvokesPollOnAllContainedServers()
		{
			monitor1.Setup(_monitor => _monitor.Poll()).Verifiable();
			monitor2.Setup(_monitor => _monitor.Poll()).Verifiable();
			monitor3.Setup(_monitor => _monitor.Poll()).Verifiable();
			aggregator.Poll();
		}

		private int queueChangedCount;
		private MonitorServerQueueChangedEventArgs lastQueueChangedEventArgs;

		[Test]
		public void QueueChangedIsFiredWheneverAnyContainedServerFiresIt()
		{
			queueChangedCount = 0;
			lastQueueChangedEventArgs = null;

			StubServerMonitor stubServerMonitor1 = new StubServerMonitor("tcp://somehost1/");
			StubServerMonitor stubServerMonitor2 = new StubServerMonitor("tcp://somehost2/");

			aggregator = new AggregatingServerMonitor(stubServerMonitor1, stubServerMonitor2);
			aggregator.QueueChanged += new MonitorServerQueueChangedEventHandler(Aggregator_QueueChanged);

			Assert.AreEqual(0, queueChangedCount);
			stubServerMonitor1.OnQueueChanged(new MonitorServerQueueChangedEventArgs(stubServerMonitor1));

			Assert.AreEqual(1, queueChangedCount);
			Assert.AreSame(stubServerMonitor1, lastQueueChangedEventArgs.ServerMonitor);
		}

		private void Aggregator_QueueChanged(object sauce, MonitorServerQueueChangedEventArgs e)
		{
			queueChangedCount++;
			lastQueueChangedEventArgs = e;
		}

		private int pollCount;
		private object lastPolledSource;
		private MonitorServerPolledEventArgs lastPolledArgs;

		[Test]
		public void PolledIsFiredWheneverAnyContainedServerFiresIt()
		{
			pollCount = 0;

			StubServerMonitor stubServerMonitor1 = new StubServerMonitor("tcp://1.2.3.4/");
			StubServerMonitor stubServerMonitor2 = new StubServerMonitor("tcp://1.2.3.5/");

			aggregator = new AggregatingServerMonitor(stubServerMonitor1, stubServerMonitor2);
			aggregator.Polled += new MonitorServerPolledEventHandler(Aggregator_Polled);

			Assert.AreEqual(0, pollCount);
			stubServerMonitor1.Poll();

			Assert.AreEqual(1, pollCount);
		}

		private void Aggregator_Polled(object source, MonitorServerPolledEventArgs args)
		{
			pollCount++;
			lastPolledSource = source;
			lastPolledArgs = args;
		}

		[Test]
		public void WhenPolledIsFiredTheSourcePointToTheAggregatorNotTheFiringServer()
		{
			StubServerMonitor stubServerMonitor1 = new StubServerMonitor("tcp://1.2.3.4/");

			aggregator = new AggregatingServerMonitor(stubServerMonitor1);
			aggregator.Polled += new MonitorServerPolledEventHandler(Aggregator_Polled);

			aggregator.Poll();

			Assert.AreSame(lastPolledSource, aggregator);
			Assert.AreSame(lastPolledArgs.ServerMonitor, stubServerMonitor1);
		}
	}
}
