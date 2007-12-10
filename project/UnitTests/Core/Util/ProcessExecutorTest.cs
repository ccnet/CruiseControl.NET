using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
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
			processInfo.TimeOut = ProcessInfo.InfiniteTimeout;
			ProcessResult result = executor.Execute(processInfo);
			AssertProcessExitsSuccessfully(result);
			Assert.AreEqual("Hello World", result.StandardOutput.Trim());			
		}

		[Test]
		public void StartProcessRunningCmdExeCallingNonExistentFile()
		{
			ProcessResult result = executor.Execute(new ProcessInfo("cmd.exe", "/C @zerk.exe foo"));

            AssertProcessExitsWithFailure(result);
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

			AssertProcessExitsSuccessfully(result);
			Assert.AreEqual("foo=bar\r\n", result.StandardOutput);
		}

		[Test]
		public void ForceProcessTimeoutBecauseTargetIsNonTerminating()
		{
			ProcessInfo processInfo = new ProcessInfo("sleeper.exe");
			processInfo.TimeOut = 100;
			ProcessResult result = executor.Execute(processInfo);

			Assert.IsTrue(result.TimedOut, "process did not time out, but it should have.");
			Assert.IsNotNull(result.StandardOutput, "some output should have been produced");
            AssertProcessExitsWithFailure(result);
		}

		[Test, ExpectedException(typeof (IOException))]
		public void SupplyInvalidFilenameAndVerifyException()
		{
			executor.Execute(new ProcessInfo("foodaddy.bat"));
		}

		[Test, ExpectedException(typeof(DirectoryNotFoundException))]
		public void ShouldThrowMeaningfulExceptionIfWorkingDirectoryDoesNotExist()
		{
			executor.Execute(new ProcessInfo("myExecutable", "", @"c:\invalid_path\that_is_invalid"));
		}

		[Test]
		public void StartNonTerminatingProcessAndAbortThreadShouldKillProcess()
		{
			Thread thread = new Thread(new ThreadStart(StartProcess));
			thread.Start();
			WaitForProcessToStart();
			thread.Abort();
			thread.Join();
			try
			{
				Assert.AreEqual(0, Process.GetProcessesByName("sleeper").Length);
			}
			catch (Exception)
			{
				Process.GetProcessesByName("sleeper")[0].Kill();
				Assert.Fail("Process was not killed.");
			}
		}

        [Test]
        public void ReadUnicodeFile()
        {
            SystemPath tempDirectory = SystemPath.UniqueTempPath().CreateDirectory();
            try
            {
                string content = "yooo ез";
                SystemPath tempFile = tempDirectory.CreateTextFile("test.txt", content);
                ProcessInfo processInfo = new ProcessInfo("cmd.exe", "/C type \"" + tempFile + "\"");
                processInfo.StreamEncoding = Encoding.UTF8;
                ProcessResult result = executor.Execute(processInfo);
                Assert.IsTrue(!result.Failed);
                Assert.AreEqual(content + Environment.NewLine, result.StandardOutput);
            }
            finally
            {
                tempDirectory.DeleteDirectory();
            }
        }

		[Test]
		public void KillDisposedProcessShouldNotThrowException()
		{
			executor.Execute(new ProcessInfo("cmd.exe", "/C echo hello"), new ProcessMonitor());
			executor.Kill();
		}

		private static void AssertProcessExitsSuccessfully(ProcessResult result)
		{
			Assert.AreEqual(ProcessResult.SUCCESSFUL_EXIT_CODE, result.ExitCode, "Process did not exit successfully");
			AssertFalse("process should not return an error", result.Failed);
		}

        private static void AssertProcessExitsWithFailure(ProcessResult result)
		{
            Assert.AreNotEqual(ProcessResult.SUCCESSFUL_EXIT_CODE, result.ExitCode);
			Assert.IsTrue(result.Failed, "process should return an error");
		}

		private static void WaitForProcessToStart()
		{
			int count = 0;
			while (Process.GetProcessesByName("sleeper").Length == 0 && count < 1000)
			{
				Thread.Sleep(50);
				count++;
			}
            Thread.Sleep(2000);
			if (count == 1000) Assert.Fail("sleeper process did not start.");
		}

        private void StartProcess()
        {
            try
            {
                ProcessInfo processInfo = new ProcessInfo("sleeper.exe");
                executor.Execute(processInfo);
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
            }
        }
    }
}
