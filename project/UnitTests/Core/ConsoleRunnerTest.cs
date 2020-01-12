using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
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
			
			var mockCruiseServerFactory = new Mock<ICruiseServerFactory>();

			ConsoleRunner runner = new ConsoleRunner(consoleArgs, (ICruiseServerFactory)mockCruiseServerFactory.Object);
			runner.Run();
			
			// FIXME: should we care for the usage text and the logging implementation?
			// If yes read it from the embedded resource
			//Assert.AreEqual(1, listener.Traces.Count);
			//Assert.IsTrue(listener.Traces[0].ToString().IndexOf(ConsoleRunnerArguments.Usage) > 0, "Wrong message was logged.");

			mockCruiseServerFactory.Verify();
			mockCruiseServerFactory.VerifyNoOtherCalls();
		}

		[Test]
		public void ForceBuildCruiseServerProject()
		{
			ConsoleRunnerArguments consoleArgs = new ConsoleRunnerArguments();
			consoleArgs.Project = "test";
			
			var mockCruiseServer = new Mock<ICruiseServer>();
            mockCruiseServer.Setup(server => server.ForceBuild(It.Is<ProjectRequest>(_request => _request.ProjectName == "test"))).Returns(new Response { Result = ResponseResult.Success }).Verifiable();
            mockCruiseServer.Setup(server => server.Stop(It.Is<ProjectRequest>(_request => _request.ProjectName == "test"))).Returns(new Response { Result = ResponseResult.Success }).Verifiable();
            mockCruiseServer.Setup(server => server.WaitForExit(It.Is<ProjectRequest>(_request => _request.ProjectName == "test"))).Verifiable();
			var mockCruiseServerFactory = new Mock<ICruiseServerFactory>();
			mockCruiseServerFactory.Setup(factory => factory.Create(consoleArgs.UseRemoting, consoleArgs.ConfigFile)).Returns(mockCruiseServer.Object).Verifiable();

			new ConsoleRunner(consoleArgs, (ICruiseServerFactory)mockCruiseServerFactory.Object).Run();

			mockCruiseServer.Verify();
		}	

		[Test]
		public void StartCruiseServerProject()
		{
			ConsoleRunnerArguments consoleArgs = new ConsoleRunnerArguments();
			var mockCruiseServer = new Mock<ICruiseServer>();
			mockCruiseServer.Setup(server => server.Start()).Verifiable();
			mockCruiseServer.Setup(server => server.WaitForExit()).Verifiable();
			var mockCruiseServerFactory = new Mock<ICruiseServerFactory>();
			mockCruiseServerFactory.Setup(factory => factory.Create(consoleArgs.UseRemoting, consoleArgs.ConfigFile)).Returns(mockCruiseServer.Object).Verifiable();

			new ConsoleRunner(consoleArgs, (ICruiseServerFactory)mockCruiseServerFactory.Object).Run();

			mockCruiseServer.Verify();
		}

        [Test]
        public void ValidateConfigFileShouldNotStartServer()
        {
            ConsoleRunnerArguments consoleArgs = new ConsoleRunnerArguments();
            consoleArgs.ValidateConfigOnly = true;
            
            var mockCruiseServer = new Mock<ICruiseServer>();
            var mockCruiseServerFactory = new Mock<ICruiseServerFactory>();
            mockCruiseServerFactory.Setup(factory => factory.Create(false, consoleArgs.ConfigFile)).Returns(mockCruiseServer.Object).Verifiable();

            new ConsoleRunner(consoleArgs, (ICruiseServerFactory)mockCruiseServerFactory.Object).Run();

            mockCruiseServer.Verify();
        }
    }
}