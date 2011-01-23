using System;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// 	
    /// </summary>
	public interface IFileWatcher : IDisposable
	{
        /// <summary>
        /// Occurs when [on file changed].	
        /// </summary>
        /// <remarks></remarks>
		event FileSystemEventHandler OnFileChanged;
        /// <summary>
        /// Adds the watcher.	
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <remarks></remarks>
	    void AddWatcher (string filename);
	}
}
