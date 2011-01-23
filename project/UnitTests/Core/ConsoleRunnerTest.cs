using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using System.Collections.Generic;
using NMock.Constraints;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	[TestFixture]
	public class ConsoleRunnerTest : CustomAssertion
	{
		private TestTraceListener listener;
		private TraceListenerBackup backup;

		[SetUp]
		public void SetUp()
		{
			backup = new TraceListenerBackup();
			listener = backup.AddTestTraceListener();
		}

		[TearDown]
		public void TearDown()
		{
			backup.Reset();
		}

		[Test]
		public void ShowHelp()
		{
			ConsoleRunnerArguments consoleArgs = new ConsoleRunnerArguments();
			consoleArgs.UseRemoting = true;
			consoleArgs.ShowHelp = true;			
			
			Mock mockCruiseServerFactory = new DynamicMock(typeof(ICruiseServerFactory));
			mockCruiseServerFactory.ExpectNoCall("Create", typeof(bool), typeof(string));

			ConsoleRunner runner = new ConsoleRunner(consoleArgs, (ICruiseServerFactory)mockCruiseServerFactory.MockInstance);
			runner.Run();
			
			// FIXME: should we care for the usage text and the logging implementation?
			// If yes read it from the embedded resource
			//Assert.AreEqual(1, listener.Traces.Count);
			//Assert.IsTrue(listener.Traces[0].ToString().IndexOf(ConsoleRunnerArguments.Usage) > 0, "Wrong message was logged.");

			mockCruiseServerFactory.Verify();
		}

		[Test]
		public void ForceBuildCruiseServerProject()
		{
			ConsoleRunnerArguments consoleArgs = new ConsoleRunnerArguments();
			consoleArgs.Project = "test";
			
			Mock mockCruiseServer = new DynamicMock(typeof(ICruiseServer));
            var projectConstraint = new ProjectRequestConstraint
            {
                ProjectName = "test"
            };
            mockCruiseServer.ExpectAndReturn("ForceBuild", new Response { Result = ResponseResult.Success }, projectConstraint);
            mockCruiseServer.ExpectAndReturn("Stop", new Response { Result = ResponseResult.Success }, projectConstraint);
            mockCruiseServer.Expect("WaitForExit", projectConstraint);
			Mock mockCruiseServerFactory = new DynamicMock(typeof(ICruiseServerFactory));
			mockCruiseServerFactory.ExpectAndReturn("Create", mockCruiseServer.MockInstance, consoleArgs.UseRemoting, consoleArgs.ConfigFile);

			new ConsoleRunner(consoleArgs, (ICruiseServerFactory)mockCruiseServerFactory.MockInstance).Run();

			mockCruiseServer.Verify();
		}	

		[Test]
		public void StartCruiseServerProject()
		{
			ConsoleRunnerArguments consoleArgs = new ConsoleRunnerArguments();
			Mock mockCruiseServer = new DynamicMock(typeof(ICruiseServer));
			mockCruiseServer.Expect("Start");
			mockCruiseServer.Expect("WaitForExit");
			Mock mockCruiseServerFactory = new DynamicMock(typeof(ICruiseServerFactory));
			mockCruiseServerFactory.ExpectAndReturn("Create", mockCruiseServer.MockInstance, consoleArgs.UseRemoting, consoleArgs.ConfigFile);

			new ConsoleRunner(consoleArgs, (ICruiseServerFactory)mockCruiseServerFactory.MockInstance).Run();

			mockCruiseServer.Verify();
		}

        [Test]
        public void ValidateConfigFileShouldNotStartServer()
        {
            ConsoleRunnerArguments consoleArgs = new ConsoleRunnerArguments();
            consoleArgs.ValidateConfigOnly = true;
            
            Mock mockCruiseServer = new DynamicMock(typeof(ICruiseServer));
            Mock mockCruiseServerFactory = new DynamicMock(typeof(ICruiseServerFactory));
            mockCruiseServerFactory.ExpectAndReturn("Create", mockCruiseServer.MockInstance, false, consoleArgs.ConfigFile);

            new ConsoleRunner(consoleArgs, (ICruiseServerFactory)mockCruiseServerFactory.MockInstance).Run();

            mockCruiseServer.Verify();
        }
    }

    public class ProjectRequestConstraint : BaseConstraint
    {
        public string ProjectName { get; set; }
        private string message = null;

        public override bool Eval(object val)
        {
            if (val is ProjectRequest)
            {
                if (!string.Equals(ProjectName, (val as ProjectRequest).ProjectName))
                {
                    message = "Project names do not match";
                }
            }
            else
            {
                message = "Expected a ProjectRequest";
            }
            return (message == null);
        }

        public override string Message
        {
            get { return message; }
        }
    }
}