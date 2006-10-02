using System;
using System.Collections;
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Tasks
{
	[TestFixture]
	public class MergeFileTaskTest : CustomAssertion
	{
		private const string TEMP_DIR = "MergeFileTaskTest";
		private IntegrationResult result;
		private MergeFilesTask task;
		private string fullPathToTempDir;

		[SetUp]
		public void CreateTempDir()
		{
			fullPathToTempDir = TempFileUtil.CreateTempDir(TEMP_DIR);
			task = new MergeFilesTask();
			result = new IntegrationResult();
		}

		[Test]
		public void StoreMergeFilesInIntegrationResult()
		{
			string fileData = @"<foo></foo>";
			string tempFile = TempFileUtil.CreateTempFile(TEMP_DIR, "MergeFileTask", fileData);

			task.MergeFiles = new string[] {tempFile};
			task.Run(result);

			Assert.AreEqual(fileData, result.TaskOutput);
		}

		[Test]
		public void ResolveWildCards()
		{
			String fileData = "<foo bar=\"4\">bat</foo>";
			TempFileUtil.CreateTempDir(TEMP_DIR + "\\sub");
			TempFileUtil.CreateTempXmlFile(TEMP_DIR, "foo.xml", fileData);
			TempFileUtil.CreateTempFile(TEMP_DIR, "foo.bat", "blah");
			TempFileUtil.CreateTempXmlFile(TEMP_DIR + "\\sub", "foo.xml", "<foo bar=\"9\">bat</foo>");

			task.MergeFiles = new string[] {fullPathToTempDir + @"\*.xml"};
			task.Run(result);
			Assert.AreEqual(fileData, result.TaskOutput);
		}

		[Test]
		public void UseProjectWorkingDirectoryAsPrefixIfMergeFileLocationIsRelative()
		{
			String fileData = "<foo bar=\"4\">bat</foo>";
			TempFileUtil.CreateTempDir(TEMP_DIR + "\\sub");
			TempFileUtil.CreateTempXmlFile(TEMP_DIR, "foo.xml", fileData);
			TempFileUtil.CreateTempFile(TEMP_DIR, "foo.bat", "blah");
			TempFileUtil.CreateTempXmlFile(TEMP_DIR + "\\sub", "foo.xml", "<foo bar=\"9\">bat</foo>");

			result.WorkingDirectory = fullPathToTempDir;
			task.MergeFiles = new string[] {@"*.xml"};
			task.Run(result);
			Assert.AreEqual(fileData, result.TaskOutput);
		}

		[Test]
		public void ResolveWildCardsForMoreThanOneMergeFiles()
		{
			string fooXmlFileData = "<foo bar=\"4\">bat</foo>";
			string fooBatFileData = @"<bat></bat>";
			string subFooXmlFileData = "<foo bar=\"9\">bat</foo>";

			TempFileUtil.CreateTempDir(TEMP_DIR + @"\sub");
			TempFileUtil.CreateTempXmlFile(TEMP_DIR, "foo.xml", fooXmlFileData);
			TempFileUtil.CreateTempFile(TEMP_DIR, "foo.bat", fooBatFileData);
			TempFileUtil.CreateTempXmlFile(TEMP_DIR + @"\sub", "foo.xml", subFooXmlFileData);

			task.MergeFiles = new string[]
				{
					fullPathToTempDir + @"\sub" + @"\*.xml",
					fullPathToTempDir + @"\foo.*"
				};

			task.Run(result);
			IList list = result.TaskResults;
			Assert.AreEqual(3, list.Count);
			AssertDataContainedInList(list, fooXmlFileData);
			AssertDataContainedInList(list, fooBatFileData);
			AssertDataContainedInList(list, subFooXmlFileData);
		}

		[Test]
		public void IgnoresFilesNotFound()
		{
			task.MergeFiles = new string[] {@"c:\nonExistantFile.txt"};
			task.Run(result);
			Assert.AreEqual(string.Empty, result.TaskOutput);
		}

		[Test]
		public void LoadFromConfig()
		{
			string xml = @"<merge><files><file>foo.xml</file><file>bar.xml</file></files></merge>";
			task = NetReflector.Read(xml) as MergeFilesTask;
			Assert.AreEqual(2, task.MergeFiles.Length);
			Assert.AreEqual("foo.xml", task.MergeFiles[0]);
			Assert.AreEqual("bar.xml", task.MergeFiles[1]);
		}

		private void AssertDataContainedInList(IList list, string data)
		{
			foreach (ITaskResult taskResult in list)
			{
				if (taskResult.Data == data)
					return;
			}
			Assert.Fail(data + " not found in the list");
		}

		[TearDown]
		public void DeleteTempDir()
		{
			TempFileUtil.DeleteTempDir(TEMP_DIR);
		}
	}
}