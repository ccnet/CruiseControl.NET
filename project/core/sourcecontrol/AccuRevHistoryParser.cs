using System;
using System.Collections;
using System.IO;
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

		/// <summary>
		/// The prefix AccuRev uses for an "absolute" (i.e., depot-relative) pathname. 
		/// </summary>
		/// <remarks>
		/// Either "  \.\" (Windows) or "  /./" (Unix).  Our constructor sets this accordingly.
		/// </remarks>		
		private String absolutePathPrefix;
		
		/// <summary>
		/// The character AccuRev uses for a pathname separator.
		/// </summary>
		/// <remarks>
		/// Either "\" (Windows) or "/" (Unix).  Our constructor sets this accordingly.
		/// </remarks>		
		private char dirSeparator;
		
		public AccuRevHistoryParser()
		{
			/* Set the depot-relative path prefix according to what kind of system we're running on. */
			dirSeparator = (new ExecutionEnvironment()).DirectorySeparator;
			absolutePathPrefix = String.Format("  {0}.{0}", dirSeparator);
		}
		/// <summary>
		/// Construct and return an array of Modifications describing the changes in
		/// the AccuRev workspace, based on the output of the "accurev hist" command.
		/// </summary>
		/// <param name="history">the stream of "accurev hist" command output</param>
		/// <param name="from">the starting date and time for the range of modifications we want.</param>
		/// <param name="to">the ending date and time for the range of modifications we want./param>
		/// <returns>the changes in the specified time range.</returns>
		public Modification[] Parse(TextReader history, DateTime from, DateTime to)
		{
			string line;
			fromDateTime = from;
			toDateTime = to;
			modificationList = new ArrayList();
			modificationTemplate = null;
			
			while ((line = history.ReadLine()) != null)
			{
				if (line.StartsWith("transaction "))
					ParseTransaction(line);
				else if (line.StartsWith("  #"))
				{
					// String together the lines of the comment, with a CRLF between adjacent lines.
					if (modificationTemplate.Comment != null)
						modificationTemplate.Comment += "\r\n ";
					modificationTemplate.Comment += line.Substring(4);
				}
				else if (line.StartsWith(absolutePathPrefix))
					ParseFileLine(line);
				else if (line.StartsWith("  ancestor:") || line.StartsWith("  type:") || line.Trim().Equals(""))
				{
					// Ignore uninteresting lines.
				}
				else
					Log.Error(string.Format("Unrecognized line in AccuRev \"accurev hist\" output: {0}", line));
					
			}
			Log.Debug(string.Format("AccuRev reported {0} modifications", modificationList.Count));
			return (Modification[]) modificationList.ToArray(typeof (Modification));
		}

		/// <summary>
		/// Parse a transaction header and set up a new template for modifications created by this transaction.
		/// </summary>
		/// <param name="line">the transaction header line to parse.</param>
		private void ParseTransaction(string line)
		{
			// Line format:
			// transaction __transNum__; __operationType__; yyyy/mm/dd hh:mm:ss ; user: __userid__
			int tokenStart = 0, tokenEnd;
			modificationTemplate = new Modification();				// Start with a clean slate
		
			// Parse "transaction __transNum__;"
			tokenStart = line.IndexOf(' ', tokenStart) + 1;
			tokenEnd = line.IndexOf(';', tokenStart);
			modificationTemplate.ChangeNumber = int.Parse(line.Substring(tokenStart, tokenEnd - tokenStart));
					
			// Parse " __operationType__;"
			tokenStart = tokenEnd + 1;
			tokenEnd = line.IndexOf(';', tokenStart);
			modificationTemplate.Type = line.Substring(tokenStart, tokenEnd - tokenStart).Trim();
					
			// Parse " yyyy/mm/dd hh:mm:ss ;" and convert to a timestamp
			tokenStart = tokenEnd + 1;
			tokenEnd = line.IndexOf(';', tokenStart);
			modificationTemplate.ModifiedTime = DateTime.Parse(line.Substring(tokenStart,
			                                                                  tokenEnd - tokenStart).Trim());
					
			// Parse " user: __userid__"
			tokenStart = line.IndexOf(":", tokenEnd + 1);
			tokenEnd = line.Length;
			modificationTemplate.UserName = line.Substring(tokenStart + 1, tokenEnd - (tokenStart + 1)).Trim();
		}

		/// <summary>
		/// Parse a file detail line and remember the modification it describes.
		/// </summary>
		/// <remarks>
		/// The fileid after the leading "\.\" (or "/./") is a fully relative Windows (or Unix) 
		/// pathname, so it can contain any other legal Windows filename characters, including 
		/// spaces.  Unfortunately it is not enquoted or escaped, so be careful when parsing 
		/// the line.
		/// </remarks>
		/// <param name="line">the file detail line to parse.</param>
		private void ParseFileLine(string line)
		{
			// Line format is:
			//    \.\__dir1__\...\__dirn__\__filename__ __real_stream__/__real_revision__ (__virtual_stream__/__virtual_revision__[,__virtual_stream__/__virtual_revision__][,...])
			// or:
			//    /./__dir1__/.../__dirn__/__filename__ __real_stream__/__real_revision__ (__virtual_stream__/__virtual_revision__[,__virtual_stream__/__virtual_revision__][,...])
			// i.e., in regex terms: "  " ("\\"|"/") "." ("\\"|"/") (DIRNAME ("\\"|"/"))* (FILENAME) " " (REALSTREAM) "/" (REALREVISION) (" (" (VIRTUALSTREAM "/" VIRTUALREVISION)+ ")" )?
			int i;		
			string tempLine;
			
			tempLine = line.Substring(absolutePathPrefix.Length).TrimStart();		// Drop "  \.\" or "  /./"
			/* Remove any virtual version identifiers ("(nnn/nnn nnn/nnn ...)"). */
			if (tempLine.EndsWith(")"))
				tempLine = tempLine.Substring(0, tempLine.LastIndexOf('(') - 1).TrimEnd();
			/* Extract the real version identifier ("nnn/nnn"). */
			i = tempLine.LastIndexOf(' ');
			modificationTemplate.Version = tempLine.Substring(i + 1);
			tempLine = tempLine.Substring(0, i);
			/* Extract the file name and folder (directory) name from the path. */
			i = tempLine.LastIndexOf(dirSeparator);
			if ( i > -1 )
			{
				modificationTemplate.FolderName = tempLine.Substring(0, i);
				modificationTemplate.FileName = tempLine.Substring(i + 1);
			}
			else
			{
				modificationTemplate.FolderName = string.Empty;
				modificationTemplate.FileName = tempLine;
			}
			// We've got all the info, add it to the modification list if it's inside our time range.
			if ((modificationTemplate.ModifiedTime >= fromDateTime) && 
					(modificationTemplate.ModifiedTime <= toDateTime))
				AddModification(modificationTemplate);		
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
