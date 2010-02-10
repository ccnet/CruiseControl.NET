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
        /// Deletes the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        void DeleteFile(string fileName);

        /// <summary>
        /// Ensures that the folder for the specified file exists.
        /// </summary>
        /// <param name="fileName">The name of the file, including the folder path.</param>
        void EnsureFolderExists(string fileName);

        /// <summary>
        /// Ensures that the folder for the specified file exists.
        /// </summary>
        /// <param name="fileName">The name of the file, including the folder path.</param>
        /// <param name="includesFileName"><c>true</c> if the filename also includes the file name.</param>
        void EnsureFolderExists(string fileName, bool includesFileName);

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

        /// <summary>
        /// Moves the file.
        /// </summary>
        /// <param name="oldFilePath">The old file path.</param>
        /// <param name="newFilePath">The new file path.</param>
        void MoveFile(string oldFilePath, string newFilePath);

        /// <summary>
        /// Creates a temp file.
        /// </summary>
        /// <returns>A <see cref="Stream"/> to the temp file.</returns>
        Stream CreateTempFile();

        /// <summary>
        /// Deletes a temp file.
        /// </summary>
        /// <param name="tempFile">The temp file to delete.</param>
        void DeleteTempFile(Stream tempFile);

        /// <summary>
        /// Resets the stream for reading.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <returns>The reset stream.</returns>
        Stream ResetStreamForReading(Stream inputStream);
    }
}