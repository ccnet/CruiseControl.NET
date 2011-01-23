namespace CruiseControl.Core.Tests.Triggers
{
    using System;
    using CruiseControl.Core.Interfaces;
    using CruiseControl.Core.Tests.Stubs;
    using CruiseControl.Core.Triggers;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class IntervalTests
    {
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
            DateTimeAssert.AreEqual(expected, actual, DateTimeCompare.IgnoreSeconds);
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
            DateTimeAssert.AreEqual(expected, actual, DateTimeCompare.IgnoreSeconds);
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

        [Test]
        public void ValidateDetectsMissingPeriod()
        {
            var errorAdded = false;
            var trigger = new Interval();
            var validation = new ValidationLogStub
                                 {
                                     OnAddErrorMessage = (m, a) =>
                                                             {
                                                                 Assert.AreEqual(
                                                                     "No period set - trigger will not fire",
                                                                     m);
                                                                 CollectionAssert.IsEmpty(a);
                                                                 errorAdded = true;
                                                             }
                                 };
            trigger.Validate(validation);
            Assert.IsTrue(errorAdded);
        }
        #endregion
    }
}
