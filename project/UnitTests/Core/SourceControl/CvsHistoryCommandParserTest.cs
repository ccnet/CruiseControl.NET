using System;
using System.IO;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol
{
	[TestFixture]
	public class CvsHistoryCommandParserTest : CustomAssertion
	{
		private FileInfo workingDir = new FileInfo("../..");
		private FileInfo cvsExe = new FileInfo("cvs.exe");
		private IMock executorMock;
		private CvsHistoryCommandParser parser;

		private string historyFormatString = "M 2005-06-09 02:58 +0000 user 1.1 li.cpp {0}/lib == <remote>";

		[SetUp]
		protected void Init()
		{
			executorMock = new DynamicMock(typeof (ProcessExecutor));
			parser = new CvsHistoryCommandParser((ProcessExecutor) executorMock.MockInstance, cvsExe.Name, workingDir.FullName);
		}

		[Test]
		public void ReadRepositoryFile()
		{
			string repositoryPath = parser.Repository;
			Assert.IsNotNull(repositoryPath);
		}

		[Test]
		public void GetChangeList()
		{
			DateTime from = new DateTime(2001, 1, 21, 20, 0, 0);
			string changeString = string.Format("M 2005-06-09 02:45 +0000 user 1.1 file.cpp {0}/lib == <remote>\n"
				+ "M 2005-06-10 23:35 +0000 user2 1.4 file2.cpp {0}/lib == <remote>\n"
				+ "M 2005-06-09 12:58 +0000 user3 1.1 file3.cpp {0}/lib == <remote>\n", parser.Repository);
			ProcessResult result = new ProcessResult(changeString, "", 0, false);

			executorMock.ExpectAndReturn("Execute", result, new IsAnything());

			string[] changes = parser.GetDirectoriesContainingChanges(from);

			Assert.IsTrue(changes.Length > 0, "Should have detected some changes, but found none");
		}

		[Test]
		public void GetChangeListLocalOnly()
		{
			DateTime from = new DateTime(2001, 1, 21, 20, 0, 0);
			parser.LocalOnly = true;
			string[] changes = parser.GetDirectoriesContainingChanges(from);

			Assert.IsTrue(changes.Length == 1, "Should have only one change");

			string actual = changes[0];
			string expected = ".";
			Assert.IsTrue(expected.Equals(actual), string.Format("Expected: {0}, but was: {1}", expected, actual));
		}

		[Test]
		public void ParseEntryCannotStartWithDirectorySeparatorChar()
		{
			string testRepo = Path.Combine(workingDir.FullName, "lib");
			parser.WorkingDirectory = testRepo;

			string baseRepo = parser.GetRepository(new DirectoryInfo(workingDir.FullName));
			string entry = string.Format(historyFormatString, baseRepo);

			string result = parser.ParseEntry(entry);

			Assert.IsFalse(result.StartsWith(Path.DirectorySeparatorChar.ToString()), string.Format("Absolute pathname '{0}' not allowed", result));
		}

		[Test]
		public void ParseEntryWithChangeInWorkingDirectory()
		{
			string testRepo = Path.Combine(workingDir.FullName, "lib");
			parser.WorkingDirectory = testRepo;

			string baseRepo = parser.GetRepository(new DirectoryInfo(workingDir.FullName));
			string entry = string.Format(historyFormatString, baseRepo);

			string result = parser.ParseEntry(entry);
			string expected = string.Format("{0}/lib", baseRepo);

			Assert.IsTrue(result.Equals(expected), string.Format("Expected: {0}, but was: {1}", expected, result));
		}

		[Test]
		public void ParseEntryDirectoryNameInFile()
		{
			string testRepo = Path.Combine(workingDir.FullName, "lib");
			parser.WorkingDirectory = testRepo;

			string baseRepo = parser.GetRepository(new DirectoryInfo(workingDir.FullName));
			string entry = string.Format(historyFormatString, baseRepo);

			string result = parser.ParseEntry(entry);
			string expected = string.Format("{0}/lib", baseRepo);

			Assert.IsTrue(result.Equals(expected), string.Format("Expected: {0}, but was: {1}", expected, result));
		}
	}
}