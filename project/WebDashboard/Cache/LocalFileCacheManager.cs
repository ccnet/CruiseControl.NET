using System;
using System.IO;
using ThoughtWorks.CruiseControl.WebDashboard.Config;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.Cache
{
	public class LocalFileCacheManager : ICacheManager
	{
		public static readonly string LocalCacheRootDirectoryConfigParameter = "cachedirectory";
		private readonly IConfigurationGetter configurationGetter;
		private readonly IPathMapper pathMapper;

		public LocalFileCacheManager(IPathMapper pathMapper, IConfigurationGetter configurationGetter)
		{
			this.pathMapper = pathMapper;
			this.configurationGetter = configurationGetter;
		}

		public void AddContent(string serverName, string projectName, string directory, string fileName, string content)
		{
			string localFilePath = GetLocalPathForFile(serverName, projectName, directory, fileName);
			CreateDirectoryForFileIfNecessary(localFilePath);
			using (StreamWriter sw = new StreamWriter(localFilePath)) 
			{
				sw.Write(content);
			}
		}

		public string GetContent(string serverName, string projectName, string directory, string fileName)
		{
			string filepath = GetLocalPathForFile(serverName, projectName, directory, fileName);

			if (File.Exists(filepath))
			{
				return ReadFile(filepath);
			}
			else
			{
				return null;	
			}
		}

		public string GetURLForFile(string serverName, string projectName, string directory, string fileName)
		{
			return pathMapper.GetAbsoluteURLForRelativePath(GetRelativePathForFile(serverName, projectName, directory, fileName));
		}

		private string GetLocalPathForFile(string serverName, string projectName, string directory, string fileName)
		{
			return pathMapper.GetLocalPathFromURLPath(GetRelativePathForFile(serverName, projectName, directory, fileName));
		}

		private string GetRelativePathForFile(string serverName, string projectName, string directory, string fileName)
		{
			string cacheRootDirectory = configurationGetter.GetSimpleConfigSetting(LocalCacheRootDirectoryConfigParameter);
			if (cacheRootDirectory == null || cacheRootDirectory == string.Empty)
			{
				throw new ApplicationException("Cache Root directory not set in config - make sure you have specified parameter [ " + LocalCacheRootDirectoryConfigParameter + " ]");
			}
			return Path.Combine(Path.Combine(Path.Combine(Path.Combine(cacheRootDirectory, serverName), projectName), directory), fileName);
		}

		private void CreateDirectoryForFileIfNecessary(string fullLocalPathOfFile)
		{
			DirectoryInfo dir = new FileInfo(fullLocalPathOfFile).Directory;
			if (! dir.Exists)
			{
				dir.Create();
			}
		}

		private string ReadFile(string path)
		{
			using (StreamReader sr = new StreamReader(path)) 
			{
				return sr.ReadToEnd();
			}
		}
	}
}
