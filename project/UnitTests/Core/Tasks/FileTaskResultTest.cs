using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
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

		[Test]
		public void ShouldReadContentsOfTempFile()
		{
			FileTaskResult result = new FileTaskResult(filename);
			Assert.AreEqual("<invalid xml>", result.Data);
		}

		[Test, ExpectedException(typeof (CruiseControlException))]
		public void ShouldThrowReadableExceptionIfFileDoesNotExist()
		{
			new FileTaskResult("unknown.file");
		}
	}
}