using System.IO;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture, Explicit]
	public class CvsHistoryCommandParserTest : CustomAssertion
	{
		private FileInfo workingDir = new FileInfo("../..");
		private CvsHistoryCommandParser parser;

		private string historyFormatString = "M 2005-06-09 02:58 +0000 user 1.1 li.cpp {0}/lib == <remote>";

		[SetUp]
		protected void Init()
		{
			parser = new CvsHistoryCommandParser();
			parser.WorkingDirectory = workingDir.FullName;
		}

		[Test]
		public void ReadRepositoryFile()
		{
			Assert.IsNotNull(parser.Repository);
		}

		[Test]
		public void GetChangeList()
		{
			string changeString = string.Format("M 2005-06-09 02:45 +0000 user 1.1 file.cpp {0}/lib == <remote>\n"
				+ "M 2005-06-10 23:35 +0000 user2 1.4 file2.cpp {0}/lib == <remote>\n"
				+ "M 2005-06-09 12:58 +0000 user3 1.1 file3.cpp {0}/lib == <remote>\n", parser.Repository);

			string[] changes = parser.ParseOutputFrom(changeString);

			Assert.IsTrue(changes.Length > 0, "Should have detected some changes, but found none");
		}

		[Test]
		public void ParseEntryCannotStartWithDirectorySeparatorChar()
		{
			parser.WorkingDirectory = Path.Combine(workingDir.FullName, "lib");

			string baseRepo = parser.ReadCvsRepositoryDirectory(new DirectoryInfo(workingDir.FullName));
			string entry = string.Format(historyFormatString, baseRepo);

			string result = parser.ParseEntry(entry);

			Assert.IsFalse(result.StartsWith(Path.DirectorySeparatorChar.ToString()), string.Format("Absolute pathname '{0}' not allowed", result));
		}

		/// This test is a duplicate of the one above it other than the assertion
		[Test]
		public void ParseEntryWithChangeInWorkingDirectory()
		{
			parser.WorkingDirectory = Path.Combine(workingDir.FullName, "lib");

			string baseRepo = parser.ReadCvsRepositoryDirectory(new DirectoryInfo(workingDir.FullName));
			string entry = string.Format(historyFormatString, baseRepo);

			Assert.AreEqual(string.Format("{0}/lib", baseRepo), parser.ParseEntry(entry));
		}

		/// This test is a duplicate of the one above it??
		[Test]
		public void ParseEntryDirectoryNameInFile()
		{
			parser.WorkingDirectory = Path.Combine(workingDir.FullName, "lib");

			string baseRepo = parser.ReadCvsRepositoryDirectory(new DirectoryInfo(workingDir.FullName));
			string entry = string.Format(historyFormatString, baseRepo);

			Assert.AreEqual(string.Format("{0}/lib", baseRepo), parser.ParseEntry(entry));
		}
	}
}