using System;
using System.Collections;
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
			if(HasWildCards(_pathPattern))
			{
				string dir = Path.GetDirectoryName(_pathPattern);
				DirectoryInfo info = new DirectoryInfo(dir);
				string pattern = Path.GetFileName(_pathPattern);
				return info.GetFiles(pattern);
			}
			else
			{
				return new FileInfo[] { new FileInfo(_pathPattern) };
			}
		}

		private bool HasWildCards(string file)
		{
			return file.IndexOf("*") > -1;
		}
	}
}
