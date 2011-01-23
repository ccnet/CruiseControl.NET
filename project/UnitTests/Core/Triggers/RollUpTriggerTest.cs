namespace ThoughtWorks.CruiseControl.UnitTests.Core.Triggers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Core.Triggers;
    using Rhino.Mocks;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Core;

    [TestFixture]
    public class RollUpTriggerTest
    {
        #region Private fields
        private MockRepository mocks;
        #endregion

        #region Setup
        [SetUp]
        public void Setup()
        {
            this.mocks = new MockRepository();
        }
        #endregion

        #region Tests
        #region NextBuild Tests
        [Test(Description = "The next build should return a time after the minimum time period")]
        public void NextBuildReturnsNextAllowedTimeForTimeRollUp()
        {
            var innerTrigger = this.mocks.StrictMock<ITrigger>();
            Expect.Call(() => { innerTrigger.IntegrationCompleted(); });
            var trigger = new RollUpTrigger();
            trigger.InnerTrigger = innerTrigger;
            trigger.MinimumTime = new Timeout(10, TimeUnits.MINUTES);
            mocks.ReplayAll();
            trigger.IntegrationCompleted();
            Assert.Greater(trigger.NextBuild, DateTime.Now.AddMinutes(9));
            mocks.VerifyAll();
        }
        #endregion

        #region IntegrationCompleted() Tests
        [Test(Description = "The inner trigger should be called when IntegrationCompleted() is called")]
        public void IntegrationCompletedCallsInnerTrigger()
        {
            var innerTrigger = this.mocks.StrictMock<ITrigger>();
            Expect.Call(() => { innerTrigger.IntegrationCompleted(); });
            var trigger = new RollUpTrigger();
            trigger.MinimumTime = new Timeout(10, TimeUnits.MINUTES);
            trigger.InnerTrigger = innerTrigger;
            mocks.ReplayAll();
            trigger.IntegrationCompleted();
            mocks.VerifyAll();
        }
        #endregion

        #region Fire() Tests
        [Test(Description = "The trigger should fire if the time period has expired")]
        public void FireFiresAfterTimePeriodHasExpired()
        {
            var innerTrigger = this.mocks.StrictMock<ITrigger>();
            Expect.Call(() => { innerTrigger.IntegrationCompleted(); });
            var expected = new IntegrationRequest(BuildCondition.IfModificationExists, "Test", null);
            Expect.Call(innerTrigger.Fire()).Return(expected);
            var clock = new TestClock { Now = DateTime.Now };
            var trigger = new RollUpTrigger(clock);
            trigger.MinimumTime = new Timeout(10, TimeUnits.MINUTES);
            trigger.InnerTrigger = innerTrigger;
            mocks.ReplayAll();
            trigger.IntegrationCompleted();
            clock.TimePasses(new TimeSpan(0, 11, 0));
            var actual = trigger.Fire();
            Assert.AreSame(expected, actual);
            mocks.VerifyAll();
        }

        [Test(Description = "The trigger should not fire if the time period has not expired")]
        public void FireDoesNotFireBeforeTimePeriodHasExpired()
        {
            var innerTrigger = this.mocks.StrictMock<ITrigger>();
            Expect.Call(() => { innerTrigger.IntegrationCompleted(); });
            var trigger = new RollUpTrigger();
            trigger.MinimumTime = new Timeout(10, TimeUnits.MINUTES);
            trigger.InnerTrigger = innerTrigger;
            mocks.ReplayAll();
            trigger.IntegrationCompleted();
            Assert.IsNull(trigger.Fire());
            mocks.VerifyAll();
        }

        [Test(Description = "The trigger should not fire if the inner trigger does not fire")]
        public void FireDoesNotFireWithoutInnerTrigger()
        {
            var innerTrigger = this.mocks.StrictMock<ITrigger>();
            Expect.Call(innerTrigger.Fire())
                .Return(null);
            var trigger = new RollUpTrigger();
            trigger.MinimumTime = new Timeout(10, TimeUnits.MINUTES);
            trigger.InnerTrigger = innerTrigger;
            mocks.ReplayAll();
            Assert.IsNull(trigger.Fire());
            mocks.VerifyAll();
        }
        #endregion
        #endregion
    }
}
