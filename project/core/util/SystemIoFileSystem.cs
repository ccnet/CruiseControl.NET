using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class SystemIoFileSystem : IFileSystem
	{
		public void Copy(string sourcePath, string destPath)
		{
			if (!File.Exists(sourcePath) && !Directory.Exists(sourcePath))
			{
				throw new IOException(string.Format("Source Path [{0}] doesn't exist", sourcePath));
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
        /// <li>Write to a new file on disk.
        /// <li>Flush all the writes to disk.
        /// <li>Rename the existing target file to the "old" file name.
        /// <li>Rename the new file to the target file.
        /// <li>Delete the old target file.
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

        public TextReader Load(string file)
		{
			using (TextReader reader = new StreamReader(file))
			{
				return new StringReader(reader.ReadToEnd());
			}
		}

		public bool FileExists(string file)
		{
			return File.Exists(file);
		}

		public bool DirectoryExists(string folder)
		{
			return Directory.Exists(folder);
		}

        /// <summary>
        /// Delete a file if it exists.
        /// </summary>
        /// <param name="filePath">The filepath to delete.</param>
        private static void DeleteFile(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists)
            {
                if ((fileInfo.Attributes & FileAttributes.ReadOnly) != 0)
                    fileInfo.Attributes ^= FileAttributes.ReadOnly;
                fileInfo.Delete();
            }
        }

        #region EnsureFolderExists()
        /// <summary>
        /// Ensures that the folder for the specified file exists.
        /// </summary>
        /// <param name="fileName">The name of the file, including the folder path.</param>
        public void EnsureFolderExists(string fileName)
        {
            string directory = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
        }
        #endregion

        #region GetFreeDiskSpace()
        /// <summary>
        /// Retrieves the free disk space for a drive.
        /// </summary>
        /// <param name="drive">The name of the drive (e.g. c:).</param>
        /// <returns>The amount of free space in bytes.</returns>
        public long GetFreeDiskSpace(string drive)
        {
            long totalBytes;
            long freeBytes;
            long freeBytesAvail;

            if (!Directory.Exists(drive))
            {
                throw new ArgumentException(string.Format("Invalid Drive {0}", drive));
            }

            GetDiskFreeSpaceEx(drive,
                out freeBytesAvail,
                out totalBytes,
                out freeBytes);

            return freeBytesAvail;
        }
        #endregion

        #region Interop methods
        #region GetDiskFreeSpaceEx()
        [DllImport("kernel32.dll", EntryPoint = "GetDiskFreeSpaceExA")]
        private static extern long GetDiskFreeSpaceEx(string lpDirectoryName,
          out long lpFreeBytesAvailableToCaller,
          out long lpTotalNumberOfBytes,
          out long lpTotalNumberOfFreeBytes);
        #endregion
        #endregion
    }
}