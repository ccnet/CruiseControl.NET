using System;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Util
{
	public class MockFileWatcher : IFileWatcher
	{
		public event FileSystemEventHandler OnFileChanged;

		public void RaiseEvent() { OnFileChanged(null, null); }
		void IDisposable.Dispose() { }
	}
}
