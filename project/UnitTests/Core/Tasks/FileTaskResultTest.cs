using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using System.Diagnostics;
using Rhino.Mocks;
using System.IO;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
	[TestFixture]
	public class FileTaskResultTest
	{
        private MockRepository mocks;
        private string filename;

		[SetUp]
		public void CreateFile()
		{
			filename = TempFileUtil.CreateTempFile("FileTaskResult", "test.xml", "<invalid xml>");
            this.mocks = new MockRepository();
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

        [Test]
        public void DeleteAfterMergeDeletesTheFile()
        {
            // Initialise the test
            var fileSystem = this.mocks.StrictMock<IFileSystem>();
            var file = new FileInfo(this.filename);
            SetupResult.For(fileSystem.FileExists(file.FullName)).Return(true);
            Expect.Call(() =>
            {
                fileSystem.DeleteFile(file.FullName);
            });

            // Run the test
            this.mocks.ReplayAll();
            var result = new FileTaskResult(file, true, fileSystem);
            result.CleanUp();

            // Check the results
            this.mocks.VerifyAll();
        }

        [Test]
        public void FileIsNotDeletedIfDeletedAfterMergeIsNotSet()
        {
            // Initialise the test
            var fileSystem = this.mocks.StrictMock<IFileSystem>();
            var file = new FileInfo(this.filename);
            SetupResult.For(fileSystem.FileExists(file.FullName)).Return(true);

            // Run the test
            this.mocks.ReplayAll();
            var result = new FileTaskResult(file, false, fileSystem);
            result.CleanUp();

            // Check the results
            this.mocks.VerifyAll();
        }
    }
}