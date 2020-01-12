using System;
using System.IO;
using Moq;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	public class ProcessExecutorTestFixtureBase : IntegrationFixture
	{
		protected const int SuccessfulExitCode = 0;
		protected const int FailedExitCode = -1;
		protected readonly string DefaultWorkingDirectory = Path.GetFullPath(Path.Combine(".", "source"));
		protected readonly string DefaultWorkingDirectoryWithSpaces = Path.GetFullPath(Path.Combine(".", "source code"));
		protected int DefaultTimeout = Timeout.DefaultTimeout.Millis;
		protected string ProcessResultOutput = "output";
		protected DateTime testDate = new DateTime(2005, 06, 06, 08, 45, 00);
		protected string testDateString = "2005-06-06";
		protected string testTimeString = "08:45:00";
        

		protected Mock<ProcessExecutor> mockProcessExecutor;
		protected string defaultExecutable;

		protected void CreateProcessExecutorMock(string executable)
		{
			mockProcessExecutor = new Mock<ProcessExecutor>(MockBehavior.Strict);
			defaultExecutable = executable;
		}

		protected void Verify()
		{
			mockProcessExecutor.Verify();
		}

		protected void ExpectToExecuteArguments(string args)
		{
			ExpectToExecute(NewProcessInfo(args, DefaultWorkingDirectory));
		}

		protected void ExpectToExecuteArguments(string args, string workingDirectory)
		{
			ExpectToExecute(NewProcessInfo(args, workingDirectory));
		}

		protected void ExpectToExecuteArguments(MockSequence sequence, string args, string workingDirectory) {
			ExpectToExecute(sequence, NewProcessInfo(args, workingDirectory));
		}

		protected void ExpectToExecute(ProcessInfo processInfo)
		{
			mockProcessExecutor.Setup(executor => executor.Execute(processInfo)).Returns(SuccessfulProcessResult()).Verifiable();
		}

		protected void ExpectToExecute(MockSequence sequence, ProcessInfo processInfo) {
			mockProcessExecutor.InSequence(sequence).Setup(executor => executor.Execute(processInfo)).Returns(SuccessfulProcessResult()).Verifiable();
		}

		protected void ExpectToExecuteAndReturn(ProcessResult result)
		{
			mockProcessExecutor.Setup(executor => executor.Execute(It.IsAny<ProcessInfo>())).Returns(result).Verifiable();
		}

		protected void ExpectToExecuteAndThrow()
		{
			mockProcessExecutor.Setup(executor => executor.Execute(It.IsAny<ProcessInfo>())).Throws(new IOException()).Verifiable();
		}

		protected virtual IIntegrationResult IntegrationResult()
		{
			return IntegrationResult(testDate);
		}

		protected IIntegrationResult IntegrationResult(DateTime start)
		{
			IntegrationResult successful = IntegrationResultMother.CreateSuccessful(start);
			successful.WorkingDirectory = DefaultWorkingDirectory;
			return successful;
		}

		protected ProcessResult SuccessfulProcessResult()
		{
			return ProcessResultFixture.CreateSuccessfulResult(ProcessResultOutput);
		}

		protected ProcessResult FailedProcessResult()
		{
			return new ProcessResult(ProcessResultOutput, null, FailedExitCode, false);
		}

		protected ProcessResult TimedOutProcessResult()
		{
			return ProcessResultFixture.CreateTimedOutResult();
		}

		protected ProcessInfo NewProcessInfo(string args, string workingDirectory)
		{
			ProcessInfo info = new ProcessInfo(defaultExecutable, args, workingDirectory);
			info.TimeOut = DefaultTimeout;
			return info;
		}

		protected IntegrationResult IntegrationResultForWorkingDirectoryTest()
		{
			return (IntegrationResult) Integration("test", "projectWorkingDirectory", "projectArtifactDirectory");
		}
	}
}
