namespace CruiseControl.Core.Tests.Tasks.Conditions
{
    using CruiseControl.Core.Tasks.Conditions;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class HasModificationsTests
    {
        #region Tests
        [Test]
        public void EvaluateReturnsTrueIfThereAreModifications()
        {
            var condition = new HasModifications();
            var contextMock = new Mock<TaskExecutionContext>(new TaskExecutionParameters());
            contextMock.Setup(ec => ec.ModificationSets).Returns(new[] { new ModificationSet() });
            var result = condition.Evaluate(contextMock.Object);
            Assert.IsTrue(result);
        }

        [Test]
        public void EvaluateReturnsFalseIfThereAreNoModifications()
        {
            var condition = new HasModifications();
            var contextMock = new Mock<TaskExecutionContext>(new TaskExecutionParameters());
            contextMock.Setup(ec => ec.ModificationSets).Returns(new ModificationSet[0]);
            var result = condition.Evaluate(contextMock.Object);
            Assert.IsFalse(result);
        }
        #endregion
    }
}
