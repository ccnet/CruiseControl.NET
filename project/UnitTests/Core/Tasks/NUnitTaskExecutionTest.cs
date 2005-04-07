using System;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
	[TestFixture]
	public class NUnitTaskExecutionTest : CustomAssertion
	{
		private const string NUNIT_DUMMY_PATH = @"D:\temp\nunit-console.exe";
		private string[] TEST_ASSEMBLIES = new string[] {"foo.dll"};

		private IMock executorMock;
		private IMock mockArguments;

		private NUnitTask task;
		private IntegrationResult result;

		[SetUp]
		public void Init()
		{
			executorMock = new DynamicMock(typeof (ProcessExecutor));
			mockArguments = new DynamicMock(typeof (NUnitArgument));
			task = new NUnitTask(executorMock.MockInstance as ProcessExecutor);
			task.NUnitPath = NUNIT_DUMMY_PATH;
			result = new IntegrationResult("testProject", @"c:\temp");
		}

		[Test]
		public void RunWithNunitPathSetExecutesNunitAndRetrivesStandardOutput()
		{
			mockArguments.Expect("Assemblies", TEST_ASSEMBLIES);
			executorMock.ExpectAndReturn("Execute", new ProcessResult("foo", String.Empty, 0, false), new IsTypeOf(typeof (ProcessInfo)));

			task.Assembly = TEST_ASSEMBLIES;
			task.Run(result);

			Assert.AreEqual("foo", result.TaskOutput);
			executorMock.Verify();
		}

		[Test, ExpectedException(typeof(CruiseControlException))]
		public void RunWithNoAssembliesDoesNotCreateTaskResult()
		{
			executorMock.ExpectNoCall("Execute", typeof (ProcessInfo));
			task.Run(result);
		}

		[Test, ExpectedException(typeof(CruiseControlException))]
		public void ShouldThrowExceptionIfTestsFailed()
		{
			executorMock.ExpectAndReturn("Execute", ProcessResultFixture.CreateNonZeroExitCodeResult(), new IsAnything());

			task = new NUnitTask((ProcessExecutor) executorMock.MockInstance);
			task.Assembly = TEST_ASSEMBLIES;
			task.Run(result);
		}
	}
}