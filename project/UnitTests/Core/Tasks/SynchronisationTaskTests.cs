//-----------------------------------------------------------------------
// <copyright file="SynchronisationTaskTests.cs" company="Craig Sutherland">
//     Copyright (c) 2009 Craig Sutherland. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using Moq;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Core.Tasks;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Core.Util;

    /// <summary>
    /// Tests for the SynchronisationTask class.
    /// </summary>
    [TestFixture]
    public class SynchronisationTaskTests
    {
        #region Private fields
        #region mocks
        /// <summary>
        /// The mocks repository.
        /// </summary>
        private MockRepository mocks;
        #endregion
        #endregion

        #region Setup
        /// <summary>
        /// Initialises the mocks for each test.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.mocks = new MockRepository(MockBehavior.Default);
        }
        #endregion

        #region Tests
        #region Run() tests
        /// <summary>
        /// Run() should run all tasks in sequence.
        /// </summary>
        [Test]
        public void RunRunsTasksInSequence()
        {
            var buildInfo = mocks.Create<BuildProgressInformation>(string.Empty, string.Empty).Object;
            var result = mocks.Create<IIntegrationResult>().Object;
            Mock.Get(result).SetupAllProperties();
            result.Status = IntegrationStatus.Success;
            Mock.Get(result).Setup(_result => _result.Clone()).Returns(result);
            Mock.Get(result).SetupGet(_result => _result.BuildProgressInformation).Returns(buildInfo);
            var logger = mocks.Create<ILogger>().Object;
            var childTask1 = new SleepingTask { SleepPeriod = 10, Result = IntegrationStatus.Success };
            var childTask2 = new SleepingTask { SleepPeriod = 10, Result = IntegrationStatus.Success };
            var task = new SynchronisationTask
            {
                Logger = logger,
                Tasks = new ITask[] {
                    childTask1,
                    childTask2
                }
            };

            task.Run(result);
            this.mocks.VerifyAll();

            Assert.AreEqual(IntegrationStatus.Success, result.Status);
        }

        /// <summary>
        /// Run() should run all tasks in sequence.
        /// </summary>
        [Test]
        public void RunFailsIfTaskFails()
        {
            var buildInfo = mocks.Create<BuildProgressInformation>(string.Empty, string.Empty).Object;
            var result = mocks.Create<IIntegrationResult>().Object;
            Mock.Get(result).SetupAllProperties();
            result.Status = IntegrationStatus.Success;
            Mock.Get(result).Setup(_result => _result.Clone()).Returns(result);
            Mock.Get(result).SetupGet(_result => _result.BuildProgressInformation).Returns(buildInfo);
            var logger = mocks.Create<ILogger>().Object;
            var childTask1 = new SleepingTask { SleepPeriod = 10, Result = IntegrationStatus.Success };
            var childTask2 = new SleepingTask { SleepPeriod = 10, Result = IntegrationStatus.Failure };
            var task = new SynchronisationTask
            {
                Logger = logger,
                Tasks = new ITask[] {
                    childTask1,
                    childTask2
                }
            };

            task.Run(result);
            this.mocks.VerifyAll();

            Assert.AreEqual(IntegrationStatus.Failure, result.Status);
        }

        [Test]
        public void RunFailsIfTaskFailsButContinueOnFailure()
        {
            var buildInfo = mocks.Create<BuildProgressInformation>(string.Empty, string.Empty).Object;
            var result = mocks.Create<IIntegrationResult>().Object;
            Mock.Get(result).SetupAllProperties();
            result.Status = IntegrationStatus.Success;
            Mock.Get(result).Setup(_result => _result.Clone()).Returns(result);
            Mock.Get(result).SetupGet(_result => _result.BuildProgressInformation).Returns(buildInfo);
            var logger = mocks.Create<ILogger>().Object;
            var childTask1 = new SleepingTask { SleepPeriod = 10, Result = IntegrationStatus.Success };
            var childTask2 = new SleepingTask { SleepPeriod = 10, Result = IntegrationStatus.Failure };
            var childTask3 = new SleepingTask { SleepPeriod = 10, Result = IntegrationStatus.Success };
            var task = new SynchronisationTask
            {
                Logger = logger,
                Tasks = new ITask[] {
                    childTask1,
                    childTask2,
                    childTask3
                },
                ContinueOnFailure = true
            };

            task.Run(result);
            this.mocks.VerifyAll();

            Assert.AreEqual(IntegrationStatus.Failure, result.Status);
        }

        /// <summary>
        /// Run() should run all tasks in sequence.
        /// </summary>
        [Test]
        public void RunFailsIfTaskErrors()
        {
            var buildInfo = mocks.Create<BuildProgressInformation>(string.Empty, string.Empty).Object;
            var result = mocks.Create<IIntegrationResult>().Object;
            Mock.Get(result).SetupAllProperties();
            result.Status = IntegrationStatus.Success;
            Mock.Get(result).Setup(_result => _result.Clone()).Returns(result);
            Mock.Get(result).SetupGet(_result => _result.BuildProgressInformation).Returns(buildInfo);
            var logger = mocks.Create<ILogger>().Object;
            var childTask1 = new SleepingTask { SleepPeriod = 10, Result = IntegrationStatus.Success };
            var childTask2 = new FailingTask();
            var task = new SynchronisationTask
            {
                Logger = logger,
                Tasks = new ITask[] {
                    childTask1,
                    childTask2
                }
            };

            task.Run(result);
            this.mocks.VerifyAll();

            Assert.AreEqual(IntegrationStatus.Failure, result.Status);
            Assert.IsNotNull(result.ExceptionResult);
            Assert.AreEqual("Task failed!", result.ExceptionResult.Message);
        }

        [Test]
        public void RunFailsIfTaskErrorsButContinueOnFailure()
        {
            var buildInfo = mocks.Create<BuildProgressInformation>(string.Empty, string.Empty).Object;
            var result = mocks.Create<IIntegrationResult>().Object;
            Mock.Get(result).SetupAllProperties();
            result.Status = IntegrationStatus.Success;
            Mock.Get(result).Setup(_result => _result.Clone()).Returns(result);
            Mock.Get(result).SetupGet(_result => _result.BuildProgressInformation).Returns(buildInfo);
            var logger = mocks.Create<ILogger>().Object;
            var childTask1 = new SleepingTask { SleepPeriod = 10, Result = IntegrationStatus.Success };
            var childTask2 = new FailingTask();
            var childTask3 = new SleepingTask { SleepPeriod = 10, Result = IntegrationStatus.Success };
            var task = new SynchronisationTask
            {
                Logger = logger,
                Tasks = new ITask[] {
                    childTask1,
                    childTask2,
                    childTask3
                },
                ContinueOnFailure = true
            };

            task.Run(result);
            this.mocks.VerifyAll();

            Assert.AreEqual(IntegrationStatus.Failure, result.Status);
            Assert.IsNotNull(result.ExceptionResult);
            Assert.AreEqual("Task failed!", result.ExceptionResult.Message);
        }

        /// <summary>
        /// Run() will add the new context and run the tasks.
        /// </summary>
        [Test]
        public void RunWorksWithCustomContextName()
        {
            var buildInfo = mocks.Create<BuildProgressInformation>(string.Empty, string.Empty).Object;
            var result = mocks.Create<IIntegrationResult>().Object;
            Mock.Get(result).SetupAllProperties();
            result.Status = IntegrationStatus.Success;
            Mock.Get(result).Setup(_result => _result.Clone()).Returns(result);
            Mock.Get(result).SetupGet(_result => _result.BuildProgressInformation).Returns(buildInfo);
            var logger = mocks.Create<ILogger>().Object;
            var childTask1 = new SleepingTask { SleepPeriod = 10, Result = IntegrationStatus.Success };
            var childTask2 = new SleepingTask { SleepPeriod = 10, Result = IntegrationStatus.Success };
            var task = new SynchronisationTask
            {
                Logger = logger,
                Tasks = new ITask[] {
                    childTask1,
                    childTask2
                },
                ContextName = "customContext"
            };

            task.Run(result);
            this.mocks.VerifyAll();

            Assert.AreEqual(IntegrationStatus.Success, result.Status);
        }

        /// <summary>
        /// Run() will run multiple tasks in their own contexts.
        /// </summary>
        [Test]
        public void RunAllowsTasksInSeparateContexts()
        {
            var logger = mocks.Create<ILogger>().Object;

            // Setup the first task
            var buildInfo1 = mocks.Create<BuildProgressInformation>(string.Empty, string.Empty).Object;
            var result1 = mocks.Create<IIntegrationResult>().Object;
            Mock.Get(result1).SetupAllProperties();
            result1.Status = IntegrationStatus.Success;
            Mock.Get(result1).Setup(_result => _result.Clone()).Returns(result1);
            Mock.Get(result1).SetupGet(_result => _result.BuildProgressInformation).Returns(buildInfo1);
            var childTask11 = new SleepingTask { SleepPeriod = 1000, Result = IntegrationStatus.Success };
            var childTask12 = new SleepingTask { SleepPeriod = 1000, Result = IntegrationStatus.Success };
            var task1 = new SynchronisationTask
            {
                Logger = logger,
                Tasks = new ITask[] {
                    childTask11,
                    childTask12
                },
                ContextName = "customContext1"
            };

            // Setup the first task
            var buildInfo2 = mocks.Create<BuildProgressInformation>(string.Empty, string.Empty).Object;
            var result2 = mocks.Create<IIntegrationResult>().Object;
            Mock.Get(result2).SetupAllProperties();
            result2.Status = IntegrationStatus.Success;
            Mock.Get(result2).Setup(_result => _result.Clone()).Returns(result2);
            Mock.Get(result2).SetupGet(_result => _result.BuildProgressInformation).Returns(buildInfo2);
            var childTask21 = new SleepingTask { SleepPeriod = 1000, Result = IntegrationStatus.Success };
            var childTask22 = new SleepingTask { SleepPeriod = 1000, Result = IntegrationStatus.Success };
            var task2 = new SynchronisationTask
            {
                Logger = logger,
                Tasks = new ITask[] {
                    childTask21,
                    childTask22
                },
                ContextName = "customContext2"
            };

            var event1 = new ManualResetEvent(false);
            var event2 = new ManualResetEvent(false);

            Assert.IsTrue(ThreadPool.QueueUserWorkItem(t1 =>
            {
                task1.Run(result1);
                event1.Set();
            }));
            Assert.IsTrue(ThreadPool.QueueUserWorkItem(t2 =>
            {
                task2.Run(result2);
                event2.Set();
            }));
            ManualResetEvent.WaitAll(new WaitHandle[] {
                event1,
                event2
            });
            this.mocks.VerifyAll();

            Assert.AreEqual(IntegrationStatus.Success, result1.Status);
            Assert.AreEqual(IntegrationStatus.Success, result2.Status);
        }

        /// <summary>
        /// Run() will add the new context and run the tasks.
        /// </summary>
        [Test]
        public void RunTimesOutIfContextNotAvailable()
        {
            var logger = mocks.Create<ILogger>().Object;

            // Setup the first task
            var buildInfo1 = mocks.Create<BuildProgressInformation>(string.Empty, string.Empty).Object;
            var result1 = mocks.Create<IIntegrationResult>().Object;
            Mock.Get(result1).SetupAllProperties();
            result1.Status = IntegrationStatus.Success;
            Mock.Get(result1).Setup(_result => _result.Clone()).Returns(result1);
            Mock.Get(result1).SetupGet(_result => _result.BuildProgressInformation).Returns(buildInfo1);
            var childTask11 = new SleepingTask { SleepPeriod = 2000, Result = IntegrationStatus.Success };
            var childTask12 = new SleepingTask { SleepPeriod = 2000, Result = IntegrationStatus.Success };
            var task1 = new SynchronisationTask
            {
                Logger = logger,
                Tasks = new ITask[] {
                    childTask11,
                    childTask12
                },
                TimeoutPeriod = 1
            };

            // Setup the first task
            var buildInfo2 = mocks.Create<BuildProgressInformation>(string.Empty, string.Empty).Object;
            var result2 = mocks.Create<IIntegrationResult>().Object;
            Mock.Get(result2).SetupAllProperties();
            result2.Status = IntegrationStatus.Success;
            Mock.Get(result2).Setup(_result => _result.Clone()).Returns(result2);
            Mock.Get(result2).SetupGet(_result => _result.BuildProgressInformation).Returns(buildInfo2);
            var childTask21 = new SleepingTask { SleepPeriod = 2000, Result = IntegrationStatus.Success };
            var childTask22 = new SleepingTask { SleepPeriod = 2000, Result = IntegrationStatus.Success };
            var task2 = new SynchronisationTask
            {
                Logger = logger,
                Tasks = new ITask[] {
                    childTask21,
                    childTask22
                },
                TimeoutPeriod = 1
            };

            var event1 = new ManualResetEvent(false);
            var event2 = new ManualResetEvent(false);

            Assert.IsTrue(ThreadPool.QueueUserWorkItem(t1 =>
            {
                task1.Run(result1);
                event1.Set();
            }));
            Assert.IsTrue(ThreadPool.QueueUserWorkItem(t2 =>
            {
                task2.Run(result2);
                event2.Set();
            }));
            ManualResetEvent.WaitAll(new WaitHandle[] {
                event1,
                event2
            });
            this.mocks.Verify();

            // Only expect one of these to be successful
            if (result2.Status == IntegrationStatus.Success)
            {
                Assert.AreEqual(IntegrationStatus.Failure, result1.Status);
            }
            else
            {
                Assert.AreEqual(IntegrationStatus.Failure, result2.Status);
            }
        }

        [Test]
        public void RunContinueOnFaillureStillRunsInnerTasks()
        {
            var logger = mocks.Create<ILogger>().Object;

            // We cannot use a mock object here because having Clone return the original result means the test 
            // will not be able to catch the error we are after.
            var result = new IntegrationResult();
            result.ProjectName = ""; // must set to an empty string because the null default value makes Clone crash

            const int innerCount = 3;
            const int leafCount = 2;

            int taskRunCount = 0;

            // Initialise the task
            var innerTasks = new List<SynchronisationTask>();
            for (var innerLoop = 1; innerLoop <= innerCount; innerLoop++)
            {
                var leafTasks = new List<MockTask>();
                for (var leafLoop = 1; leafLoop <= leafCount; leafLoop++)
                    leafTasks.Add(((innerLoop == 2) && (leafLoop == 2)) ?
                        new MockTask
                        {
                            RunAction = ir =>
                            {
                                Thread.Sleep(10);
                                taskRunCount++;
                                ir.Status = IntegrationStatus.Failure;
                            }
                        }
                        :
                        new MockTask
                        {
                            RunAction = ir =>
                            {
                                Thread.Sleep(10);
                                taskRunCount++;
                                ir.Status = IntegrationStatus.Success;
                            }
                        }
                        );

                innerTasks.Add(new SynchronisationTask { Logger = logger, ContinueOnFailure = false, Tasks = leafTasks.ToArray() });
            }
            
            var task = new SynchronisationTask
            {
                Logger = logger,
                Tasks = innerTasks.ToArray(),
                ContinueOnFailure = true
            };

            task.Run(result);
            this.mocks.VerifyAll();

            Assert.AreEqual(IntegrationStatus.Failure, result.Status, "Status does not match");
            Assert.AreEqual(innerCount * leafCount, taskRunCount, "Bad task run count");
        }

        #endregion
        #endregion

        #region Private classes
        private class SleepingTask
            : ITask
        {
            public int? SleepPeriod { get;set;}
            public IntegrationStatus Result { get; set; }

            public void Run(IIntegrationResult result)
            {
                Thread.Sleep(SleepPeriod ?? 1);
                result.Status = Result;
            }
        }

        private class FailingTask
            : ITask
        {
            public void Run(IIntegrationResult result)
            {
                throw new CruiseControlException("Task failed!");
            }
        }
        #endregion
    }
}
