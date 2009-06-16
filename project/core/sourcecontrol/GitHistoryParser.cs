using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	public class GitHistoryParser : IHistoryParser
	{
		private static readonly Regex modificationList =
			new Regex(
				"Commit:(?<Hash>[a-z0-9]{40})\nTime:(?<Time>.+?)\nAuthor:(?<Author>.+?)\nE-Mail:(?<Mail>.+?)\nMessage:(?<Message>.*?)\nChanges:\n(?<Changes>.*?)\n\n",
				RegexOptions.Compiled | RegexOptions.Singleline);

		private static readonly Regex changeList = new Regex("(?<Type>[A-Z]{1})\t(?<FileName>.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>
		/// Parse and filter the supplied modifications.  The position of each modification in the list is used as the ChangeNumber.
		/// </summary>
		/// <param name="history"></param>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public Modification[] Parse(TextReader history, DateTime from, DateTime to)
		{
			List<Modification> result = new List<Modification>();

			if (history.Peek() < 1)
				return result.ToArray();

			foreach (Match mod in modificationList.Matches(history.ReadToEnd()))
			{
				Log.Debug(string.Concat("[Git] Found commit: ", mod.Value));
				result.AddRange(GetCommitModifications(mod, from, to));
			}

			return result.ToArray();
		}

		/// <summary>
		/// Parse a commit for modifications and returns a list with every modification in the date/time limits.
		/// </summary>
		/// <param name="commitMatch"></param>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		private static IList<Modification> GetCommitModifications(Match commitMatch, DateTime from, DateTime to)
		{
			IList<Modification> result = new List<Modification>();

			string hash = commitMatch.Groups["Hash"].Value;
			DateTime modifiedTime = DateTime.Parse(commitMatch.Groups["Time"].Value);
			string username = commitMatch.Groups["Author"].Value;
			string emailAddress = commitMatch.Groups["Mail"].Value;
			string comment = commitMatch.Groups["Message"].Value;
			string changes = commitMatch.Groups["Changes"].Value;

			if (modifiedTime < from || modifiedTime > to)
			{
				Log.Debug(string.Concat("[Git] Ignore commit '", hash, "' from '", modifiedTime.ToUniversalTime(),
				                        "' because it is older then '",
				                        from.ToUniversalTime(), "' or newer then '", to.ToUniversalTime(), "'."));
				return result;
			}

			foreach (Match change in changeList.Matches(changes))
			{
				Modification mod = new Modification();
				mod.ChangeNumber = hash;
				mod.Comment = comment;
				mod.EmailAddress = emailAddress;
				mod.ModifiedTime = modifiedTime;
				mod.UserName = username;

				mod.Type = GetModificationType(change.Groups["Type"].Value);

				string fullFilePath = change.Groups["FileName"].Value;
				mod.FileName = GetFileFromPath(fullFilePath);
				mod.FolderName = GetFolderFromPath(fullFilePath);

				result.Add(mod);
			}

			return result;
		}

		/// <summary>
		/// Convert a "git log --name-status" action value to a modification type name.
		/// </summary>
		/// <param name="actionAbbreviation">The action abbreviation.</param>
		/// <returns>The modification type name.</returns>
		private static string GetModificationType(string actionAbbreviation)
		{
			switch (actionAbbreviation.ToLowerInvariant())
			{
				case "a":
					return "Added";
				case "d":
					return "Deleted";
				case "m":
					return "Modified";
				default:
					return string.Concat("Unknown action: ", actionAbbreviation);
			}
		}

		/// <summary>
		/// Extract the folder name from a file path name in a "git log --name-status" command.
		/// </summary>
		/// <param name="fullFileName">The path name.</param>
		/// <returns>The folder name.</returns>
		private static string GetFolderFromPath(string fullFileName)
		{
			if (fullFileName.LastIndexOf("/") <= 0)
				return string.Empty;

			return fullFileName.Substring(0, fullFileName.LastIndexOf("/"));
		}

		/// <summary>
		/// Extract the file name from a file path name in a "git log --name-status" command.
		/// </summary>
		/// <param name="fullFileName">The path name.</param>
		/// <returns>The file name.</returns>
		private static string GetFileFromPath(string fullFileName)
		{
			return fullFileName.Substring(fullFileName.LastIndexOf("/") + 1);
		}
	}
}
