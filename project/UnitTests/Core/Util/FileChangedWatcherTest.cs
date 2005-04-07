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
			tempFile = TempFileUtil.CreateTempXmlFile("FileChangedWatcherTest", "foo.xml", "<derek><zoolander/></derek>");
			monitor = new ManualResetEvent(false);
		}

		[TearDown]
		protected void TearDown()
		{
			TempFileUtil.DeleteTempDir("FileChangedWatcherTest");
		}

		[Test]
		public void HandleFileChanged()
		{
			using (FileChangedWatcher watcher = new FileChangedWatcher(tempFile))
			{
				watcher.OnFileChanged += new FileSystemEventHandler(FileChanged);

				UpdateFile("<rob><schneider/></rob>");
				Assert.AreEqual(1, filechangedCount);

				UpdateFile("<joseph><conrad/></joseph");
				Assert.AreEqual(2, filechangedCount);
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
