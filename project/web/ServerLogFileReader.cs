using System;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace ThoughtWorks.CruiseControl.Web
{
	public class ServerLogFileReader
	{
		private string			_filename;
		private int				_maxLines;
		private BufferedStream	_reader = null;
		private int				_readLines = 0;

		public ServerLogFileReader(string filename, int maxLines)
		{
			_filename = filename;
			_maxLines = maxLines;
		}

		public string Read() 
		{ 
			StringBuilder builder = new StringBuilder();
			try
			{
				if (OpenFile())
				{
					SeekPastEnd();
					while (MovePrevious())
					{
						if (!LineLimitReached(builder))
						{
							builder.Insert(0, ReadChar());
						}
						else
						{
							break;
						}
					}
				}
			}
			finally
			{
				CloseFile();
			}

			return builder.ToString().TrimStart(null);
		}

		private bool OpenFile()
		{
			Debug.Assert(_reader == null && _readLines == 0);
			if (_filename != null)
			{
				_reader = new BufferedStream(new FileStream(_filename, FileMode.Open, FileAccess.Read));
				return _reader.Length > 0;
			}
			else
			{
				return false;
			}
		}

		private void CloseFile()
		{
			if (_reader != null)
			{
				_reader.Close();
				_reader = null;
			}

			Debug.Assert(_reader == null);
		}
		
		private void SeekPastEnd()
		{
			Debug.Assert(_reader != null && _reader.CanRead && _reader.Length > 0);
			_readLines = 0;
			_reader.Seek(1, SeekOrigin.End);
		}

		private bool MovePrevious()
		{
			if (_reader.Position > 1)
			{
				_reader.Seek(-2, SeekOrigin.Current);
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool LineLimitReached(StringBuilder builder)
		{
			if (builder.Length >= Environment.NewLine.Length)
			{
				if (builder.ToString(0, Environment.NewLine.Length).Equals(Environment.NewLine))
				{
					_readLines++;
				}
			}

			return _readLines >= _maxLines;
		}

		private char ReadChar()
		{
			return (char)_reader.ReadByte();
		}
	}
}
