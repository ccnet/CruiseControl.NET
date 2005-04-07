using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class PathFilterTest
	{
		[Test]
		public void TestExactFileNameMatch()
		{
			ExactFileNameTestSet.RunTests();
		}

		[Test]
		public void TestAnyFileNameMatch()
		{
			AnyFileNameTestSet.RunTests();
		}

		[Test]
		public void TestExactFolderAnyNameMatch()
		{
			ExactFolderAnyNameTestSet.RunTests();
		}

		[Test]
		public void TestAnyFolderExactNameMatch()
		{
			AnyFolderExactNameTestSet.RunTests();
		}

		[Test]
		public void TestExactSubfolderAnyNameMatch()
		{
			ExactSubfolderAnyNameTestSet.RunTests();
		}

		[Test]
		public void TestAcceptAllMatch()
		{
			AcceptAllTestSet.RunTests();
		}

		[Test]
		public void TestPartialNameMatch() 
		{
			PartialNameTestSet.RunTests();
		}

		[Test]
		public void TestAnyFolderExactExtensionMatch()
		{
			AnyFolderExactExtensionTestSet.RunTests();
		}

		[Test]
		public void TestPartialFolderAnyNameMatch() 
		{
			PartialFolderAnyNameTestSet.RunTests();
		}

		[Test]
		public void TestAnyFolderPartialExtensionMatch()
		{
			AnyFolderPartialExtentsionTestSet.RunTests();
		}

		[Test]
		public void TestPartialPathAnyNameMatch() 
		{
			PartialPathAnyNameTestSet.RunTests();
		}

		private static Modification[] Modifications = 
			{
				ModificationMother.CreateModification("theName.dat", ""),
				ModificationMother.CreateModification("TheName.dat", ""),
				ModificationMother.CreateModification("theName.dat", "/theFolder"),
				ModificationMother.CreateModification("theFile.bin", ""),
				ModificationMother.CreateModification("theFile.bin", "/theFolder"),
				ModificationMother.CreateModification("theName.dat", "/TheFolder"),
				ModificationMother.CreateModification("theName.dat", "/theFolder/theSubFolder"),
				ModificationMother.CreateModification("theName.dat", "/theFolder/theSubFolder/theSubSubFolder"),
				ModificationMother.CreateModification("theName.dat", "/theFolder/theSubFolder/theSubFolder"),
				ModificationMother.CreateModification("theName.dat", "/theFolder/theSubFolder/ThesubFolder"),
				ModificationMother.CreateModification("theName", ""),
				ModificationMother.CreateModification("theName", "/theFolder/theSubFolder"),
				ModificationMother.CreateModification("theName.dav", "\\theFolder\\theSubFolder"),
				ModificationMother.CreateModification("theName.dav", "\\theFolder")
			};

		public static readonly string ExactFileNameFilterXml =
			@"<pathFilter>
                  <pattern>theName.dat</pattern>
              </pathFilter>";

		public static readonly PathFilterTestHelper ExactFileNameTestSet =
			new PathFilterTestHelper(
			ExactFileNameFilterXml, 
			Modifications, 
			new bool[] {true, false, false, false, false, 
						   false, false, false, false, false, 
						   false, false, false, false});

		public static readonly string AnyFileNameFilterXml =
			@"<pathFilter>
			      <pattern>*.*</pattern>
              </pathFilter>";

		public static readonly PathFilterTestHelper AnyFileNameTestSet =
			new PathFilterTestHelper(
			AnyFileNameFilterXml,
			Modifications,
			new bool[] {true, true, false, true, false, 
						   false, false, false, false, false, 
						   true, false, false, false});

		public static string ExactFolderAnyNameFilterXml =
			@"<pathFilter>
                  <pattern>/theFolder/*.*</pattern>
              </pathFilter>";

		public static readonly PathFilterTestHelper ExactFolderAnyNameTestSet =
			new PathFilterTestHelper(
			ExactFolderAnyNameFilterXml,
			Modifications,
			new bool[] {false, false, true, false, true, 
						   false, false, false, false, false, 
						   false, false, false, true});

		public static string AnyFolderExactNameFilterXml =
			@"<pathFilter>
                  <pattern>**/theName.dat</pattern>
              </pathFilter>";

		public static readonly PathFilterTestHelper AnyFolderExactNameTestSet =
			new PathFilterTestHelper(
			AnyFolderExactNameFilterXml,
			Modifications,
			new bool[] {true, false, true, false, false, 
						   true, true, true, true, true, 
						   false, false, false, false});

		public static string ExactSubfolderAnyNameFilterXml = 
			@"<pathFilter>
                  <pattern>**/theSubFolder/*.*</pattern>
              </pathFilter>";

		public static readonly PathFilterTestHelper ExactSubfolderAnyNameTestSet =
			new PathFilterTestHelper(
			ExactSubfolderAnyNameFilterXml,
			Modifications,
			new bool[] {false, false, false, false, false, 
						   false, true, false, true, false, 
						   false, true, true, false});

		public static string AcceptAllFilterXml =
			@"<pathFilter>
                  <pattern>**/*.*</pattern>
              </pathFilter>"; 

		public static readonly PathFilterTestHelper AcceptAllTestSet =
			new PathFilterTestHelper(
			AcceptAllFilterXml,
			Modifications,
			new bool[] {true, true, true, true, true, 
						   true, true, true, true, true, 
						   true, true, true, true});

		public static string PartialNameFilterXml =
			@"<pathFilter>
                  <pattern>**/the*.dat</pattern>
              </pathFilter>";

		public static readonly PathFilterTestHelper PartialNameTestSet =
			new PathFilterTestHelper(
			PartialNameFilterXml,
			Modifications,
			new bool[] {true, false, true, false, false, 
						   true, true, true, true, true, 
						   false, false, false, false});

		public static string AnyFolderExactExtensionFilterXml =
			@"<pathFilter>
                  <pattern>**/*.bin</pattern>
              </pathFilter>";

		public static readonly PathFilterTestHelper AnyFolderExactExtensionTestSet =
			new PathFilterTestHelper(
			AnyFolderExactExtensionFilterXml,
			Modifications,
			new bool[] {false, false, false, true, true, 
						   false, false, false, false, false, 
						   false, false, false, false});

		public static string PartialFolderAnyNameFilterXml =
			@"<pathFilter>
                  <pattern>**/the*Folder/*.*</pattern>
              </pathFilter>";

		public static readonly PathFilterTestHelper PartialFolderAnyNameTestSet =
			new PathFilterTestHelper(
			PartialFolderAnyNameFilterXml,
			Modifications,
			new bool[] {false, false, true, false, true, 
						   false, true, true, true, false, 
						   false, true, true, true});

		public static string AnyFolderPartialExtensionFilterXml =
			@"<pathFilter>
                  <pattern>**/*.da*</pattern>
              </pathFilter>";

		public static PathFilterTestHelper AnyFolderPartialExtentsionTestSet =
			new PathFilterTestHelper(
			AnyFolderPartialExtensionFilterXml,
			Modifications,
			new bool[] {true, true, true, false, false,
						   true, true, true, true, true,
						   false, false, true, true});

		public static string PartialPathAnyNameFilterXml =
			@"<pathFilter>
                  <pattern>/theFolder/**/*.*</pattern>
              </pathFilter>";

		public static PathFilterTestHelper PartialPathAnyNameTestSet =
			new PathFilterTestHelper(
			PartialPathAnyNameFilterXml,
			Modifications,
			new bool[] {false, false, true, false, true,
						   false, true, true, true, true,
						   false, true, true, true});
	}

	public class PathFilterTestHelper 
	{
		private PathFilter _filter;
		private Modification[] _modifications;
		private bool[] _expectedResults;

		public PathFilterTestHelper(string xmlFragment,
			Modification[] modifications,
			bool[] expectedResults) 
		{
			_filter = new PathFilter();
			NetReflector.Read(xmlFragment, _filter);

			_modifications = modifications;
			_expectedResults = expectedResults;
		}

		public void RunTests() 
		{
			int i = 0;

			foreach (Modification m in _modifications) 
			{
				Assert.IsTrue(_filter.Accept(m) == _expectedResults[i], 
					DescribeExpectedResult(m, _expectedResults[i]));
				i++;
			}
		}

		private string DescribeExpectedResult(
			Modification m,
			bool expectedResult) 
		{
			return string.Format("{0}/{1} should be {2}.",
				m.FolderName,
				m.FileName,
				expectedResult ? "accepted" : "rejected");
		}
	}
}