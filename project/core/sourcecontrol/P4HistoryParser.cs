using System;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;

namespace tw.ccnet.core.sourcecontrol
{
	public class P4HistoryParser : IHistoryParser
	{

		private static Regex modRegex = new Regex(@"info1: (?<folder>//.*)/(?<file>.*)#\d+ (?<type>\w+)");
		private static Regex changeRegex = new Regex(@"text: Change \w+ by (?<email>(?<user>.*)@.*) on (?<date>\d{4}/\d{2}/\d{2} \d{2}:\d{2}:\d{2})");

		public Modification[] Parse(TextReader reader)
		{
			ArrayList mods = new ArrayList();
			string line;
			string email = null, user = null, comment = null;
			DateTime date = DateTime.Now;
			while((line = reader.ReadLine()) != null)
			{
				Match match = modRegex.Match(line);
				if (match.Success)
				{
					Modification mod = new Modification();
					mod.FolderName = match.Groups["folder"].Value;
					mod.FileName = match.Groups["file"].Value;
					mod.Type = match.Groups["type"].Value;
					mod.EmailAddress = email;
					mod.UserName = user;
					mod.ModifiedTime = date;
					mod.Comment = comment.Trim();
					mods.Add(mod);
				}
				else 
				{
					match = changeRegex.Match(line);
					if (match.Success)
					{
						email = match.Groups["email"].Value;
						user = match.Groups["user"].Value;
						date = DateTime.Parse(match.Groups["date"].Value);
						comment = "";
					}
					else if (line.StartsWith("text:   "))
					{
						comment += line.Substring(8) + "\r\n";
					}
				}
			}
			
			return (Modification[]) mods.ToArray(typeof(Modification));
		}

		public string ParseChanges(String changes)
		{
			if (!changes.TrimEnd().EndsWith("exit: 0"))
			{
				throw new CruiseControlException("Perforce exit status 1");
			}
			StringBuilder result = new StringBuilder();
			Regex regex = new Regex(@"info: Change (?<num>\d+) ");
			foreach(Match match in regex.Matches(changes))
			{
				result.Append(match.Groups["num"]);
				result.Append(' ');
			}
			return result.ToString().Trim();
		}

	}
}
