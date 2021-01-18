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

            //create mock files
            string rootLevelPath = TempFileUtil.CreateTempDir(Path.Combine(tempFolderFullPath, "RootLevel"));
            TempFileUtil.CreateTempFile(rootLevelPath, "FileB.txt");

            string firstLevelAPath = TempFileUtil.CreateTempDir(Path.Combine(rootLevelPath, "FirstLevelA"));
            TempFileUtil.CreateTempFile(firstLevelAPath, "FileA.txt");
            string firstLevelASecondLevelAPath = TempFileUtil.CreateTempDir(Path.Combine(firstLevelAPath, "SecondLevelA"));
            string firstLevelAThirdLevelAPath = TempFileUtil.CreateTempDir(Path.Combine(firstLevelASecondLevelAPath, "ThirdLevelA"));
            TempFileUtil.CreateTempFile(firstLevelAThirdLevelAPath, "FileA.txt");

            string firstLevelBPath = TempFileUtil.CreateTempDir(Path.Combine(rootLevelPath, "FirstLevelB"));
            TempFileUtil.CreateTempFile(firstLevelBPath, "FileB.txt");
            string firstLevelBSecondLevelAPath = TempFileUtil.CreateTempDir(Path.Combine(firstLevelBPath, "SecondLevelA"));
            TempFileUtil.CreateTempFile(firstLevelBSecondLevelAPath, "FileA.txt");
            TempFileUtil.CreateTempFile(firstLevelBSecondLevelAPath, "FileB.txt");
            string firstLevelBSecondLevelAThirdLevelAPath = TempFileUtil.CreateTempDir(Path.Combine(firstLevelBSecondLevelAPath, "ThirdLevelA"));
            TempFileUtil.CreateTempFile(firstLevelBSecondLevelAThirdLevelAPath, "FileA.txt");
            TempFileUtil.CreateTempFile(firstLevelBSecondLevelAThirdLevelAPath, "FileB.txt");
            string firstLevelBSecondLevelBPath = TempFileUtil.CreateTempDir(Path.Combine(firstLevelBPath, "SecondLevelB"));
            TempFileUtil.CreateTempFile(firstLevelBSecondLevelBPath, "FileA.txt");
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
            Assert.Fail(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Element {0} not found in the list", s));
        }

        [Test]
        public void StringWithWildcardsInPathShouldReturnOneTxtFileInsideRootLevel()
        {
            WildCardPath wildCard = new WildCardPath(Path.Combine(tempFolderFullPath, "RootLevel", "**", "FileA.txt"));

            FileInfo[] fileMatches = wildCard.GetFiles();

            Assert.AreEqual(5, fileMatches.Length);
        }

        [Test]
        public void StringWithWildcardsInPathShouldGetAllTxtFilesInsideRootLevel()
        {
            WildCardPath wildCard = new WildCardPath(Path.Combine(tempFolderFullPath, "RootLevel", "**", "*.txt"));

            FileInfo[] fileMatches = wildCard.GetFiles();

            Assert.AreEqual(9, fileMatches.Length);
        }

        [Test]
        public void StringWithWildcardsInPathShouldUseWildcardsFollowedByFolderNameSegment()
        {
            WildCardPath wildCard = new WildCardPath(Path.Combine(new string [] {tempFolderFullPath, "RootLevel", "**", "ThirdLevelA", "*.txt"}));
            FileInfo[] fileMatches = wildCard.GetFiles();

            Assert.AreEqual(3, fileMatches.Length);
        }

        [Test]
        public void StringWithWildcardsInPathShouldUseWildcardsInTwoFolderSegments()
        {
            WildCardPath wildCard = new WildCardPath(Path.Combine(new string [] {tempFolderFullPath, "RootLevel", "**", "SecondLevelA", "**", "*.txt"}));

            FileInfo[] fileMatches = wildCard.GetFiles();

            Assert.AreEqual(5, fileMatches.Length);
        }

        [Test]
        public void StringWithWildcardsInPathShouldUseFolderWildcardsAndSimpleWildcardForAFolderName()
        {
            WildCardPath wildCard = new WildCardPath(Path.Combine(new string [] {tempFolderFullPath, "RootLevel", "**", "SecondLevelA", "Thir*", "*.txt"}));

            FileInfo[] fileMatches = wildCard.GetFiles();

            Assert.AreEqual(3, fileMatches.Length);
        }

        [TearDown]
        public void DeleteTempDir()
        {
            TempFileUtil.DeleteTempDir(TEMP_FOLDER);
        }
    }
}