using System.Configuration;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Logging
{
	public class ServerLogFileReader
	{
		private const int DefaultMaxLines = 80;
		private string filename;
		private int maxLines;

		public ServerLogFileReader() : this(ReadFilenameFromConfig(), ReadMaxLinesFromConfig())
		{}

		public ServerLogFileReader(string filename, int maxLines)
		{
			this.filename = filename;
			this.maxLines = maxLines;
		}

		public string Read()
		{
			return Read(EnumeratorDirection.Forward);
		}

		public string Read(EnumeratorDirection direction)
		{
			return Read(direction, null);
		}

		public string Read(string project)
		{
			return Read(EnumeratorDirection.Forward, '[' + project);
		}

		private string Read(EnumeratorDirection direction, string match)
		{
			CircularArray buffer = new CircularArray(maxLines);
			using (Stream stream = OpenFile())
			{
				using (StreamReader reader = new StreamReader(stream))
				{
					string line;
					while ((line = reader.ReadLine()) != null)
					{
						if (match == null || line.IndexOf(match) >= 0)
							buffer.Add(line);
					}
				}
			}
			return buffer.ToString(direction);
		}

		private Stream OpenFile()
		{
			return new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		}

		private static string ReadFilenameFromConfig()
		{
			string filename = ConfigurationManager.AppSettings["ServerLogFilePath"];
			return StringUtil.IsBlank(filename) ? "ccnet.log" : filename;
		}

		private static int ReadMaxLinesFromConfig()
		{
            string linesToReadConfig = ConfigurationManager.AppSettings["ServerLogFileLines"];
            return (linesToReadConfig != null) ? int.Parse(ConfigurationManager.AppSettings["ServerLogFileLines"]) : DefaultMaxLines;
		}
	}
}