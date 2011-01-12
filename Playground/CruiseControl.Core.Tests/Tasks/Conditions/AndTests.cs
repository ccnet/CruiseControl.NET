namespace CruiseControl.Core.Tests.Tasks.Conditions
{
    using CruiseControl.Core.Interfaces;
    using CruiseControl.Core.Tasks.Conditions;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class AndTests
    {
        #region Tests
        [Test]
        public void ValidateValidatesChildren()
        {
            var condition = new And();
            var conditionMock = new Mock<TaskCondition>(MockBehavior.Strict);
            conditionMock.Setup(c => c.Validate(It.IsAny<IValidationLog>())).Verifiable();
            condition.Children.Add(conditionMock.Object);
            condition.Validate(null);
            conditionMock.Verify();
        }

        [Test]
        public void EvaluateReturnsTrueIfAllChildrenAreTrue()
        {
            var condition = new And();
            GenerateChildConditionMock(condition, true);
            GenerateChildConditionMock(condition, true);
            var contextMock = new Mock<TaskExecutionContext>(new TaskExecutionParameters());
            var result = condition.Evaluate(contextMock.Object);
            Assert.IsTrue(result);
        }

        [Test]
        public void EvaluateReturnsFalseIfAChildFails()
        {
            var condition = new And();
            GenerateChildConditionMock(condition, true);
            GenerateChildConditionMock(condition, false);
            var contextMock = new Mock<TaskExecutionContext>(new TaskExecutionParameters());
            var result = condition.Evaluate(contextMock.Object);
            Assert.IsFalse(result);
        }

        private static void GenerateChildConditionMock(And condition, bool passes)
        {
            var conditionMock = new Mock<TaskCondition>(MockBehavior.Strict);
            conditionMock.Setup(c => c.Evaluate(It.IsAny<TaskExecutionContext>()))
                .Returns(passes);
            condition.Children.Add(conditionMock.Object);
        }
        #endregion
    }
}
