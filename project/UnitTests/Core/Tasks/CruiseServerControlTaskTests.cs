namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    using NUnit.Framework;
    using Rhino.Mocks;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Tasks;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;
    using System.Collections.Generic;
    using System;
using ThoughtWorks.CruiseControl.Core.Config;

    [TestFixture]
    public class CruiseServerControlTaskTests
    {
        #region Private fields
        private MockRepository mocks;
        private ILogger logger;
        private ICruiseServerClientFactory factory;
        private IIntegrationResult result;
        private BuildProgressInformation buildInfo;
        private CruiseServerClientBase client;
        #endregion

        #region Setup
        [SetUp]
        public void Setup()
        {
            this.mocks = new MockRepository();
            this.logger = this.mocks.StrictMock<ILogger>();
            this.factory = this.mocks.StrictMock<ICruiseServerClientFactory>();
            this.result = this.mocks.StrictMock<IIntegrationResult>();
            SetupResult.For(this.result.Status).PropertyBehavior();
            this.result.Status = IntegrationStatus.Unknown;
            this.buildInfo = this.mocks.StrictMock<BuildProgressInformation>("somewhere", "test");
            SetupResult.For(this.result.BuildProgressInformation).Return(this.buildInfo);
            this.client = this.mocks.StrictMock<CruiseServerClientBase>();
        }
        #endregion

        #region Tests
        #region Run() tests
        /// <summary>
        /// While a configuration with no actions is invalid, the task should not crash and burn if encountered!
        /// </summary>
        [Test]
        public void RunHandlesNoActions()
        {
            // Initialise the test
            var task = new CruiseServerControlTask();
            task.Logger = this.logger;
            task.ClientFactory = this.factory;
            using (mocks.Ordered())
            {
                this.InitialiseStandardBuildInfo();
                this.InitialiseClient("tcp://localhost:21234", "Test", "Dummy");
                this.InitialiseTermination(0);
            }

            // Run the test
            this.mocks.ReplayAll();
            task.Run(result);

            // Verify the results
            Assert.AreEqual(IntegrationStatus.Success, result.Status);
            this.mocks.VerifyAll();
        }

        /// <summary>
        /// Sends the correct command to a single project.
        /// </summary>
        [Test]
        public void RunASingleActionOnSingleProject()
        {
            // Initialise the test
            var task = new CruiseServerControlTask();
            task.Logger = this.logger;
            task.ClientFactory = this.factory;
            using (mocks.Ordered())
            {
                this.InitialiseStandardBuildInfo();
                this.InitialiseClient("tcp://localhost:21234", "Test", "Dummy");
                this.InitialiseActionEvents(
                    "Dummy", 
                    "Performing start project action", 
                    p => 
                    {
                        this.client.StartProject(p);
                    },
                    "Dummy");
                this.InitialiseTermination(1);
            }

            // Add the action
            task.Actions = new CruiseServerControlTaskAction[] 
            {
                new CruiseServerControlTaskAction
                {
                    Project = "Dummy",
                    Type = CruiseServerControlTaskActionType.StartProject
                }
            };

            // Run the test
            this.mocks.ReplayAll();
            task.Run(result);

            // Verify the results
            Assert.AreEqual(IntegrationStatus.Success, result.Status);
            this.mocks.VerifyAll();
        }

        /// <summary>
        /// Sends the correct command to multiple projects.
        /// </summary>
        [Test]
        public void RunASingleActionOnMultipleProjects()
        {
            // Initialise the test
            var task = new CruiseServerControlTask();
            task.Logger = this.logger;
            task.ClientFactory = this.factory;
            using (mocks.Ordered())
            {
                this.InitialiseStandardBuildInfo();
                this.InitialiseClient("tcp://localhost:21234", "Test", "Dummy");
                this.InitialiseActionEvents(
                    "*",
                    "Performing start project action",
                    p =>
                    {
                        this.client.StartProject(p);
                    },
                    "Test",
                    "Dummy");
                this.InitialiseTermination(2);
            }

            // Add the action
            task.Actions = new CruiseServerControlTaskAction[] 
            {
                new CruiseServerControlTaskAction
                {
                    Project = "*",
                    Type = CruiseServerControlTaskActionType.StartProject
                }
            };

            // Run the test
            this.mocks.ReplayAll();
            task.Run(result);

            // Verify the results
            Assert.AreEqual(IntegrationStatus.Success, result.Status);
            this.mocks.VerifyAll();
        }

        /// <summary>
        /// Sends the correct commands to multiple projects.
        /// </summary>
        [Test]
        public void RunMultipleCommands()
        {
            // Initialise the test
            var task = new CruiseServerControlTask();
            task.Logger = this.logger;
            task.ClientFactory = this.factory;
            using (mocks.Ordered())
            {
                this.InitialiseStandardBuildInfo();
                this.InitialiseClient("tcp://localhost:21234", "Test1", "Dummy", "Test2");
                this.InitialiseActionEvents(
                    "Test?",
                    "Performing start project action",
                    p =>
                    {
                        this.client.StartProject(p);
                    },
                    "Test1",
                    "Test2");
                this.InitialiseActionEvents(
                    "Dummy",
                    "Performing stop project action",
                    p =>
                    {
                        this.client.StopProject(p);
                    },
                    "Dummy");
                this.InitialiseTermination(3);
            }

            // Add the action
            task.Actions = new CruiseServerControlTaskAction[] 
            {
                new CruiseServerControlTaskAction
                {
                    Project = "Test?",
                    Type = CruiseServerControlTaskActionType.StartProject
                },
                new CruiseServerControlTaskAction
                {
                    Project = "Dummy",
                    Type = CruiseServerControlTaskActionType.StopProject
                }
            };

            // Run the test
            this.mocks.ReplayAll();
            task.Run(result);

            // Verify the results
            Assert.AreEqual(IntegrationStatus.Success, result.Status);
            this.mocks.VerifyAll();
        }
        #endregion

        #region Validate() tests
        /// <summary>
        /// If the configuration is valid, then no errors or warnings should be generated.
        /// </summary>
        [Test]
        public void ValidateHandlesValidConfig()
        {
            var task = new CruiseServerControlTask();
            task.Actions = new CruiseServerControlTaskAction[]
            {
                new CruiseServerControlTaskAction
                {
                    Project = "*",
                    Type = CruiseServerControlTaskActionType.StopProject
                }
            };
            var processor = this.mocks.StrictMock<IConfigurationErrorProcesser>();
            this.mocks.ReplayAll();
            task.Validate(null, null, processor);
            this.mocks.VerifyAll();
        }

        /// <summary>
        /// If the configuration is valid, then no errors or warnings should be generated.
        /// </summary>
        [Test]
        public void ValidateGeneratesWarningWithNullTasks()
        {
            var task = new CruiseServerControlTask();
            task.Actions = null;
            var processor = this.mocks.StrictMock<IConfigurationErrorProcesser>();
            Expect.Call(() =>
            {
                processor.ProcessWarning("This task will not do anything - no actions specified");
            });
            this.mocks.ReplayAll();
            task.Validate(null, null, processor);
            this.mocks.VerifyAll();
        }

        /// <summary>
        /// If the configuration is valid, then no errors or warnings should be generated.
        /// </summary>
        [Test]
        public void ValidateGeneratesWarningWithNoTasks()
        {
            var task = new CruiseServerControlTask();
            task.Actions = new CruiseServerControlTaskAction[0];
            var processor = this.mocks.StrictMock<IConfigurationErrorProcesser>();
            Expect.Call(() =>
            {
                processor.ProcessWarning("This task will not do anything - no actions specified");
            });
            this.mocks.ReplayAll();
            task.Validate(null, null, processor);
            this.mocks.VerifyAll();
        }
        #endregion
        #endregion

        #region Helpers
        private void InitialiseStandardBuildInfo()
        {
            Expect.Call(() =>
            {
                this.buildInfo.SignalStartRunTask("Performing server actions");
            });
            Expect.Call(() =>
            {
                this.logger.Info("Performing server actions");
            });
            Expect.Call(() =>
            {
                this.logger.Debug("Initialising client");
            });
        }

        private void InitialiseClient(string address, params string[] projects)
        {
            Expect.Call(this.factory.GenerateClient(address))
                .Return(this.client);
            Expect.Call(() =>
            {
                this.logger.Info("Retrieving projects from server");
            });

            var list = new List<ProjectStatus>();
            foreach (var project in projects)
            {
                list.Add(new ProjectStatus(project, IntegrationStatus.Unknown, DateTime.Now));
            }

            Expect.Call(this.client.GetProjectStatus())
                .Return(list.ToArray());
            Expect.Call(() =>
            {
                this.logger.Debug(projects.Length.ToString() + " project(s) retrieved");
            });
        }

        private void InitialiseActionEvents(string pattern, string actionMessage, Action<string> action, params string[] projects)
        {
            Expect.Call(() =>
            {
                this.logger.Info("Found " + projects.Length + " project(s) for pattern '" + pattern + "'");
            });
            Expect.Call(() =>
            {
                this.logger.Info(actionMessage);
            });

            foreach (var project in projects)
            {
                Expect.Call(() =>
                {
                    this.logger.Debug("Sending action to " + project);
                });
                Expect.Call(() =>
                {
                    action(project);
                });
            }
        }

        private void InitialiseTermination(int commandCount)
        {
            Expect.Call(() =>
            {
                this.logger.Info("Server actions completed: " + commandCount + " command(s) sent");
            });
        }
        #endregion
    }
}
