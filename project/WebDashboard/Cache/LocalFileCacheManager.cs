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
			CreateDirectoryIfNecessary(serverName, projectName, directory);
			using (StreamWriter sw = new StreamWriter(GetPathforFile(serverName, projectName, directory, fileName))) 
			{
				sw.Write(content);
			}
		}

		public string GetContent(string serverName, string projectName, string directory, string fileName)
		{
			string filepath = GetPathforFile(serverName, projectName, directory, fileName);

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
			return pathMapper.GetAbsoluteURLForRelativePath(GetPathforFile(serverName, projectName, directory, fileName));
		}

		private string GetPathforFile(string serverName, string projectName, string directory, string fileName)
		{
			return Path.Combine(GetDirectory(serverName, projectName, directory), fileName);
		}

		private string GetDirectory(string serverName, string projectName, string directory)
		{
			string cacheRootDirectory = configurationGetter.GetSimpleConfigSetting(LocalCacheRootDirectoryConfigParameter);
			if (cacheRootDirectory == null || cacheRootDirectory == string.Empty)
			{
				throw new ApplicationException("Cache Root directory not set in config - make sure you have specified parameter [ " + LocalCacheRootDirectoryConfigParameter + " ]");
			}
			return Path.Combine(Path.Combine(Path.Combine(cacheRootDirectory, serverName), projectName), directory);
		}

		private void CreateDirectoryIfNecessary(string serverName, string projectName, string directory)
		{
			Directory.CreateDirectory(GetDirectory(serverName, projectName, directory));
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
