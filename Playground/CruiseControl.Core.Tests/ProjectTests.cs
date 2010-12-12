namespace CruiseControl.Core.Tests
{
    using CruiseControl.Core.Tasks;
    using NUnit.Framework;
    using Moq;
    using System.Diagnostics;
using System;

    public class ProjectTests
    {
        #region Tests
        [Test]
        public void ConstructorSetsNameAndTasks()
        {
            var name = "TestProject";
            var task = new Comment();
            var project = new Project(name, task);
            Assert.AreEqual(name, project.Name);
            Assert.AreEqual(1, project.Tasks.Count);
            Assert.AreSame(task, project.Tasks[0]);
        }

        [Test]
        public void TriggerWillStartAnIntegrationWithNoHost()
        {
            var wasStarted = false;
            var project = new ProjectStub
                {
                    OnIntegration = ic => wasStarted = true
                };
            project.Trigger();

            Assert.IsTrue(wasStarted);
        }

        [Test]
        public void TriggerWillAskHostToStartAnIntegration()
        {
            var hostMock = new Mock<ServerItem>(MockBehavior.Strict);
            hostMock.Setup(h => h.AskToIntegrate(It.IsAny<IntegrationContext>()))
                .Verifiable();
            var wasStarted = false;
            var project = new ProjectStub
                {
                    OnIntegration = ic => wasStarted = true,
                    Host = hostMock.Object
                };
            project.Trigger();

            Assert.IsTrue(wasStarted);
            hostMock.Verify();
        }

        [Test]
        public void TriggerWillNotIntegrateIfHostCancels()
        {
            var hostMock = new Mock<ServerItem>(MockBehavior.Strict);
            hostMock.Setup(h => h.AskToIntegrate(It.IsAny<IntegrationContext>()))
                .Callback((IntegrationContext ic) => ic.Cancel())
                .Verifiable();
            var wasStarted = false;
            var project = new ProjectStub
                {
                    OnIntegration = ic => wasStarted = true,
                    Host = hostMock.Object
                };
            project.Trigger();

            Assert.IsFalse(wasStarted);
            hostMock.Verify();
        }
        #endregion

        #region Private classes
        private class ProjectStub
            : Project
        {
            public Action<IntegrationContext> OnIntegration { get; set; }
            public override void Integrate(IntegrationContext context)
            {
                if (this.OnIntegration != null)
                {
                    this.OnIntegration(context);
                }
                else
                {
                    base.Integrate(context);
                }
            }
        }
        #endregion
    }
}
