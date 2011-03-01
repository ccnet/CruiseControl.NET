namespace CruiseControl.Common.Tests.Messages
{
    using Common.Messages;
    using NUnit.Framework;

    public class OperationReportTests
    {
        #region Tests
        [Test]
        public void ValuesArePassedCorrectly()
        {
            var message = "Some Message";
            var report = new OperationReport
                             {
                                 WasSuccess = false,
                                 Message = message
                             };
            var result = report.DoRoundTripTest();
            Assert.IsFalse(result.WasSuccess);
            Assert.AreEqual(message, result.Message);
        }

        [Test]
        public void ConstructorSetsValues()
        {
            var message = "Some Message";
            var report = new OperationReport(true, message);
            Assert.IsTrue(report.WasSuccess);
            Assert.AreEqual(message, report.Message);
        }
        #endregion
    }
}
