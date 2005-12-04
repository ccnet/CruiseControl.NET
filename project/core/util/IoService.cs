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

		private void DeleteDirectoryEvenIfReadOnly(string filename)
		{
			if (Directory.Exists(filename))
			{
				foreach (string subdir in Directory.GetDirectories(filename))
				{
					DeleteDirectoryEvenIfReadOnly(Path.Combine(filename, subdir));
				}

				foreach (string file in Directory.GetFiles(filename))
				{
					DeleteFileEvenIfReadOnly(Path.Combine(filename, file));
				}

				File.SetAttributes(filename, FileAttributes.Normal);
				Directory.Delete(filename);
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