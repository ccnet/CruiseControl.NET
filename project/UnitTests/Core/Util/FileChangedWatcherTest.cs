using System.IO;
using System.Threading;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Util
{
	[TestFixture]
	public class FileChangedWatcherTest
	{
		private string tempFile;
		private int filechangedCount;
		private ManualResetEvent monitor;

		[SetUp]
		protected void SetUp()
		{
			TempFileUtil.CreateTempDir("FileChangedWatcherTest");
			monitor = new ManualResetEvent(false);
			filechangedCount = 0;
		}

		[TearDown]
		protected void TearDown()
		{
			TempFileUtil.DeleteTempDir("FileChangedWatcherTest");
		}

		[Test]
		public void HandleFileChanged()
		{
			tempFile = TempFileUtil.CreateTempXmlFile("FileChangedWatcherTest", "foo.xml", "<derek><zoolander/></derek>");
			using (FileChangedWatcher watcher = new FileChangedWatcher(tempFile))
			{
				watcher.OnFileChanged += new FileSystemEventHandler(FileChanged);

				UpdateFile("<rob><schneider/></rob>");
				Assert.AreEqual(1, filechangedCount);

				UpdateFile("<joseph><conrad/></joseph");
				Assert.AreEqual(2, filechangedCount);
			}
		}

		[Test]
		public void HandleFileMove()
		{
			tempFile = TempFileUtil.GetTempFilePath("FileChangedWatcherTest", "foo.xml");
			using (FileChangedWatcher watcher = new FileChangedWatcher(tempFile))
			{
				watcher.OnFileChanged += new FileSystemEventHandler(FileChanged);

				string file = TempFileUtil.CreateTempXmlFile("FileChangedWatcherTest", "bar.xml", "<adam><sandler /></adam>");
				new FileInfo(file).MoveTo(tempFile);

				Assert.IsTrue(monitor.WaitOne(5000, false));
				monitor.Reset();
				Assert.AreEqual(1, filechangedCount);
			}
		}

		[Test]
		public void DirectoryNameOnDifferentDrive()
		{
			Assert.AreEqual("e:\\foo", new FileInfo("e:\\foo\\ccnet.config").DirectoryName);
		}

		private void UpdateFile(string text)
		{
			TempFileUtil.CreateTempXmlFile("FileChangedWatcherTest", "foo.xml", text);
			Assert.IsTrue(monitor.WaitOne(5000, false));
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
