using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote.Monitor;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Monitor
{
    [TestFixture]
    public class PollingServerWatcherTests
    {
        #region Private fields
        private MockRepository mocks;
        #endregion

        #region Setup
        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository(MockBehavior.Default);
        }
        #endregion

        #region Tests
        #region Constructor tests
        [Test]
        public void ConstructorDoesNotAllowNullClient()
        {
            try
            {
                var watcher = new PollingServerWatcher(null);
                Assert.Fail("ArgumentNullException was expected");
            }
            catch (ArgumentNullException) { }
        }
        #endregion

        #region Refresh() tests
        [Test]
        [Ignore("Not sure how to change this so it will work")]
        public void RefreshCallsClientAndFiresEvent()
        {
            var snapshot = new CruiseServerSnapshot();
            var client = mocks.Create<CruiseServerClientBase>(MockBehavior.Strict).Object;
            using (var watcher = new PollingServerWatcher(client))
            {
                Mock.Get(client).Setup(_client => _client.GetCruiseServerSnapshot()).Returns(snapshot).Verifiable();

                var eventFired = false;
                watcher.Update += (o, e) =>
                {
                    eventFired = true;
                };
                watcher.Refresh();

                mocks.VerifyAll();
                Assert.IsTrue(eventFired);
            }
        }
        #endregion

        #region Polling tests
        [Test]
        [Ignore("Not sure how to change this so it will work")]
        public void PollingCallsClientAndFiresEvent()
        {
            var monitor = new ManualResetEvent(false);
            var snapshot = new CruiseServerSnapshot();
            var client = mocks.Create<CruiseServerClientBase>(MockBehavior.Strict).Object;
            using (var watcher = new PollingServerWatcher(client))
            {
                Mock.Get(client).Setup(_client => _client.GetCruiseServerSnapshot()).Returns(snapshot).Verifiable();

                var eventFired = false;
                watcher.Update += (o, e) =>
                {
                    eventFired = true;
                    monitor.Set();
                };
                monitor.WaitOne(new TimeSpan(0, 0, 10), false);

                mocks.VerifyAll();
                Assert.IsTrue(eventFired);
            }
        }
        #endregion
        #endregion
    }
}
