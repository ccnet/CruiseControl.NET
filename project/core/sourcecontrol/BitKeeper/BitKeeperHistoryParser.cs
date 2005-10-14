using System;
using System.Collections;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	public class BitKeeperHistoryParser : IHistoryParser
	{
		/// <summary>
		/// This is the keyword that precedes a change set in the bk log information.
		/// </summary>
		private static readonly string BK_CHANGESET_LINE = "ChangeSet";

		private string currentLine;

		public Modification[] Parse(TextReader bkLog, DateTime from, DateTime to)
		{
			// Read to the first ChangeSet. The first entry in the log
			// information will begin with this line. If no ChangeSet file
			// lines are found then there is nothing to do.
			currentLine = ReadToNotPast(bkLog, BK_CHANGESET_LINE, null);

			ArrayList mods = new ArrayList();
			while (currentLine != null)
			{
				// Parse the ChangeSet entry and read till next ChangeSet
				Modification mod = ParseEntry(bkLog);

				// Add all the modifications to the local list.
				mods.Add(mod);

				// Read to the next non-blank line.
				currentLine = bkLog.ReadLine();
			}
			return (Modification[]) mods.ToArray(typeof (Modification));
		}

		private Modification ParseEntry(TextReader bkLog)
		{
			// Example: "ChangeSet\n1.201 05/09/08 14:52:49 hunth@spankyham. +1 -0\nComments"
			Regex regex = new Regex(@"(?<version>[\d.]+)\s+(?<datetime>\d{2,4}/\d{2}/\d{2} \d{2}:\d{2}:\d{2})\s+(?<username>\S+).*"); // , RegexOptions.Compiled);

			Modification mod = new Modification();

			// Get file name information
			char[] trims = new char[2];
			trims[0] = ' ';
			trims[1] = '\t';
			currentLine = currentLine.TrimStart(trims);
			mod.FileName = ParseFileName(currentLine);
			mod.FolderName = ParseFolderName(currentLine);

			// Get the next line with change info
			currentLine = bkLog.ReadLine();

			Match match = regex.Match(currentLine);
			if (!match.Success)
				throw new Exception("Unable to parse line: " + currentLine);

			mod.ModifiedTime = ParseDate(match.Result("${datetime}"));
			mod.Type = "Modified";
			mod.UserName = match.Result("${username}");
			mod.Version = match.Result("${version}");;

			// Read all lines of the comment and flatten them
			mod.Comment = ParseComment(bkLog);

			return mod;
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

		private DateTime ParseDate(string date)
		{
			// BK is funny - we can't guarantee that the year will be two or four digits,
			// so we have to check how many digits we got and deal with it

			string format;
			int firstSep = date.IndexOf("/");
			if (firstSep == 4)
				format = "yyyy'/'MM'/'dd HH:mm:ss";
			else
				format = "yy'/'MM'/'dd HH:mm:ss";

			return DateTime.ParseExact(date, format, DateTimeFormatInfo.InvariantInfo);
		}

		private string ParseComment(TextReader bkLog)
		{
			// All the text from now to the next blank line constitues the comment
			string message = string.Empty;
			char[] trimChars = new char[1];
			bool multiLine = false;

			trimChars[0] = ' ';

			currentLine = bkLog.ReadLine().TrimStart(trimChars);
			while (currentLine != null && currentLine.Length != 0)
			{
				if (multiLine)
				{
					message += System.Environment.NewLine;
				}
				else
				{
					multiLine = true;
				}
				message += currentLine;

				// Go to the next line.
				currentLine = bkLog.ReadLine();
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
	}
} 