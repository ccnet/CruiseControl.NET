using System;
using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Test;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test
{
	[TestFixture]
	public class FileSourceControlTest : CustomAssertion
	{
		private string _tempDir;
		private string _tempSubDir;
		private FileSourceControl _sc;

		[SetUp]
		public void SetUp()
		{
			_tempDir = TempFileUtil.CreateTempDir("repo");
			_tempSubDir = TempFileUtil.CreateTempDir("repo\\subrepo");

			_sc = new FileSourceControl();
			_sc.RepositoryRoot = _tempDir;
		}

		[TearDown]
		public void TearDown() 
		{
			TempFileUtil.DeleteTempDir(_tempSubDir);
			TempFileUtil.DeleteTempDir(_tempDir);
		}

		[Test]
		public void GetModifications_EmptyLocal()
		{
			string file1 = TempFileUtil.CreateTempFile("repo", "file1.txt", "foo");
			string file2 = TempFileUtil.CreateTempFile("repo", "file2.txt", "bar");
			string file3 = TempFileUtil.CreateTempFile("repo\\subrepo", "file3.txt", "bat");

			Modification[] mods = _sc.GetModifications(DateTime.MinValue, DateTime.MaxValue);

			AssertEquals(3, mods.Length);
			AssertEquals("file1.txt", mods[0].FileName);
			AssertEquals("file2.txt", mods[1].FileName);
			AssertEquals("file3.txt", mods[2].FileName);
			AssertEquals(Path.GetDirectoryName(file1), mods[0].FolderName);
			AssertEquals(Path.GetDirectoryName(file2), mods[1].FolderName);
			AssertEquals(Path.GetDirectoryName(file3), mods[2].FolderName);
			
			AssertEquals(new FileInfo(file1).LastWriteTime, mods[0].ModifiedTime);
			AssertEquals(new FileInfo(file2).LastWriteTime, mods[1].ModifiedTime);
			AssertEquals(new FileInfo(file3).LastWriteTime, mods[2].ModifiedTime);

			mods = _sc.GetModifications(DateTime.Now, DateTime.MaxValue);
			AssertEquals(0, mods.Length);
		}

		[Test]
		public void GetModifications_EmptyRepository()
		{
			Modification[] mods = _sc.GetModifications(DateTime.MinValue, DateTime.MaxValue);
			AssertNotNull(mods);
			AssertEquals(0, mods.Length);
		}

		[Test]
		public void GetModifications_OneUnmodifiedFile()
		{
			string file1 = TempFileUtil.CreateTempFile("repo", "file1.txt", "foo");
			DateTime from = DateTime.Now;
			System.Threading.Thread.Sleep(100);
			string file2 = TempFileUtil.CreateTempFile("repo", "file2.txt", "bar");

			Modification[] mods = _sc.GetModifications(from, DateTime.MaxValue);
			AssertEquals(1, mods.Length);
			AssertEquals("file2.txt", mods[0].FileName);
		}

		[Test]
		public void ShouldRun()
		{
			Assert(_sc.ShouldRun(new IntegrationResult()));
			Assert(_sc.ShouldRun(IntegrationResultMother.CreateSuccessful()));
			AssertFalse(_sc.ShouldRun(IntegrationResultMother.CreateFailed()));
			AssertFalse(_sc.ShouldRun(IntegrationResultMother.CreateExceptioned()));
		}
	}
}
