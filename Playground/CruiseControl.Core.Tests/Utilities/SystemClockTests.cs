namespace CruiseControl.Core.Tests.Utilities
{
    using System;
    using CruiseControl.Core.Utilities;
    using NUnit.Framework;

    [TestFixture]
    public class SystemClockTests
    {
        #region Tests
        [Test]
        public void NowReturnsCurrentTime()
        {
            var clock = new SystemClock();
            var now = DateTime.Now;
            var actual = clock.Now;
            Assert.IsTrue(actual >= now && actual <= DateTime.Now);
        }

        [Test]
        public void TodayReturnsCurrentDate()
        {
            var clock = new SystemClock();
            var actual = clock.Today;
            Assert.AreEqual(DateTime.Today, actual);
        }
        #endregion
    }
}
