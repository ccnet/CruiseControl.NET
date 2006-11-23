using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	public class MksHistoryParser : IHistoryParser
	{
		private static Regex MODIFICATION_SEARCH_REGEX = new Regex(@"^(?<ModificationType>(Revision|Added|Deleted))\s(changed|member):\s+(?<Filename>.*?)(was|now).*\s(to|at)\s(?<NewRevision>\d+(\.\d+)*)(\r\n|$)", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.Multiline);
		private static Regex MEMBER_INFO_REGEX = new Regex(@".*?\s+Created\sBy:\s(?<UserName>\w+)\son\s(?<ModifiedTime>.*?)\r\n(.|\r\n)*?\s+Revision\sDescription:\r\n(?<Comment>(.|\r\n)*?)\s+Labels:(.|\r\n|$)*?", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

		public virtual Modification[] Parse(TextReader history, DateTime from, DateTime to)
		{
			ArrayList result = new ArrayList();
			MatchCollection revisionMatches = MODIFICATION_SEARCH_REGEX.Matches(history.ReadToEnd());
			foreach (Match revisionMatch in revisionMatches)
			{
				result.Add(CreateModification(revisionMatch));
			}

			return (Modification[]) result.ToArray(typeof (Modification));
		}

		public virtual void ParseMemberInfoAndAddToModification(Modification modification, StringReader reader)
		{
			MatchCollection memberInfoMatches = MEMBER_INFO_REGEX.Matches(reader.ReadToEnd());
			foreach (Match match in memberInfoMatches)
			{
				modification.UserName = match.Groups["UserName"].Value.Trim();
				modification.ModifiedTime = DateTime.Parse(match.Groups["ModifiedTime"].Value.Trim(), CultureInfo.InvariantCulture);
				modification.Comment = match.Groups["Comment"].Value.Trim();
			}			
		}

		private Modification CreateModification(Match match)
		{
			Modification modification = new Modification();
			ParseFileAndFolderName(match.Groups["Filename"].Value.Trim(), modification);
			modification.Version = match.Groups["NewRevision"].Value.Trim();
			modification.Type = ParseModificationType(match.Groups["ModificationType"].Value.Trim());
			return modification;
		}

		private string ParseModificationType(string modificationType)
		{
			return ("Revision" == modificationType) ? "Modified" : modificationType;
		}

		private void ParseFileAndFolderName(string file, Modification modification)
		{
			int lastIndexOfFrontSlash = file.LastIndexOf("/");
			modification.FileName = file.Substring(lastIndexOfFrontSlash + 1);
			if (lastIndexOfFrontSlash > 0)
			{
				modification.FolderName = file.Substring(0, lastIndexOfFrontSlash);
			}
		}
	}
}
