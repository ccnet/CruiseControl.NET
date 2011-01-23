namespace ThoughtWorks.CruiseControl.UnitTests.Core.Sourcecontrol.Mercurial
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using ThoughtWorks.CruiseControl.Core;
	using ThoughtWorks.CruiseControl.Core.Util;

	/// <summary>
	/// A stub for IFileSystem to allow for testing the Mercurial source control.
	/// </summary>
	public class StubFileSystem : IFileSystem
	{
		public void AtomicSave(string file, string content)
		{
		}

		public void AtomicSave(string file, string content, Encoding encoding)
		{
		}

		public void Copy(string sourcePath, string destPath)
		{
		}

		public void CreateDirectory(string folder)
		{
		}

		public void DeleteDirectory(string folder)
		{
		}

		public void DeleteDirectory(string folder, bool recursive)
		{
		}

		public void DeleteFile(string filePath)
		{
		}

		public bool DirectoryExists(string folder)
		{
			return true;
		}

		public void EnsureFileExists(string fileName)
		{
			throw new NotImplementedException();
		}

		public void EnsureFolderExists(string fileName)
		{
		}

		public bool FileExists(string file)
		{
			return true;
		}

		public string[] GetFilesInDirectory(string directory)
		{
			return new string[0];
		}

		public string[] GetFilesInDirectory(string directory, bool includeSubDirectories)
		{
			return new string[0];
		}

		public IEnumerable<string> GetFilesInDirectory(string path, string pattern, SearchOption searchOption)
		{
			throw new NotImplementedException();
		}

		public long GetFileLength(string fullName)
		{
			throw new NotImplementedException();
		}

		public long GetFreeDiskSpace(string driveName)
		{
			return int.MaxValue;
		}

		public DateTime GetLastWriteTime(string fileName)
		{
			return DateTime.MinValue;
		}

		public ITaskResult GenerateTaskResultFromFile(string fileName)
		{
			return null;
		}

		public ITaskResult GenerateTaskResultFromFile(string fileName, bool deleteAfterMerge)
		{
			return null;
		}

		public Version GetFileVersion(string filePath)
		{
			throw new NotImplementedException();
		}

		public TextReader Load(string file)
		{
			return null;
		}

		public Stream OpenOutputStream(string fileName)
		{
			return null;
		}

		public Stream OpenInputStream(string fileName)
		{
			return null;
		}

		public void Save(string file, string content)
		{
		}
	}
}
