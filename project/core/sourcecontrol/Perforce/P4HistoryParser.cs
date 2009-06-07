using System;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce
{
	public class P4HistoryParser : IHistoryParser
	{
		private static Regex modRegex = new Regex(@"info1: (?<folder>//.*)/(?<file>.*)#(?<revision>\d+) (?<type>\w+)");
		private static Regex changeRegex = new Regex(@"text: Change (?<change>.*) by (?<email>(?<user>.*)@.*) on (?<date>\d{4}/\d{2}/\d{2} \d{2}:\d{2}:\d{2})");

		/// <summary>
		/// Used to extract changelist numbers from p4.exe output of format
		/// <code>info: Change 123 456 789</code>
		/// </summary>
		/// <param name="changes"></param>
		/// <returns></returns>
		public string ParseChanges(String changes)
		{
			StringBuilder result = new StringBuilder();
			Regex regex = new Regex(@"info: Change (?<num>\d+) ");
			foreach(Match match in regex.Matches(changes))
			{
				result.Append(match.Groups["num"]);
				result.Append(' ');
			}
			return result.ToString().Trim();
		}

		/// <summary>
		/// Parses output from p4.exe obtained using arguments <code>p4 -s describe -s 123</code>
		/// where 123 (etc...) are changelist numbers.  This output looks like this:
		/// <p>
		/// <code>
		/// text: Change 123 by user@hostname on 2002/08/21 14:39:52
		/// text:
		/// text:   The checkin comment
		/// text:
		/// text: Affected files ...
		/// text:
		/// info1: //view/path/filename.java#1 add
		/// text:
		/// exit: 0
		/// </code>
		/// </p>
		/// the type appears at the end of the info1 line, and may be add, edit, delete etc...
		/// Two regex strings are used to match the first line, and the 'info1:' line.
		/// NOTE there's a tab character before comment text.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public Modification[] Parse(TextReader reader, DateTime from, DateTime to)
		{
			ArrayList mods = new ArrayList();
			string line;
			string change = null, email = null, user = null, comment = string.Empty;
			DateTime date = DateTime.Now;
			while((line = reader.ReadLine()) != null)
			{
				Match modificationMatch = modRegex.Match(line);
				if (modificationMatch.Success)
				{
					// when this line is matched, we're finished with this mod, so add it
					Modification mod = new Modification();
					mod.ChangeNumber = change;
					mod.Version = modificationMatch.Groups["revision"].Value;
					mod.FolderName = modificationMatch.Groups["folder"].Value;
					mod.FileName = modificationMatch.Groups["file"].Value;
					mod.Type = modificationMatch.Groups["type"].Value;
					mod.EmailAddress = email;
					mod.UserName = user;
					mod.ModifiedTime = date;
					mod.Comment = comment.Trim();
					mods.Add(mod);
				}
				else 
				{
					Match changeMatch = changeRegex.Match(line);
					if (changeMatch.Success)
					{
						// set these values while they're available
						change = changeMatch.Groups["change"].Value;
						email = changeMatch.Groups["email"].Value;
						user = changeMatch.Groups["user"].Value;
						date = DateTime.Parse(changeMatch.Groups["date"].Value);
						// TODO this is necessary, could someone explain why?
						comment = "";
					}
					else 
					{
						string checkinCommentPrefix = "text: \t";
						if (line.StartsWith(checkinCommentPrefix))
						{
							comment += line.Substring(checkinCommentPrefix.Length) + "\r\n";
						}
					}
				}
			}
			
			return (Modification[]) mods.ToArray(typeof(Modification));
		}
	}
}
