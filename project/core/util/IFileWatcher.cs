using System;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public interface IFileWatcher : IDisposable
	{
		event FileSystemEventHandler OnFileChanged;
	}
}
