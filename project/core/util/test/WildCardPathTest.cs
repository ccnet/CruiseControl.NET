using System;
using System.Collections;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Util.Test
{
	[TestFixture]
	public class WildCardPathTest : CustomAssertion
	{
		private const string TEMP_FOLDER="WildCardPathTest";
		private string _tempFolderFullPath;
		[SetUp]
			public void CreateTempDir()
		{
		    _tempFolderFullPath = TempFileUtil.CreateTempDir(TEMP_FOLDER);
		}

		[Test]
		public void StringWithNoWildCardsReturnsSingleFile()
		{
			WildCardPath wildCard = new WildCardPath("foo.xml");
			IList files=wildCard.GetFiles();
			AssertEquals(1, files.Count);
		}
		[Test]
		
		public void StringWithWildcardsReturnsAllMatchingFiles()
		{
		    string tempFile1Path = TempFileUtil.CreateTempFile(TEMP_FOLDER,"foo.txt","foofoo");
		    string tempFile2Path = TempFileUtil.CreateTempFile(TEMP_FOLDER,"bar.txt","barbar");
			string tempFile3Path = TempFileUtil.CreateTempFile(TEMP_FOLDER,"bat.bat","batbat");
			WildCardPath wildCard = new WildCardPath(_tempFolderFullPath+@"\"+"*.txt");
			IList files=wildCard.GetFiles();
			AssertEquals(2, files.Count);
			AssertListContainsPath(files,tempFile2Path);
			AssertListContainsPath(files,tempFile1Path);
		}

	    void AssertListContainsPath(IList list,string s)
	    {
			foreach( FileInfo info in list)
			{
			    if(info.FullName== s)return;
			}
			Fail(String.Format("Element {0} not found in the list",s));

		}


		[TearDown]
		public void DeleteTempDir()
		{
			TempFileUtil.DeleteTempDir(TEMP_FOLDER);
		}
	}
}
