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
			ProcessResult result = executor.Execute("cmd.exe", "/C @echo Hello World");
			AssertEquals("Hello World", result.StandardOutput.Trim());
			AssertEquals(false, result.HasError);
		}

		[Test]
		public void ExecuteProcessAndEchoResultsBackThroughStandardOutWhereALargeAmountOfOutputIsProduced()
		{
			ProcessResult result = executor.Execute("cmd.exe", "/C @dir " + Environment.SystemDirectory);
			Assert("process should not have timed out", ! result.TimedOut);
			AssertEquals(false, result.HasError);
		}

		[Test]
		public void StartProcessRunningBatchFileCallingNonExistentFile()
		{
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
			ProcessInfo processInfo = new ProcessInfo("cmd.exe", "/C set foo", null);
			processInfo.EnvironmentVariables["foo"] = "bar";
			ProcessResult result = executor.Execute(processInfo);

			AssertEquals("foo=bar\r\n", result.StandardOutput);
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
				AssertFalse(result.HasError);
				AssertEquals(-1, result.ExitCode);
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
			executor.Execute("foodaddy.bat", null);
		}
	}
}