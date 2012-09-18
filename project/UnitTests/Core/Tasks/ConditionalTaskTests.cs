namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    using System;
    using System.Collections.Generic;
    using CruiseControl.Core;
    using CruiseControl.Core.Config;
    using CruiseControl.Core.Util;
    using CruiseControl.Remote;
    using NUnit.Framework;
    using Rhino.Mocks;
    using ThoughtWorks.CruiseControl.Core.Tasks;
    using ThoughtWorks.CruiseControl.UnitTests.Core.Tasks.Conditions;

    [TestFixture]
    public class ConditionalTaskTests
    {
        private MockRepository mocks;

        #region Setup
        [SetUp]
        public void Setup()
        {
            this.mocks = new MockRepository();
        }
        #endregion

        #region Tests
        [Test]
        public void ConstructorInitialisesEmptyTaskLists()
        {
            var task = new ConditionalTask();
            Assert.AreEqual(0, task.Tasks.Length);
            Assert.AreEqual(0, task.ElseTasks.Length);
        }

        [Test]
        public void ValidateValidatesConditions()
        {
            var wasValidated = false;
            var conditionMock = new MockCondition
                                    {
                                        ValidateAction = (c, t, ep) => wasValidated = true
                                    };
            var task = new ConditionalTask
                           {
                               TaskConditions = new[] {conditionMock}
                           };
            task.Validate(null, ConfigurationTrace.Start(this), null);
            Assert.IsTrue(wasValidated);
        }

        [Test]
        public void ValidateValidatesTasks()
        {
            var wasValidated = false;
            var taskMock = new MockTask()
                               {
                                   ValidateAction = (c, t, ep) => wasValidated = true
                               };
            var task = new ConditionalTask
                           {
                               Tasks = new[] {taskMock}
                           };
            task.Validate(null, ConfigurationTrace.Start(this), null);
            Assert.IsTrue(wasValidated);
        }

        [Test]
        public void ValidateValidatesElseTasks()
        {
            var wasValidated = false;
            var taskMock = new MockTask()
                               {
                                   ValidateAction = (c, t, ep) => wasValidated = true
                               };
            var task = new ConditionalTask
                           {
                               ElseTasks = new[] {taskMock}
                           };
            task.Validate(null, ConfigurationTrace.Start(this), null);
            Assert.IsTrue(wasValidated);
        }

        [Test]
        public void InitialiseStatusUpdatesTheStatus()
        {
            var task = new ConditionalTask
                           {
                               Tasks = new[]
                                           {
                                               new MockTask()
                                           }
                           };
            task.InitialiseStatus(ItemBuildStatus.Pending);
            Assert.AreEqual(ItemBuildStatus.Pending, task.CurrentStatus.Status);
        }

        [Test]
        public void ExecuteRunsTasksWhenConditionsPass()
        {
            var passRan = false;
            var failRan = false;
            var mockCondition = new MockCondition
                                    {
                                        EvalFunction = ir => true
                                    };
            var passTask = new MockTask
                               {
                                   RunAction = ir => passRan = true
                               };
            var failTask = new MockTask
                               {
                                   RunAction = ir => failRan = true
                               };
            var task = new ConditionalTask
                           {
                               Tasks = new[] {passTask},
                               ElseTasks = new[] {failTask},
                               TaskConditions = new[] {mockCondition}
                           };
            var resultMock = this.GenerateResultMock();
            
            this.mocks.ReplayAll();
            resultMock.Status = IntegrationStatus.Success;
            task.Run(resultMock);

            this.mocks.VerifyAll();
            Assert.IsTrue(passRan);
            Assert.IsFalse(failRan);
        }

        [Test]
        public void ExecuteRunsElseTasksWhenConditionsFail()
        {
            var passRan = false;
            var failRan = false;
            var mockCondition = new MockCondition
                                    {
                                        EvalFunction = ir => false
                                    };
            var passTask = new MockTask
                               {
                                   RunAction = ir => passRan = true
                               };
            var failTask = new MockTask
                               {
                                   RunAction = ir => failRan = true
                               };
            var task = new ConditionalTask
                           {
                               Tasks = new[] {passTask},
                               ElseTasks = new[] {failTask},
                               TaskConditions = new[] {mockCondition}
                           };
            var resultMock = this.GenerateResultMock();

            this.mocks.ReplayAll();
            resultMock.Status = IntegrationStatus.Success;
            task.Run(resultMock);

            this.mocks.VerifyAll();
            Assert.IsFalse(passRan);
            Assert.IsTrue(failRan);
        }

        [Test]
        public void ExecuteRunsAllTasksWhenConditionsPassAndContinueOnFailure()
        {
            var firstPassRan = false;
            var secondPassRan = false;
            var thirdPassRan = false;
            var failRan = false;
            var mockCondition = new MockCondition
            {
                EvalFunction = ir => true
            };
            var firstPassTask = new MockTask
            {
                RunAction = ir => firstPassRan = true
            };
            var secondPassTask = new MockTask
            {
                RunAction = ir => { secondPassRan = true; ir.Status = IntegrationStatus.Failure; }
            };
            var thirdPassTask = new MockTask
            {
                RunAction = ir => thirdPassRan = true
            };
            var failTask = new MockTask
            {
                RunAction = ir => failRan = true
            };
            var task = new ConditionalTask
            {
                Tasks = new[] { firstPassTask, secondPassTask, thirdPassTask },
                ElseTasks = new[] { failTask },
                TaskConditions = new[] { mockCondition }
            };
            var resultMock = this.GenerateResultMock();
            AddResultMockExpectedClone(resultMock);
            AddResultMockExpectedClone(resultMock);
            AddResultMockExpectedMerge(resultMock);
            AddResultMockExpectedMerge(resultMock);

            this.mocks.ReplayAll();
            resultMock.Status = IntegrationStatus.Success;
            task.Run(resultMock);

            this.mocks.VerifyAll();
            Assert.IsTrue(firstPassRan);
            Assert.IsTrue(secondPassRan);
            Assert.IsTrue(thirdPassRan);
            Assert.IsFalse(failRan);
        }

        [Test]
        public void ExecuteRunsAllTasksWhenConditionsFailAndContinueOnFailure()
        {
            var passRan = false;
            var firstFailRan = false;
            var secondFailRan = false;
            var thirdFailRan = false;
            var mockCondition = new MockCondition
            {
                EvalFunction = ir => false
            };
            var passTask = new MockTask
            {
                RunAction = ir => passRan = true
            };
            var firstFailTask = new MockTask
            {
                RunAction = ir => firstFailRan = true
            };
            var secondFailTask = new MockTask
            {
                RunAction = ir => { secondFailRan = true; ir.Status = IntegrationStatus.Failure; }
            };
            var thirdFailTask = new MockTask
            {
                RunAction = ir => thirdFailRan = true
            };
            var task = new ConditionalTask
            {
                Tasks = new[] { passTask },
                ElseTasks = new[] { firstFailTask, secondFailTask, thirdFailTask },
                TaskConditions = new[] { mockCondition }
            };
            var resultMock = this.GenerateResultMock();
            AddResultMockExpectedClone(resultMock);
            AddResultMockExpectedClone(resultMock);
            AddResultMockExpectedMerge(resultMock);
            AddResultMockExpectedMerge(resultMock);

            this.mocks.ReplayAll();
            resultMock.Status = IntegrationStatus.Success;
            task.Run(resultMock);

            this.mocks.VerifyAll();
            Assert.IsFalse(passRan);
            Assert.IsTrue(firstFailRan);
            Assert.IsTrue(secondFailRan);
            Assert.IsTrue(thirdFailRan);
        }

        [Test]
        public void ExecuteRunsAllTasksWhenConditionsPassAndNotContinueOnFailure()
        {
            var firstPassRan = false;
            var secondPassRan = false;
            var thirdPassRan = false;
            var failRan = false;
            var mockCondition = new MockCondition
            {
                EvalFunction = ir => true
            };
            var firstPassTask = new MockTask
            {
                RunAction = ir => firstPassRan = true
            };
            var secondPassTask = new MockTask
            {
                RunAction = ir => { secondPassRan = true; ir.Status = IntegrationStatus.Failure; }
            };
            var thirdPassTask = new MockTask
            {
                RunAction = ir => thirdPassRan = true
            };
            var failTask = new MockTask
            {
                RunAction = ir => failRan = true
            };
            var task = new ConditionalTask
            {
                Tasks = new[] { firstPassTask, secondPassTask, thirdPassTask },
                ElseTasks = new[] { failTask },
                TaskConditions = new[] { mockCondition },
                ContinueOnFailure = false
            };
            var resultMock = this.GenerateResultMock();
            AddResultMockExpectedClone(resultMock);
            AddResultMockExpectedMerge(resultMock);

            this.mocks.ReplayAll();
            resultMock.Status = IntegrationStatus.Success;
            task.Run(resultMock);

            this.mocks.VerifyAll();
            Assert.IsTrue(firstPassRan);
            Assert.IsTrue(secondPassRan);
            Assert.IsFalse(thirdPassRan);
            Assert.IsFalse(failRan);
        }

        [Test]
        public void ExecuteRunsAllTasksWhenConditionsFailAndNotContinueOnFailure()
        {
            var passRan = false;
            var firstFailRan = false;
            var secondFailRan = false;
            var thirdFailRan = false;
            var mockCondition = new MockCondition
            {
                EvalFunction = ir => false
            };
            var passTask = new MockTask
            {
                RunAction = ir => passRan = true
            };
            var firstFailTask = new MockTask
            {
                RunAction = ir => firstFailRan = true
            };
            var secondFailTask = new MockTask
            {
                RunAction = ir => { secondFailRan = true; ir.Status = IntegrationStatus.Failure; }
            };
            var thirdFailTask = new MockTask
            {
                RunAction = ir => thirdFailRan = true
            };
            var task = new ConditionalTask
            {
                Tasks = new[] { passTask },
                ElseTasks = new[] { firstFailTask, secondFailTask, thirdFailTask },
                TaskConditions = new[] { mockCondition },
                ContinueOnFailure = false
            };
            var resultMock = this.GenerateResultMock();
            AddResultMockExpectedClone(resultMock);
            AddResultMockExpectedMerge(resultMock);

            this.mocks.ReplayAll();
            resultMock.Status = IntegrationStatus.Success;
            task.Run(resultMock);

            this.mocks.VerifyAll();
            Assert.IsFalse(passRan);
            Assert.IsTrue(firstFailRan);
            Assert.IsTrue(secondFailRan);
            Assert.IsFalse(thirdFailRan);
        }
        #endregion

        private IIntegrationResult GenerateResultMock()
        {
            var buildInfo = mocks.DynamicMock<BuildProgressInformation>(string.Empty, string.Empty);
            var result = mocks.StrictMock<IIntegrationResult>();
            SetupResult.For(result.BuildProgressInformation).Return(buildInfo);
            SetupResult.For(result.Status).PropertyBehavior();
            SetupResult.For(result.ExceptionResult).PropertyBehavior();
            result.Status = IntegrationStatus.Success;
            AddResultMockExpectedClone(result);
            AddResultMockExpectedMerge(result);
            return result;
        }

        private void AddResultMockExpectedClone(IIntegrationResult result)
        {
            Expect.Call(result.Clone()).
                Do((Func<IIntegrationResult>)(() =>
                {
                    var clone = mocks.StrictMock<IIntegrationResult>();
                    SetupResult.For(clone.BuildProgressInformation).Return(result.BuildProgressInformation);
                    SetupResult.For(clone.Status).PropertyBehavior();
                    SetupResult.For(clone.ExceptionResult).PropertyBehavior();
                    clone.Status = result.Status;
                    clone.Replay();
                    return clone;
                }));
        }

        private void AddResultMockExpectedMerge(IIntegrationResult result)
        {
            Expect.Call(() => result.Merge(Arg<IIntegrationResult>.Is.NotNull)).
                Do((Action<IIntegrationResult>)((otherResult) =>
                {
                    result.Status = otherResult.Status;
                }));
        }
    }
}
