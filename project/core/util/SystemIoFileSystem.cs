using System.IO;

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

			File.Copy(sourcePath, destPath, true);
		}
	}
}
