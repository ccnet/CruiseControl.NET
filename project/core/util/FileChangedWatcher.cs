using System;
using System.IO;
using System.Timers;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class FileChangedWatcher : IFileWatcher
	{
		private FileSystemWatcher _watcher;
		private Timer _timer;

		public FileChangedWatcher(string filename)
		{
			_watcher = new FileSystemWatcher();
			_watcher.Filter = Path.GetFileName(filename);
			_watcher.Path = new FileInfo(filename).DirectoryName;
			_watcher.NotifyFilter = NotifyFilters.LastWrite;
			_watcher.Changed += new FileSystemEventHandler(HandleFileChanged);

			_timer = new Timer(500);
			_timer.AutoReset = false;
			_timer.Elapsed += new ElapsedEventHandler(HandleTimerElapsed);

			_watcher.EnableRaisingEvents = true;
		}

		public event FileSystemEventHandler OnFileChanged;

		private void HandleFileChanged(object sender, FileSystemEventArgs args)
		{
			_timer.Start();
		}

		private void HandleTimerElapsed(object sender, ElapsedEventArgs args)
		{
			_timer.Stop();
			OnFileChanged(sender, null);
		}

		void IDisposable.Dispose()
		{
			_watcher.EnableRaisingEvents = false;
			_watcher.Dispose();
		}
	}
}
