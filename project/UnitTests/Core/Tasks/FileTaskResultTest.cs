using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using System.Diagnostics;

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

		[Test]
		public void ShouldThrowReadableExceptionIfFileDoesNotExist()
		{
            Assert.That(delegate { new FileTaskResult("unknown.file"); },
                        Throws.TypeOf<CruiseControlException>());
		}
	}
}