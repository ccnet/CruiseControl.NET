namespace CruiseControl.Core.Tests
{
    using System;
    using System.IO;
    using System.Threading;
    using CruiseControl.Core.Interfaces;
    using CruiseControl.Core.Structure;
    using CruiseControl.Core.Tasks;
    using CruiseControl.Core.Tests.Stubs;
    using CruiseControl.Core.Utilities;
    using Moq;
    using NUnit.Framework;
    using Messages = CruiseControl.Common.Messages;

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
            var stateLoaded = false;
            var project = new ProjectStub("Test")
                              {
                                  OnLoadState = () => stateLoaded = true
                              };
            project.Start();

            // Give the project time to start
            Thread.Sleep(100);
            Assert.AreEqual(ProjectState.Running, project.State);
            Assert.IsTrue(stateLoaded);
        }

        [Test]
        public void StartFailsIfAlreadyStarted()
        {
            var project = new ProjectStub("Test")
                              {
                                  OnLoadState = () => { }
                              };
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
            var project = new ProjectStub("Test")
                              {
                                  OnLoadState = () => { }
                              };
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
            var project = new ProjectStub("Test")
                              {
                                  OnLoadState = () => { }
                              };
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
            var project = new ProjectStub("Test")
                              {
                                  OnLoadState = () => { }
                              };
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
            var executionFactoryMock = new Mock<ITaskExecutionFactory>(MockBehavior.Strict);
            InitialiseExecutionContext(executionFactoryMock);
            var project = new Project("test", dummy)
                              {
                                  TaskExecutionFactory = executionFactoryMock.Object
                              };
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
            var executionFactoryMock = new Mock<ITaskExecutionFactory>(MockBehavior.Strict);
            InitialiseExecutionContext(executionFactoryMock);
            var project = new Project("test", dummy)
                              {
                                  TaskExecutionFactory = executionFactoryMock.Object
                              };
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
            var executionFactoryMock = new Mock<ITaskExecutionFactory>(MockBehavior.Strict);
            InitialiseExecutionContext(executionFactoryMock);
            var project = new Project("test", dummy)
                              {
                                  TaskExecutionFactory = executionFactoryMock.Object
                              };
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
            var executionFactoryMock = new Mock<ITaskExecutionFactory>(MockBehavior.Strict);
            InitialiseExecutionContext(executionFactoryMock);
            var project = new Project("test")
                              {
                                  TaskExecutionFactory = executionFactoryMock.Object
                              };
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
            var executionFactoryMock = new Mock<ITaskExecutionFactory>(MockBehavior.Strict);
            InitialiseExecutionContext(executionFactoryMock);
            var project = new Project("test")
                              {
                                  TaskExecutionFactory = executionFactoryMock.Object
                              };
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
            var executionFactoryMock = new Mock<ITaskExecutionFactory>(MockBehavior.Strict);
            InitialiseExecutionContext(executionFactoryMock);
            var project = new ProjectStub("test")
                              {
                                  TaskExecutionFactory = executionFactoryMock.Object
                              };
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
            var executionFactoryMock = new Mock<ITaskExecutionFactory>(MockBehavior.Strict);
            InitialiseExecutionContext(executionFactoryMock);
            var project = new Project("test")
                              {
                                  TaskExecutionFactory = executionFactoryMock.Object
                              };
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
            var executionFactoryMock = new Mock<ITaskExecutionFactory>(MockBehavior.Strict);
            var contextMock = InitialiseExecutionContext(executionFactoryMock);
            contextMock.Setup(ec => ec.AddEntryToBuildLog("Task 'TaskStub' has been skipped"));
            var project = new Project("test", dummy)
                              {
                                  TaskExecutionFactory = executionFactoryMock.Object
                              };
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
            var executionFactoryMock = new Mock<ITaskExecutionFactory>(MockBehavior.Strict);
            InitialiseExecutionContext(executionFactoryMock);
            var project = new ProjectStub("Test", task)
                              {
                                  TaskExecutionFactory = executionFactoryMock.Object,
                                  OnLoadState = () => { },
                                  OnSaveState = () => { },
                                  Clock = new SystemClock()
                              };
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
        public void IntegrationHandlesExceptionDuringIntegration()
        {
            var triggered = false;
            var trigger = GenerateRunOnceTrigger(() => triggered = true);
            var executionFactoryMock = new Mock<ITaskExecutionFactory>(MockBehavior.Strict);
            InitialiseExecutionContext(executionFactoryMock);
            var project = new ProjectStub("Test")
                              {
                                  TaskExecutionFactory = executionFactoryMock.Object,
                                  OnLoadState = () => { },
                                  OnSaveState = () => { },
                                  Clock = new SystemClock(),
                                  OnIntegrate = ir =>
                                                    {
                                                        throw new Exception();
                                                    }
                              };
            project.Triggers.Add(trigger);
            project.Start();

            // Give the project time to start
            Thread.Sleep(100);
            project.Stop();

            // Give the project time to stop
            Thread.Sleep(1000);
            Assert.IsTrue(triggered);
            Assert.IsNull(project.MainThreadException);
            Assert.AreEqual(IntegrationStatus.Error, project.PersistedState.LastIntegration.Status);
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
            var executionFactoryMock = new Mock<ITaskExecutionFactory>(MockBehavior.Strict);
            InitialiseExecutionContext(executionFactoryMock);
            var project = new ProjectStub("Test", task)
                              {
                                  Host = hostMock.Object,
                                  TaskExecutionFactory = executionFactoryMock.Object,
                                  OnLoadState = () => { },
                                  OnSaveState = () => { },
                                  Clock = new SystemClock()
                              };
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
            var project = new ProjectStub("Test", task)
                              {
                                  Host = hostMock.Object,
                                  OnLoadState = () => { },
                                  OnSaveState = () => { },
                                  Clock = new SystemClock()
                              };
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

        [Test]
        public void LoadPersistedStateLoadsAStateFile()
        {
            var configFile = Path.Combine(
                Environment.CurrentDirectory,
                "Name",
                "project.state");
            var fileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);
            fileSystemMock.Setup(fs => fs.CheckIfFileExists(configFile))
                .Returns(true)
                .Verifiable();
            using (var stream = AssemblyHelper.RetrieveExampleFile("ExampleProjectState"))
            {
                fileSystemMock.Setup(fs => fs.OpenFileForRead(configFile))
                    .Returns(stream)
                    .Verifiable();
                var project = new Project("Name")
                                  {
                                      FileSystem = fileSystemMock.Object
                                  };
                project.LoadPersistedState();
                Assert.IsNotNull(project.PersistedState);
                Assert.IsNotNull(project.PersistedState.LastIntegration);
                Assert.AreEqual(new DateTime(2010, 1, 1, 12, 1, 1), project.PersistedState.LastIntegration.StartTime);
                fileSystemMock.Verify();
            }
        }

        [Test]
        public void LoadPersistedStateHandlesACorruptedFile()
        {
            var configFile = Path.Combine(
                Environment.CurrentDirectory,
                "Name",
                "project.state");
            var fileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);
            fileSystemMock.Setup(fs => fs.CheckIfFileExists(configFile))
                .Returns(true)
                .Verifiable();
            using (var stream = new MemoryStream())
            {
                fileSystemMock.Setup(fs => fs.OpenFileForRead(configFile))
                    .Returns(stream)
                    .Verifiable();
                var project = new Project("Name")
                                  {
                                      FileSystem = fileSystemMock.Object
                                  };
                project.LoadPersistedState();
                Assert.IsNotNull(project.PersistedState);
                Assert.IsNull(project.PersistedState.LastIntegration);
                fileSystemMock.Verify();
            }
        }

        [Test]
        public void LoadPersistedStateStartsANewState()
        {
            var configFile = Path.Combine(
                Environment.CurrentDirectory,
                "Name",
                "project.state");
            var fileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);
            fileSystemMock.Setup(fs => fs.CheckIfFileExists(configFile))
                .Returns(false)
                .Verifiable();
            var project = new Project("Name")
                              {
                                  FileSystem = fileSystemMock.Object
                              };
            project.LoadPersistedState();
            Assert.IsNotNull(project.PersistedState);
            Assert.IsNull(project.PersistedState.LastIntegration);
            fileSystemMock.Verify();
        }

        [Test]
        public void SavePersistedStateSavesTheCurrentState()
        {
            var configFile = Path.Combine(
                Environment.CurrentDirectory,
                "Name",
                "project.state");
            var fileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);
            fileSystemMock.Setup(fs => fs.CheckIfFileExists(configFile))
                .Returns(false)
                .Verifiable();
            using (var stream = new MemoryStream())
            {
                fileSystemMock.Setup(fs => fs.OpenFileForWrite(configFile))
                    .Returns(stream)
                    .Verifiable();
                var project = new Project("Name")
                                  {
                                      FileSystem = fileSystemMock.Object
                                  };
                project.LoadPersistedState();
                project.SavePersistedState();
                fileSystemMock.Verify();
            }
        }

        [Test]
        public void StartMessageStartsProject()
        {
            var started = false;
            var project = new ProjectStub
                              {
                                  Name = "Test",
                                  OnLoadState = () => { },
                                  OnStart = () => started = true
                              };
            var request = new Messages.Blank();
            project.Start(request);
            Thread.Sleep(100);
            Assert.IsTrue(started);
        }

        [Test]
        public void StopMessageStopsProject()
        {
            var stopped = false;
            var project = new ProjectStub
                              {
                                  Name = "Test",
                                  OnLoadState = () => { },
                                  OnStop = () => stopped = true
                              };
            var request = new Messages.Blank();
            project.Start();
            Thread.Sleep(100);
            project.Stop(request);
            Thread.Sleep(500);
            Assert.IsTrue(stopped);
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

        private static Mock<TaskExecutionContext> InitialiseExecutionContext(
            Mock<ITaskExecutionFactory> executionFactoryMock)
        {
            var childContextMock = new Mock<TaskExecutionContext>(MockBehavior.Loose, new TaskExecutionParameters());
            var contextMock = new Mock<TaskExecutionContext>(MockBehavior.Strict, new TaskExecutionParameters());
            contextMock.Setup(ec => ec.StartChild(It.IsAny<Task>())).Returns(childContextMock.Object);
            contextMock.Setup(ec => ec.Complete());
            contextMock.Setup(ec => ec.CurrentStatus).Returns(IntegrationStatus.Success);
            executionFactoryMock.Setup(ef => ef.StartNew(It.IsAny<Project>(), It.IsAny<IntegrationRequest>()))
                .Returns(contextMock.Object);
            return contextMock;
        }
        #endregion
    }
}
