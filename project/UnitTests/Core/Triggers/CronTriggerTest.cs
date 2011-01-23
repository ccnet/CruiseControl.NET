namespace ThoughtWorks.CruiseControl.UnitTests.Core.Triggers
{
    using System;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Core.Triggers;
    using System.Threading;
    using Rhino.Mocks;
    using ThoughtWorks.CruiseControl.Core.Util;

    [TestFixture]
    public class CronTriggerTest
    {
        private MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            this.mocks = new MockRepository();
        }

        [Test]
        public void TestX()
        {
            var c = new CronTrigger();

            c.CronExpression = "* * 1 1 *"; // first januari of each year

            c.Fire();

            DateTime expected;
            if (DateTime.Now.DayOfYear == 1)
            {
                expected = new DateTime(DateTime.Now.Year, 1, 1);
            }
            else
            {
                expected = new DateTime(DateTime.Now.Year + 1, 1, 1);
            }

            Assert.AreEqual(expected, c.NextBuild);
        }

        [Test]
        public void NameReturnsTypeName()
        {
            var trigger = new CronTrigger();
            Assert.AreEqual(typeof(CronTrigger).Name, trigger.Name);
        }

        [Test]
        public void NameReturnsSetName()
        {
            var name = "testName";
            var trigger = new CronTrigger { Name = name };
            Assert.AreEqual(name, trigger.Name);
        }

        [Test]
        public void IntegrationCompletedDoesNothingIfNotTriggered()
        {
            var trigger = new CronTrigger
                {
                    CronExpression = "* * 1 1 *"
                };
            trigger.Fire();

            var nextTime = trigger.NextBuild;
            trigger.StartDate = DateTime.Now.AddHours(2);
            trigger.IntegrationCompleted();
            Assert.AreEqual(nextTime, trigger.NextBuild);
        }

        [Test]
        public void FireReturnsRequestIfMatched()
        {
            var today = DateTime.Today;
            var trigger = new CronTrigger
                {
                    CronExpression = today.ToString("* * d * *")
                };
            trigger.StartDate = DateTime.Today;
            var actual = trigger.Fire();
            Assert.IsNotNull(actual);
        }

        [Test]
        public void IntegrationCompletedUpdatesNextBuildIfTriggered()
        {
            var today = DateTime.Today;
            var trigger = new CronTrigger
                {
                    CronExpression = today.ToString("* * d * *")
                };
            trigger.StartDate = DateTime.Today;
            trigger.Fire();
            var nextTime = trigger.NextBuild;
            trigger.StartDate = DateTime.Now.AddHours(2);
            trigger.Fire();
            trigger.IntegrationCompleted();
            Assert.AreNotEqual(nextTime, trigger.NextBuild);
        }
    }
}
