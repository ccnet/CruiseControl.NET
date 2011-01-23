namespace CruiseControl.Core.Tests.Utilities
{
    using System;
    using CruiseControl.Core.Utilities;
    using NUnit.Framework;

    [TestFixture]
    public class LoggingValidationLogTests
    {
        #region Tests
        [Test]
        public void AddErrorWithMessageIncreasesCount()
        {
            var log = new LoggingValidationLog();
            log.AddError("Test");
            Assert.AreEqual(1, log.NumberOfErrors);
        }

        [Test]
        public void AddErrorWithExceptionIncreasesCount()
        {
            var log = new LoggingValidationLog();
            log.AddError(new Exception("Test"));
            Assert.AreEqual(1, log.NumberOfErrors);
        }

        [Test]
        public void AddWarningWithMessageIncreasesCount()
        {
            var log = new LoggingValidationLog();
            log.AddWarning("Test");
            Assert.AreEqual(1, log.NumberOfWarnings);
        }

        [Test]
        public void AddWarningWithExceptionIncreasesCount()
        {
            var log = new LoggingValidationLog();
            log.AddWarning(new Exception("Test"));
            Assert.AreEqual(1, log.NumberOfWarnings);
        }

        [Test]
        public void ResetSetsCountsToZero()
        {
            var log = new LoggingValidationLog();
            log.AddError("Test");
            log.AddWarning("Test");
            log.Reset();
            Assert.AreEqual(0, log.NumberOfErrors);
            Assert.AreEqual(0, log.NumberOfWarnings);
        }
        #endregion
    }
}
