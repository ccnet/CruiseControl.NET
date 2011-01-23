namespace CruiseControl.Core.Tests.Triggers
{
    using System;
    using CruiseControl.Core.Interfaces;
    using CruiseControl.Core.Tests.Stubs;
    using CruiseControl.Core.Triggers;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class MultipleTests
    {
        #region Tests
        [Test]
        public void ValidateValidatesChildren()
        {
            var validated = false;
            var child = new TriggerStub
                            {
                                OnValidate = vl => validated = true
                            };
            var trigger = new Multiple(child);
            var validationMock = new Mock<IValidationLog>();
            trigger.Validate(validationMock.Object);
            Assert.IsTrue(validated);
        }

        [Test]
        public void InitialiseInitialisesChildren()
        {
            var initialised = false;
            var child = new TriggerStub
            {
                OnInitialise = () => initialised = true
            };
            var trigger = new Multiple(child);
            trigger.Initialise();
            Assert.IsTrue(initialised);
        }

        [Test]
        public void CleanUpCleansChildren()
        {
            var cleaned = false;
            var child = new TriggerStub
                            {
                                OnCleanUp = () => cleaned = true
                            };
            var trigger = new Multiple(child);
            trigger.CleanUp();
            Assert.IsTrue(cleaned);
        }

        [Test]
        public void ResetResetsChildren()
        {
            var reset = false;
            var child = new TriggerStub
                            {
                                OnResetAction = () => reset = true
                            };
            var trigger = new Multiple(child);
            trigger.Reset();
            Assert.IsTrue(reset);
        }

        [Test]
        public void SettingProjectSetsChildrenProjects()
        {
            var project = new Core.Project();
            var child = new TriggerStub();
            new Multiple(child) { Project = project };
            Assert.AreSame(project, child.Project);
        }

        [Test]
        public void NextTimeReturnsLowestValidTime()
        {
            var child1 = new TriggerStub();
            var child2 = new TriggerStub { NextTimeValue = DateTime.Now };
            var child3 = new TriggerStub { NextTimeValue = DateTime.Now.AddHours(1) };
            var trigger = new Multiple(child1, child2, child3);
            var actual = trigger.NextTime;
            Assert.AreEqual(child2.NextTimeValue, actual);
        }

        [Test]
        public void CheckReturnsNullIfAllChildrenNullWithOr()
        {
            var child1 = new TriggerStub { OnCheckAction = () => null };
            var child2 = new TriggerStub { OnCheckAction = () => null };
            var trigger = new Multiple(child1, child2);
            var actual = trigger.Check();
            Assert.IsNull(actual);
        }

        [Test]
        public void CheckReturnsRequestIfAnyChildSetWithOr()
        {
            var child1 = new TriggerStub { OnCheckAction = () => null };
            var child2 = new TriggerStub { OnCheckAction = () => new IntegrationRequest("Dummy") };
            var trigger = new Multiple(child1, child2);
            var actual = trigger.Check();
            Assert.IsNotNull(actual);
            Assert.AreEqual("Multiple", actual.SourceTrigger);
        }

        [Test]
        public void CheckReturnsNullIfAnyChildNullWithAnd()
        {
            var child1 = new TriggerStub { OnCheckAction = () => null };
            var child2 = new TriggerStub { OnCheckAction = () => new IntegrationRequest("Dummy") };
            var trigger = new Multiple(child1, child2)
                              {
                                  Condition = CombinationOperator.And
                              };
            var actual = trigger.Check();
            Assert.IsNull(actual);
        }

        [Test]
        public void CheckReturnsRequestIfAllChildrenSetWithAnd()
        {
            var child1 = new TriggerStub { OnCheckAction = () => new IntegrationRequest("Dummy") };
            var child2 = new TriggerStub { OnCheckAction = () => new IntegrationRequest("Dummy") };
            var trigger = new Multiple(child1, child2)
                              {
                                  Condition = CombinationOperator.And
                              };
            var actual = trigger.Check();
            Assert.IsNotNull(actual);
            Assert.AreEqual("Multiple", actual.SourceTrigger);
        }
        #endregion
    }
}
