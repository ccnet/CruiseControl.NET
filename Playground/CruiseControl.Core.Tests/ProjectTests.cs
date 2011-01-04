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
            Assert.IsNull(project.MainThreadException);
        }

        [Test]
        public void StopCleansUpTriggers()
        {
            var cleaned = false;
            var trigger = new TriggerStub
                              {
                                  OnCleanUp = () => cleaned = true,
                                  OnCheckAction = () => null
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
            Assert.IsNull(project.MainThreadException);
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
            var request = new IntegrationRequest("Dummy");
            project.Integrate(request);
            Assert.IsTrue(initialised);
            Assert.IsTrue(ran);
            Assert.IsTrue(cleanedUp);
        }

        [Test]
        public void IntegrateHandlesErrorDuringInitialisation()
        {
            var ran = false;
            var cleanedUp = false;
            var dummy = new TaskStub
                            {
                                OnInitialiseAction = () =>
                                                         {
                                                             throw new Exception("Oops!");
                                                         },
                                OnRunAction = c =>
                                                  {
                                                      ran = true;
                                                      return null;
                                                  },
                                OnCleanUpAction = () => cleanedUp = true
                            };
            var project = new Project("test", dummy);
            var request = new IntegrationRequest("Dummy");
            project.Integrate(request);
            Assert.IsFalse(ran);
            Assert.IsTrue(cleanedUp);
        }

        [Test]
        public void IntegrateHandlesErrorDuringRun()
        {
            var initialised = false;
            var cleanedUp = false;
            var dummy = new TaskStub
                            {
                                OnInitialiseAction = () => initialised = true,
                                OnRunAction = c =>
                                                  {
                                                      throw new Exception("Oops!");
                                                  },
                                OnCleanUpAction = () => cleanedUp = true
                            };
            var project = new Project("test", dummy);
            var request = new IntegrationRequest("Dummy");
            project.Integrate(request);
            Assert.IsTrue(initialised);
            Assert.IsTrue(cleanedUp);
        }

        [Test]
        public void IntegrateHandlesErrorDuringTaskCleanUp()
        {
            var initialised = false;
            var ran = false;
            var dummy = new TaskStub
                            {
                                OnInitialiseAction = () => initialised = true,
                                OnRunAction = c =>
                                                  {
                                                      ran = true;
                                                      return null;
                                                  },
                                OnCleanUpAction = () =>
                                                      {
                                                          throw new Exception("Oops");
                                                      }
                            };
            var project = new Project("test", dummy);
            var request = new IntegrationRequest("Dummy");
            project.Integrate(request);
            Assert.IsTrue(initialised);
            Assert.IsTrue(ran);
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
            var request = new IntegrationRequest("Dummy");
            project.Integrate(request);
            Assert.IsTrue(initialised);
            Assert.IsTrue(cleanedUp);
        }

        [Test]
        public void IntegrateHandlesErrorDuringSourceControlCleanUp()
        {
            var initialised = false;
            var dummy = new SourceControlBlockStub
            {
                OnInitialise = () => initialised = true,
                OnCleanUp = () =>
                                {
                                    throw new Exception("Oops");
                                }
            };
            var project = new Project("test");
            project.SourceControl.Add(dummy);
            var request = new IntegrationRequest("Dummy");
            project.Integrate(request);
            Assert.IsTrue(initialised);
        }

        [Test]
        public void IntegrateResetsTriggers()
        {
            var reset = false;
            var dummy = new TriggerStub
                            {
                                OnResetAction = () => reset = true
                            };
            var project = new Project("test");
            project.Triggers.Add(dummy);
            var request = new IntegrationRequest("Dummy");
            project.Integrate(request);
            Assert.IsTrue(reset);
        }

        [Test]
        public void IntegrateHandlesErrorDuringTriggerReset()
        {
            var dummy = new TriggerStub
                            {
                                OnResetAction = () =>
                                                    {
                                                        throw new Exception("Oops");
                                                    }
                            };
            var project = new Project("test");
            project.Triggers.Add(dummy);
            var request = new IntegrationRequest("Dummy");
            project.Integrate(request);
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
            var request = new IntegrationRequest("Dummy");
            project.Integrate(request);
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

        [Test]
        public void IntegrationStartsFromTrigger()
        {
            var triggered = false;
            var trigger = GenerateRunOnceTrigger(() => triggered = true);
            var ran = false;
            var task = new TaskStub
                           {
                               OnRunAction = tc =>
                                                 {
                                                     ran = true;
                                                     return null;
                                                 }
                           };
            var project = new Project("Test", task);
            project.Triggers.Add(trigger);
            project.Start();

            // Give the project time to start
            Thread.Sleep(100);
            project.Stop();

            // Give the project time to stop
            Thread.Sleep(1000);
            Assert.IsTrue(triggered);
            Assert.IsTrue(ran);
            Assert.IsNull(project.MainThreadException);
        }

        [Test]
        public void IntegrationAsksHostAndContinuesOnAllowed()
        {
            var triggered = false;
            var trigger = GenerateRunOnceTrigger(() => triggered = true);
            var ran = false;
            var task = new TaskStub
                           {
                               OnRunAction = tc =>
                                                 {
                                                     ran = true;
                                                     return null;
                                                 }
                           };
            var hostMock = new Mock<ServerItem>(MockBehavior.Strict);
            hostMock.Setup(h => h.AskToIntegrate(It.IsAny<IntegrationContext>()));
            var project = new Project("Test", task) { Host = hostMock.Object };
            project.Triggers.Add(trigger);
            project.Start();

            // Give the project time to start
            Thread.Sleep(100);
            project.Stop();

            // Give the project time to stop
            Thread.Sleep(1000);
            Assert.IsTrue(triggered);
            Assert.IsTrue(ran);
            Assert.IsNull(project.MainThreadException);
        }

        [Test]
        public void IntegrationAsksHostAndStopsOnDenied()
        {
            var triggered = false;
            var trigger = GenerateRunOnceTrigger(() => triggered = true);
            var ran = false;
            var task = new TaskStub
                           {
                               OnRunAction = tc =>
                                                 {
                                                     ran = true;
                                                     return null;
                                                 }
                           };
            var hostMock = new Mock<ServerItem>(MockBehavior.Strict);
            hostMock.Setup(h => h.AskToIntegrate(It.IsAny<IntegrationContext>()))
                .Callback((IntegrationContext c) => c.Cancel());
            var project = new Project("Test", task) { Host = hostMock.Object };
            project.Triggers.Add(trigger);
            project.Start();

            // Give the project time to start
            Thread.Sleep(100);
            project.Stop();

            // Give the project time to stop
            Thread.Sleep(1000);
            Assert.IsTrue(triggered);
            Assert.IsFalse(ran);
            Assert.IsNull(project.MainThreadException);
        }

        [Test]
        public void LocateMatchesSelf()
        {
            var task = new Comment();
            var project = new Project("TestProject", task);
            var queue = new Queue("RootQueue", project);
            var server = new Server("Local", queue);
            var actual = server.Locate("urn:ccnet:local:testProject");
            Assert.AreSame(project, actual);
        }

        [Test]
        public void LocateMatchesSelfByFullName()
        {
            var task = new Comment();
            var project = new Project("TestProject", task);
            var queue = new Queue("RootQueue", project);
            var server = new Server("Local", queue);
            var actual = server.Locate("urn:ccnet:local:rootQueue:testProject");
            Assert.AreSame(project, actual);
        }

        [Test]
        public void LocateMatchesTask()
        {
            var task = new Comment();
            var project = new Project("TestProject", task);
            var queue = new Queue("RootQueue", project);
            var server = new Server("Local", queue);
            var actual = server.Locate("urn:ccnet:local:testProject:comment");
            Assert.AreSame(task, actual);
        }
        #endregion

        #region Private methods
        private static Trigger GenerateRunOnceTrigger(Action onRun)
        {
            var triggered = false;
            var trigger = new TriggerStub
                              {
                                  OnCheckAction = () =>
                                                      {
                                                          if (!triggered)
                                                          {
                                                              triggered = true;
                                                              onRun();
                                                              return new IntegrationRequest("Dummy");
                                                          }

                                                          return null;
                                                      }
                              };
            return trigger;
        }
        #endregion
    }
}
