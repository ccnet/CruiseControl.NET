using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
	[TestFixture]
	public class FileTaskResultTest
	{
		private string filename;

		[SetUp]
		public void CreateFile()
		{
			filename = TempFileUtil.CreateTempFile("FileTaskResult", "test.xml", "<invalid xml>");			
		}

		[TearDown]
		public void DestroyFile()
		{
			TempFileUtil.DeleteTempDir("FileTaskResult");
		}

		[Test, ExpectedException(typeof(CruiseControlException), "Unable to read the contents of the merge file: test.xml")]
		public void ShouldThrowReadableExceptionIfFileContainsInvalidXml()
		{
			FileInfo info = new FileInfo(filename);
			FileTaskResult result = new FileTaskResult(info);
			string data = result.Data;
		}
	}
}
