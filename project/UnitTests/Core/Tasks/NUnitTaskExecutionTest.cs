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
	public class NUnitTaskExecutionTest : ProcessExecutorTestFixtureBase
	{
		private const string NUnitConsolePath = @"D:\temp\nunit-console.exe";
		private string[] TEST_ASSEMBLIES = new string[] {"foo.dll"};
        const string WORKING_DIRECTORY = @"c:\temp";
        const string ARTIFACT_DIRECTORY = @"c:\temp";

		private IMock executorMock;
		private NUnitTask task;
		private IIntegrationResult result;
	    private SystemPath tempOutputFile;

	    [SetUp]
		protected void Init()
		{
		    tempOutputFile = new TempDirectory().CreateTextFile("results.xml", "foo");
		    executorMock = new DynamicMock(typeof (ProcessExecutor));

			task = new NUnitTask(executorMock.MockInstance as ProcessExecutor);
			task.Assemblies = TEST_ASSEMBLIES;
			task.NUnitPath = NUnitConsolePath;
	        task.OutputFile = tempOutputFile.ToString();
			result = Integration("testProject", WORKING_DIRECTORY, ARTIFACT_DIRECTORY);
		}

		[TearDown]
		protected void DeleteTempFile()
		{
            tempOutputFile.DeleteDirectory();
		}

		[Test]
		public void ExecuteNUnitConsoleAndRetrieveResultsFromFile()
		{
		    string args = string.Format(@"/xml=""{0}"" /nologo foo.dll", task.OutputFile);
		    ProcessInfo info = new ProcessInfo(NUnitConsolePath, args, WORKING_DIRECTORY);
			info.TimeOut = NUnitTask.DefaultTimeout * 1000;
			executorMock.ExpectAndReturn("Execute", new ProcessResult("", String.Empty, 0, false), new object[] { info, new IsAnything() });

			task.Run(result);

			Assert.AreEqual("foo", result.TaskOutput);
			executorMock.Verify();
		}

		[Test, ExpectedException(typeof(CruiseControlException))]
		public void ShouldThrowExceptionIfTestsFailed()
		{
			executorMock.ExpectAndReturn("Execute", ProcessResultFixture.CreateNonZeroExitCodeResult(), new object[] { new IsAnything(), new IsAnything() });

			task = new NUnitTask((ProcessExecutor) executorMock.MockInstance);
			task.Run(result);
		}
	}
}
