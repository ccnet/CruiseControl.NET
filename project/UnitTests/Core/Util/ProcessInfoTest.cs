using System.Diagnostics;
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
			ProcessInfo info = new ProcessInfo(@"""c:\nant\nant.exe""", null, @"""c:\working""");
			Assert.AreEqual(@"c:\nant\nant.exe", info.FileName);
		}
	}
}
