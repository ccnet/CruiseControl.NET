using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class IoService : IFileDirectoryDeleter
	{
		public void DeleteIncludingReadOnlyObjects(string filename)
		{
			DeleteDirectoryEvenIfReadOnly(filename);
			DeleteFileEvenIfReadOnly(filename);
		}

		/// <summary>
		/// Deletes all contents of a directory, even those that are read-only, without deleting the directory itself.
		/// </summary>
		/// <param name="directoryPath"></param>
		public void EmptyDirectoryIncludingReadOnlyObjects(string directoryPath)
		{
			if (Directory.Exists(directoryPath))
			{
				foreach (string subdir in Directory.GetDirectories(directoryPath))
				{
					DeleteDirectoryEvenIfReadOnly(Path.Combine(directoryPath, subdir));
				}

				foreach (string file in Directory.GetFiles(directoryPath))
				{
					DeleteFileEvenIfReadOnly(Path.Combine(directoryPath, file));
				}
			}
		}

		private void DeleteDirectoryEvenIfReadOnly(string directoryPath)
		{
			if (Directory.Exists(directoryPath))
			{
				EmptyDirectoryIncludingReadOnlyObjects(directoryPath);
				File.SetAttributes(directoryPath, FileAttributes.Normal);
				Directory.Delete(directoryPath, true);
			}

		}

		public void DeleteFileEvenIfReadOnly(string filename)
		{
			if (File.Exists(filename))
			{
				File.SetAttributes(filename, FileAttributes.Normal);
				File.Delete(filename);
			}
		}
	}
}