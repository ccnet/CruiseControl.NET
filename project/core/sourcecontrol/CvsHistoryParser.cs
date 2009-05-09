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

		public Modification[] Parse(TextReader cvsLog, DateTime from, DateTime to)
		{
			ArrayList mods = new ArrayList();

			// Read to the first RCS file name. The first entry in the log
			// information will begin with this line. A CVS_FILE_DELIMITER is NOT
			// present. If no RCS file lines are found then there is nothing to do.			
			while ((currentLine = ReadToNotPast(cvsLog, CVS_RCSFILE_LINE, null)) != null)
			{
				// Parse the single file entry, which may include several modifications.
				IList entryList = ParseFileEntry(currentLine, cvsLog);

				//Add all the modifications to the local list.
				mods.AddRange(entryList);
			}
			return (Modification[]) mods.ToArray(typeof (Modification));
		}

		private IList ParseFileEntry(string rcsFileLine, TextReader cvsLog)
		{
			ArrayList mods = new ArrayList();

			string rcsFile = ParseFileNameAndPath(rcsFileLine);
			string fileName = ParseFileName(rcsFile);
			string folderName = ParseFolderName(rcsFile);

			currentLine = ReadToNotPast(cvsLog, CvsModificationDelimiter, CVS_FILE_DELIM);
			while (currentLine != null && !currentLine.StartsWith(CVS_FILE_DELIM))
			{
				Modification mod = ParseModification(cvsLog, folderName, fileName);
				if (IsFileAddedOnBranch(mod)) continue;
				mods.Add(mod);
			}
			return mods;
		}

		private readonly Regex rcsfileRegex = new Regex(@"^RCS file:\s+(.+),v\s*$");

		private string ParseFileNameAndPath(string rcsFileLine)
		{
			return rcsfileRegex.Match(rcsFileLine).Groups[1].Value;
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

		private Modification ParseModification(TextReader reader, string folderName, string fileName)
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
				modification.FileName = fileName;
				modification.FolderName = folderName;

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
				&& !currentLine.StartsWith(CvsModificationDelimiter))
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

		private string ParseFileName(string rcsFilePath)
		{
			int lastSlashIndex = rcsFilePath.LastIndexOf("/");
			return rcsFilePath.Substring(lastSlashIndex + 1);
		}

		/// <summary>
		/// Strip the filename, Attic folder (if the file has been deleted) and repository folder prefix to get folder name.
		/// </summary>
		private string ParseFolderName(string rcsFilePath)
		{
			return StripAtticFolder(StripFilename(rcsFilePath));
		}

		private static string StripFilename(string rcsFilePath)
		{
			int lastSlashIndex = rcsFilePath.LastIndexOf("/");
			if (lastSlashIndex != -1)
			{
				return rcsFilePath.Substring(0, lastSlashIndex);
			}
			return string.Empty;
		}

		private static string StripAtticFolder(string rcsFilePath)
		{
			if (rcsFilePath.EndsWith("Attic"))
				rcsFilePath = rcsFilePath.Substring(0, rcsFilePath.LastIndexOf("/"));
			return rcsFilePath;
		}

		private readonly Regex dateLineRegex = new Regex(@"date:\s+(?<date>\S+)\s+(?<time>\S+)\s*(?<timezone>\S*);\s+author:\s+(?<author>.*);\s+state:\s+(?<state>\S*);(\s+lines:\s+\+(?<line1>\d+)\s+-(?<line2>\d+))?");

		private void ParseDateLine(Modification modification, string dateLine)
		{
			Match match = dateLineRegex.Match(dateLine);
			try
			{
				modification.ModifiedTime = ParseModifiedTime(match.Groups["date"].Value, match.Groups["time"].Value, match.Groups["timezone"].Value);
				modification.UserName = match.Groups["author"].Value;
				modification.Type = ParseType(match.Groups["state"].Value, match.Groups["line1"].Value);
			}
			catch (Exception ex)
			{
				throw new CruiseControlException("Unable to parse CVS date line: " + dateLine, ex);
			}
		}

		private DateTime ParseModifiedTime(string dateStamp, string timeStamp, string timezone)
		{
            if(String.IsNullOrEmpty(timezone))
                timezone = "+0";
			string dateTimeString = string.Format("{0} {1} {2}", dateStamp, timeStamp, timezone);
			return DateTime.Parse(dateTimeString, DateTimeFormatInfo.GetInstance(CultureInfo.InvariantCulture));
		}

		private string ParseType(string stateKeyword, string line1)
		{
			if (StringUtil.EqualsIgnoreCase(stateKeyword, CVS_REVISION_DEAD))
			{
				return "deleted";
			}
            else if (string.IsNullOrEmpty(line1) || Convert.ToInt32(line1) == 0)
			{
				return "added";
			}
			else
			{
				return "modified";
			}
		}

		private static bool IsFileAddedOnBranch(Modification mod)
		{
			return mod.Type == "deleted" && mod.Version == "1.1";
		}
	}
}
