using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Util.Test
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
			AssertEquals("Hello World", result.StandardOutput.Trim());
			AssertProcessExitsSuccessfully(result);
		}

		[Test]
		public void ExecuteProcessAndEchoResultsBackThroughStandardOutWhereALargeAmountOfOutputIsProduced()
		{
			ProcessResult result = executor.Execute(new ProcessInfo("cmd.exe", "/C @dir " + Environment.SystemDirectory));
			Assert("process should not have timed out", ! result.TimedOut);
			AssertProcessExitsSuccessfully(result);
		}

		[Test]
		public void StartProcessRunningCmdExeCallingNonExistentFile()
		{
			ProcessResult result = executor.Execute(new ProcessInfo("cmd.exe", "/C @zerk.exe foo"));

			AssertProcessExitsWithFailure(result, 1);
			AssertEquals(@"'zerk.exe' is not recognized as an internal or external command,
operable program or batch file.", result.StandardError.Trim());
			AssertEquals(string.Empty, result.StandardOutput);
			Assert("process should not have timed out", ! result.TimedOut);
		}

		[Test]
		public void SetEnvironmentVariables()
		{
			ProcessInfo processInfo = new ProcessInfo("cmd.exe", "/C set foo", null);
			processInfo.EnvironmentVariables["foo"] = "bar";
			ProcessResult result = executor.Execute(processInfo);

			AssertEquals("foo=bar\r\n", result.StandardOutput);
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

				Assert("process should have timed out", result.TimedOut);
				AssertNotNull("some output should have been produced", result.StandardOutput);
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
			AssertEquals(ProcessResult.SUCCESSFUL_EXIT_CODE, result.ExitCode);
			AssertFalse("process should not return an error", result.Failed);
		}

		private void AssertProcessExitsWithFailure(ProcessResult result, int expectedExitCode)
		{
			AssertEquals(expectedExitCode, result.ExitCode);
			Assert("process should return an error", result.Failed);
		}
	}
}