namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
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
        #endregion

        private IIntegrationResult GenerateResultMock()
        {
            var buildInfo = mocks.DynamicMock<BuildProgressInformation>(string.Empty, string.Empty);
            var result = mocks.StrictMock<IIntegrationResult>();
            SetupResult.For(result.BuildProgressInformation).Return(buildInfo);
            SetupResult.For(result.Status).PropertyBehavior();
            Expect.Call(result.Clone()).Return(null);
            Expect.Call(() => result.Merge(Arg<IIntegrationResult>.Is.Anything));
            return result;
        }
    }
}
