namespace CruiseControl.Core.Tests.Triggers
{
    using System;
    using CruiseControl.Core.Interfaces;
    using CruiseControl.Core.Tests.Stubs;
    using CruiseControl.Core.Triggers;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class RollUpTests
    {
        #region Tests
        [Test]
        public void TriggerFiresIfOutsidePeriod()
        {
            var clockMock = new Mock<IClock>();
            var time = DateTime.Now;
            clockMock.Setup(c => c.Now).Returns(() =>
                                                    {
                                                        time = time.AddMinutes(10);
                                                        return time;
                                                    });
            var inner = new TriggerStub
                            {
                                OnCheckAction = () => new IntegrationRequest("Test")
                            };
            var trigger = new RollUp(TimeSpan.FromMinutes(5))
                              {
                                  InnerTrigger = inner,
                                  Clock = clockMock.Object
                              };
            trigger.Reset();
            var request = trigger.Check();
            Assert.IsNotNull(request);
        }

        [Test]
        public void TriggerDoesNotFireInsidePeriod()
        {
            var clockMock = new Mock<IClock>();
            clockMock.Setup(c => c.Now).Returns(() => DateTime.Now);
            var inner = new TriggerStub
                            {
                                OnCheckAction = () => new IntegrationRequest("Test")
                            };
            var trigger = new RollUp(TimeSpan.FromMinutes(5))
                              {
                                  InnerTrigger = inner,
                                  Clock = clockMock.Object
                              };
            trigger.Reset();
            var request = trigger.Check();
            Assert.IsNull(request);
        }

        [Test]
        public void ResetSetsTheNextTime()
        {
            var clockMock = new Mock<IClock>();
            clockMock.Setup(c => c.Now).Returns(() => DateTime.Now);
            var trigger = new RollUp(TimeSpan.FromMinutes(5))
                              {
                                  Clock = clockMock.Object
                              };
            trigger.Reset();
            var expected = DateTime.Now.AddMinutes(5);
            var actual = trigger.NextTime.Value;
            DateTimeAssert.AreEqual(expected, actual, DateTimeCompare.IgnoreSeconds);
        }

        [Test]
        public void ValidateDetectsMissingPeriod()
        {
            var errorAdded = false;
            var trigger = new RollUp();
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
