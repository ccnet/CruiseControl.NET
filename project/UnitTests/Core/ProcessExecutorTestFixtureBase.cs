using System;
using NMock;
using NMock.Constraints;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	public class ProcessExecutorTestFixtureBase : CustomAssertion
	{
		protected const string DefaultWorkingDirectory = @"c:\source\";
		protected IMock mockProcessExecutor;
		protected string defaultExecutable;

		protected void CreateProcessExecutorMock(string executable)
		{
			mockProcessExecutor = new DynamicMock(typeof(ProcessExecutor)); 
			mockProcessExecutor.Strict = true;
			defaultExecutable = executable;
		}

		protected void Verify()
		{
			mockProcessExecutor.Verify();	
		}

		protected void ExpectThatExecuteWillNotBeCalled()
		{
			mockProcessExecutor.ExpectNoCall("Execute", typeof(ProcessInfo));
		}

		protected void ExpectToExecuteArguments(string args)
		{
			ExpectToExecute(ProcessInfo(args));
		}

		protected void ExpectToExecute(ProcessInfo processInfo)
		{
			mockProcessExecutor.ExpectAndReturn("Execute", SuccessfulProcessResult(), processInfo);
		}

		protected void ExpectToExecuteAndReturn(ProcessResult result)
		{
			mockProcessExecutor.ExpectAndReturn("Execute", result, new IsAnything());
		}

		protected IIntegrationResult IntegrationResult(DateTime now)
		{
			return IntegrationResultMother.CreateSuccessful(now);
		}

		protected ProcessResult SuccessfulProcessResult()
		{
			return ProcessResultFixture.CreateSuccessfulResult();
		}

		protected ProcessInfo ProcessInfo(string args)
		{
			return new ProcessInfo(defaultExecutable, args, DefaultWorkingDirectory);
		}		
	}
}
