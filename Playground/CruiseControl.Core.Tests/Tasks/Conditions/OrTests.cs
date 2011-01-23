namespace CruiseControl.Core.Tests.Tasks.Conditions
{
    using CruiseControl.Core.Interfaces;
    using CruiseControl.Core.Tasks.Conditions;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class OrTests
    {
        #region Tests
        [Test]
        public void ValidateValidatesChildren()
        {
            var condition = new Or();
            var conditionMock = new Mock<TaskCondition>(MockBehavior.Strict);
            conditionMock.Setup(c => c.Validate(It.IsAny<IValidationLog>())).Verifiable();
            condition.Children.Add(conditionMock.Object);
            condition.Validate(null);
            conditionMock.Verify();
        }

        [Test]
        public void EvaluateReturnsFalseIfAllChildrenAreFalse()
        {
            var condition = new Or();
            GenerateChildConditionMock(condition, false);
            GenerateChildConditionMock(condition, false);
            var contextMock = new Mock<TaskExecutionContext>(new TaskExecutionParameters());
            var result = condition.Evaluate(contextMock.Object);
            Assert.IsFalse(result);
        }

        [Test]
        public void EvaluateReturnsTrueIfAChildPasses()
        {
            var condition = new Or();
            GenerateChildConditionMock(condition, true);
            GenerateChildConditionMock(condition, false);
            var contextMock = new Mock<TaskExecutionContext>(new TaskExecutionParameters());
            var result = condition.Evaluate(contextMock.Object);
            Assert.IsTrue(result);
        }

        private static void GenerateChildConditionMock(Or condition, bool passes)
        {
            var conditionMock = new Mock<TaskCondition>(MockBehavior.Strict);
            conditionMock.Setup(c => c.Evaluate(It.IsAny<TaskExecutionContext>()))
                .Returns(passes);
            condition.Children.Add(conditionMock.Object);
        }
        #endregion
    }
}
