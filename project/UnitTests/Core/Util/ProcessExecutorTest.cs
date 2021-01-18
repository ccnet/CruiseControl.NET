using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Util
{
	[TestFixture]
	public class ProcessExecutorTest : CustomAssertion
	{
		private ProcessExecutor executor;
		private volatile bool runnerThreadCompletedNormally;
		private volatile bool runnerThreadWasAborted;
		private const string PROJECT_NAME = "testing";

		[SetUp]
		protected void CreateExecutor()
		{
			executor = new ProcessExecutor();
			if (Thread.CurrentThread.Name == null)
			{
				Thread.CurrentThread.Name = PROJECT_NAME;
			}
			runnerThreadCompletedNormally = false;
			runnerThreadWasAborted = false;
		}

		[Test]
		public void ExecuteProcessAndEchoResultsBackThroughStandardOut()
		{
			ProcessResult result = executor.Execute(Platform.IsWindows ? new ProcessInfo("cmd.exe", "/C @echo Hello World") : new ProcessInfo("echo", "Hello World"));
			Assert.AreEqual("Hello World", result.StandardOutput.Trim());
			AssertProcessExitsSuccessfully(result);
		}

		[Test]
		public void ExecuteProcessAndEchoResultsBackThroughStandardOutWhereALargeAmountOfOutputIsProduced()
		{
			ProcessResult result = executor.Execute(Platform.IsWindows ? new ProcessInfo("cmd.exe", "/C @dir " + Environment.SystemDirectory) :  new ProcessInfo("ls", Environment.SystemDirectory));
			Assert.IsTrue(! result.TimedOut);
			AssertProcessExitsSuccessfully(result);
		}

		[Test]
		public void ShouldNotUseATimeoutIfTimeoutSetToInfiniteOnProcessInfo()
		{
			ProcessInfo processInfo = Platform.IsWindows ? new ProcessInfo("cmd.exe", "/C @echo Hello World") : new ProcessInfo("bash", "-c \"echo Hello World\"");
			processInfo.TimeOut = ProcessInfo.InfiniteTimeout;
			ProcessResult result = executor.Execute(processInfo);
			AssertProcessExitsSuccessfully(result);
			Assert.AreEqual("Hello World", result.StandardOutput.Trim());			
		}

		[Test]
		public void StartProcessRunningCmdExeCallingNonExistentFile()
		{
			ProcessResult result = executor.Execute(Platform.IsWindows ? new ProcessInfo("cmd.exe", "/C @zerk.exe foo") : new ProcessInfo("bash", "-c \"zerk.exe foo\""));

			AssertProcessExitsWithFailure(result);
			AssertContains("zerk.exe", result.StandardError);
			Assert.AreEqual(string.Empty, result.StandardOutput);
			Assert.IsTrue(! result.TimedOut);
		}

		[Test]
		public void SetEnvironmentVariables()
		{
			ProcessInfo processInfo = Platform.IsWindows ? new ProcessInfo("cmd.exe", "/C set foo", null) : new ProcessInfo("bash", "-c \"echo foo=$foo\"", null);
			processInfo.EnvironmentVariables["foo"] = "bar";
			ProcessResult result = executor.Execute(processInfo);

			AssertProcessExitsSuccessfully(result);
			Assert.AreEqual("foo=bar" + Environment.NewLine, result.StandardOutput);
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

		[Test]
		public void SupplyInvalidFilenameAndVerifyException()
		{
            Assert.That(delegate { executor.Execute(new ProcessInfo("foodaddy.bat")); },
                        Throws.TypeOf<IOException>());
		}

		[Test]
		public void ShouldThrowMeaningfulExceptionIfWorkingDirectoryDoesNotExist()
		{
            Assert.That(delegate { executor.Execute(new ProcessInfo("myExecutable", "", @"c:\invalid_path\that_is_invalid")); },
                        Throws.TypeOf<DirectoryNotFoundException>());
		}

		[Test]
		public void StartNonTerminatingProcessAndAbortThreadShouldKillProcessAndAbortThread()
		{
			// ARRANGE
			Thread thread = new Thread(StartSleeperProcess);
			thread.Name = "sleeper thread";
			thread.Start();
			WaitForProcessToStart();

			// ACT
			thread.Abort();

			// ASSERT
			thread.Join();
			Assert.IsTrue(runnerThreadWasAborted, "Runner thread should be aborted.");
			// Ensure the external process was killed
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
		public void StartNonTerminatingProcessAndInterruptCurrentProcessShouldKillProcessButLeaveThreadRunning()
		{
			// ARRANGE
			Thread thread = new Thread(StartSleeperProcess);
			thread.Name = "sleeper thread";
			thread.Start();
			WaitForProcessToStart();

			// ACT
			ProcessExecutor.KillProcessCurrentlyRunningForProject("sleeper thread");

			// ASSERT
			// Sleeper runs for 60 seconds. We need to give up early and fail the test if it takes longer than 50.
			// If it runs for the full 60 seconds it will look the same as being interrupted, and the test will pass
			// incorrectly.
			Assert.IsTrue(thread.Join(TimeSpan.FromSeconds(50)), "Thread did not exit in reasonable time."); 
			Assert.IsTrue(runnerThreadCompletedNormally, "Runner thread should have exited through normally.");
			// Ensure the external process was killed
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
				const string content = "yooo ез";
				SystemPath tempFile = tempDirectory.CreateTextFile("test.txt", content);
				ProcessInfo processInfo = Platform.IsWindows ? new ProcessInfo("cmd.exe", "/C type \"" + tempFile + "\"") : new ProcessInfo("cat", "\"" + tempFile + "\"");
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
		public void ProcessInfoDeterminesSuccessOfProcess()
		{
			int[] successExitCodes = { 1, 3, 5 };

			ProcessInfo processInfo1 = Platform.IsWindows ? new ProcessInfo("cmd.exe", "/C @echo Hello World & exit 1", null, ProcessPriorityClass.AboveNormal, successExitCodes) : new ProcessInfo("bash", "-c \"echo Hello World ; exit 1\"", null, ProcessPriorityClass.AboveNormal, successExitCodes);

			ProcessResult result1 = executor.Execute(processInfo1);
			Assert.AreEqual("Hello World", result1.StandardOutput.Trim());
			Assert.AreEqual(1, result1.ExitCode, "Process did not exit successfully");
			AssertFalse("process should not return an error", result1.Failed);

            ProcessInfo processInfo2 = Platform.IsWindows ? new ProcessInfo("cmd.exe", "/C @echo Hello World & exit 3", null, ProcessPriorityClass.AboveNormal, successExitCodes) : new ProcessInfo("bash", "-c \"echo Hello World ; exit 3\"", null, ProcessPriorityClass.AboveNormal, successExitCodes);

			ProcessResult result2 = executor.Execute(processInfo2);
			Assert.AreEqual("Hello World", result2.StandardOutput.Trim());
			Assert.AreEqual(3, result2.ExitCode, "Process did not exit successfully");
			AssertFalse("process should not return an error", result2.Failed);

            ProcessInfo processInfo3 = Platform.IsWindows ? new ProcessInfo("cmd.exe", "/C @echo Hello World & exit 5", null, ProcessPriorityClass.AboveNormal, successExitCodes) : new ProcessInfo("bash", "-c \"echo Hello World ; exit 5\"", null, ProcessPriorityClass.AboveNormal, successExitCodes);

			ProcessResult result3 = executor.Execute(processInfo3);
			Assert.AreEqual("Hello World", result3.StandardOutput.Trim());
			Assert.AreEqual(5, result3.ExitCode, "Process did not exit successfully");
			AssertFalse("process should not return an error", result3.Failed);

            ProcessInfo processInfo4 = Platform.IsWindows ? new ProcessInfo("cmd.exe", "/C @echo Hello World", null, ProcessPriorityClass.AboveNormal, successExitCodes) : new ProcessInfo("bash", "-c \"echo Hello World\"", null, ProcessPriorityClass.AboveNormal, successExitCodes);

			ProcessResult result4 = executor.Execute(processInfo4);
			Assert.AreEqual("Hello World", result4.StandardOutput.Trim());
			Assert.AreEqual(ProcessResult.SUCCESSFUL_EXIT_CODE, result4.ExitCode, "Process did not exit successfully");
			Assert.IsTrue(result4.Failed, "process should return an error");
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
        
        private static bool SleeperProcessExists()
        {
            if (Platform.IsMono)
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "bash";
                    process.StartInfo.Arguments = "-c \"ps -Aef\"";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.Start();
        
                    StreamReader reader = process.StandardOutput;
                    string output = reader.ReadToEnd();
                    
                    process.WaitForExit();
                    
                    return output.Contains("sleeper");
                }
            }
            else
            {
                return Process.GetProcessesByName("sleeper").Length != 0;
            }
        }

		private static void WaitForProcessToStart()
		{
			int count = 0;
            
			while (!SleeperProcessExists() && count < 1000)
			{
				Thread.Sleep(50);
				count++;
			}
			Thread.Sleep(2000);
			if (count == 1000) Assert.Fail("sleeper process did not start.");
		}

		private void StartSleeperProcess()
		{
			try
			{
				ProcessInfo processInfo = new ProcessInfo("sleeper.exe");
				executor.Execute(processInfo);
				runnerThreadCompletedNormally = true;
			}
			catch (ThreadAbortException)
			{
				runnerThreadWasAborted = true;
				Thread.ResetAbort();
			}
		}
	}
}
