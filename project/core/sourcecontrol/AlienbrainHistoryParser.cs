using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	public class AlienbrainHistoryParser : IHistoryParser
	{
		public static readonly string FILE_REGEX = ".*|.*|.*|.*|.*|.*|.*|.*";
		public static readonly char DELIMITER = '|';

		public Modification[] Parse(TextReader history, DateTime from, DateTime to)
		{
			string historyLog = history.ReadToEnd();

			Regex regex = new Regex(Alienbrain.NO_CHANGE);
			if (regex.Match(historyLog).Success == true)
			{
				return new Modification[0];
			}

			regex = new Regex(FILE_REGEX);
            var result = new List<Modification>();
			string oldfile = ",";

			for (Match match = regex.Match(historyLog); match.Success; match = match.NextMatch())
			{
				string[] modificationParams = AllModificationParams(match.Value);
				if (modificationParams.Length > 1)
				{
					string file = modificationParams[1];
					if (file != oldfile)
					{
						result.Add(ParseModification(modificationParams));
						oldfile = file;
					}
				}
			}
			return result.ToArray();
		}

		// strip carriage return, new line, and all leading and trailing characters from parameters
		public string[] AllModificationParams(string matchedLine)
		{
			matchedLine = matchedLine.Replace("\n", "");
			matchedLine = matchedLine.Replace("\r", "");
			string[] modificationParams = matchedLine.Split(DELIMITER);
			for (int ii = 0; ii < modificationParams.Length; ii++)
			{
				modificationParams[ii] = modificationParams[ii].Trim(' ');
			}
			return modificationParams;
		}

		// #CheckInComment#|#Name#|#DbPath#|#SCIT#|#Mime Type#|#LocalPath#|#Changed By#|#NxN_VersionNumber#
		public Modification ParseModification(string[] modificationParams)
		{
			Modification modification = new Modification();
			modification.Comment = modificationParams[0];
			modification.FileName = modificationParams[1];
			modification.FolderName = "ab:/" + modificationParams[2].Replace("/" + modificationParams[1], "");
			modification.ModifiedTime = DateTime.FromFileTime(long.Parse(modificationParams[3]));
			modification.Type = modificationParams[4];
			modification.Url = modificationParams[5];
			modification.UserName = modificationParams[6];
			modification.Version = modificationParams[7];

			return modification;
		}
	}
}