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
			CircularArray buffer = new CircularArray(maxLines);
			using (Stream stream = OpenFile())
			{
				using (StreamReader reader = new StreamReader(stream))
				{
					string line;
					while ((line = reader.ReadLine()) != null)
					{
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
			string filename = ConfigurationSettings.AppSettings["ServerLogFilePath"];
			return StringUtil.IsBlank(filename) ? "ccnet.log" : filename;
		}

		private static int ReadMaxLinesFromConfig()
		{
			string linesToReadConfig = ConfigurationSettings.AppSettings["ServerLogFileLines"];
			return (linesToReadConfig != null) ? int.Parse(ConfigurationSettings.AppSettings["ServerLogFileLines"]) : DefaultMaxLines;
		}
	}
}