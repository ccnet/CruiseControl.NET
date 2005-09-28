using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	public class CvsHistoryParser : IHistoryParser
	{
		/// <summary>
		///  This line delimits seperate files in the CVS log information.
		///  </summary>
		private static readonly string CVS_FILE_DELIM =
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

		private string currentLine;
		private string workingFileName;
		private Regex dateLineRegex = new Regex(@"date:\s+(?<date>\S+)\s+(?<time>\S+)\s*(?<timezone>\S*);\s+author:\s+(?<author>.*);\s+state:\s+(?<state>.*);(\s+lines:\s+\+(?<line1>\d+)\s+-(?<line2>\d+))?");

		public Modification[] Parse(TextReader cvsLog, DateTime from, DateTime to)
		{
			// Read to the first RCS file name. The first entry in the log
			// information will begin with this line. A CVS_FILE_DELIMITER is NOT
			// present. If no RCS file lines are found then there is nothing to do.
			currentLine = ReadToNotPast(cvsLog, CVS_RCSFILE_LINE, null);
			ArrayList mods = new ArrayList();

			while (currentLine != null)
			{
				// Parse the single file entry, which may include several
				// modifications.
				IList entryList = ParseFileEntry(cvsLog);

				//Add all the modifications to the local list.
				mods.AddRange(entryList);

				// Read to the next RCS file line. The CVS_FILE_DELIMITER may have
				// been consumed by the parseEntry method, so we cannot read to it.
				currentLine = ReadToNotPast(cvsLog, CVS_RCSFILE_LINE, null);
			}

			return (Modification[]) mods.ToArray(typeof (Modification));
		}

		private IList ParseFileEntry(TextReader cvsLog)
		{
			ArrayList mods = new ArrayList();

			workingFileName = ParseFileNameAndPath(cvsLog);

			currentLine = ReadToNotPast(cvsLog, CVS_REVISION_DATE, CVS_FILE_DELIM);
			while (currentLine != null && !currentLine.StartsWith(CVS_FILE_DELIM))
			{
				Modification mod = ParseModification(cvsLog);
				mods.Add(mod);

				if (currentLine != null && !currentLine.StartsWith(CVS_FILE_DELIM))
				{
					currentLine = ReadToNotPast(cvsLog, CVS_REVISION_DATE, CVS_FILE_DELIM);
				}
			}
			return mods;
		}

		private string ParseFileNameAndPath(TextReader cvsLog)
		{
			// Read to the working file name line to get the filename. It is ASSUMED
			// that a line will exist with the working file name on it.
			currentLine = ReadToNotPast(cvsLog, CVS_WORKINGFILE_LINE, null);
			string workingFileNameAndPath = null;
			if (currentLine != null)
			{
				workingFileNameAndPath = currentLine.Substring(CVS_WORKINGFILE_LINE.Length);
			}
			return workingFileNameAndPath;
		}

		private string ReadToNotPast(TextReader reader, string startsWith, string notPast)
		{
			currentLine = reader.ReadLine();
			while (currentLine != null && !currentLine.StartsWith(startsWith))
			{
				if ((notPast != null) && currentLine.StartsWith(notPast))
				{
					return null;
				}
				currentLine = reader.ReadLine();
			}
			return currentLine;
		}

		private Modification ParseModification(TextReader reader)
		{
			Modification nextModification = ParseDateLine(currentLine);
			nextModification.FileName = ParseFileName(workingFileName);
			nextModification.FolderName = ParseFolderName(workingFileName);
			nextModification.Comment = ParseComment(reader);
			return nextModification;
		}

		private string ParseComment(TextReader cvsLog)
		{
			// All the text from now to the next revision delimiter or working
			// file delimiter constitutes the comment.
			string message = string.Empty;
			bool multiLine = false;

			currentLine = cvsLog.ReadLine();
			while (currentLine != null && !currentLine.StartsWith(CVS_FILE_DELIM)
				&& !currentLine.StartsWith(CVS_REVISION_DELIM))
			{
				if (multiLine)
				{
					message += NEW_LINE;
				}
				else
				{
					multiLine = true;
				}
				message += currentLine;

				//Go to the next line.
				currentLine = cvsLog.ReadLine();
			}
			return message;
		}

		private string ParseFileName(string workingFileName)
		{
			int lastSlashIndex = workingFileName.LastIndexOf("/");
			return workingFileName.Substring(lastSlashIndex + 1);
		}

		private string ParseFolderName(string workingFileName)
		{
			int lastSlashIndex = workingFileName.LastIndexOf("/");
			string folderName = string.Empty;
			if (lastSlashIndex != -1)
			{
				folderName = workingFileName.Substring(0, lastSlashIndex);
			}
			return folderName;
		}

		private Modification ParseDateLine(string dateLine)
		{
			try
			{
				Match match = dateLineRegex.Match(dateLine);

				Modification nextModification = new Modification();
				nextModification.ModifiedTime = ParseModifiedTime(match.Groups["date"].Value, match.Groups["time"].Value);
				nextModification.UserName = match.Groups["author"].Value;
				nextModification.Type = ParseType(match.Groups["state"].Value, match.Groups["line1"].Value);
				return nextModification;
			}
			catch (Exception ex)
			{
				throw new CruiseControlException("Unable to parse CVS date line: " + dateLine, ex);
			}
		}

		private DateTime ParseModifiedTime(string dateStamp, string timeStamp)
		{
			string dateTimeString = string.Format("{0} {1} GMT", dateStamp, timeStamp);
			return DateTime.Parse(dateTimeString, DateTimeFormatInfo.GetInstance(CultureInfo.InvariantCulture));
		}

		private string ParseType(string stateKeyword, string line1)
		{
			if (StringUtil.EqualsIngnoreCase(stateKeyword, CVS_REVISION_DEAD))
			{
				return "deleted";
			}
			else if (StringUtil.IsBlank(line1) || Convert.ToInt32(line1) == 0)
			{
				return "added";
			}
			else
			{
				return "modified";
			}
		}
	}
}