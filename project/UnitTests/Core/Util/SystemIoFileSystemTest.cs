using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Util
{
	[TestFixture]
	public class SystemIoFileSystemTest
	{
		private string tempDir;
		private string tempSubDir;
		private string tempOtherDir;

		[SetUp]
		public void Setup()
		{
			tempDir = TempFileUtil.CreateTempDir("repo");
			tempSubDir = TempFileUtil.CreateTempDir("repo\\subrepo");
			tempOtherDir = TempFileUtil.CreateTempDir("other");
		}

		[TearDown]
		public void TearDown()
		{
			TempFileUtil.DeleteTempDir(tempOtherDir);
			TempFileUtil.DeleteTempDir(tempSubDir);
			TempFileUtil.DeleteTempDir(tempDir);
		}

		[Test]
		public void ShouldCopyFileToDirectory()
		{
			TempFileUtil.CreateTempFile(tempDir, "File1");
			new SystemIoFileSystem().Copy(Path.Combine(tempDir, "File1"), tempSubDir);
			Assert.IsTrue(File.Exists(Path.Combine(tempSubDir, "File1")));
		}

		[Test]
		public void ShouldCopyFileToFile()
		{
			TempFileUtil.CreateTempFile(tempDir, "File1");
			new SystemIoFileSystem().Copy(Path.Combine(tempDir, "File1"), Path.Combine(tempDir, "File2"));
			Assert.IsTrue(File.Exists(Path.Combine(tempDir, "File2")));
		}

		[Test]
		public void ShouldAllowOverwrites()
		{
			TempFileUtil.CreateTempFile(tempDir, "File1");
			TempFileUtil.CreateTempFile(tempDir, "File2");
			new SystemIoFileSystem().Copy(Path.Combine(tempDir, "File1"), Path.Combine(tempDir, "File2"));
			Assert.IsTrue(File.Exists(Path.Combine(tempDir, "File2")));
		}

		[Test]
		public void ShouldAllowOverwritesEvenWhenDestinationHasReadOnlyAttributeSet()
		{
			string file1 = TempFileUtil.CreateTempFile(tempDir, "File1");
			string file2 = TempFileUtil.CreateTempFile(tempDir, "File2");
			File.SetAttributes(file2, FileAttributes.ReadOnly);
			new SystemIoFileSystem().Copy(file1, file2);

			Assert.IsTrue(File.Exists(Path.Combine(tempDir, "File2")));
		}

		[Test]
		public void ShouldCopyDirectoryToDirectoryRecursively()
		{
			TempFileUtil.CreateTempFile(tempDir, "File1");
			TempFileUtil.CreateTempFile(tempSubDir, "File2");
			new SystemIoFileSystem().Copy(tempDir, tempOtherDir);

			Assert.IsTrue(File.Exists(Path.Combine(tempOtherDir, "File1")));
			Assert.IsTrue(File.Exists(Path.Combine(Path.Combine(tempOtherDir, "subrepo"), "File2")));
		}

		[Test]
		public void ShouldSaveToFile()
		{
			string tempFile = Path.Combine(tempDir, "foo.txt");
			Assert.IsFalse(File.Exists(tempFile));
			new SystemIoFileSystem().Save(tempFile, "bar");
			Assert.IsTrue(File.Exists(tempFile));
			using (StreamReader reader = File.OpenText(tempFile))
			{
				Assert.AreEqual("bar", reader.ReadToEnd());
			}
		}

		[Test]
		public void ShouldSaveUnicodeToFile()
		{
			string tempFile = Path.Combine(tempDir, "foo.txt");
			Assert.IsFalse(File.Exists(tempFile));
			new SystemIoFileSystem().Save(tempFile, "hi there? håkan! \u307b");
			Assert.IsTrue(File.Exists(tempFile));
			using (StreamReader reader = File.OpenText(tempFile))
			{
				Assert.AreEqual("hi there? håkan! \u307b", reader.ReadToEnd());
			}
		}

		[Test]
		public void LoadReadsFileContentCorrectly()
		{
			TempFileUtil.CreateTempFile(tempDir, "foo.txt", "bar");
			Assert.AreEqual("bar", new SystemIoFileSystem().Load(Path.Combine(tempDir, "foo.txt")).ReadToEnd());
		}
	}
}