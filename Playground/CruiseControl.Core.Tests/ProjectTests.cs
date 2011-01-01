namespace CruiseControl.Core.Tests
{
    using System;
    using System.Threading;
    using CruiseControl.Core.Interfaces;
    using CruiseControl.Core.Structure;
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
        public void StartInitialisesTriggers()
        {
            var initialised = false;
            var trigger = new TriggerStub
                              {
                                  OnInitialise = () => initialised = true
                              };
            var project = new Project("Test");
            project.Triggers.Add(trigger);
            project.Start();

            // Give the project time to start
            Thread.Sleep(100);
            Assert.IsTrue(initialised);
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
        public void StopCleansUpTriggers()
        {
            var cleaned = false;
            var trigger = new TriggerStub
                              {
                                  OnCleanUp = () => cleaned = true
                              };
            var project = new Project("Test");
            project.Triggers.Add(trigger);
            project.Start();

            // Give the project time to start
            Thread.Sleep(100);
            project.Stop();

            // Give the project time to stop
            Thread.Sleep(1000);
            Assert.IsTrue(cleaned);
        }

        [Test]
        public void ValidateValidatesTasks()
        {
            var validateCalled = false;
            var project = new Project("Test",
                                      new TaskStub
                                          {
                                              OnValidateAction = vl => validateCalled = true
                                          });
            var validationMock = new Mock<IValidationLog>();
            project.Validate(validationMock.Object);
            Assert.IsTrue(validateCalled);
        }

        [Test]
        public void ValidateValidatesSourceControlBlocks()
        {
            var validateCalled = false;
            var sourceControl = new SourceControlBlockStub
                                    {
                                        OnValidate = vl => validateCalled = true
                                    };
            var project = new Project("Test");
            project.SourceControl.Add(sourceControl);
            var validationMock = new Mock<IValidationLog>();
            project.Validate(validationMock.Object);
            Assert.IsTrue(validateCalled);
        }

        [Test]
        public void ValidateValidatesTriggers()
        {
            var validateCalled = false;
            var trigger = new TriggerStub
                              {
                                  OnValidate = vl => validateCalled = true
                              };
            var project = new Project("Test");
            project.Triggers.Add(trigger);
            var validationMock = new Mock<IValidationLog>();
            project.Validate(validationMock.Object);
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
        public void IntegrateInitialisesAndCleansUpSourceControl()
        {
            var initialised = false;
            var cleanedUp = false;
            var dummy = new SourceControlBlockStub
                {
                    OnInitialise = () => initialised = true,
                    OnCleanUp = () => cleanedUp = true
                };
            var project = new Project("test");
            project.SourceControl.Add(dummy);
            project.Integrate();
            Assert.IsTrue(initialised);
            Assert.IsTrue(cleanedUp);
        }

        [Test]
        public void IntegrateSkipsTasksWhoseConditionsFail()
        {
            var conditionMock = new Mock<TaskCondition>(MockBehavior.Strict);
            conditionMock.Setup(c => c.Evaluate(It.IsAny<TaskExecutionContext>()))
                .Returns(false);
            var initialised = false;
            var ran = false;
            var cleanedUp = false;
            var dummy = new TaskStub
                            {
                                OnInitialiseAction = () => initialised = true,
                                OnRunAction = c =>
                                                  {
                                                      ran = true;
                                                      return null;
                                                  },
                                OnCleanUpAction = () => cleanedUp = true
                            };
            dummy.Conditions.Add(conditionMock.Object);
            var project = new Project("test", dummy);
            project.Integrate();
            Assert.IsTrue(initialised);
            Assert.IsFalse(ran);
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

        [Test]
        public void ProjectNameIsCheckedInValidation()
        {
            var project = new Project();
            var verified = false;
            var validationStub = new ValidationLogStub
                                     {
                                         OnAddErrorMessage = (m, a) =>
                                                                 {
                                                                     Assert.AreEqual("The {0} has no name specified.", m);
                                                                     Assert.AreEqual("Project", a[0]);
                                                                     verified = true;
                                                                 }
                                     };
            project.Validate(validationStub);
            Assert.IsTrue(verified);
        }

        [Test]
        public void RemovingAChildResetsProjectOnItem()
        {
            var task = new Comment();
            var project = new Project("TestProject", task);

            project.Tasks.Remove(task);
            Assert.IsNull(task.Project);
        }

        [Test]
        public void UniversalNameHandlesProjectAsRoot()
        {
            var project = new Project("ProjectName");
            new Server(project)
                {
                    Name = "ServerName"
                };
            var actual = project.UniversalName;
            Assert.AreEqual("urn:ccnet:ServerName:ProjectName", actual);
        }

        [Test]
        public void UniversalNameHandlesProjectAsChild()
        {
            var project = new Project("ProjectName");
            var queue = new Queue("QueueName", project);
            new Server(queue)
                {
                    Name = "ServerName"
                };
            var actual = project.UniversalName;
            Assert.AreEqual("urn:ccnet:ServerName:ProjectName", actual);
        }

        [Test]
        public void ListProjectsReturnsOnlyItself()
        {
            var project = new Project();
            var projects = project.ListProjects();
            var expected = new[] { project };
            CollectionAssert.AreEqual(expected, projects);
        }
        #endregion
    }
}
