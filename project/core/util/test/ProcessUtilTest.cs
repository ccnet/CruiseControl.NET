using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using tw.ccnet.core.util;

namespace tw.ccnet.core.util.test
{
	[TestFixture]
	public class ProcessUtilTest
	{
		private string _tempDir = "processTestTemp";

		public void TestExecuteRedirected()
		{			
			string executable = CreateExecutableFile();
			Process process = ProcessUtil.CreateProcess(executable, "");
			TextReader reader = ProcessUtil.ExecuteRedirected(process);		
			
			Assertion.Assert("expected process not to have exited", process.HasExited == false);
			
			string read = reader.ReadToEnd();
			Assertion.Assert("did not find bubba in string", StringUtil.StringContains(read, "bubba"));
			
			Assertion.Assert("expected process to have exited", process.HasExited);
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