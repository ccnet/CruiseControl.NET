using NUnit.Framework;
using System;
using System.IO;
using System.Threading;

namespace ThoughtWorks.CruiseControl.Core.Util.Test
{
	[TestFixture]
	public class FileChangedWatcherTest : Assertion
	{
		private string tempFile;
		private int filechangedCount;
		private ManualResetEvent monitor;

		[SetUp]
		protected void SetUp()
		{
			tempFile = TempFileUtil.CreateTempXmlFile("FileChangedWatcherTest", "foo.xml", "<derek><zoolander/></derek>");
			monitor = new ManualResetEvent(false);
		}

		[TearDown]
		protected void TearDown()
		{
			File.Delete(tempFile);
		}

		[Test]
		public void HandleFileChanged()
		{
			using (FileChangedWatcher watcher = new FileChangedWatcher(tempFile))
			{
				watcher.OnFileChanged += new FileSystemEventHandler(FileChanged);

				UpdateFile("<rob><schneider/></rob>");
				AssertEquals(1, filechangedCount);

				UpdateFile("<joseph><conrad/></joseph");
				AssertEquals(2, filechangedCount);
			}
		}

		private void UpdateFile(string text)
		{
			TempFileUtil.CreateTempXmlFile("FileChangedWatcherTest", "foo.xml", text);
			monitor.WaitOne();
			monitor.Reset();
		}

		private void FileChanged(object sender, FileSystemEventArgs e)
		{
			using (File.OpenWrite(tempFile))
			{
				filechangedCount++;
			}
			monitor.Set();
		}
	}
}
