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
				modification.ModifiedTime = ParseDate(match.Groups["ModifiedTime"].Value.Trim());
				modification.Comment = match.Groups["Comment"].Value.Trim();
			}			
		}

        // Dates returned from MKS seem to be in format Aug 26, 2005 - 5:32 AM but I haven't been able to verify this for all locales
        // This format is not supported by DateTime.Parse under .NET 2.0. So we TryParseExact to see if just this format can be read.
	    private static DateTime ParseDate(string dateString)
	    {
	        DateTime date;
            if (DateTime.TryParseExact(dateString, "MMM d, yyyy - h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out date))
                return date;
	        return Convert.ToDateTime(dateString);
	    }

	    private static Modification CreateModification(Match match)
		{
			Modification modification = new Modification();
			ParseFileAndFolderName(match.Groups["Filename"].Value.Trim(), modification);
			modification.Version = match.Groups["NewRevision"].Value.Trim();
			modification.Type = ParseModificationType(match.Groups["ModificationType"].Value.Trim());
			return modification;
		}

		private static string ParseModificationType(string modificationType)
		{
			return ("Revision" == modificationType) ? "Modified" : modificationType;
		}

		private static void ParseFileAndFolderName(string file, Modification modification)
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
