using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Util.Test
{
	[TestFixture]
	public class ProcessUtilTest : CustomAssertion
	{
		private string _tempDir = "processTestTemp";

		public void TestExecuteRedirected()
		{			
			string executable = CreateExecutableFile();
			Process process = ProcessUtil.CreateProcess(executable, "");
			TextReader reader = ProcessUtil.ExecuteRedirected(process);		
			
			Assert("expected process not to have exited", process.HasExited == false);
			
			string read = reader.ReadToEnd();
			Assert("did not find bubba in string", StringUtil.StringContains(read, "bubba"));
			
			Assert("expected process to have exited", process.HasExited);
		}

		[TearDown]
		public void TearDown()
		{
			TempFileUtil.DeleteTempDir(_tempDir);
		}

		private string CreateExecutableFile()
		{
			string content = 
@"
:beginning
dir
dir
dir
dir
dir
dir
dir
dir
dir
dir
dir
dir
dir
dir
dir
dir
echo bubba
";

			string name = "callPause.bat";
			return TempFileUtil.CreateTempFile(_tempDir, name, content);
		}		
	}
}