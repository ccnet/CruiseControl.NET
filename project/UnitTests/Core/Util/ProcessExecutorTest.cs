using NUnit.Framework;
using System;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Util
{
	[TestFixture]
	public class ProcessExecutorTest : CustomAssertion
	{
		private ProcessExecutor executor;

		[SetUp]
		protected void CreateExecutor()
		{
			executor = new ProcessExecutor();
		}

		[Test]
		public void ExecuteProcessAndEchoResultsBackThroughStandardOut()
		{
			ProcessResult result = executor.Execute(new ProcessInfo("cmd.exe", "/C @echo Hello World"));
			Assert.AreEqual("Hello World", result.StandardOutput.Trim());
			AssertProcessExitsSuccessfully(result);
		}

		[Test]
		public void ExecuteProcessAndEchoResultsBackThroughStandardOutWhereALargeAmountOfOutputIsProduced()
		{
			ProcessResult result = executor.Execute(new ProcessInfo("cmd.exe", "/C @dir " + Environment.SystemDirectory));
			Assert.IsTrue(! result.TimedOut);
			AssertProcessExitsSuccessfully(result);
		}

		[Test]
		public void ShouldNotUseATimeoutIfTimeoutSetToZeroOnProcessInfo()
		{
			ProcessInfo processInfo = new ProcessInfo("cmd.exe", "/C @echo Hello World");
			processInfo.TimeOut = ProcessInfo.INFINITE_TIMEOUT;
			ProcessResult result = executor.Execute(processInfo);
			Assert.AreEqual("Hello World", result.StandardOutput.Trim());
			AssertProcessExitsSuccessfully(result);
		}

		[Test]
		public void StartProcessRunningCmdExeCallingNonExistentFile()
		{
			ProcessResult result = executor.Execute(new ProcessInfo("cmd.exe", "/C @zerk.exe foo"));

			AssertProcessExitsWithFailure(result, 1);
			AssertContains("zerk.exe", result.StandardError);
			Assert.AreEqual(string.Empty, result.StandardOutput);
			Assert.IsTrue(! result.TimedOut);
		}

		[Test]
		public void SetEnvironmentVariables()
		{
			ProcessInfo processInfo = new ProcessInfo("cmd.exe", "/C set foo", null);
			processInfo.EnvironmentVariables["foo"] = "bar";
			ProcessResult result = executor.Execute(processInfo);

			Assert.AreEqual("foo=bar\r\n", result.StandardOutput);
			AssertProcessExitsSuccessfully(result);
		}

		[Test]
		public void ForceProcessTimeoutBecauseTargetIsNonTerminating()
		{
			string filename = TempFileUtil.CreateTempFile("ProcessTest", "run.bat", "@:foo\r\ngoto foo");
			try
			{
				ProcessInfo processInfo = new ProcessInfo(filename, null);
				processInfo.TimeOut = 10;
				ProcessResult result = executor.Execute(processInfo);

				Assert.IsTrue(result.TimedOut);
				Assert.IsNotNull(result.StandardOutput, "some output should have been produced");
				AssertProcessExitsWithFailure(result, ProcessResult.TIMED_OUT_EXIT_CODE);
			}
			finally
			{
				TempFileUtil.DeleteTempDir("ProcessTest");
			}
		}

		[Test, ExpectedException(typeof(System.ComponentModel.Win32Exception))]
		public void SupplyInvalidFilenameAndVerifyException()
		{
			ProcessExecutor executor = new ProcessExecutor();
			executor.Execute(new ProcessInfo("foodaddy.bat"));
		}

		private void AssertProcessExitsSuccessfully(ProcessResult result)
		{
			Assert.AreEqual(ProcessResult.SUCCESSFUL_EXIT_CODE, result.ExitCode);
			AssertFalse("process should not return an error", result.Failed);
		}

		private void AssertProcessExitsWithFailure(ProcessResult result, int expectedExitCode)
		{
			Assert.AreEqual(expectedExitCode, result.ExitCode);
			Assert.IsTrue(result.Failed, "process should return an error");
		}
	}
}