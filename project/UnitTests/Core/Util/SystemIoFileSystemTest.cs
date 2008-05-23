using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Util
{
	[TestFixture]
	public class SystemIoFileSystemTest
	{
		private SystemPath tempRoot;
		private SystemPath tempSubRoot;
		private SystemPath tempOtherRoot;

		[SetUp]
		public void Setup()
		{
			tempRoot = SystemPath.UniqueTempPath().CreateDirectory();
			tempSubRoot = tempRoot.CreateSubDirectory("subrepo");
			tempOtherRoot = SystemPath.UniqueTempPath().CreateDirectory();
		}

		[TearDown]
		public void TearDown()
		{
			tempRoot.DeleteDirectory();
			tempOtherRoot.DeleteDirectory();
		}

		[Test]
		public void ShouldCopyFileToDirectory()
		{
			SystemPath file1 = tempRoot.CreateEmptyFile("File1");
			new SystemIoFileSystem().Copy(file1.ToString(), tempSubRoot.ToString());
			Assert.IsTrue(tempSubRoot.Combine("File1").Exists());
		}

		[Test]
		public void ShouldCopyFileToFile()
		{
			SystemPath sourceFile = tempRoot.CreateEmptyFile("File1");
			SystemPath targetFile = tempSubRoot.Combine("File2");
			new SystemIoFileSystem().Copy(sourceFile.ToString(), targetFile.ToString());
			Assert.IsTrue(targetFile.Exists());
		}

		[Test]
		public void ShouldAllowOverwrites()
		{			
			SystemPath sourceFile = tempRoot.CreateEmptyFile("File1");
			SystemPath targetFile = tempSubRoot.CreateEmptyFile("File2");
			new SystemIoFileSystem().Copy(sourceFile.ToString(), targetFile.ToString());
			Assert.IsTrue(targetFile.Exists());
		}

		[Test]
		public void ShouldAllowOverwritesEvenWhenDestinationHasReadOnlyAttributeSet()
		{
			SystemPath sourceFile = tempRoot.CreateEmptyFile("File1");
			SystemPath targetFile = tempSubRoot.CreateEmptyFile("File2");
			File.SetAttributes(targetFile.ToString(), FileAttributes.ReadOnly);
			new SystemIoFileSystem().Copy(sourceFile.ToString(), targetFile.ToString());

			Assert.IsTrue(targetFile.Exists());
		}

		[Test]
		public void ShouldCopyDirectoryToDirectoryRecursively()
		{
			tempRoot.CreateEmptyFile("File1");
			tempSubRoot.CreateEmptyFile("File2");
			new SystemIoFileSystem().Copy(tempRoot.ToString(), tempOtherRoot.ToString());

			Assert.IsTrue(tempOtherRoot.Combine("File1").Exists());
			Assert.IsTrue(tempOtherRoot.Combine("subrepo").Combine("File2").Exists());
		}

		[Test]
		public void ShouldSaveToFile()
		{
			SystemPath tempFile = tempRoot.Combine("foo.txt");
			Assert.IsFalse(tempFile.Exists());
			new SystemIoFileSystem().Save(tempFile.ToString(), "bar");
			Assert.IsTrue(tempFile.Exists());
			using (StreamReader reader = File.OpenText(tempFile.ToString()))
			{
				Assert.AreEqual("bar", reader.ReadToEnd());
			}
		}

		[Test]
		public void ShouldSaveUnicodeToFile()
		{
			SystemPath tempFile = tempRoot.Combine("foo.txt");
			Assert.IsFalse(tempFile.Exists());
			new SystemIoFileSystem().Save(tempFile.ToString(), "hi there? håkan! \u307b");
			Assert.IsTrue(tempFile.Exists());
			using (StreamReader reader = File.OpenText(tempFile.ToString()))
			{
				Assert.AreEqual("hi there? håkan! \u307b", reader.ReadToEnd());
			}
		}

        [Test]
        public void ShouldSaveToFileAtomically()
        {
            SystemPath tempFile = tempRoot.Combine("foo.txt");
            Assert.IsFalse(tempFile.Exists());
            new SystemIoFileSystem().AtomicSave(tempFile.ToString(), "bar");
            Assert.IsTrue(tempFile.Exists());
            using (StreamReader reader = File.OpenText(tempFile.ToString()))
            {
                Assert.AreEqual("bar", reader.ReadToEnd());
            }
            new SystemIoFileSystem().AtomicSave(tempFile.ToString(), "baz");
            Assert.IsTrue(tempFile.Exists());
            using (StreamReader reader = File.OpenText(tempFile.ToString()))
            {
                Assert.AreEqual("baz", reader.ReadToEnd());
            }
        }

        [Test]
        public void ShouldSaveUnicodeToFileAtomically()
        {
            SystemPath tempFile = tempRoot.Combine("foo.txt");
            Assert.IsFalse(tempFile.Exists());
            new SystemIoFileSystem().AtomicSave(tempFile.ToString(), "hi there? håkan! \u307b");
            Assert.IsTrue(tempFile.Exists());
            using (StreamReader reader = File.OpenText(tempFile.ToString()))
            {
                Assert.AreEqual("hi there? håkan! \u307b", reader.ReadToEnd());
            }
            new SystemIoFileSystem().AtomicSave(tempFile.ToString(), "hi there? håkan! \u307b sadfasdf");
            Assert.IsTrue(tempFile.Exists());
            using (StreamReader reader = File.OpenText(tempFile.ToString()))
            {
                Assert.AreEqual("hi there? håkan! \u307b sadfasdf", reader.ReadToEnd());
            }
        }

        [Test]
		public void LoadReadsFileContentCorrectly()
		{
			SystemPath tempFile = tempRoot.CreateTextFile("foo.txt", "bar");
			Assert.AreEqual("bar", new SystemIoFileSystem().Load(tempFile.ToString()).ReadToEnd());
		}
	}
}