using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	public class MksHistoryParser : IHistoryParser
	{
		public static readonly string FILE_REGEX = "CCNet-1.*\n";

		public MksHistoryParser()
		{
		}

		public Modification[] Parse(TextReader history, DateTime from, DateTime to)
		{
			string historyLog = history.ReadToEnd();
			Regex regex = new Regex(FILE_REGEX);
			ArrayList result = new ArrayList();
			string oldfile = ",";

			for (Match match = regex.Match(historyLog); match.Success; match = match.NextMatch())
			{
				string[] modificationParams = AllModificationParams(match.Value);
				string file = modificationParams[1];
				if (file != oldfile)
				{
					result.Add(ParseModification(modificationParams));
					oldfile = file;
				}
			}
			return (Modification[]) result.ToArray(typeof (Modification));
		}

		// strip carriage return, new line, and all leading and trailing characters from parameters
		public string[] AllModificationParams(string matchedLine)
		{
			matchedLine = matchedLine.Replace("\n", "");
			matchedLine = matchedLine.Replace("\r", "");
			string[] modificationParams = matchedLine.Split(Mks.DELIMITER);
			for (int ii = 0; ii < modificationParams.Length; ii++)
			{
				modificationParams[ii] = modificationParams[ii].Trim(' ');
			}
			return modificationParams;
		}

		private Modification ParseModification(string[] modificationParams)
		{
			Modification modification = new Modification();
			int lastIndexOfBackslash = modificationParams[1].LastIndexOf("\\");
			modification.FileName = modificationParams[1].Substring(modificationParams[1].LastIndexOf("\\") + 1);
			if (lastIndexOfBackslash > 0)
			{
				modification.FolderName = modificationParams[1].Substring(0, (modificationParams[1].LastIndexOf("\\")));
			}
			modification.ChangeNumber = int.Parse(modificationParams[2]);
			modification.ModifiedTime = DateTime.Parse(modificationParams[3]);
			modification.UserName = modificationParams[4];
			modification.Comment = modificationParams[5];
			return modification;
		}
	}
}