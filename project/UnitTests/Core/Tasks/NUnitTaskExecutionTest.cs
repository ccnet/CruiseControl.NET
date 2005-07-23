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
		private const string NUnitConsolePath = @"D:\temp\nunit-console.exe";
		private string[] TEST_ASSEMBLIES = new string[] {"foo.dll"};
		const string WORKING_DIRECTORY = @"c:\temp";

		private IMock executorMock;
		private NUnitTask task;
		private IntegrationResult result;

		[SetUp]
		protected void Init()
		{
			executorMock = new DynamicMock(typeof (ProcessExecutor));

			task = new NUnitTask(executorMock.MockInstance as ProcessExecutor);
			task.Assemblies = TEST_ASSEMBLIES;
			task.NUnitPath = NUnitConsolePath;
			task.OutputFile = TempFileUtil.CreateTempFile("NUnitTask", "results.xml", "foo");
			result = new IntegrationResult("testProject", WORKING_DIRECTORY);
			result.ArtifactDirectory = WORKING_DIRECTORY;
		}

		[TearDown]
		protected void DeleteTempFile()
		{
			TempFileUtil.DeleteTempFile(task.OutputFile);
		}

		[Test]
		public void ExecuteNUnitConsoleAndRetrieveResultsFromFile()
		{
			ProcessInfo info = new ProcessInfo(NUnitConsolePath, @" /xml=" + task.OutputFile + "  /nologo  foo.dll ", WORKING_DIRECTORY);
			info.TimeOut = NUnitTask.DefaultTimeout * 1000;
			executorMock.ExpectAndReturn("Execute", new ProcessResult("", String.Empty, 0, false), info);

			task.Run(result);

			Assert.AreEqual("foo", result.TaskOutput);
			executorMock.Verify();
		}

		[Test, ExpectedException(typeof(CruiseControlException))]
		public void ShouldThrowExceptionIfTestsFailed()
		{
			executorMock.ExpectAndReturn("Execute", ProcessResultFixture.CreateNonZeroExitCodeResult(), new IsAnything());

			task = new NUnitTask((ProcessExecutor) executorMock.MockInstance);
			task.Run(result);
		}
	}
}