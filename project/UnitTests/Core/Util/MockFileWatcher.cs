using System;

namespace ThoughtWorks.CruiseControl.Core.Util.Test
{
	public class MockFileWatcher : IFileWatcher
	{
		public event System.IO.FileSystemEventHandler OnFileChanged;

		public void RaiseEvent() { OnFileChanged(null, null); }
		void IDisposable.Dispose() { }
	}
}
