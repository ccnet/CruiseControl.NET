namespace CruiseControl.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CruiseControl.Core.Tests.Stubs;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class TaskTests
    {
        #region Tests
        [Test]
        public void CanExecuteReturnsTrueWithNoConditions()
        {
            var context = new TaskExecutionContext();
            var task = new TaskStub();
            var expected = task.CanRun(context);
            Assert.IsTrue(expected);
        }

        [Test]
        public void CanExecuteReturnsTrueWhenAllConditionsPass()
        {
            var context = new TaskExecutionContext();
            var condition1Mock = new Mock<TaskCondition>();
            var condition2Mock = new Mock<TaskCondition>();
            condition1Mock.Setup(c => c.Evaluate(context)).Returns(true);
            condition2Mock.Setup(c => c.Evaluate(context)).Returns(true);
            var task = new TaskStub();
            task.Conditions.Add(condition1Mock.Object);
            task.Conditions.Add(condition2Mock.Object);
            var expected = task.CanRun(context);
            Assert.IsTrue(expected);
        }

        [Test]
        public void CanExecuteReturnsFalseWhenAnyConditionIsFalse()
        {
            var context = new TaskExecutionContext();
            var condition1Mock = new Mock<TaskCondition>();
            var condition2Mock = new Mock<TaskCondition>();
            condition1Mock.Setup(c => c.Evaluate(context)).Returns(true);
            condition2Mock.Setup(c => c.Evaluate(context)).Returns(false);
            var task = new TaskStub();
            task.Conditions.Add(condition1Mock.Object);
            task.Conditions.Add(condition2Mock.Object);
            var expected = task.CanRun(context);
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
            task.Skip();
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
            var result = task.Run(new TaskExecutionContext());
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
            var result = task.Run(new TaskExecutionContext());
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
        #endregion
    }
}
