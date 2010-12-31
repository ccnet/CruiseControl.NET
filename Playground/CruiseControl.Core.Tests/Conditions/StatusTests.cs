namespace CruiseControl.Core.Tests.Conditions
{
    using CruiseControl.Core.Tasks.Conditions;
    using NUnit.Framework;

    public class StatusTests
    {
        #region Tests
        [Test]
        public void EvaluateIsTrueWhenStatusMatches()
        {
            var condition = new Status
                                {
                                    Value = IntegrationStatus.Success
                                };
            var context = new TaskExecutionContext
                              {
                                  CurrentStatus = IntegrationStatus.Success
                              };
            var result = condition.Evaluate(context);
            Assert.IsTrue(result);
        }

        [Test]
        public void EvaluateIsFalseWhenStatusDoesNotMatch()
        {
            var condition = new Status
                                {
                                    Value = IntegrationStatus.Success
                                };
            var context = new TaskExecutionContext
                              {
                                  CurrentStatus = IntegrationStatus.Failure
                              };
            var result = condition.Evaluate(context);
            Assert.IsFalse(result);
        }
        #endregion
    }
}
