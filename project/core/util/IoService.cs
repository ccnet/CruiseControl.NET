using System;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// 	
    /// </summary>
    public class IoService : IFileDirectoryDeleter
    {
        private readonly IExecutionEnvironment executionEnvironment = new ExecutionEnvironment();

        /// <summary>
        /// Deletes the including read only objects.	
        /// </summary>
        /// <param name="path">The path.</param>
        /// <remarks></remarks>
        public void DeleteIncludingReadOnlyObjects(string path)
        {
            try
            {
                // check whether path is a file or directory
                if (File.Exists(path))
                {
                    File.SetAttributes(path, FileAttributes.Normal);
                    File.Delete(path);
                }
                else if (Directory.Exists(path))
                {
                    var dirInfo = new DirectoryInfo(path);
                    SetReadOnlyRecursive(dirInfo);
                    dirInfo.Delete(true);
                }
                else
                {
                    Log.Warning("[IoService] File or directory not found: '{0}'", path);
                }
            }
            catch (PathTooLongException pathTooLongEx)
            {
                Log.Error("[IoService] Unable to delete path '{0}'.{1}{2}", path, Environment.NewLine, pathTooLongEx);
                
                if (executionEnvironment.IsRunningOnWindows)
                {
                    DeleteDirectoryWithLongPath(path);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        /// <summary>
        /// Deletes a directory on Windows with a commandline call.
        /// 
        /// Reason:
        /// .NET only supports filenames up to 260 characters long for backwards compability
        /// read more at: http://blogs.msdn.com/bclteam/archive/2007/02/13/long-paths-in-net-part-1-of-3-kim-hamilton.aspx
        /// this is a Windows only limitation
        /// </summary>
        /// <param name="path">Path to delete.</param>
        static void DeleteDirectoryWithLongPath(string path)
        {
            Log.Info("[IoService] Try running 'cmd.exe /C RD /S /Q' to delete '{0}'", path);

            // call a commandline delete as fallback
            var executor = new ProcessExecutor();
            var processInfo = new ProcessInfo("cmd.exe",
                string.Concat("/C RD /S /Q ", StringUtil.AutoDoubleQuoteString(path)));

            var pr = executor.Execute(processInfo);
            if (pr.Failed)
                throw new CruiseControlException(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Unable to delete path '{0}'.", path));
        }

        /// <summary>
        /// Sets directory and file attributes to "normal" recursive
        /// </summary>
        /// <param name="path">Root path to start from.</param>
        static void SetReadOnlyRecursive(DirectoryInfo path)
        {

            foreach (var dirInfo in path.GetDirectories())
            {
                try
                {
                    dirInfo.Attributes = FileAttributes.Normal;
                }
                catch (Exception ex)
                {
                    Log.Error("[IoService] Unable to remove read-only attribute from '{0}'.", dirInfo.FullName);
                    Log.Error(ex);
                    throw;
                }

                SetReadOnlyRecursive(dirInfo);
            }

            foreach (var fileInfo in path.GetFiles())
            {
                try
                {
                    fileInfo.Attributes = FileAttributes.Normal;
                }
                catch (Exception ex)
                {
                    Log.Error("[IoService] Unable to remove read-only attribute from '{0}'.", fileInfo.FullName);
                    Log.Error(ex);
                    throw;
                }
            }
        }
    }
}
