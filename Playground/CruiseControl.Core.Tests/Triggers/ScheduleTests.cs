namespace CruiseControl.Core.Tests.Triggers
{
    using System;
    using CruiseControl.Core.Interfaces;
    using CruiseControl.Core.Tests.Stubs;
    using CruiseControl.Core.Triggers;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class ScheduleTests
    {
        #region Tests
        [Test]
        public void InitialiseSetsTheNextTime()
        {
            var clockMock = new Mock<IClock>(MockBehavior.Strict);
            clockMock.Setup(c => c.Today).Returns(DateTime.Today);
            clockMock.Setup(c => c.Now).Returns(DateTime.Today.AddHours(9));
            var trigger = new Schedule(TimeSpan.Parse("17:00"))
                              {
                                  Clock = clockMock.Object
                              };
            trigger.Initialise();
            var expected = DateTime.Today.AddHours(17);
            var actual = trigger.NextTime.Value;
            DateTimeAssert.AreEqual(expected, actual, DateTimeCompare.IgnoreSeconds);
        }

        [Test]
        public void ResetSetsTheNextTime()
        {
            var clockMock = new Mock<IClock>(MockBehavior.Strict);
            clockMock.Setup(c => c.Today).Returns(DateTime.Today);
            clockMock.Setup(c => c.Now).Returns(DateTime.Today.AddHours(20));
            var trigger = new Schedule(TimeSpan.Parse("17:00"))
                              {
                                  Clock = clockMock.Object
                              };
            trigger.Reset();
            var expected = DateTime.Today.AddDays(1).AddHours(17);
            var actual = trigger.NextTime.Value;
            DateTimeAssert.AreEqual(expected, actual, DateTimeCompare.IgnoreSeconds);
        }

        [Test]
        public void CheckReturnsNullIfWithinPeriod()
        {
            var clockMock = new Mock<IClock>(MockBehavior.Strict);
            clockMock.Setup(c => c.Today).Returns(DateTime.Today);
            clockMock.Setup(c => c.Now).Returns(DateTime.Today.AddHours(9));
            var trigger = new Schedule(TimeSpan.Parse("17:00"))
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
            var clockMock = new Mock<IClock>(MockBehavior.Strict);
            var time = DateTime.Today.AddHours(15).AddMinutes(30);
            clockMock.Setup(c => c.Today).Returns(DateTime.Today);
            clockMock.Setup(c => c.Now).Returns(() =>
                                                    {
                                                        time = time.AddHours(1);
                                                        return time;
                                                    });
            var trigger = new Schedule(TimeSpan.Parse("17:00"))
                              {
                                  Clock = clockMock.Object
                              };
            var now = DateTime.Now;
            trigger.Reset();
            var actual = trigger.Check();
            Assert.IsNotNull(actual);
            Assert.AreEqual("Schedule", actual.SourceTrigger);
            Assert.IsTrue(actual.Time >= now && actual.Time <= DateTime.Now);
        }

        [Test]
        public void ValidateDetectsMissingTime()
        {
            var errorAdded = false;
            var trigger = new Schedule();
            var validation = new ValidationLogStub
                                 {
                                     OnAddErrorMessage = (m, a) =>
                                                             {
                                                                 Assert.AreEqual(
                                                                     "No time set - trigger will not fire",
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
