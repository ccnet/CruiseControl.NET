using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Util
{
	[TestFixture]
	public class SystemIoFileSystemTest
	{
		private string _tempDir;
		private string _tempSubDir;
		private string _tempOtherDir;

		[SetUp]
		public void Setup()
		{
			_tempDir = TempFileUtil.CreateTempDir("repo");
			_tempSubDir = TempFileUtil.CreateTempDir("repo\\subrepo");
			_tempOtherDir = TempFileUtil.CreateTempDir("other");
		}

		[TearDown]
		public void TearDown()
		{
			TempFileUtil.DeleteTempDir(_tempOtherDir);
			TempFileUtil.DeleteTempDir(_tempSubDir);
			TempFileUtil.DeleteTempDir(_tempDir);
		}

		[Test]
		public void ShouldCopyFileToDirectory()
		{
			TempFileUtil.CreateTempFile(_tempDir, "File1");

			new SystemIoFileSystem().Copy(Path.Combine(_tempDir, "File1"), _tempSubDir);

			Assert.IsTrue(File.Exists(Path.Combine(_tempSubDir, "File1")));
		}

		[Test]
		public void ShouldCopyFileToFile()
		{
			TempFileUtil.CreateTempFile(_tempDir, "File1");

			new SystemIoFileSystem().Copy(Path.Combine(_tempDir, "File1"), Path.Combine(_tempDir, "File2"));

			Assert.IsTrue(File.Exists(Path.Combine(_tempDir, "File2")));
		}


		[Test]
		public void ShouldAllowOverwrites()
		{
			TempFileUtil.CreateTempFile(_tempDir, "File1");
			TempFileUtil.CreateTempFile(_tempDir, "File2");

			new SystemIoFileSystem().Copy(Path.Combine(_tempDir, "File1"), Path.Combine(_tempDir, "File2"));

			Assert.IsTrue(File.Exists(Path.Combine(_tempDir, "File2")));
		}

		[Test]
		public void ShouldAllowOverwritesEvenWhenDestinationHasReadOnlyAttributeSet()
		{
			string file1 = TempFileUtil.CreateTempFile(_tempDir, "File1");
			string file2 = TempFileUtil.CreateTempFile(_tempDir, "File2");
			File.SetAttributes(file2, FileAttributes.ReadOnly);

			new SystemIoFileSystem().Copy(file1, file2);

			Assert.IsTrue(File.Exists(Path.Combine(_tempDir, "File2")));
		}

		[Test]
		public void ShouldCopyDirectoryToDirectoryRecursively()
		{
			TempFileUtil.CreateTempFile(_tempDir, "File1");
			TempFileUtil.CreateTempFile(_tempSubDir, "File2");

			new SystemIoFileSystem().Copy(_tempDir, _tempOtherDir);

			Assert.IsTrue(File.Exists(Path.Combine(_tempOtherDir, "File1")));
			Assert.IsTrue(File.Exists(Path.Combine(Path.Combine(_tempOtherDir, "subrepo"), "File2")));
		}
	}
}
