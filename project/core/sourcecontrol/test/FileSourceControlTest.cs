using System;
using System.IO;
using NUnit.Framework;
using tw.ccnet.core.util;

namespace tw.ccnet.core.sourcecontrol.test
{
	[TestFixture]
	public class FileSourceControlTest
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

		public void TestGetModifications_EmptyLocal()
		{
			string file1 = TempFileUtil.CreateTempFile("repo", "file1.txt", "foo");
			string file2 = TempFileUtil.CreateTempFile("repo", "file2.txt", "bar");
			string file3 = TempFileUtil.CreateTempFile("repo\\subrepo", "file3.txt", "bat");

			Modification[] mods = _sc.GetModifications(DateTime.MinValue, DateTime.MaxValue);

			Assertion.AssertEquals(3, mods.Length);
			Assertion.AssertEquals("file1.txt", mods[0].FileName);
			Assertion.AssertEquals("file2.txt", mods[1].FileName);
			Assertion.AssertEquals("file3.txt", mods[2].FileName);
			Assertion.AssertEquals(Path.GetDirectoryName(file1), mods[0].FolderName);
			Assertion.AssertEquals(Path.GetDirectoryName(file2), mods[1].FolderName);
			Assertion.AssertEquals(Path.GetDirectoryName(file3), mods[2].FolderName);
			
			Assertion.AssertEquals(new FileInfo(file1).LastWriteTime, mods[0].ModifiedTime);
			Assertion.AssertEquals(new FileInfo(file2).LastWriteTime, mods[1].ModifiedTime);
			Assertion.AssertEquals(new FileInfo(file3).LastWriteTime, mods[2].ModifiedTime);

			mods = _sc.GetModifications(DateTime.Now, DateTime.MaxValue);
			Assertion.AssertEquals(0, mods.Length);
		}

		public void TestGetModifications_EmptyRepository()
		{
			Modification[] mods = _sc.GetModifications(DateTime.MinValue, DateTime.MaxValue);
			Assertion.AssertNotNull(mods);
			Assertion.AssertEquals(0, mods.Length);
		}

		public void TestGetModifications_OneUnmodifiedFile()
		{
			string file1 = TempFileUtil.CreateTempFile("repo", "file1.txt", "foo");
			DateTime from = DateTime.Now;
			System.Threading.Thread.Sleep(100);
			string file2 = TempFileUtil.CreateTempFile("repo", "file2.txt", "bar");

			Modification[] mods = _sc.GetModifications(from, DateTime.MaxValue);
			Assertion.AssertEquals(1, mods.Length);
			Assertion.AssertEquals("file2.txt", mods[0].FileName);
		}

	}
}
