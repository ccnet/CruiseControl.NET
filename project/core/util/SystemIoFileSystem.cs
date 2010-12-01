
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System;
using ThoughtWorks.CruiseControl.Core.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// 	
    /// </summary>
	public class SystemIoFileSystem : IFileSystem
	{
        /// <summary>
        /// Copies the specified source path.	
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="destPath">The dest path.</param>
        /// <remarks></remarks>
		public void Copy(string sourcePath, string destPath)
		{
			if (!File.Exists(sourcePath) && !Directory.Exists(sourcePath))
			{
				throw new IOException(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Source Path [{0}] doesn't exist", sourcePath));
			}

			if (Directory.Exists(sourcePath))
				CopyDirectoryToDirectory(sourcePath, destPath);
			else
				CopyFile(sourcePath, destPath);
		}

		private void CopyDirectoryToDirectory(string sourcePath, string destPath)
		{
			foreach (DirectoryInfo subdir in new DirectoryInfo(sourcePath).GetDirectories())
			{
				CopyDirectoryToDirectory(subdir.FullName, Path.Combine(destPath, subdir.Name));
			}
			foreach (string file in Directory.GetFiles(sourcePath))
			{
				CopyFileToDirectory(Path.Combine(sourcePath, file), destPath);
			}
		}

		private void CopyFile(string sourcePath, string destPath)
		{
			if (Directory.Exists(destPath))
				CopyFileToDirectory(sourcePath, destPath);
			else
				CopyFileToFile(sourcePath, destPath);
		}

		private void CopyFileToDirectory(string sourcePath, string destPath)
		{
			CopyFileToFile(sourcePath, Path.Combine(destPath, Path.GetFileName(sourcePath)));
		}

		private void CopyFileToFile(string sourcePath, string destPath)
		{
			string destDir = Path.GetDirectoryName(destPath);
			if (! Directory.Exists(destDir))
				Directory.CreateDirectory(destDir);

			if (File.Exists(destPath))
				File.SetAttributes(destPath, FileAttributes.Normal);

			File.Copy(sourcePath, destPath, true);
		}

        /// <summary>
        /// Saves the specified file.	
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="content">The content.</param>
        /// <remarks></remarks>
		public void Save(string file, string content)
		{
			using (StreamWriter stream = File.CreateText(file))
			{
				stream.Write(content);
			}
		}

        /// <summary>
        /// Write the specified data in UTF8 encoding to the specified file in an atomic fashion, such
        /// that the file is either completely replaced on disk or not altered at all.
        /// </summary>
        /// <param name="file">The pathname of the file to write to.</param>
        /// <param name="content">The content to write to the file.</param>
        public void AtomicSave(string file, string content)
        {
            AtomicSave(file, content, Encoding.UTF8);
        }

        /// <summary>
        /// Write the specified data in the specified encoding to the specified file in an atomic fashion,
        /// such that the file is either completely replaced on disk or not altered at all.
        /// </summary>
        /// <param name="file">The pathname of the file to write to.</param>
        /// <param name="content">The content to write to the file.</param>
        /// <param name="encoding">The encoding of the data.</param>
        /// <remarks>
        /// Not all file systems provide an atomic-file-replace operation, therefore we implement this 
        /// ourselves.
        /// <ol>
        /// <li>Write to a new file on disk.</li>
        /// <li>Flush all the writes to disk.</li>
        /// <li>Rename the existing target file to the "old" file name.</li>
        /// <li>Rename the new file to the target file.</li>
        /// <li>Delete the old target file.</li>
        /// </ol>
        /// </remarks>
        public void AtomicSave(string file, string content, Encoding encoding)
        {
            FileStream newFile = null;
            string targetFilePath = file;
            string newFilePath = targetFilePath + "-NEW";
            string oldFilePath = targetFilePath + "-OLD";
            try
            {
                // Clean up any old copies of our temporary files:
                DeleteFile(newFilePath);
                DeleteFile(oldFilePath);

                // Step 1: Write to a new file on disk, forcing the writes out to disk.
                newFile = new FileStream(newFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.None,
                    4096, FileOptions.WriteThrough | FileOptions.SequentialScan);
                byte[] fileBytes = encoding.GetBytes(content);
                newFile.Write(fileBytes, 0, fileBytes.Length);

                // Step 2: Flush any remaining writes to disk.
                newFile.Close();
                newFile.Dispose();
                newFile = null;

                // Step 3: Rename the existing target file to the "old" file name, if it exists.
                if (File.Exists(targetFilePath))
                    File.Move(targetFilePath, oldFilePath);

                // Step 4: Rename the new file to the target file.
                File.Move(newFilePath, targetFilePath);

                // Step 5: Delete the old target file if we renamed it in step 3.
                if (File.Exists(oldFilePath))
                    DeleteFile(oldFilePath);
            }
            catch
            {
                if (newFile != null)
                {	
                    // Don't leave open files laying around.
                    newFile.Close();
                    newFile.Dispose();
                }
                throw;
            }
        }

        /// <summary>
        /// Loads the specified file.	
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public TextReader Load(string file)
		{
			using (TextReader reader = new StreamReader(file))
			{
				return new StringReader(reader.ReadToEnd());
			}
		}

        /// <summary>
        /// Files the exists.	
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public bool FileExists(string file)
		{
			return File.Exists(file);
		}

        /// <summary>
        /// Directories the exists.	
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public bool DirectoryExists(string folder)
		{
			return Directory.Exists(folder);
        }

        #region DeleteFile()
        /// <summary>
        /// Delete a file if it exists.
        /// </summary>
        /// <param name="filePath">The filepath to delete.</param>
        public void DeleteFile(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists)
            {
                if ((fileInfo.Attributes & FileAttributes.ReadOnly) != 0)
                    fileInfo.Attributes ^= FileAttributes.ReadOnly;
                fileInfo.Delete();
            }
        }
        #endregion

        #region EnsureFolderExists()
        /// <summary>
        /// Ensures that the folder for the specified file exists.
        /// </summary>
        /// <param name="fileName">The name of the file, including the folder path.</param>
        public void EnsureFolderExists(string fileName)
        {
            string directory = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(directory))
            {
                Log.Debug("Creating folder [" + directory + "]");
                Directory.CreateDirectory(directory);
            }
        }
        #endregion

        /// <summary>
        /// Ensures that the specified file exists.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        public void EnsureFileExists(string fileName)
        {
            EnsureFolderExists(fileName);
            using (File.Create(fileName)) { }
        }

        #region GetFreeDiskSpace()
        /// <summary>
        /// Retrieves the free disk space for a drive.
        /// </summary>
		/// <param name="driveName">The name of the drive (e.g. c:).</param>
        /// <returns>The amount of free space in bytes.</returns>
        public long GetFreeDiskSpace(string driveName)
        {
			DriveInfo drive = new DriveInfo(driveName);

        	return drive.AvailableFreeSpace;
        }
        #endregion

        #region GetFilesInDirectory()
        /// <summary>
        /// Lists all the files within a directory.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public string[] GetFilesInDirectory(string directory)
        {
            return Directory.GetFiles(directory);
        }

        /// <summary>
        /// Lists all the files within a directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="includeSubDirectories">
        /// If set to <c>true</c> then files in sub directories will be included.
        /// </param>
        /// <returns></returns>
        public string[] GetFilesInDirectory(string directory, bool includeSubDirectories)
        {
            return Directory.GetFiles(
                directory,
                "*.*",
                includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }
        #endregion

        #region GetLastWriteTime()
        /// <summary>
        /// Retrieves the last write time of a file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public DateTime GetLastWriteTime(string fileName)
        {
            var writeTime = DateTime.MinValue;
            var fileInfo = new FileInfo(fileName);
            if (fileInfo.Exists) writeTime = fileInfo.LastWriteTime;
            return writeTime;
        }
        #endregion

        #region GenerateTaskResultFromFile()
        /// <summary>
        /// Generates a task result from a file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public ITaskResult GenerateTaskResultFromFile(string fileName)
        {
            return new FileTaskResult(fileName);
        }
        #endregion

        #region OpenOutputStream()
        /// <summary>
        /// Opens an output stream for saving data.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Stream OpenOutputStream(string fileName)
        {
            var stream = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            return stream;
        }
        #endregion

        #region OpenOutputStream()
        /// <summary>
        /// Opens an input stream for loading data.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Stream OpenInputStream(string fileName)
        {
            var stream = File.OpenRead(fileName);
            return stream;
        }
        #endregion

        #region CreateDirectory()
        /// <summary>
        /// Creates a directory.
        /// </summary>
        /// <param name="folder">The name of the folder to create.</param>
        public void CreateDirectory(string folder)
        {
            Directory.CreateDirectory(folder);
        }
        #endregion

        #region DeleteDirectory()
        /// <summary>
        /// Deletes a directory.
        /// </summary>
        /// <param name="folder">The name of the folder to delete.</param>
        public void DeleteDirectory(string folder)
        {
            Directory.Delete(folder);
        }

        /// <summary>
        /// Deletes a directory, optionally deleting all sub-directories.
        /// </summary>
        /// <param name="folder">The name of the folder to delete.</param>
        /// <param name="recursive">If set to <c>true</c> recursively delete folders.</param>
        public void DeleteDirectory(string folder, bool recursive)
        {
            Directory.Delete(folder, recursive);
        }
        #endregion

        /// <summary>
        /// Gets the length of the file.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        /// <returns>The length of the file in bytes.</returns>
        public long GetFileLength(string fullName)
        {
            var info = new FileInfo(fullName);
            return info.Length;
        }

        /// <summary>
        /// Gets the files in directory.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="searchOption">The search option.</param>
        /// <returns>The files in the directory that match the pattern.</returns>
        public IEnumerable<string> GetFilesInDirectory(string path, string pattern, SearchOption searchOption)
        {
            return Directory.GetFiles(path, pattern, searchOption);
        }

        /// <summary>
        /// Gets the version of a file.
        /// </summary>
        /// <param name="filePath">The full path to the file.</param>
        /// <returns>The version number of the file it it exists; <c>null</c> otherwise.</returns>
        public Version GetFileVersion(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            var fileInfo = FileVersionInfo.GetVersionInfo(filePath);
            return new Version(fileInfo.FileVersion);
        }
    }
}