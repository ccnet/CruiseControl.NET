using System;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks.Test
{
	[TestFixture]
	public class NUnitTaskExecutionTest : CustomAssertion
	{
		private const string NUNIT_DUMMY_PATH = @"D:\temp\nunit-console.exe";
		private string[] TEST_ASSEMBLIES = new string[] {"foo.dll"};

		private IMock _processExecutor;
		private IMock _processArguments;

		private NUnitTask _task;
		private IntegrationResult _result;

		[SetUp]
		public void Init()
		{
			_processExecutor = new DynamicMock(typeof (ProcessExecutor));
			_processArguments = new DynamicMock(typeof (NUnitArgument));
			_task = new NUnitTask(_processExecutor.MockInstance as ProcessExecutor);
			_task.NUnitPath = NUNIT_DUMMY_PATH;
			_result = new IntegrationResult("testProject", @"c:\temp");
		}

		[Test]
		public void RunWithNunitPathSetExecutesNunitAndRetrivesStandardOutput()
		{
			_task.Assembly = TEST_ASSEMBLIES;

			string setupData = "foo";
			_processArguments.Expect("Assemblies", TEST_ASSEMBLIES);
			_processExecutor.ExpectAndReturn("Execute", new ProcessResult(setupData, String.Empty, 0, false), new IsTypeOf(typeof (ProcessInfo)));
			_task.Run(_result);

			Assert.AreEqual(setupData, _result.TaskOutput);
			_processExecutor.Verify();
		}

		[Test]
		public void RunWithNoAssembliesDoesNotCreateTaskResult()
		{
			_processExecutor.ExpectNoCall("Execute", typeof (ProcessInfo));
			_task.Run(_result);
			_processExecutor.Verify();
		}
	}
}