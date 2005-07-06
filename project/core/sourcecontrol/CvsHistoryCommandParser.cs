using System.Collections;
using System.Configuration;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	/// <summary>
	/// Summary description for CvsHistoryCommandParser.
	/// </summary>
	public class CvsHistoryCommandParser
	{
		public const string COMMAND_DATE_FORMAT = "yyyy-MM-dd HH:mm:ss 'GMT'";
		private static readonly string cvsRepositoryFilePath = Path.Combine("CVS", "Repository");

		private string repositoryDirectory = null;

		public string WorkingDirectory = ".";

		public string Repository
		{
			get
			{
				if (repositoryDirectory == null)
				{
					repositoryDirectory = ReadCvsRepositoryDirectory(new DirectoryInfo(WorkingDirectory));
				}
				return repositoryDirectory;
			}
		}

		public virtual string[] ParseOutputFrom(string output)
		{
			StringReader reader = new StringReader(output);
			ArrayList entryList = new ArrayList();
			string line;
			while ((line = reader.ReadLine()) != null)
			{
				string dir = ParseEntry(line);
				if (dir == null)
				{
					continue;
				}

				// We need to replace forward slash returned by cvs to directory separator of
				// the platform this is running on.
				dir = FormatPathForOS(dir);

				string repoFile = GetRepository();

				string entry;
				if (!dir.Equals(FormatPathForOS(repoFile)))
				{
					entry = dir.Substring(Repository.Length + 1);
				}
				else
				{
					entry = ".";
				}

				if (!entryList.Contains(entry))
				{
					Log.Debug("Added entry: " + entry);
					entryList.Add(entry);
				}
			}

			return (string[]) entryList.ToArray(typeof (string));
		}

		public string GetRepository()
		{
			try
			{
				return Repository;
			}
			catch (ConfigurationException)
			{
				Log.Info(string.Format("Couldn't find directory: {0}", WorkingDirectory));
				Log.Info(string.Format("Trying directory: {0}", Directory.GetParent(WorkingDirectory)));

				return ReadCvsRepositoryDirectory(Directory.GetParent(WorkingDirectory));
			}
		}

		/// <summary>
		/// Find the directory listed by the history command
		/// </summary>
		/// <param name="line"></param>
		/// <returns></returns>
		public string ParseEntry(string line)
		{
			int startIdx = line.IndexOf(Repository);
			int endIdx = line.IndexOf("==") - startIdx;

			if (startIdx < 0)
			{
				return null;
			}

			string retVal = line.Substring(startIdx, endIdx).Trim();
			// Check to make sure that retVal is does not just contain
			// the repository in its value
			// i.e. Repository = dirA/dirB/dirC
			//      retVal = dirA/dirB/dirC_Something
			char[] chars = FormatPathForOS(retVal).ToCharArray();
			if (chars.Length > Repository.Length)
			{
				char directorySeparator = chars[Repository.Length];
				if (!directorySeparator.Equals(Path.DirectorySeparatorChar))
				{
					// If the char at Repository.Length is anything but a directory separator
					// we need to return null as the retVal is not a valid return value
					return null;
				}
			}

			return retVal;
		}

		/// <summary>
		/// Check for directory separators of unix and windows and replace with
		/// the correct path separator for the current OS.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		private string FormatPathForOS(string path)
		{
			// Replace all unix style directory separators with current os separator
			string retVal = path.Replace('/', Path.DirectorySeparatorChar);

			// Replace all windows style directory separators with current os separator
			retVal = retVal.Replace('\\', Path.DirectorySeparatorChar);

			return retVal;
		}

		public string ReadCvsRepositoryDirectory(DirectoryInfo dir)
		{
			FileInfo repo = new FileInfo(Path.Combine(dir.FullName, cvsRepositoryFilePath));
			if (!repo.Exists)
			{
				throw new ConfigurationException(string.Format("Working directory {0} is not a CVS directory.", WorkingDirectory));
			}

			using (StreamReader reader = new StreamReader(repo.OpenRead()))
			{
				repositoryDirectory = reader.ReadLine();
			}
			return repositoryDirectory;
		}
	}
}