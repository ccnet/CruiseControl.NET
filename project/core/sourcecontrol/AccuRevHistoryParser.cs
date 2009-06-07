using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	/// <summary>
	/// Parser for AccuRev Inc.'s (www.accurev.com) eponymous source code control product's
	/// "accurev hist" command output.
	/// </summary>
	/// <remarks>
	/// This code is based on code\sourcecontrol\ClearCase.cs.
	/// </remarks>
	public class AccuRevHistoryParser : IHistoryParser
	{
		/*
		 * Format of "accurev hist" output (uses "\" on Windows, "/" on Unix):
		 * transaction 12245; add; 2006/11/22 11:11:00 ; user: rayoub
		 *   # New Project for accessing SICS/nt web services
		 *   \.\Dev\Server\SICSInterface\Properties\AssemblyInfo.cs 62/1 (62/1)
		 *   ancestor: (none - initial version)
		 * 
		 * transaction 12244; add; 2006/11/22 11:10:44 ; user: rayoub
		 *   # New Project for accessing SICS/nt web services
		 *   \.\Dev\Server\SICSInterface 62/2 (62/2)
		 *   ancestor: 62/1
		 *   type: dir
		 * 
		 *   \.\Dev\Server\SICSInterface\App.config 62/1 (62/1)
		 *   ancestor: (none - initial version)
		 * 
		 *   \.\Dev\Server\SICSInterface\CommonTypes.cs 62/1 (62/1)
		 *   ancestor: (none - initial version)
		 * 
		 *   \.\Dev\Server\SICSInterface\Connection.cs 62/1 (62/1)
		 *   ancestor: (none - initial version)
		 * ...  
		 */

		/// <summary>
		/// The starting date and time for the range of modifications we want.
		/// </summary>
		private DateTime fromDateTime;
		
		/// <summary>
		/// The ending date and time for the range of modifications we want.
		/// </summary>
		private DateTime toDateTime;
		
		/// <summary>
		/// The list of modifications we find.
		/// </summary>
		private ArrayList modificationList;
		
		/// <summary>
		/// A temporary Modification, used to make new modificationList entries.
		/// </summary>
		private Modification modificationTemplate;

		public AccuRevHistoryParser()
		{
		}

		/// <summary>
		/// Construct and return an array of Modifications describing the changes in
		/// the AccuRev workspace, based on the output of the "accurev hist" command.
		/// </summary>
		/// <param name="history">the stream of "accurev hist" command output</param>
		/// <param name="from">the starting date and time for the range of modifications we want.</param>
		/// <param name="to">the ending date and time for the range of modifications we want.</param>
		/// <returns>the changes in the specified time range.</returns>
		public Modification[] Parse(TextReader history, DateTime from, DateTime to)
		{
			string line;
			fromDateTime = from;
			toDateTime = to;
			modificationList = new ArrayList();
			modificationTemplate = null;
			Regex firstTokenPattern = new Regex(@"^\s*(\S+)");
            Regex absolutePathPrefixPattern = new Regex(@"(\\|/)\.(\\|/)");
            Regex commentTextPattern = new Regex(@"^\s*# (.*)$");
			
			while ((line = history.ReadLine()) != null)
			{
				// Get the first non-whitespace token in the line and decide how to parse based on it:
				Match parsed = firstTokenPattern.Match(line);
				string firstToken = "";
                if (parsed.Success)
                    firstToken = parsed.Groups[1].ToString();
                switch (firstToken)
                {
                    case "transaction":
                        ParseTransaction(line);
                        break;
                    case "#":
                        // String together the lines of the comment, with a newline sequence between 
                        // adjacent lines.
                        if (modificationTemplate.Comment != null)
                            modificationTemplate.Comment += System.Environment.NewLine;
                        Match commentText = commentTextPattern.Match(line);
                        if (commentText.Groups.Count != 0)
                            modificationTemplate.Comment += commentText.Groups[1].ToString();
                        break;
                    case "ancestor:":
                    case "type:":
                    case "":
                        // Ignore uninteresting lines.
                        break;
                    default:
                        if (absolutePathPrefixPattern.IsMatch(firstToken))
                            ParseFileLine(line);
                        else
                            Log.Error(string.Format("Unrecognized line in AccuRev \"accurev hist\" output: {0}", line));
                        break;
                }
            }
			Log.Debug(string.Format("AccuRev reported {0} modifications", modificationList.Count));
			return (Modification[]) modificationList.ToArray(typeof (Modification));
		}

		/// <summary>
		/// Parse a transaction header and set up a new template for modifications created by this transaction.
		/// </summary>
		/// <param name="line">the transaction header line to parse.</param>
		/// <remarks>
		/// Line format:
		/// <br/>
		/// transaction __transNum__; __operationType__; yyyy/mm/dd hh:mm:ss ; user: __userid__
		/// </remarks>
		private void ParseTransaction(string line)
		{
			// This is a new transaction, so start with a new template for modifications:
			modificationTemplate = new Modification();				
			
			// Parsing regular expression.  Groups used are:
			//	1: transaction number (__transNum__)
			//	2: operation type (__operationType__)
			//	3: date and time (yyyy/mm/dd hh:mm:ss)
			//	4: userid (__userid__)
			Regex pattern = new Regex(@"^\s*transaction\s+(\d*)\s*;\s+(\S+)s*;\s+(\d{4}/\d{2}/\d{2} \d{2}:\d{2}:\d{2})\s*;\s+user:\s+(\S+)\s*$");

			Match parsed = pattern.Match(line);
			if (parsed.Success)
			{
				// Parsing worked, fill the transaction summary into the template modification:
				GroupCollection tokens = parsed.Groups;
				modificationTemplate.ChangeNumber = tokens[1].ToString();
				modificationTemplate.Type = tokens[2].ToString();
				modificationTemplate.ModifiedTime = DateTime.Parse(tokens[3].ToString());
				modificationTemplate.UserName = tokens[4].ToString();
            }
			else
			{
                Log.Error(string.Format("Illegal transaction line in AccuRev \"accurev hist\" output: {0}", line));
			}
		}

		/// <summary>
		/// Parse a file detail line and remember the modification it describes.
		/// </summary>
		/// <remarks>
		/// The fileid after the leading "\.\" (or "/./") is a fully relative Windows (or Unix) 
		/// pathname, so it can contain any other legal Windows filename characters, including 
		/// spaces.  Unfortunately it is not enquoted or escaped, so be careful when parsing 
		/// the line.
		/// <br/>
		/// Line format is:
		/// <br/>
		///    |.|__dir1__|...|__dirn__|__filename__ __real_stream__/__real_revision__ (__virtual_stream__/__virtual_revision__[,__virtual_stream__/__virtual_revision__][,...])
		/// <br/>
		/// where any "|" delimiter can be either "/" or "\" and can vary in the same line.
		/// </remarks>
		/// <param name="line">the file detail line to parse.</param>
		private void ParseFileLine(string line)
		{

			// Pattern for line.  Groups used are:
			//	3: dirname (__dir1__ ... __dirn__)
			//	6: filename (__filename__)
			//	7: real version id (__real_stream__/__real_revision__)
			Regex pattern = new Regex(
										@"^" +							// Start of line
										@"\s+" +						// Leading spaces
										@"(\\|/)\.(\\|/)" +				// Leading "\.\" or "/./"
										@"(([^\\/]*(\\|/))*)?" +		// 3: Fully-qualified directory name, if any
										@"(.*)" +						// 6: File name
										@"\s+" +
										@"(\S*/\S*)" +					// 7: Real version id
										@"\s+" +
										@"\(\S*/\S*(,\S*/\S*)*\)?\s*" +	// Virtual version id(s), if any
										@"\s*" +
										@"$"							// End of line
									);
			Match results = pattern.Match(line);
			if (results.Success)
			{
				// Parsing worked, get the tokens we care about and update the template for our modifications
				GroupCollection tokens = results.Groups;
				modificationTemplate.Version = tokens[7].ToString();
				modificationTemplate.FolderName = tokens[3].ToString();
				modificationTemplate.FileName = tokens[6].ToString();
				// Add this to the modification list if it's inside our time range.
				if ((modificationTemplate.ModifiedTime >= fromDateTime) && 
				    (modificationTemplate.ModifiedTime <= toDateTime))
					AddModification(modificationTemplate);
			}
			else
			{
				Log.Error(String.Format("Illegal file detail line in AccuRev \"accurev hist\" output: {0}", line));
			}
		}

		/// <summary>
		/// Record a modification like this one.
		/// </summary>
		/// <remarks>
		/// This might be better done by a Clone() capability, or through reflection.  
		/// But for now, it's good enough to copy every field explicitly.
		/// </remarks> 
		/// <param name="template">the template Modification to clone.</param>
		private void AddModification(Modification template)
		{
			Modification entry = new Modification();

			entry.ChangeNumber = template.ChangeNumber;
			entry.Comment = template.Comment;
			entry.EmailAddress = template.EmailAddress;
			entry.FileName = template.FileName;
			entry.FolderName = template.FolderName;
			entry.ModifiedTime = template.ModifiedTime;
			entry.Type = template.Type;
			entry.Url = template.Url;
			entry.UserName = template.UserName;
			entry.Version = template.Version;

			Log.Debug(string.Format("Added a modification: {0}", entry));
			modificationList.Add(entry);
		}

	}
}
