using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Util
{
	[TestFixture]
	public class ProcessInfoTest : CustomAssertion
	{
		[Test]
		public void IfStandardInputContentIsSetThenStandardInputIsRedirected()
		{
			ProcessInfo info = new ProcessInfo("temp");
			info.StandardInputContent = "Some content";

			Process process = info.CreateProcess();
			Assert.IsTrue(process.StartInfo.RedirectStandardInput);
			Assert.IsTrue(!process.StartInfo.UseShellExecute);
		}

		[Test]
		public void IfStandardInputContentIsNotSetThenStandardInputIsNotRedirected()
		{
			ProcessInfo info = new ProcessInfo("temp");
			Process process = info.CreateProcess();
			Assert.IsTrue(!process.StartInfo.RedirectStandardInput);
		}

		[Test]
		public void IfExecutableIsFoundInWorkingDirectoryThenUseCombinedPathAsExecutablePath()
		{
			string workingDir = TempFileUtil.CreateTempDir("working");
			string executablePath = TempFileUtil.CreateTempFile("working", "myExecutable");

			ProcessInfo infoWithoutPathQualifiedExecutable = new ProcessInfo("myExecutable", "", workingDir);
			ProcessInfo infoWithPreQualifiedExecutable = new ProcessInfo(executablePath, "", workingDir);

			Assert.AreEqual(infoWithPreQualifiedExecutable, infoWithoutPathQualifiedExecutable);
		}

		[Test]
		public void StripQuotesFromQuotedExecutablePath()
		{
			ProcessInfo info = new ProcessInfo(@"""c:\nant\nant.exe""", null, string.Format(@"""{0}""", Path.GetTempPath()));
			Assert.AreEqual(@"c:\nant\nant.exe", info.FileName);
			Assert.AreEqual(Path.GetTempPath(), info.WorkingDirectory);
		}

		[Test]
		public void ProcessSuccessIsDeterminedBySuccessExitCodes()
		{
			int[] successExitCodes = { 1, 3, 5 };

			ProcessInfo info = new ProcessInfo(@"""c:\nant\nant.exe""", null, string.Format(@"""{0}""", Path.GetTempPath()), successExitCodes);

			Assert.IsFalse(info.ProcessSuccessful(0));
			Assert.IsTrue(info.ProcessSuccessful(1));
			Assert.IsFalse(info.ProcessSuccessful(2));
			Assert.IsTrue(info.ProcessSuccessful(3));
			Assert.IsFalse(info.ProcessSuccessful(4));
			Assert.IsTrue(info.ProcessSuccessful(5));
			Assert.IsFalse(info.ProcessSuccessful(6));
		}

		[Test]
		public void ProcessSuccessRequiresZeroExitCode()
		{
			int[] successExitCodes = { 1, 3, 5 };

			ProcessInfo info = new ProcessInfo(@"""c:\nant\nant.exe""", null, string.Format(@"""{0}""", Path.GetTempPath()));

			Assert.IsTrue(info.ProcessSuccessful(0));
			Assert.IsFalse(info.ProcessSuccessful(1));
			Assert.IsFalse(info.ProcessSuccessful(2));
			Assert.IsFalse(info.ProcessSuccessful(-1));
		}

	}
}
