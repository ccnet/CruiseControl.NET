using System;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Util
{
	public class MockFileWatcher : IFileWatcher
	{
		public event System.IO.FileSystemEventHandler OnFileChanged;

		public void RaiseEvent() { OnFileChanged(null, null); }
		void IDisposable.Dispose() { }
	}
}
