namespace CruiseControl.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CruiseControl.Core.Interfaces;
    using CruiseControl.Core.Tests.Stubs;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class TaskTests
    {
        #region Tests
        [Test]
        public void NewTaskHasAStateOfLoaded()
        {
            var task = new TaskStub();
            Assert.AreEqual(TaskState.Loaded, task.State);
        }

        [Test]
        public void ValidateSetsStateAndFiresOnValidate()
        {
            var validated = false;
            var task = new TaskStub
                           {
                               OnValidateAction = vl => validated = true
                           };
            var validationMock = new Mock<IValidationLog>();
            task.Validate(validationMock.Object);
            Assert.AreEqual(TaskState.Validated, task.State);
            Assert.IsTrue(validated);
        }

        [Test]
        public void CanExecuteReturnsTrueWithNoConditions()
        {
            var contextMock = GenerateContextMock();
            var task = new TaskStub();
            var expected = task.CanRun(contextMock.Object);
            Assert.IsTrue(expected);
        }

        [Test]
        public void CanExecuteReturnsTrueWhenAllConditionsPass()
        {
            var contextMock = GenerateContextMock();
            var condition1Mock = new Mock<TaskCondition>();
            var condition2Mock = new Mock<TaskCondition>();
            condition1Mock.Setup(c => c.Evaluate(contextMock.Object)).Returns(true);
            condition2Mock.Setup(c => c.Evaluate(contextMock.Object)).Returns(true);
            var task = new TaskStub();
            task.Conditions.Add(condition1Mock.Object);
            task.Conditions.Add(condition2Mock.Object);
            var expected = task.CanRun(contextMock.Object);
            Assert.IsTrue(expected);
        }

        [Test]
        public void CanExecuteReturnsFalseWhenAnyConditionIsFalse()
        {
            var contextMock = GenerateContextMock();
            var condition1Mock = new Mock<TaskCondition>();
            var condition2Mock = new Mock<TaskCondition>();
            condition1Mock.Setup(c => c.Evaluate(contextMock.Object)).Returns(true);
            condition2Mock.Setup(c => c.Evaluate(contextMock.Object)).Returns(false);
            var task = new TaskStub();
            task.Conditions.Add(condition1Mock.Object);
            task.Conditions.Add(condition2Mock.Object);
            var expected = task.CanRun(contextMock.Object);
            Assert.IsFalse(expected);
        }

        [Test]
        public void InitialiseSetsStateToPending()
        {
            var initialised = false;
            var task = new TaskStub
                           {
                               OnInitialiseAction = () => initialised = true
                           };
            task.Initialise();
            Assert.AreEqual(TaskState.Pending, task.State);
            Assert.IsTrue(initialised);
        }

        [Test]
        public void SkipSetsStateToSkipped()
        {
            var task = new TaskStub();
            var contextMock = GenerateContextMock();
            contextMock.Setup(ec => ec.AddEntryToBuildLog("Task 'TaskStub' has been skipped"));
            task.Skip(contextMock.Object);
            Assert.AreEqual(TaskState.Skipped, task.State);
        }

        [Test]
        public void RunSetsStateToCompleted()
        {
            var ran = false;
            var intermediateState = TaskState.Unknown;
            Task task = null;
            Func<TaskExecutionContext, IEnumerable<Task>> action = c =>
                                                                       {
                                                                           ran = true;
                                                                           intermediateState = task.State;
                                                                           return null;
                                                                       };
            task = new TaskStub
                           {
                               OnRunAction = action
                           };
            var contextMock = GenerateContextMock();
            var result = task.Run(contextMock.Object);
            Assert.AreEqual(result.Count(), 0); // This line is needed to actually trigger the method
            Assert.AreEqual(TaskState.Completed, task.State);
            Assert.AreEqual(TaskState.Executing, intermediateState);
            Assert.IsTrue(ran);
        }

        [Test]
        public void RunReturnsChildTasks()
        {
            var ran = false;
            var childTask = new TaskStub
                                {
                                    OnRunAction = c =>
                                                      {
                                                          // This should not be triggered as Run() does not run the child
                                                          // tasks - instead it is the caller's job to run them
                                                          ran = true;
                                                          return null;
                                                      }
                                };
            var task = new TaskStub
                           {
                               OnRunAction = c => new[] { childTask }
                           };
            var contextMock = GenerateContextMock();
            var result = task.Run(contextMock.Object);
            Assert.AreEqual(result.Count(), 1); // This line is needed to actually trigger the method
            Assert.IsFalse(ran);
        }

        [Test]
        public void CleanUpSetsStateToSkippedIfStatusIsPending()
        {
            var cleanedUp = false;
            var task = new TaskStub
                           {
                               OnCleanUpAction = () => cleanedUp = true
                           };
            task.Initialise();
            task.CleanUp();
            Assert.AreEqual(TaskState.Skipped, task.State);
            Assert.IsTrue(cleanedUp);
        }

        [Test]
        public void CleanUpSetsStateToTerminatedIfStatusIsExecuting()
        {
            var task = new TaskStub
                           {
                               OnRunAction = c =>
                                                 {
                                                     // This will break the run leaving the task in
                                                     // the executing state
                                                     throw new Exception();
                                                 }
                           };
            task.Initialise();
            var result = task.Run(null);
            Assert.Throws<Exception>(() => result.Count());
            task.CleanUp();
            Assert.AreEqual(TaskState.Terminated, task.State);
        }

        [Test]
        public void ValidateValidatesConditions()
        {
            var validated = false;
            var conditionStub = new TaskConditionStub
                                    {
                                        OnValidate = vl => validated = true
                                    };
            var task = new TaskStub();
            task.Conditions.Add(conditionStub);
            var validationMock = new Mock<IValidationLog>();
            task.Validate(validationMock.Object);
            Assert.IsTrue(validated);
        }

        [Test]
        public void ValidateValidatesFailureActions()
        {
            var validated = false;
            var failureActionStub = new TaskFailureActionStub
                                    {
                                        OnValidate = vl => validated = true
                                    };
            var task = new TaskStub();
            task.FailureActions.Add(failureActionStub);
            var validationMock = new Mock<IValidationLog>();
            task.Validate(validationMock.Object);
            Assert.IsTrue(validated);
        }
        #endregion

        #region Helpers
        private static Mock<TaskExecutionContext> GenerateContextMock()
        {
            return new Mock<TaskExecutionContext>(MockBehavior.Strict, null, null, null, null);
        }
        #endregion
    }
}
