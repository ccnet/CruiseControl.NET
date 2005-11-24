using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
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
		/// This line delimits the different revisions of a file in the CVS log information.
		/// </summary>
		private static readonly string CvsModificationDelimiter = "----------------------------";

		/// <summary>
		/// This is the keyword that precedes the name of the RCS filename in the CVS log information.
		/// </summary>
		private static readonly string CVS_RCSFILE_LINE = "RCS file: ";

		/// <summary>
		/// This is the keyword that precedes the name of the working filename in the CVS log information.
		/// </summary>
		private static readonly string CVS_WORKINGFILE_LINE = "Working file: ";

		/// <summary>
		/// This is the keyword that precedes the timestamp of a file revision in the CVS log information.
		/// </summary>
		private static readonly string CVS_REVISION_DATE = "date:";

		/// <summary>
		/// This is a state keyword which indicates that a revision to a file was not
		/// relevant to the current branch, or the revision consisted of a deletion
		/// of the file (removal from branch..).
		/// </summary>
		private static readonly string CVS_REVISION_DEAD = "dead";

		private string currentLine;
		private string workingFileName;

		public Modification[] Parse(TextReader cvsLog, DateTime from, DateTime to)
		{
			ArrayList mods = new ArrayList();

			// Read to the first RCS file name. The first entry in the log
			// information will begin with this line. A CVS_FILE_DELIMITER is NOT
			// present. If no RCS file lines are found then there is nothing to do.			
			while ((currentLine = ReadToNotPast(cvsLog, CVS_RCSFILE_LINE, null)) != null)
			{
				// Parse the single file entry, which may include several modifications.
				IList entryList = ParseFileEntry(cvsLog);

				//Add all the modifications to the local list.
				mods.AddRange(entryList);
			}
			return (Modification[]) mods.ToArray(typeof (Modification));
		}

		private IList ParseFileEntry(TextReader cvsLog)
		{
			ArrayList mods = new ArrayList();

			workingFileName = ParseFileNameAndPath(cvsLog);

			currentLine = ReadToNotPast(cvsLog, CvsModificationDelimiter, CVS_FILE_DELIM);
			while (currentLine != null && !currentLine.StartsWith(CVS_FILE_DELIM))
			{
				mods.Add(ParseModification(cvsLog));
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
			currentLine = reader.ReadLine();
			Modification modification = new Modification();
			if (currentLine.StartsWith("revision"))
			{
				modification.Version = currentLine.Substring("revision".Length).Trim();
				currentLine = reader.ReadLine();
			}
			if (currentLine.StartsWith(CVS_REVISION_DATE))
			{
				ParseDateLine(modification, currentLine);
				modification.FileName = ParseFileName(workingFileName);
				modification.FolderName = ParseFolderName(workingFileName);

				currentLine = reader.ReadLine();
				modification.Comment = ParseComment(reader);
			}
			return modification;
		}

		private string ParseComment(TextReader cvsLog)
		{
			// All the text from now to the next revision delimiter or working
			// file delimiter constitutes the comment.
			StringBuilder message = new StringBuilder();
			while (currentLine != null && !currentLine.StartsWith(CVS_FILE_DELIM)
				&& !currentLine.StartsWith(CvsHistoryParser.CvsModificationDelimiter))
			{
				if (message.Length > 0)
				{
					message.Append(Environment.NewLine);
				}
				message.Append(currentLine);

				//Go to the next line.
				currentLine = cvsLog.ReadLine();
			}
			return message.ToString();
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

		private Regex dateLineRegex = new Regex(@"date:\s+(?<date>\S+)\s+(?<time>\S+)\s*(?<timezone>\S*);\s+author:\s+(?<author>.*);\s+state:\s+(?<state>\S*);(\s+lines:\s+\+(?<line1>\d+)\s+-(?<line2>\d+))?");

		private void ParseDateLine(Modification modification, string dateLine)
		{
			Match match = dateLineRegex.Match(dateLine);
			try
			{
				modification.ModifiedTime = ParseModifiedTime(match.Groups["date"].Value, match.Groups["time"].Value);
				modification.UserName = match.Groups["author"].Value;
				modification.Type = ParseType(match.Groups["state"].Value, match.Groups["line1"].Value);
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