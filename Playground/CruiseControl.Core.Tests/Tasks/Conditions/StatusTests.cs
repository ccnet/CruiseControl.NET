namespace CruiseControl.Core.Tests.Tasks.Conditions
{
    using CruiseControl.Core.Tasks.Conditions;
    using Moq;
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
            var contextMock = new Mock<TaskExecutionContext>(new TaskExecutionParameters());
            contextMock.Setup(c => c.CurrentStatus).Returns(IntegrationStatus.Success);
            var result = condition.Evaluate(contextMock.Object);
            Assert.IsTrue(result);
        }

        [Test]
        public void EvaluateIsFalseWhenStatusDoesNotMatch()
        {
            var condition = new Status
                                {
                                    Value = IntegrationStatus.Success
                                };
            var contextMock = new Mock<TaskExecutionContext>(new TaskExecutionParameters());
            contextMock.Setup(c => c.CurrentStatus).Returns(IntegrationStatus.Failure);
            var result = condition.Evaluate(contextMock.Object);
            Assert.IsFalse(result);
        }
        #endregion
    }
}
