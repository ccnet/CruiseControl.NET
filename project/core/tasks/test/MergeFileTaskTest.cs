using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;
using System.IO;
using System.Collections;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Tasks.Test
{
    [TestFixture]
    public class MergeFileTaskTest : CustomAssertion
    {
        private const string TEMP_DIR = "MergeFileTaskTest";
		private IntegrationResult _result; 
		private MergeFilesTask _task;
		private string _fullPathToTempDir;

        [SetUp]
        public void CreateTempDir()
        {
            _fullPathToTempDir= TempFileUtil.CreateTempDir(TEMP_DIR);
            _task = new MergeFilesTask();
            _result= new IntegrationResult();
        }

        [Test]
        public void StoreMergeFilesInIntegrationResult()
        {
			string fileData = @"<foo></foo>";
			string tempFile = TempFileUtil.CreateTempFile(TEMP_DIR, "MergeFileTask", fileData);

            _task.MergeFiles.Add(tempFile);
            _task.Run(_result);

			AssertEquals(1, _result.TaskResults.Count);
            ITaskResult taskResult = (ITaskResult) _result.TaskResults[0];
            AssertEquals(fileData, taskResult.Data);

        }

		[Test]
		public void ResolveWildCards()
		{
			String fileData = "<foo bar=\"4\">bat</foo>";
			TempFileUtil.CreateTempDir(TEMP_DIR + "\\sub");
			TempFileUtil.CreateTempXmlFile(TEMP_DIR, "foo.xml", fileData);
			TempFileUtil.CreateTempFile(TEMP_DIR, "foo.bat", "blah");
			TempFileUtil.CreateTempXmlFile(TEMP_DIR + "\\sub", "foo.xml", "<foo bar=\"9\">bat</foo>");

		    _task.MergeFiles.Add(_fullPathToTempDir+ @"\*.xml");
		    _task.Run(_result);
			IList list = _result.TaskResults;
			AssertEquals(1, list.Count);
			AssertEquals(fileData, ((ITaskResult) list[0]).Data);
		}

		[Test]
		public void ResolveWildCardsForMoreThanOneMergeFiles()
		{
			string fooXmlFileData = "<foo bar=\"4\">bat</foo>";
			string fooBatFileData=@"<bat></bat>";
			string subFooXmlFileData="<foo bar=\"9\">bat</foo>";

			TempFileUtil.CreateTempDir(TEMP_DIR + @"\sub");
			TempFileUtil.CreateTempXmlFile(TEMP_DIR, "foo.xml", fooXmlFileData);
			TempFileUtil.CreateTempFile(TEMP_DIR, "foo.bat", fooBatFileData);
			TempFileUtil.CreateTempXmlFile(TEMP_DIR + @"\sub", "foo.xml", subFooXmlFileData);

			_task.MergeFiles.Add(_fullPathToTempDir+@"\sub"+ @"\*.xml");
			_task.MergeFiles.Add(_fullPathToTempDir+ @"\foo.*");

			_task.Run(_result);
			IList list = _result.TaskResults;
			AssertEquals(3, list.Count);
			AssertDataContainedInList(list,fooXmlFileData);
			AssertDataContainedInList(list,fooBatFileData);
			AssertDataContainedInList(list,subFooXmlFileData);
		}

		[Test]
		public void LoadFromConfig()
		{
			string xml=@"<mergefiles><files><file>foo.xml</file> <file>bar.xml</file> </files> </mergefiles>";
			MergeFilesTask task = NetReflector.Read(xml) as MergeFilesTask;
			AssertEquals(2,task.MergeFiles.Count);
			AssertEquals("foo.xml",task.MergeFiles[0]);
			AssertEquals("bar.xml",task.MergeFiles[1]);
		}

        private void AssertDataContainedInList(IList list, string data)
        { 
			foreach( ITaskResult result in list)
			{
				if(result.Data == data)
					return;
			}
			Fail(data+" not found in the list");
        }

        [TearDown]
        public void DeleteTempDir()
        {
            TempFileUtil.DeleteTempDir(TEMP_DIR);
        }
    }
}