using NUnit.Framework;

using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Util.Test
{
	[TestFixture]
	public class ProcessInfoTest : CustomAssertion
	{
		[Test]
		public void IfStandardInputContentIsSetThenStandardInputIsRedirected()
		{
			ProcessInfo info = new ProcessInfo("temp");
			info.StandardInputContent = "Some content";

			Assert.IsTrue(info.StartInfo.RedirectStandardInput);
			Assert.IsTrue(!info.StartInfo.UseShellExecute);
		}

		[Test]
		public void IfStandardInputContentIsNotSetThenStandardInputIsNotRedirected()
		{
			ProcessInfo info = new ProcessInfo("temp");

			Assert.IsTrue(!info.StartInfo.RedirectStandardInput);
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
	}
}
