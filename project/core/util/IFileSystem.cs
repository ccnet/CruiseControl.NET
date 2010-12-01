using System.IO;
using System.Text;
using System;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// 	
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        /// Copies the specified source path.	
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="destPath">The dest path.</param>
        /// <remarks></remarks>
        void Copy(string sourcePath, string destPath);
        /// <summary>
        /// Saves the specified file.	
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="content">The content.</param>
        /// <remarks></remarks>
        void Save(string file, string content);
        /// <summary>
        /// Atomics the save.	
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="content">The content.</param>
        /// <remarks></remarks>
        void AtomicSave(string file, string content);
        /// <summary>
        /// Atomics the save.	
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="content">The content.</param>
        /// <param name="encoding">The encoding.</param>
        /// <remarks></remarks>
        void AtomicSave(string file, string content, Encoding encoding);
        /// <summary>
        /// Loads the specified file.	
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        TextReader Load(string file);
        /// <summary>
        /// Files the exists.	
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        bool FileExists(string file);
        /// <summary>
        /// Directories the exists.	
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        bool DirectoryExists(string folder);

        /// <summary>
        /// Ensures that the folder for the specified file exists.
        /// </summary>
        /// <param name="fileName">The name of the file, including the folder path.</param>
        /// <returns>The fileName.</returns>
        void EnsureFolderExists(string fileName);

        /// <summary>
        /// Ensures that the specified file exists.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        void EnsureFileExists(string fileName);

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
        /// Lists all the files within a directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="includeSubDirectories">
        /// If set to <c>true</c> then files in sub directories will be included.
        /// </param>
        /// <returns></returns>
        string[] GetFilesInDirectory(string directory, bool includeSubDirectories);

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
        /// Creates a directory.
        /// </summary>
        /// <param name="folder">The name of the folder to create.</param>
        void CreateDirectory(string folder);

        /// <summary>
        /// Deletes a file.
        /// </summary>
        /// <param name="path">The path of the file to delete.</param>
        void DeleteFile(string path);

        /// <summary>
        /// Deletes a directory.
        /// </summary>
        /// <param name="folder">The name of the folder to delete.</param>
        void DeleteDirectory(string folder);

        /// <summary>
        /// Deletes a directory, optionally deleting all sub-directories.
        /// </summary>
        /// <param name="folder">The name of the folder to delete.</param>
        /// <param name="recursive">If set to <c>true</c> recursively delete folders.</param>
        void DeleteDirectory(string folder, bool recursive);

        /// <summary>
        /// Gets the length of the file.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        /// <returns>The length of the file in bytes.</returns>
        long GetFileLength(string fullName);

        /// <summary>
        /// Gets the files in directory.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="searchOption">The search option.</param>
        /// <returns>The files in the directory that match the pattern.</returns>
        IEnumerable<string> GetFilesInDirectory(string path, string pattern, SearchOption searchOption);

        /// <summary>
        /// Gets the version of a file.
        /// </summary>
        /// <param name="filePath">The full path to the file.</param>
        /// <returns>The version number of the file it it exists; <c>null</c> otherwise.</returns>
        Version GetFileVersion(string filePath);
    }
}