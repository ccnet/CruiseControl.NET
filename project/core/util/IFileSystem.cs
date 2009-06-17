using System.IO;
using System.Text;
using System;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public interface IFileSystem
	{
		void Copy(string sourcePath, string destPath);
		void Save(string file, string content);
        void AtomicSave(string file, string content);
        void AtomicSave(string file, string content, Encoding encoding);
        TextReader Load(string file);
		bool FileExists(string file);
		bool DirectoryExists(string folder);

        /// <summary>
        /// Ensures that the folder for the specified file exists.
        /// </summary>
        /// <param name="fileName">The name of the file, including the folder path.</param>
        /// <returns>The fileName.</returns>
        void EnsureFolderExists(string fileName);

        /// <summary>
        /// Retrieves the free disk space for a drive.
        /// </summary>
		/// <param name="driveName">The name of the drive (e.g. c:).</param>
        /// <returns>The amount of free space in bytes.</returns>
        long GetFreeDiskSpace(string driveName);

        /// <summary>
        /// Lists all the files within a directory.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        string[] GetFilesInDirectory(string directory);

        /// <summary>
        /// Retrieves the last write time of a file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        DateTime GetLastWriteTime(string fileName);

        /// <summary>
        /// Generates a task result from a file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        ITaskResult GenerateTaskResultFromFile(string fileName);

        /// <summary>
        /// Opens an output stream for saving data.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Stream OpenOutputStream(string fileName);

        /// <summary>
        /// Opens an input stream for loading data.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Stream OpenInputStream(string fileName);
    }
}