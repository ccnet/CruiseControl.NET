using System;
using System.IO;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class FileSourceControlTest : IntegrationFixture
	{
		private FileSourceControl sc;
		private Mock<IFileSystem> fileSystemMock;
		private SystemPath tempRoot;
		private SystemPath tempSubRoot;

		[SetUp]
		public void SetUp()
		{
			fileSystemMock = new Mock<IFileSystem>();

			tempRoot = SystemPath.UniqueTempPath();
			tempSubRoot = tempRoot.Combine("subrepo");

			sc = new FileSourceControl((IFileSystem) fileSystemMock.Object);
			sc.RepositoryRoot = tempRoot.ToString();
		}

		[TearDown]
		public void TearDown()
		{
			tempRoot.DeleteDirectory();
		}

		[Test]
		public void MissingDirectoryThrowsException()
		{
			Assert.IsFalse(tempRoot.Exists(), "Temporary directory should not exist: " + tempRoot.ToString());
            Assert.That(delegate { sc.GetModifications(IntegrationResult(DateTime.MinValue), IntegrationResult(DateTime.MaxValue)); },
                        Throws.TypeOf<DirectoryNotFoundException>());
		}

		[Test]
		public void IgnoreMissingDirectoryReturnsZeroMods()
		{
			Assert.IsFalse(tempRoot.Exists(), "Temporary directory should not exist: " + tempRoot.ToString());
			sc.IgnoreMissingRoot = true;
			try
			{
				Modification[] mods = sc.GetModifications(IntegrationResult(DateTime.MinValue), IntegrationResult(DateTime.MaxValue));
				Assert.AreEqual(0, mods.Length, "Modifications found in a missing directory");
			}
			finally
			{
				sc.IgnoreMissingRoot = false;
			}
		}

		[Test]
		public void GetModifications_EmptyLocal()
		{
			tempRoot.CreateDirectory();
			tempSubRoot.CreateDirectory();
			string file1 = tempRoot.CreateTextFile("file1.txt", "foo").ToString();
			string file2 = tempRoot.CreateTextFile("file2.txt", "bar").ToString();
			string file3 = tempSubRoot.CreateTextFile("file3.txt", "bat").ToString();

			Modification[] mods = sc.GetModifications(IntegrationResult(DateTime.MinValue), IntegrationResult(DateTime.MaxValue));

			Assert.AreEqual(4, mods.Length);
			Assert.AreEqual("file1.txt", mods[0].FileName);
			Assert.AreEqual("file2.txt", mods[1].FileName);
            Assert.AreEqual(Path.GetFileName(tempSubRoot.ToString()), mods[2].FileName);
            Assert.AreEqual("file3.txt", mods[3].FileName);
			Assert.AreEqual(Path.GetDirectoryName(file1), mods[0].FolderName);
			Assert.AreEqual(Path.GetDirectoryName(file2), mods[1].FolderName);
            Assert.AreEqual(Path.GetFileName(tempSubRoot.ToString()), mods[2].FolderName);
            Assert.AreEqual(Path.GetDirectoryName(file3), mods[3].FolderName);

			Assert.AreEqual(new FileInfo(file1).LastWriteTime, mods[0].ModifiedTime);
			Assert.AreEqual(new FileInfo(file2).LastWriteTime, mods[1].ModifiedTime);
            Assert.AreEqual(new FileInfo(tempSubRoot.ToString()).LastWriteTime, mods[2].ModifiedTime);
            Assert.AreEqual(new FileInfo(file3).LastWriteTime, mods[3].ModifiedTime);

			mods = sc.GetModifications(IntegrationResult(DateTime.Now.AddHours(1)), IntegrationResult(DateTime.MaxValue));
			Assert.AreEqual(0, mods.Length);
		}

		[Test]
		public void GetModificationsWhenRepositoryFolderIsEmpty()
		{
			tempRoot.CreateDirectory();
			Modification[] mods = sc.GetModifications(IntegrationResult(DateTime.MinValue), IntegrationResult(DateTime.MaxValue));
			Assert.IsNotNull(mods);
			Assert.AreEqual(0, mods.Length);
		}

		[Test]
		public void GetModificationsWhenRepositoryRootContainsOneUnmodifiedFile()
		{
			tempRoot.CreateDirectory();
			tempRoot.CreateTextFile("file1.txt", "foo");
			string file2 = tempRoot.CreateTextFile("file2.txt", "bar").ToString();
			new FileInfo(file2).LastWriteTime = DateTime.Now.AddHours(2);

			Modification[] mods = sc.GetModifications(IntegrationResult(DateTime.Now.AddHours(1)), IntegrationResult(DateTime.MaxValue));
			Assert.AreEqual(1, mods.Length);
			Assert.AreEqual("file2.txt", mods[0].FileName);
		}

		[Test]
		public void ShouldCopyRespositoryRootToWorkingDirectoryForGetSource()
		{
			IntegrationResult result = (IntegrationResult) Integration("foo", "myWorkingDirectory", "myArtifactDirectory");
			sc.AutoGetSource = true;
			fileSystemMock.Setup(fileSystem => fileSystem.Copy(tempRoot.ToString(), "myWorkingDirectory")).Verifiable();

			sc.GetSource(result);

			fileSystemMock.Verify();
		}

		[Test]
		public void ShouldNotCopySourceIfAutoGetSourceNotBeenSetToTrue()
		{
            IIntegrationResult result = Integration("foo", "myWorkingDirectory", "myArtifactDirectory");

			sc.GetSource(result);

			fileSystemMock.Verify();
			fileSystemMock.VerifyNoOtherCalls();
		}

		private IntegrationResult IntegrationResult(DateTime date)
		{
			return IntegrationResultMother.CreateSuccessful(date);
		}

        [Test]
        public void GetModifications_NonRecursive()
        {
            tempRoot.CreateDirectory();
            tempSubRoot.CreateDirectory();
            string file1 = tempRoot.CreateTextFile("file1.txt", "foo").ToString();
            string file2 = tempRoot.CreateTextFile("file2.txt", "bar").ToString();
            string file3 = tempSubRoot.CreateTextFile("file3.txt", "bat").ToString();

            sc.CheckRecursively = false;

            Modification[] mods = sc.GetModifications(IntegrationResult(DateTime.MinValue), IntegrationResult(DateTime.MaxValue));

            Assert.AreEqual(3, mods.Length);
            Assert.AreEqual("file1.txt", mods[0].FileName);
            Assert.AreEqual("file2.txt", mods[1].FileName);
            Assert.AreEqual(Path.GetFileName(tempSubRoot.ToString()), mods[2].FileName);
            Assert.AreEqual(Path.GetDirectoryName(file1), mods[0].FolderName);
            Assert.AreEqual(Path.GetDirectoryName(file2), mods[1].FolderName);
            Assert.AreEqual(Path.GetFileName(tempSubRoot.ToString()), mods[2].FolderName);

            Assert.AreEqual(new FileInfo(file1).LastWriteTime, mods[0].ModifiedTime);
            Assert.AreEqual(new FileInfo(file2).LastWriteTime, mods[1].ModifiedTime);
            Assert.AreEqual(new FileInfo(tempSubRoot.ToString()).LastWriteTime, mods[2].ModifiedTime);

            mods = sc.GetModifications(IntegrationResult(DateTime.Now.AddHours(1)), IntegrationResult(DateTime.MaxValue));
            Assert.AreEqual(0, mods.Length);
        }
	}
}