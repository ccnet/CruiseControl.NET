using System.Diagnostics;
using System.IO;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;

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
            this.mocks = new MockRepository(MockBehavior.Default);
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
            var fileSystem = this.mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            var file = new FileInfo(this.filename);
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.FileExists(file.FullName)).Returns(true);
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.DeleteFile(file.FullName)).Verifiable();

            // Run the test
            var result = new FileTaskResult(file, true, fileSystem);
            result.CleanUp();

            // Check the results
            this.mocks.VerifyAll();
        }

        [Test]
        public void FileIsNotDeletedIfDeletedAfterMergeIsNotSet()
        {
            // Initialise the test
            var fileSystem = this.mocks.Create<IFileSystem>(MockBehavior.Strict).Object;
            var file = new FileInfo(this.filename);
            Mock.Get(fileSystem).Setup(_fileSystem => _fileSystem.FileExists(file.FullName)).Returns(true);

            // Run the test
            var result = new FileTaskResult(file, false, fileSystem);
            result.CleanUp();

            // Check the results
            this.mocks.VerifyAll();
        }
    }
}