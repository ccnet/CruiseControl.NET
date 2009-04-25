using System;
using System.Collections;
using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Util
{
	[TestFixture]
	public class WildCardPathTest : CustomAssertion
	{
		private const string TEMP_FOLDER = "WildCardPathTest";
		private string tempFolderFullPath;

		[SetUp]
		public void CreateTempDir()
		{
			tempFolderFullPath = TempFileUtil.CreateTempDir(TEMP_FOLDER);
		}

		[Test]
		public void StringWithNoWildCardsReturnsSingleFile()
		{
			WildCardPath wildCard = new WildCardPath("foo.xml");
			IList files = wildCard.GetFiles();
			Assert.AreEqual(1, files.Count);
		}

		[Test]
		public void InvalidWildCardPathReturnsNoFiles()
		{
			WildCardPath wildCard = new WildCardPath(Path.Combine("nonexistantfolder", "*"));
			IList files = wildCard.GetFiles();
			Assert.AreEqual(0, files.Count);
		}

		[Test]
		public void HandlesWhiteSpaceInTheFileName()
		{
			WildCardPath wildCard = new WildCardPath("fooo.xml    ");
			FileInfo[] files = wildCard.GetFiles();
			Assert.AreEqual(1, files.Length);
			Assert.AreEqual("fooo.xml", files[0].Name);
		}

		[Test]
		public void StringWithWildcardsReturnsAllMatchingFiles()
		{
			string tempFile1Path = TempFileUtil.CreateTempFile(TEMP_FOLDER, "foo.txt", "foofoo");
			string tempFile2Path = TempFileUtil.CreateTempFile(TEMP_FOLDER, "bar.txt", "barbar");
			WildCardPath wildCard = new WildCardPath(Path.Combine(tempFolderFullPath, "*.txt"));
			IList files = wildCard.GetFiles();
			Assert.AreEqual(2, files.Count);
			AssertListContainsPath(files, tempFile2Path);
			AssertListContainsPath(files, tempFile1Path);
		}

		[Test]
		public void StringWithPrefixAndWildcardsReturnsAllMatchingFiles()
		{
			string tempFile1Path = TempFileUtil.CreateTempFile(TEMP_FOLDER, "prefix-foo.txt", "foofoo");
			string tempFile2Path = TempFileUtil.CreateTempFile(TEMP_FOLDER, "prefix-bar.txt", "barbar");
			WildCardPath wildCard = new WildCardPath(Path.Combine(tempFolderFullPath, "prefix-*.txt"));
			IList files = wildCard.GetFiles();
			Assert.AreEqual(2, files.Count);
			AssertListContainsPath(files, tempFile2Path);
			AssertListContainsPath(files, tempFile1Path);
		}

		private void AssertListContainsPath(IList list, string s)
		{
			foreach (FileInfo info in list)
			{
				if (info.FullName == s)
					return;
			}
			Assert.Fail(String.Format("Element {0} not found in the list", s));
		}

		[TearDown]
		public void DeleteTempDir()
		{
			TempFileUtil.DeleteTempDir(TEMP_FOLDER);
		}
	}
}