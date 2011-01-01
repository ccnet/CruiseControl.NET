namespace CruiseControl.Core.Tests.Triggers
{
    using System;
    using CruiseControl.Core.Interfaces;
    using CruiseControl.Core.Triggers;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class IntervalTests
    {
        private const string CompareFormat = "yyyyMMddHHmm";

        #region Tests
        [Test]
        public void InitialiseSetsTheNextTime()
        {
            var clockMock = new Mock<IClock>();
            clockMock.Setup(c => c.Now).Returns(() => DateTime.Now);
            var trigger = new Interval(TimeSpan.FromMinutes(5))
                              {
                                  Clock = clockMock.Object
                              };
            trigger.Initialise();
            var expected = DateTime.Now.AddMinutes(5);
            var actual = trigger.NextTime.Value;
            Assert.AreEqual(expected.ToString(CompareFormat), actual.ToString(CompareFormat));
        }

        [Test]
        public void ResetSetsTheNextTime()
        {
            var clockMock = new Mock<IClock>();
            clockMock.Setup(c => c.Now).Returns(() => DateTime.Now);
            var trigger = new Interval(TimeSpan.FromMinutes(5))
                              {
                                  Clock = clockMock.Object
                              };
            trigger.Reset();
            var expected = DateTime.Now.AddMinutes(5);
            var actual = trigger.NextTime.Value;
            Assert.AreEqual(expected.ToString(CompareFormat), actual.ToString(CompareFormat));
        }

        [Test]
        public void CheckReturnsNullIfWithinPeriod()
        {
            var clockMock = new Mock<IClock>();
            clockMock.Setup(c => c.Now).Returns(() => DateTime.Now);
            var trigger = new Interval(TimeSpan.FromMinutes(5))
            {
                Clock = clockMock.Object
            };
            trigger.Reset();
            var actual = trigger.Check();
            Assert.IsNull(actual);
        }

        [Test]
        public void CheckReturnsIntegrationRequestIfBeyondPeriod()
        {
            var clockMock = new Mock<IClock>();
            var time = DateTime.Now;
            clockMock.Setup(c => c.Now).Returns(() =>
                                                    {
                                                        time = time.AddMinutes(6);
                                                        return time;
                                                    });
            var trigger = new Interval(TimeSpan.FromMinutes(5))
                              {
                                  Clock = clockMock.Object
                              };
            trigger.Reset();
            var now = DateTime.Now;
            var actual = trigger.Check();
            Assert.IsNotNull(actual);
            Assert.AreEqual("Interval", actual.SourceTrigger);
            Assert.IsTrue(actual.Time >= now && actual.Time <= DateTime.Now);
        }
        #endregion
    }
}
