using System;
using System.Collections;
using System.Globalization;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	public class CvsHistoryParser : IHistoryParser
	{
		/// <summary>
		///  This line delimits seperate files in the CVS log information.
		///  </summary>
		internal static readonly string CVS_FILE_DELIM =
			"=============================================================================";

		/// <summary>
		/// This is the keyword that precedes the name of the RCS filename in the CVS
		/// log information.
		/// </summary>
		private static readonly string CVS_RCSFILE_LINE = "RCS file: ";

		/// <summary>
		/// This is the keyword that precedes the name of the working filename in the
		/// CVS log information.
		/// </summary>
		private static readonly string CVS_WORKINGFILE_LINE = "Working file: ";

		/// <summary>
		/// This line delimits the different revisions of a file in the CVS log
		/// information.
		/// </summary>
		private static readonly string CVS_REVISION_DELIM =
			"----------------------------";

		/// <summary>
		/// This is the keyword that precedes the timestamp of a file revision in the
		/// CVS log information.
		/// </summary>
		private static readonly string CVS_REVISION_DATE = "date:";

		/// <summary>
		/// This is a state keyword which indicates that a revision to a file was not
		/// relevant to the current branch, or the revision consisted of a deletion
		/// of the file (removal from branch..).
		/// </summary>
		private static readonly string CVS_REVISION_DEAD = "dead";

		/// <summary>
		/// System dependent new line seperator.
		/// </summary>
		private static readonly string NEW_LINE = "\n";

		/// <summary>
		/// This is the date format required by commands passed to CVS.	 
		/// </summary>
		internal static readonly string CVSDATE_FORMAT = "yyyy-MM-dd HH:mm:ss 'GMT'";

		private string _currentLine;
		private string _workingFileName;

		public void Init()
		{
			_currentLine = string.Empty;
			_workingFileName = string.Empty;
		}

		public Modification[] Parse(TextReader cvsLog, DateTime from, DateTime to)
		{
			// Read to the first RCS file name. The first entry in the log
			// information will begin with this line. A CVS_FILE_DELIMITER is NOT
			// present. If no RCS file lines are found then there is nothing to do.
			_currentLine = ReadToNotPast(cvsLog, CVS_RCSFILE_LINE, null);
			ArrayList mods = new ArrayList();

			while (_currentLine != null)
			{
				// Parse the single file entry, which may include several
				// modifications.
				IList entryList = ParseFileEntry(cvsLog);

				//Add all the modifications to the local list.
				mods.AddRange(entryList);

				// Read to the next RCS file line. The CVS_FILE_DELIMITER may have
				// been consumed by the parseEntry method, so we cannot read to it.
				_currentLine = ReadToNotPast(cvsLog, CVS_RCSFILE_LINE, null);
			}

			return (Modification[]) mods.ToArray(typeof (Modification));
		}

		internal IList ParseFileEntry(TextReader cvsLog)
		{
			ArrayList mods = new ArrayList();

			_workingFileName = ParseFileNameAndPath(cvsLog);

			_currentLine = ReadToNotPast(cvsLog, CVS_REVISION_DATE, CVS_FILE_DELIM);
			while (_currentLine != null && !_currentLine.StartsWith(CVS_FILE_DELIM))
			{
				Modification mod = ParseModification(cvsLog);
				mods.Add(mod);

				if (!_currentLine.StartsWith(CVS_FILE_DELIM))
				{
					_currentLine = ReadToNotPast(cvsLog, CVS_REVISION_DATE, CVS_FILE_DELIM);
				}
			}
			return mods;
		}

		internal string ParseFileNameAndPath(TextReader cvsLog)
		{
			// Read to the working file name line to get the filename. It is ASSUMED
			// that a line will exist with the working file name on it.
			_currentLine = ReadToNotPast(cvsLog, CVS_WORKINGFILE_LINE, null);
			string workingFileNameAndPath = null;
			if (_currentLine != null)
			{
				workingFileNameAndPath = _currentLine.Substring(CVS_WORKINGFILE_LINE.Length);
			}
			return workingFileNameAndPath;
		}


		internal string ReadToNotPast(TextReader reader, string startsWith, string notPast)
		{
			_currentLine = reader.ReadLine();
			while (_currentLine != null && !_currentLine.StartsWith(startsWith))
			{
				if ((notPast != null) && _currentLine.StartsWith(notPast))
				{
					return null;
				}
				_currentLine = reader.ReadLine();
			}
			return _currentLine;
		}

		internal Modification ParseModification(TextReader reader)
		{
			Modification nextModification = ParseDateLine(_currentLine);
			nextModification.FileName = ParseFileName(_workingFileName);
			nextModification.FolderName = ParseFolderName(_workingFileName);
			nextModification.Comment = ParseComment(reader);
			return nextModification;
		}

		internal string ParseComment(TextReader cvsLog)
		{
			// All the text from now to the next revision delimiter or working
			// file delimiter constitutes the comment.
			string message = string.Empty;
			bool multiLine = false;

			_currentLine = cvsLog.ReadLine();
			while (_currentLine != null && !_currentLine.StartsWith(CVS_FILE_DELIM)
				&& !_currentLine.StartsWith(CVS_REVISION_DELIM))
			{
				if (multiLine)
				{
					message += NEW_LINE;
				}
				else
				{
					multiLine = true;
				}
				message += _currentLine;

				//Go to the next line.
				_currentLine = cvsLog.ReadLine();
			}
			return message;
		}

		internal string ParseFileName(string workingFileName)
		{
			int lastSlashIndex = workingFileName.LastIndexOf("/");
			return workingFileName.Substring(lastSlashIndex + 1);
		}

		internal string ParseFolderName(string workingFileName)
		{
			int lastSlashIndex = workingFileName.LastIndexOf("/");
			string folderName = string.Empty;
			if (lastSlashIndex != -1)
			{
				folderName = workingFileName.Substring(0, lastSlashIndex);
			}
			return folderName;
		}

		internal Modification ParseDateLine(string dateLine)
		{
			string[] tokens = Split(dateLine);
			if (tokens.Length < 11)
			{
				throw new ArgumentException(string.Format(
					"Required at least 11 tokens but found {0} in Dateline: {1}", tokens.Length, dateLine));
			}
			// First token is the keyword for date, then the next two should be
			// the date and time stamps.				
			string dateStamp = tokens[1];
			string timeStamp = tokens[2];

			// The next token should be the author keyword, then the author name.			
			string authorName = tokens[6];

			// The next token should be the state keyword, then the state name.
			string stateKeyword = tokens[10];

			Modification nextModification = new Modification();
			nextModification.ModifiedTime = ParseModifiedTime(dateStamp, timeStamp);
			nextModification.UserName = authorName;
			nextModification.Type = ParseType(stateKeyword, IsAdded(tokens));
			return nextModification;
		}

		internal DateTime ParseModifiedTime(string dateStamp, string timeStamp)
		{
			string dateTimeString = string.Format("{0} {1} GMT", dateStamp, timeStamp);
			return DateTime.Parse(dateTimeString, DateTimeFormatInfo.GetInstance(CultureInfo.InvariantCulture));
		}

		internal string ParseType(string stateKeyword, bool isAdded)
		{
			string type;
			if (CaseInsensitiveComparer.Default.Compare(stateKeyword, CVS_REVISION_DEAD) == 0)
			{
				type = "deleted";
			}
			else if (isAdded)
			{
				type = "added";
			}
			else
			{
				type = "modified";
			}
			return type;
		}

		internal string[] Split(string line)
		{
			return line.Split(new char[] {' ', '\t', '\n', '\r', '\f', ';'});
		}

		internal bool IsAdded(string[] tokens)
		{
			// if no lines keyword then file is added
			bool hasLinesKeyword = tokens.Length > 13 && tokens[13].StartsWith("lines:");
			return !hasLinesKeyword;
		}
	}
}