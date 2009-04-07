using System.IO;
using System.Text;

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
        /// <param name="drive">The name of the drive (e.g. c:).</param>
        /// <returns>The amount of free space in bytes.</returns>
        long GetFreeDiskSpace(string drive);
	}
}