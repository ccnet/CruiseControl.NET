namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    using System;
    using System.Collections.Generic;
    using CruiseControl.Core;
    using CruiseControl.Core.Config;
    using CruiseControl.Core.Util;
    using CruiseControl.Remote;
    using Moq;
    using NUnit.Framework;
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
            this.mocks = new MockRepository(MockBehavior.Default);
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
            
            resultMock.Status = IntegrationStatus.Success;
            task.Run(resultMock);

            this.mocks.Verify();
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

            resultMock.Status = IntegrationStatus.Success;
            task.Run(resultMock);

            this.mocks.Verify();
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

            resultMock.Status = IntegrationStatus.Success;
            task.Run(resultMock);

            this.mocks.Verify();
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

            resultMock.Status = IntegrationStatus.Success;
            task.Run(resultMock);

            this.mocks.Verify();
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

            resultMock.Status = IntegrationStatus.Success;
            task.Run(resultMock);

            this.mocks.Verify();
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

            resultMock.Status = IntegrationStatus.Success;
            task.Run(resultMock);

            this.mocks.Verify();
            Assert.IsFalse(passRan);
            Assert.IsTrue(firstFailRan);
            Assert.IsTrue(secondFailRan);
            Assert.IsFalse(thirdFailRan);
        }
        [Test]
        public void ExecuteRunsAllInnerTasksWhenConditionsPassAndContinueOnFailure()
        {
            const int innerCount = 3;
            const int leafCount = 2;

            bool failRan = false;
            int taskRunCount = 0;

            var mockCondition = new MockCondition
            {
                EvalFunction = ir => true
            };
            var failTask = new MockTask
            {
                RunAction = ir => failRan = true
            };

            var innerTasks = new List<ConditionalTask>();
            for (var innerLoop = 1; innerLoop <= innerCount; innerLoop++)
            {
                var leafTasks = new List<MockTask>();
                for (var leafLoop = 1; leafLoop <= leafCount; leafLoop++)
                    leafTasks.Add(((innerLoop == 2) && (leafLoop == 2)) ?
                        new MockTask 
                        { 
                            RunAction = ir =>
                            {
                                taskRunCount++;
                                ir.Status = IntegrationStatus.Failure;
                            }
                        }
                        :
                        new MockTask
                        {
                            RunAction = ir =>
                            {
                                taskRunCount++;
                                ir.Status = IntegrationStatus.Success;
                            }
                        }
                        );

                innerTasks.Add(new ConditionalTask 
                    { 
                        ContinueOnFailure = false, 
                        Tasks = leafTasks.ToArray(),
                        TaskConditions = new[] { mockCondition }
                    });
            }

            var task = new ConditionalTask
            {
                Tasks = innerTasks.ToArray(),
                ElseTasks = new[] { failTask },
                TaskConditions = new[] { mockCondition },
                ContinueOnFailure = true
            };
            var resultMock = this.GenerateResultMock(leafCount, leafCount);
            AddResultMockExpectedClone(resultMock, leafCount, leafCount);
            AddResultMockExpectedClone(resultMock, leafCount, leafCount);
            AddResultMockExpectedMerge(resultMock);
            AddResultMockExpectedMerge(resultMock);

            resultMock.Status = IntegrationStatus.Success;
            task.Run(resultMock);

            this.mocks.Verify();
            Assert.AreEqual(innerCount * leafCount, taskRunCount, "Bad task run count");
            Assert.IsFalse(failRan);
        }

        [Test]
        public void ExecuteRunsAllInnerTasksWhenConditionsFailAndContinueOnFailure()
        {
            const int innerCount = 3;
            const int leafCount = 2;

            var passRan = false;
            int taskRunCount = 0;

            var mockCondition = new MockCondition
            {
                EvalFunction = ir => false
            };
            var passTask = new MockTask
            {
                RunAction = ir => passRan = true
            };


            var innerTasks = new List<ConditionalTask>();
            for (var innerLoop = 1; innerLoop <= innerCount; innerLoop++)
            {
                var leafTasks = new List<MockTask>();
                for (var leafLoop = 1; leafLoop <= leafCount; leafLoop++)
                    leafTasks.Add(((innerLoop == 2) && (leafLoop == 2)) ?
                        new MockTask 
                        { 
                            RunAction = ir =>
                            {
                                taskRunCount++;
                                ir.Status = IntegrationStatus.Failure;
                            }
                        }
                        :
                        new MockTask
                        {
                            RunAction = ir =>
                            {
                                taskRunCount++;
                                ir.Status = IntegrationStatus.Success;
                            }
                        }
                    );

                innerTasks.Add(new ConditionalTask 
                    { 
                        ContinueOnFailure = false,
                        ElseTasks = leafTasks.ToArray(),
                        TaskConditions = new[] { mockCondition }
                    });
            }

            var task = new ConditionalTask
            {
                Tasks = new[] { passTask },
                ElseTasks = innerTasks.ToArray(),
                TaskConditions = new[] { mockCondition },
                ContinueOnFailure = true
            };
            var resultMock = this.GenerateResultMock(leafCount, leafCount);
            AddResultMockExpectedClone(resultMock, leafCount, leafCount);
            AddResultMockExpectedClone(resultMock, leafCount, leafCount);
            AddResultMockExpectedMerge(resultMock);
            AddResultMockExpectedMerge(resultMock);

            resultMock.Status = IntegrationStatus.Success;
            task.Run(resultMock);

            this.mocks.Verify();
            Assert.IsFalse(passRan);
            Assert.AreEqual(innerCount * leafCount, taskRunCount, "Bad task run count");
        }

        #endregion

        private IIntegrationResult GenerateResultMock(int cloneCloneCount, int cloneMergeCount)
        {
            var buildInfo = mocks.Create<BuildProgressInformation>(string.Empty, string.Empty).Object;
            var result = mocks.Create<IIntegrationResult>(MockBehavior.Strict).Object;
            Mock.Get(result).SetupGet(_result => _result.BuildProgressInformation).Returns(buildInfo);
            Mock.Get(result).SetupProperty(_result => _result.Status);
            Mock.Get(result).SetupProperty(_result => _result.ExceptionResult);
            result.Status = IntegrationStatus.Success;
            AddResultMockExpectedClone(result, cloneCloneCount, cloneMergeCount);
            AddResultMockExpectedMerge(result);
            return result;
        }

        private IIntegrationResult GenerateResultMock(int cloneCloneCount)
        {
            return GenerateResultMock(cloneCloneCount, 0);
        }

        private IIntegrationResult GenerateResultMock()
        {
            return GenerateResultMock(0);
        }

        private void AddResultMockExpectedClone(IIntegrationResult result, int cloneCloneCount, int cloneMergeCount)
        {
            Mock.Get(result).Setup(_result => _result.Clone()).
                Returns(() =>
                {
                    var clone = mocks.Create<IIntegrationResult>(MockBehavior.Strict).Object;
                    Mock.Get(clone).SetupGet(_clone => _clone.BuildProgressInformation).Returns(result.BuildProgressInformation);
                    Mock.Get(clone).SetupProperty(_clone => _clone.Status);
                    Mock.Get(clone).SetupProperty(_clone => _clone.ExceptionResult);
                    for (int i = 0; i < cloneCloneCount; i++)
                        AddResultMockExpectedClone(clone);
                    for (int i = 0; i < cloneMergeCount; i++)
                        AddResultMockExpectedMerge(clone);
                    clone.Status = result.Status;
                    return clone;
                }).Verifiable();
        }

        private void AddResultMockExpectedClone(IIntegrationResult result, int cloneCloneCount)
        {
            AddResultMockExpectedClone(result, cloneCloneCount, 0);
        }

        private void AddResultMockExpectedClone(IIntegrationResult result)
        {
            AddResultMockExpectedClone(result, 0);
        }

        private void AddResultMockExpectedMerge(IIntegrationResult result)
        {
            Mock.Get(result).Setup(_result => _result.Merge(It.IsNotNull<IIntegrationResult>())).
                Callback<IIntegrationResult>(otherResult =>
                {
                    // Apply some rules to the status merging - basically apply a hierachy of status codes
                    if ((otherResult.Status == IntegrationStatus.Exception) || (result.Status == IntegrationStatus.Unknown))
                    {
                        result.Status = otherResult.Status;
                    }
                    else if (((otherResult.Status == IntegrationStatus.Failure) ||
                        (otherResult.Status == IntegrationStatus.Cancelled)) &&
                        (result.Status != IntegrationStatus.Exception))
                    {
                        result.Status = otherResult.Status;
                    }
                }).Verifiable();
        }
    }
}
