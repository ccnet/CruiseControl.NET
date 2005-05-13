using System;
using System.IO;
using System.Threading;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class FileSourceControlTest : CustomAssertion
	{
		private string _tempDir;
		private string _tempSubDir;
		private FileSourceControl _sc;
		private DynamicMock fileSystemMock;

		[SetUp]
		public void SetUp()
		{
			fileSystemMock = new DynamicMock(typeof(IFileSystem));

			_tempDir = TempFileUtil.CreateTempDir("repo");
			_tempSubDir = TempFileUtil.CreateTempDir("repo\\subrepo");

			_sc = new FileSourceControl((IFileSystem) fileSystemMock.MockInstance);
			_sc.RepositoryRoot = _tempDir;
		}

		[TearDown]
		public void TearDown() 
		{
			TempFileUtil.DeleteTempDir(_tempSubDir);
			TempFileUtil.DeleteTempDir(_tempDir);
		}

		[Test, ExpectedException(typeof(DirectoryNotFoundException))]
		public void MissingDirectoryThrowsException()
		{
			TempFileUtil.DeleteTempDir(_tempSubDir);
			TempFileUtil.DeleteTempDir(_tempDir);

			_sc.GetModifications(IntegrationResult(DateTime.MinValue), IntegrationResult(DateTime.MaxValue));
		}

		[Test]
		public void IgnoreMissingDirectoryReturnsZeroMods()
		{
			TempFileUtil.DeleteTempDir(_tempSubDir);
			TempFileUtil.DeleteTempDir(_tempSubDir);

			_sc.IgnoreMissingRoot = true;
			try 
			{
				Modification[] mods = _sc.GetModifications(IntegrationResult(DateTime.MinValue), IntegrationResult(DateTime.MaxValue));
				Assert.AreEqual(0, mods.Length, "Modifications found in a missing directory");
			} 
			finally 
			{
				_sc.IgnoreMissingRoot = false;
			}
		}

		[Test]
		public void GetModifications_EmptyLocal()
		{
			string file1 = TempFileUtil.CreateTempFile("repo", "file1.txt", "foo");
			string file2 = TempFileUtil.CreateTempFile("repo", "file2.txt", "bar");
			string file3 = TempFileUtil.CreateTempFile("repo\\subrepo", "file3.txt", "bat");

			Modification[] mods = _sc.GetModifications(IntegrationResult(DateTime.MinValue), IntegrationResult(DateTime.MaxValue));

			Assert.AreEqual(3, mods.Length);
			Assert.AreEqual("file1.txt", mods[0].FileName);
			Assert.AreEqual("file2.txt", mods[1].FileName);
			Assert.AreEqual("file3.txt", mods[2].FileName);
			Assert.AreEqual(Path.GetDirectoryName(file1), mods[0].FolderName);
			Assert.AreEqual(Path.GetDirectoryName(file2), mods[1].FolderName);
			Assert.AreEqual(Path.GetDirectoryName(file3), mods[2].FolderName);
			
			Assert.AreEqual(new FileInfo(file1).LastWriteTime, mods[0].ModifiedTime);
			Assert.AreEqual(new FileInfo(file2).LastWriteTime, mods[1].ModifiedTime);
			Assert.AreEqual(new FileInfo(file3).LastWriteTime, mods[2].ModifiedTime);

			mods = _sc.GetModifications(IntegrationResult(DateTime.Now), IntegrationResult(DateTime.MaxValue));
			Assert.AreEqual(0, mods.Length);
		}

		[Test]
		public void GetModifications_EmptyRepository()
		{
			Modification[] mods = _sc.GetModifications(IntegrationResult(DateTime.MinValue), IntegrationResult(DateTime.MaxValue));
			Assert.IsNotNull(mods);
			Assert.AreEqual(0, mods.Length);
		}

		[Test]
		public void GetModifications_OneUnmodifiedFile()
		{
			TempFileUtil.CreateTempFile("repo", "file1.txt", "foo");
			DateTime from = DateTime.Now;
			Thread.Sleep(100);
			TempFileUtil.CreateTempFile("repo", "file2.txt", "bar");

			Modification[] mods = _sc.GetModifications(IntegrationResult(from), IntegrationResult(DateTime.MaxValue));
			Assert.AreEqual(1, mods.Length);
			Assert.AreEqual("file2.txt", mods[0].FileName);
		}

		[Test]
		public void ShouldCopyRespositoryRootToWorkingDirectoryForGetSource()
		{
			IntegrationResult result = new IntegrationResult("foo", "myWorkingDirectory");
            _sc.AutoGetSource = true;
			fileSystemMock.Expect("Copy", _tempDir, "myWorkingDirectory");

			_sc.GetSource(result);

			fileSystemMock.Verify();
		}

		[Test]
		public void ShouldNotCopySourceIfAutoGetSourceNotBeenSetToTrue()
		{
			IntegrationResult result = new IntegrationResult("foo", "myWorkingDirectory");
			fileSystemMock.ExpectNoCall("Copy", typeof(string), typeof(string));

			_sc.GetSource(result);

			fileSystemMock.Verify();	
		}

		private IntegrationResult IntegrationResult(DateTime date)
		{
			return IntegrationResultMother.CreateSuccessful(date);
		}
	}
}
