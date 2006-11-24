using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Util
{
	[TestFixture]
	public class PathUtilTest
	{
		private static readonly string[] names = {
		                                         	"theName.dat", "TheName.dat",
		                                         	"/theFolder/theName.dat", "theFile.bin",
		                                         	"/theFolder/theFile.bin", "/TheFolder/theName.dat"
		                                         	, "/theFolder/theSubFolder/theName.dat",
		                                         	"/theFolder/theSubFolder/theSubSubFolder/theName.dat"
		                                         	,
		                                         	"/theFolder/theSubFolder/theSubFolder/theName.dat"
		                                         	,
		                                         	"/theFolder/theSubFolder/ThesubFolder/theName.dat"
		                                         	, "theName", "/theFolder/theSubFolder/theName",
		                                         	"\\theFolder\\theSubFolder/theName.dav",
		                                         	"\\theFolder/theName.dav"
		                                         };

		private static readonly int[] namesTokenLen = {1, 1, 2, 1, 2, 2, 3, 4, 4, 4, 1, 3, 3, 2};

		[Test]
		public void TheFolderCaseInsensitive()
		{
			new TestCase("/thefolder/**/*", false, names,
			             new bool[]
			             	{
			             		false, false, true, false, true, true, true, true, true, true, false,
			             		true, true, true
			             	}).RunTest();
		}

		[Test]
		public void AnySubFolder()
		{
			new TestCase("**/theSubFolder/**/*", true, names,
			             new bool[]
			             	{
			             		false, false, false, false, false, false, true, true, true, true,
			             		false, true, true, false
			             	}).RunTest();
		}

		[Test]
		public void SingleCharacterInName()
		{
			new TestCase("**/the???e.da?", true, names,
			             new bool[]
			             	{
			             		true, false, true, false, false, true, true, true, true, true, false,
			             		false, true, true
			             	}).RunTest();
		}

		[Test]
		public void CaseInsensitiveMatch()
		{
			Assert.IsTrue(PathUtils.Match("thename", "TheName", false));
		}

		[Test]
		public void CaseInsensitiveMisMatch()
		{
			Assert.IsFalse(PathUtils.Match("thefile", "TheName", false));
		}

		[Test]
		public void CaseInsensitiveStarMismatch()
		{
			Assert.IsFalse(PathUtils.Match("the*", "ratBone", false));
		}

		[Test]
		public void MatchWithStarAtEnd()
		{
			Assert.IsTrue(PathUtils.Match("thename*", "thename", false));
		}

		[Test]
		public void MisMatchWithMatchBeforeStar()
		{
			Assert.IsFalse(PathUtils.Match("thename*x", "thename", false));
		}

		[Test]
		public void MisMatchWithStar()
		{
			Assert.IsFalse(PathUtils.Match("the*name", "thexfile", false));
		}

		[Test]
		public void MatchWithExhaustingStrings()
		{
			Assert.IsTrue(PathUtils.Match("the*name*back", "thexnameisBack", false));
		}

		[Test]
		public void MatchMiddleStar()
		{
			Assert.IsFalse(PathUtils.Match("t*a*x", "tx", false));
		}

		[Test]
		public void MatchTwoStars()
		{
			Assert.IsTrue(PathUtils.Match("t**a*x", "txax", false));
		}

		[Test]
		public void MisMatchWithTwoStarsExhaustedStrings()
		{
			Assert.IsFalse(PathUtils.Match("t*ay*x", "txax", false));
		}

		[Test]
		public void MatchStarStarStarStar()
		{
			Assert.IsTrue(PathUtils.MatchPath("**/**/*", "/f/dat", false));
		}

		[Test]
		public void MismatchExhaustedStrings()
		{
			Assert.IsFalse(PathUtils.Match("the*name*back", "thexnamisBack", false));
		}

		[Test]
		public void MismatchStringExhausted()
		{
			Assert.IsFalse(PathUtils.MatchPath("/f/dat/**/x", "/f/dat", false));
		}

		[Test]
		public void MismatchExhaustedStringNoStarAtEnd()
		{
			Assert.IsFalse(PathUtils.Match("the*name*backxx", "thexnameisBack", false));
		}

		[Test]
		public void TokenizeTest()
		{
			for (int i = 0; i < names.Length; i++)
			{
				Assert.AreEqual(namesTokenLen[i], PathUtils.SplitPath(names[i]).Length);
			}
		}

		[Test]
		public void TokenizeComplexTest()
		{
			string[] r = PathUtils.SplitPath(names[7]);
			Assert.AreEqual(4, r.Length);
			Assert.AreEqual("theFolder", r[0]);
			Assert.AreEqual("theSubFolder", r[1]);
			Assert.AreEqual("theSubSubFolder", r[2]);
			Assert.AreEqual("theName.dat", r[3]);
		}

		[Test]
		public void TokenizeFunnySlants()
		{
			string[] r = PathUtils.SplitPath("\\theFolder/theName.dav");
			Assert.AreEqual(2, r.Length);
			Assert.AreEqual("theFolder", r[0]);
			Assert.AreEqual("theName.dav", r[1]);
		}

		[Test]
		public void FileNameMatch()
		{
			new TestCase("theName.dat", true, names,
			             new bool[]
			             	{
			             		true, false, false, false, false, false, false, false, false, false,
			             		false, false, false, false
			             	}).RunTest();
		}

		[Test]
		public void AnyFileNameMatch()
		{
			new TestCase("*", true, names,
			             new bool[]
			             	{
			             		true, true, false, true, false, false, false, false, false, false,
			             		true, false, false, false
			             	}).RunTest();
		}

		[Test]
		public void ExactFolderAnyFile()
		{
			new TestCase("/theFolder/*.*", true, names,
			             new bool[]
			             	{
			             		false, false, true, false, true, false, false, false, false, false,
			             		false, false, false, true
			             	}).RunTest();
		}

		[Test]
		public void AnyFolderExactFolder()
		{
			new TestCase("**/theName.dat", true, names,
			             new bool[]
			             	{
			             		true, false, true, false, false, true, true, true, true, true, false,
			             		false, false, false
			             	}).RunTest();
		}

		[Test]
		public void ExactSubFolderAnyFile()
		{
			new TestCase("**/theSubFolder/*.*", true, names,
			             new bool[]
			             	{
			             		false, false, false, false, false, false, true, false, true, false,
			             		false, true, true, false
			             	}).RunTest();
		}

		[Test]
		public void AcceptAll()
		{
			new TestCase("**/*.*", true, names,
			             new bool[]
			             	{
			             		true, true, true, true, true, true, true, true, true, true, true, true
			             		, true, true
			             	}).RunTest();
		}

		[Test]
		public void PartialFileNames()
		{
			new TestCase("**/the*.dat", true, names,
			             new bool[]
			             	{
			             		true, false, true, false, false, true, true, true, true, true, false,
			             		false, false, false
			             	}).RunTest();
		}

		[Test]
		public void SpecificExtensionInAnyFolder()
		{
			new TestCase("**/*.bin", true, names,
			             new bool[]
			             	{
			             		false, false, false, true, true, false, false, false, false, false,
			             		false, false, false, false
			             	}).RunTest();
		}

		[Test]
		public void PartialFolderAnyFile()
		{
			new TestCase("**/the*Folder/*.*", true, names,
			             new bool[]
			             	{
			             		false, false, true, false, true, false, true, true, true, false, false
			             		, true, true, true
			             	}).RunTest();
		}

		[Test]
		public void AnyFolderPartialExtension()
		{
			new TestCase("**/*.da*", true, names,
			             new bool[]
			             	{
			             		true, true, true, false, false, true, true, true, true, true, false,
			             		false, true, true
			             	}).RunTest();
		}

		[Test]
		public void PathPrefixAnyFolder()
		{
			new TestCase("/theFolder/**/*.*", true, names,
			             new bool[]
			             	{
			             		false, false, true, false, true, false, true, true, true, true, false,
			             		true, true, true
			             	}).RunTest();
		}

		[Test]
		public void SingledOutFunnySlants()
		{
			Assert.IsTrue(PathUtils.MatchPath("/theFolder/*", "\\theFolder/theName.dav", true));
		}

		[Test]
		public void SingledOutStarStarNoExtension()
		{
			Assert.IsTrue(PathUtils.Match("*.*", "theName", true));
			Assert.IsTrue(PathUtils.Match("*.*", "theName.dat", true));
			Assert.IsTrue(PathUtils.Match("*", "theName", true));
			Assert.IsTrue(PathUtils.Match("*", "theName.dat", true));
		}
	}

	internal class TestCase
	{
		private string pattern;
		private string[] strs;
		private bool caseSensitive;
		private bool[] expected;

		public TestCase(string pattern, bool caseSensitive, string[] strs, bool[] expected)
		{
			this.pattern = pattern;
			this.strs = strs;
			this.caseSensitive = caseSensitive;
			this.expected = expected;
		}

		public void RunTest()
		{
			Assert.AreEqual(strs.Length, expected.Length);
			for (int i = 0; i < strs.Length; i++)
			{
				if (PathUtils.MatchPath(pattern, strs[i], caseSensitive) != expected[i])
				{
					Assert.Fail(
						String.Format("[{4}] pattern={0} str={1} caseSensitive={2} expected={3}",
						              pattern, strs[i], caseSensitive, expected[i], i));
				}
			}
		}
	}
}