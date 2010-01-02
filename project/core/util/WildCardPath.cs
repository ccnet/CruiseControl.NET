
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class WildCardPath
	{
		private string pathPattern;

		public WildCardPath(string pathPattern)
		{
			this.pathPattern = pathPattern;
		}

		public FileInfo[] GetFiles()
		{
			FileInfo[] files = new FileInfo[0];
			if (HasWildCards(pathPattern))
			{
				string dir = Path.GetDirectoryName(pathPattern);
				DirectoryInfo info = new DirectoryInfo(dir);
				string pattern = Path.GetFileName(pathPattern);
				if (info.Exists)
					files = info.GetFiles(pattern);
			}
			else
			{
				files = new FileInfo[] {new FileInfo(pathPattern.Trim())};
			}
			return files;
		}

		private bool HasWildCards(string file)
		{
			return file.IndexOf("*") > -1;
		}
	}
}