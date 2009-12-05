using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// A default instance of the shadow copier.
    /// </summary>
    public class DefaultShadowCopier
        : IShadowCopier
    {
        #region Private fields
        private static ShadowStore store = new ShadowStore();
        #endregion

        #region Public methods
        #region RetrieveFilePath()
        /// <summary>
        /// Retrieves the path to a file that has been shadow copied.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>The full path to the shadow copied file, if it exists, null otherwise.</returns>
        public virtual string RetrieveFilePath(string fileName)
        {
            // All the work is done by the static store - this allows multiple instances to work off the backing directory
            return store.CopyFile(fileName);
        }
        #endregion
        #endregion

        #region Private classes
        #region ShadowStore
        /// <summary>
        /// A bacing store for shadow-copied files.
        /// </summary>
        private class ShadowStore
            : IDisposable
        {
            private readonly string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            private List<string> copiedFiles = new List<string>();
            private readonly object lockObject = new object();

            public ShadowStore()
            {
                if (!Directory.Exists(tempPath))
                    Directory.CreateDirectory(tempPath);
            }

            /// <summary>
            /// Checks if a file already exists, if not, it attempts to copy it over.
            /// </summary>
            /// <param name="fileName"></param>
            /// <returns></returns>
            public string CopyFile(string fileName)
            {
                var filePath = Path.Combine(tempPath, fileName);
                if (!File.Exists(filePath))
                {
                    // Make sure the file exists in the original location
                    var sourceFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
                    if (File.Exists(sourceFile))
                    {
                        // Only copy it within a lock, just in case another thread has already copied it
                        lock (lockObject)
                        {
                            // Make sure noone else copied it while waiting for the lock
                            if (!File.Exists(filePath))
                            {
                                File.Copy(sourceFile, filePath);
                                copiedFiles.Add(filePath);
                            }
                        }
                    }
                    else
                    {
                        filePath = null;
                    }
                }
                return filePath;
            }

            /// <summary>
            /// Delete any copied files.
            /// </summary>
            public void Dispose()
            {
                // Only do this in a lock in case someone else is also cleaning up
                lock (lockObject)
                {
                    foreach (var file in copiedFiles)
                    {
                        File.Delete(file);
                    }
                    copiedFiles.Clear();
                }
            }
        }
        #endregion
        #endregion
    }
}
