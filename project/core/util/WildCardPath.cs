using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class WildCardPath
	{
		private string _pathPattern;
		public WildCardPath(string pathPattern)
		{										 
			_pathPattern = pathPattern;	
		}

		public FileInfo[] GetFiles()
		{
			FileInfo[] files= new FileInfo[0];
			if(HasWildCards(_pathPattern))
			{
				string dir = Path.GetDirectoryName(_pathPattern);
				DirectoryInfo info = new DirectoryInfo(dir);
				string pattern = Path.GetFileName(_pathPattern);
				if(info.Exists)
					files =info.GetFiles(pattern);
			}
			else
			{
				files = new FileInfo[] { new FileInfo(_pathPattern.Trim()) };
			}
			return files;
		}

		private bool HasWildCards(string file)
		{
			return file.IndexOf("*") > -1;
		}
	}
}
