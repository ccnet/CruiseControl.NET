using System;
using System.IO;
using System.Timers;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class FileChangedWatcher : IFileWatcher
	{
		private FileSystemWatcher watcher;
		private Timer timer;

		public FileChangedWatcher(string filename)
		{
			watcher = new FileSystemWatcher();
			watcher.Filter = Path.GetFileName(filename);
			watcher.Path = new FileInfo(filename).DirectoryName;
			watcher.Changed += new FileSystemEventHandler(HandleFileChanged);

			timer = new Timer(500);
			timer.AutoReset = false;
			timer.Elapsed += new ElapsedEventHandler(HandleTimerElapsed);

			watcher.EnableRaisingEvents = true;
		}

		public event FileSystemEventHandler OnFileChanged;

		private void HandleFileChanged(object sender, FileSystemEventArgs args)
		{
			timer.Start();
		}

		private void HandleTimerElapsed(object sender, ElapsedEventArgs args)
		{
			timer.Stop();
			OnFileChanged(sender, null);
		}

		void IDisposable.Dispose()
		{
			watcher.EnableRaisingEvents = false;
			watcher.Dispose();
		}
	}
}
