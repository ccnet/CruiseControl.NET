using System;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	/// <summary>
	/// Implementation of IHistoryParser to handle StarTeam output that
	/// describes modifications within the version control system.
	/// </summary>
	public class StarTeamHistoryParser : IHistoryParser
	{
		#region Constants

		internal readonly static string FolderInfoSeparator  = "Folder: ";
		internal readonly static string FileHistorySeparator = "----------------------------";

		// The regular expression to capture info about each folder
		public const string FolderRegEx = @"(?m:^Folder: (?<folder_name>.+)  \(working dir: (?<working_directory>.+)\)(?s:.*?)(?=^Folder: ))";


		// The regular expression to capture info about each file in a folder
		// KEEP IT AS IT IS, DO NOT ALIGN LINES
		internal readonly static string FileRegEx = @"(?m:History for: (?<file_name>.+)
Description:(?<file_description>.*)
Locked by:(?<locked_by>.*)
Status:(?<file_status>.+)
-{28}(?# the file history separator ---...)
(?s:(?<file_history>.*?))
={77}(?# the file info separator ====....))";


		// The regular expression to capture the history of a file
		// KEEP IT AS IT IS, DO NOT ALIGN LINES
		internal readonly static string FileHistoryRegEx = @"(?m:Revision: (?<file_revision>\S+) View: (?<view_name>.+) Branch Revision: (?<branch_revision>\S+)
Author: (?<author_name>.*?) Date: (?<date_string>\d{01,2}/\d{1,2}/\d\d \d{1,2}:\d\d:\d\d (A|P)M).*\n(?s:(?<change_comment>.*?))-{28})";

		#endregion

		readonly Regex folderRegex;
		readonly Regex fileRegex;
		readonly Regex historyRegex;
		DateTimeFormatInfo dfi;

		#region Constructor

		public StarTeamHistoryParser()
		{
			// Create the regular expression objects needed to parse
			// the StarTeam history log stream
			folderRegex = new Regex(StarTeamHistoryParser.FolderRegEx);        
			fileRegex = new Regex(StarTeamHistoryParser.FileRegEx);
			historyRegex = new Regex(StarTeamHistoryParser.FileHistoryRegEx);
        
			// Create DateTimeFormatInfo
			dfi = new DateTimeFormatInfo();
			dfi.AMDesignator = "AM";
			dfi.PMDesignator = "PM";
			dfi.MonthDayPattern = @"M/d/yy h:mm:ss tt";
		}


		#endregion

		#region Parsing modifications from StarTeam output

		/// <summary>
		/// Method implementaion for IHistoryParser
		/// </summary>
		/// <param name="starTeamLog"></param>
		/// <returns></returns>
		public Modification[] Parse(TextReader starTeamLog)
		{
			// Temporary holder of Modification objects
			ArrayList modList = new ArrayList();

			// Read conetent of the stream as a string
			// ASSUMPTION: entire log fits into the available memory
			String s = starTeamLog.ReadToEnd();
			
			// Append folder info separator at the end of the string so
			// that the regular expression engine does not miss the last
			// folder's information. This is required because of the way
			// the expression FolderRegEx is constructed.
			s += StarTeamHistoryParser.FolderInfoSeparator;

			// Parse the whole content to separate the info about each
			// folder and the files it has
			for (Match mFolder = folderRegex.Match(s); mFolder.Success; mFolder = mFolder.NextMatch()) 
			{
				// Working folder
				String folder = mFolder.Result("${working_directory}");

				// Scan changes for each file in the folder
				for (Match mFile = fileRegex.Match(mFolder.Value); mFile.Success; mFile = mFile.NextMatch())
				{
					// Create a Modification object for the current file
					Modification mod = new Modification();

					// Set the modification attributes
					mod.FolderName = folder;
					mod.FileName = mFile.Result("${file_name}");
					mod.Type = mFile.Result("${file_status}");
					
					// Substring that contains file history. Append a new line 
					// followed by the FileHistorySeparator so that the parse
					// engine can extract the comments for the last history
					String fileHistory = mFile.Result("${file_history}") + "\n" +
						                 StarTeamHistoryParser.FileHistorySeparator;
					
	    			// Only get the first match which describes the 
					// most recent changes
					Match mHistory = historyRegex.Match(fileHistory);
					if (mHistory.Success)
					{
						mod.EmailAddress = "N/A";
						mod.UserName = mHistory.Result("${author_name}");

						// date_string looks like "12/9/02 10:33:36 AM"
						// ASSUMPTION: StarTeam server and this application
						// runs in the same TIMEZONE
						mod.ModifiedTime = DateTime.Parse(mHistory.Result("${date_string}"), dfi);
						mod.Comment = mHistory.Result("${change_comment}");
					}
					modList.Add(mod);
				}	    
			}
			return (Modification[])modList.ToArray(typeof(Modification));
		}

		#endregion
	}
}
