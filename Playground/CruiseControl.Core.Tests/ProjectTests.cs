namespace CruiseControl.Core.Tests
{
    using System;
    using System.Threading;
    using CruiseControl.Core.Tasks;
    using CruiseControl.Core.Tests.Stubs;
    using Moq;
    using NUnit.Framework;

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
        public void ItemTypeIsProject()
        {
            var project = new Project();
            Assert.AreEqual("Project", project.ItemType);
        }

        [Test]
        public void StartStartsTheProject()
        {
            var project = new Project("Test");
            project.Start();

            // Give the project time to start
            Thread.Sleep(100);
            Assert.AreEqual(ProjectState.Running, project.State);
        }

        [Test]
        public void StartFailsIfAlreadyStarted()
        {
            var project = new Project("Test");
            project.Start();

            // Give the project time to start
            Thread.Sleep(100);
            Assert.Throws<InvalidOperationException>(project.Start);
        }

        [Test]
        public void ProjectCannotStartWithoutAName()
        {
            var project = new Project();
            Assert.Throws<InvalidOperationException>(project.Start);
        }

        [Test]
        public void StopFailsIfNotStarted()
        {
            var project = new Project();
            Assert.Throws<InvalidOperationException>(project.Stop);
        }

        [Test]
        public void StopStopsAStartedProject()
        {
            var project = new Project("Test");
            project.Start();

            // Give the project time to start
            Thread.Sleep(100);
            project.Stop();

            // Give the project time to stop
            Thread.Sleep(1000);
            Assert.AreEqual(ProjectState.Stopped, project.State);
        }

        [Test]
        public void ValidateValidatesTasks()
        {
            var validateCalled = false;
            var project = new Project("Test",
                                      new TaskStub
                                          {
                                              OnValidateAction = () => validateCalled = true
                                          });
            project.Validate();
            Assert.IsTrue(validateCalled);
        }

        [Test]
        public void IntegrateInitialisesRunsAndCleansUp()
        {
            var initialised = false;
            var ran = false;
            var cleanedUp = false;
            var dummy = new TaskStub
                            {
                                OnInitialiseAction = () => initialised = true,
                                OnRunAction = c => { ran = true; return null; },
                                OnCleanUpAction = () => cleanedUp = true
                            };
            var project = new Project("test", dummy);
            project.Integrate();
            Assert.IsTrue(initialised);
            Assert.IsTrue(ran);
            Assert.IsTrue(cleanedUp);
        }

        [Test]
        public void AskToIntegrateDoesNothingWithNoHost()
        {
            var project = new Project();
            var context = new IntegrationContext(project);
            project.AskToIntegrate(context);
            Assert.IsTrue(context.Wait(TimeSpan.FromMilliseconds(1)));
        }

        [Test]
        public void AskToIntegrateAsksHostToContinue()
        {
            var context = new IntegrationContext(null);
            var wasCalled = false;
            var hostMock = new Mock<ServerItem>(MockBehavior.Strict);
            hostMock.Setup(h => h.AskToIntegrate(context)).Callback(() => wasCalled = true);
            var project = new Project
                              {
                                  Host = hostMock.Object
                              };
            project.AskToIntegrate(context);
            Assert.IsTrue(context.Wait(TimeSpan.FromMilliseconds(1)));
            Assert.IsTrue(wasCalled);
        }
        #endregion
    }
}
