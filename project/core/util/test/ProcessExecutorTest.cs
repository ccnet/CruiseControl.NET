using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Util.Test
{
	[TestFixture]
	public class ProcessExecutorTest : Assertion
	{
		[Test]
		public void ExecuteProcessAndEchoResultsBackThroughStandardOut()
		{
			ProcessExecutor executor = new ProcessExecutor();
			ProcessResult result = executor.Execute("cmd.exe", "/C @echo Hello World");
			AssertEquals("Hello World", result.StandardOutput.Trim());
			AssertEquals(false, result.HasError);
		}

		[Test]
		public void ExecuteProcessAndEchoResultsBackThroughStandardOutWhereALargeAmountOfOutputIsProduced()
		{
			ProcessExecutor executor = new ProcessExecutor();
			ProcessResult result = executor.Execute("cmd.exe", "/C @dir " + Environment.SystemDirectory);
			Assert("process should not have timed out", ! result.TimedOut);
			AssertEquals(false, result.HasError);
		}

		[Test]
		public void StartProcessRunningBatchFileCallingNonExistentFile()
		{
			ProcessExecutor executor = new ProcessExecutor();
			ProcessResult result = executor.Execute("cmd.exe", "/C @zerk.exe foo");

			AssertEquals(true, result.HasError);
			AssertEquals(@"'zerk.exe' is not recognized as an internal or external command,
operable program or batch file.", result.StandardError.Trim());
			AssertEquals(string.Empty, result.StandardOutput);
			Assert("process should not have timed out", ! result.TimedOut);
		}

		[Test]
		public void SetEnvironmentVariables()
		{
			ProcessExecutor executor = new ProcessExecutor();
			ProcessInfo processInfo = new ProcessInfo("cmd.exe", "/C set foo", null);
			processInfo.EnvironmentVariables["foo"] = "bar";
			ProcessResult result = executor.Execute(processInfo);

			AssertEquals("foo=bar\r\n", result.StandardOutput);
		}

		[Test, Ignore("This fails as the file cannot be deleted. Igrnored till i figure this out.")]
		public void ForceProcessTimeoutBecauseOfBlockingInput()
		{
			string filename = TempFileUtil.CreateTempFile("ProcessTest", "run.bat", "@:foo\r\ngoto foo");
			try
			{
				ProcessExecutor executor = new ProcessExecutor();
				executor.Timeout = 10;
				ProcessResult result = executor.Execute(filename, null);

				Assert("process should have timed out", result.TimedOut);
				AssertNull(result.StandardOutput);
				AssertNull(result.StandardError);
			}
			finally
			{
				TempFileUtil.DeleteTempFile(filename);
			}
		}

		[Test, ExpectedException(typeof(System.ComponentModel.Win32Exception))]
		public void SupplyInvalidFilenameAndVerifyException()
		{
			ProcessExecutor executor = new ProcessExecutor();
			executor.Execute("foodaddy.bat", null);
		}
	}
}