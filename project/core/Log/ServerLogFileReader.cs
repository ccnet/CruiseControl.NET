using System;
using System.Configuration;
using System.IO;
using System.Text;

namespace ThoughtWorks.Core.Log
{
	public class ServerLogFileReader
	{
		private string			_filename;
		private int				_maxLines;

		public ServerLogFileReader()
		{
			string linesToReadConfig = ConfigurationSettings.AppSettings["ServerLogFileLines"];
			_maxLines = (linesToReadConfig != null) ? int.Parse(ConfigurationSettings.AppSettings["ServerLogFileLines"]) : 80;

			_filename = ConfigurationSettings.AppSettings["ServerLogFilePath"];
			if (_filename == null || _filename == string.Empty)
			{
				_filename = "ccnet.log";
			}
		}

		public ServerLogFileReader(string filename, int maxLines)
		{
			_filename = filename;
			_maxLines = maxLines;
		}

		public string Read() 
		{ 
			StringBuilder builder = new StringBuilder(10000);
			int readLines = 0;
			using (Stream stream = OpenFile())
			{
				SeekPastEnd(stream);
				while (MovePrevious(stream) && readLines < _maxLines)
				{
					builder.Insert(0, ReadChar(stream));
					if (HasReadNewLine(builder))
					{
						readLines++;
					}
				}
			}
			return builder.ToString().TrimStart();
		}

		private Stream OpenFile()
		{
			return new BufferedStream(new FileStream(_filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
		}

		private void SeekPastEnd(Stream stream)
		{
			stream.Seek(1, SeekOrigin.End);
		}

		private bool MovePrevious(Stream stream)
		{
			if (stream.Position > 1)
			{
				stream.Seek(-2, SeekOrigin.Current);
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool HasReadNewLine(StringBuilder builder)
		{
			if (builder.Length >= Environment.NewLine.Length)
			{
				if (builder.ToString(0, Environment.NewLine.Length).Equals(Environment.NewLine))
				{
					return true;
				}
			}
			return false;
		}

		private char ReadChar(Stream stream)
		{
			return (char)stream.ReadByte();
		}
	}
}
