using System;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{	
	public class VssHistoryParser : IHistoryParser
	{		
		internal const string DELIMITER_VERSIONED_START = "*****************  ";
		internal const string DELIMITER_VERSIONED_END = "  *****************";

		internal const string DELIMITER_UNVERSIONED_START = "*****  ";
		internal const string DELIMITER_UNVERSIONED_END = "  *****";

		public CultureInfo CultureInfo = CultureInfo.CurrentCulture;

		public Modification[] Parse(TextReader history, DateTime from, DateTime to)
		{
			string[] entries = this.ReadAllEntries(history);
			
			return parseModifications(entries);
		}
		

		internal Modification[] parseModifications(string[] entries)
		{
			// not every entry will yield a valid modification so we can't use
			// an array, but we can assume that most will so starting our 
			// arraylist to be at least as big as the array will save
			// some resizing
			ArrayList modifications = new ArrayList(entries.Length);

			foreach (string entry in entries) 
			{
				VSSParser parser = VSSParserFactory.CreateParser(entry, CultureInfo);
				Modification mod = parser.Parse();
				if (mod != null)
					modifications.Add(mod);
			}

			return (Modification[]) modifications.ToArray(typeof(Modification));
		}

		internal string[] ReadAllEntries(TextReader history)
		{
			ArrayList entries = new ArrayList();
			string currentLine = history.ReadLine();
			Log.Debug("VSSPublisher: " + currentLine);
			while(IsEndOfFile(currentLine) == false) 
			{
				if(IsEntryDelimiter(currentLine)) 
				{
					StringBuilder b = new StringBuilder();
					b.Append(currentLine).Append("\n");
					currentLine = history.ReadLine();
					while (!IsEntryDelimiter(currentLine))
					{
						b.Append(currentLine).Append("\n");
						currentLine = history.ReadLine();
					}
					entries.Add(b.ToString());
				}
				else 
				{
					currentLine = history.ReadLine();
				}
			}
			return (string[]) entries.ToArray(typeof(string));
		}

		internal bool IsEntryDelimiter(string line) 
		{						
			return IsEndOfFile(line) ||
				(line.StartsWith(DELIMITER_UNVERSIONED_START) && line.EndsWith(DELIMITER_UNVERSIONED_END)) ||
				line.StartsWith(DELIMITER_VERSIONED_START) && line.EndsWith(DELIMITER_VERSIONED_END);
		}

		internal bool IsEndOfFile(string line)
		{
			return line == null;
		}
	}

	internal class VSSParserFactory 
	{
		public static VSSParser CreateParser(string entry, CultureInfo cultureInfo) 
		{
			int commentIndex = entry.IndexOf("Comment");
			commentIndex = commentIndex > -1 ? commentIndex : entry.Length;
			string nonCommentEntry = entry.Substring(0, commentIndex);
			if (nonCommentEntry.IndexOf("Checked in") > -1) 
			{
				return new CheckInParser(entry, cultureInfo);
			}
			else if (nonCommentEntry.IndexOf("added") > -1) 
			{
				return new AddedParser(entry, cultureInfo);
			}
			else if (nonCommentEntry.IndexOf("deleted") > -1)
				return new DeletedParser(entry, cultureInfo);
			else if (nonCommentEntry.IndexOf("destroyed") > -1)
				return new DestroyedParser(entry, cultureInfo);

			return new NullParser(entry, cultureInfo);
		}
	}

	internal abstract class VSSParser 
	{
		private static readonly Regex REGEX_USER_DATE_LINE = 
			new Regex(@"User:(.+)Date:(.+)Time:(.+)$",RegexOptions.Multiline);
		private static readonly Regex REGEX_FILE_NAME = new Regex(@"\*+([\w\s\.]+)", RegexOptions.Multiline);

		private DateTimeFormatInfo dateTimeFormatInfo;
		protected string entry;

		internal const string DELIMITER_VERSIONED_START = "*****************  ";

		public VSSParser(string entry, CultureInfo culture)
		{
			this.entry = entry;
			this.dateTimeFormatInfo = CreateDateTimeInfo(culture);
		}

		public static DateTimeFormatInfo CreateDateTimeInfo(CultureInfo culture) 
		{
			DateTimeFormatInfo dateTimeFormatInfo = culture.DateTimeFormat.Clone() as DateTimeFormatInfo;
			dateTimeFormatInfo.AMDesignator = "a";
			dateTimeFormatInfo.PMDesignator = "p";
			return dateTimeFormatInfo;
		}

		public virtual Modification Parse() 
		{
			Modification mod = new Modification();
			SetType(mod);
			ParseUsernameAndDate(mod);
			ParseComment(mod);
			mod.FileName = this.ParseFileName();
			mod.FolderName = this.ParseFolderName();
			return mod;
		}

		internal abstract void SetType(Modification mod);

		internal abstract string ParseFileName();

		internal void ParseUsernameAndDate(Modification mod)
		{
			Match match = REGEX_USER_DATE_LINE.Match(entry);
			if (! match.Success)
			{
				throw new CruiseControlException("Invalid data retrieved from VSS.  Unable to parse username and date from text. " + entry);
			}
			
			mod.UserName = match.Groups[1].Value.Trim();
			string date = match.Groups[2].Value.Trim();
			string time = match.Groups[3].Value.Trim();

			// vss gives am and pm as a and p, so we append an m
			string suffix = (time.EndsWith("a") || time.EndsWith("p")) ? "m" : String.Empty;
			string dateAndTime = string.Format("{0};{1}{2}", date, time, suffix);
			mod.ModifiedTime = DateTime.Parse(dateAndTime, dateTimeFormatInfo);
		}

		internal void ParseComment(Modification mod)
		{		
			int index = entry.IndexOf("Comment:");
			if (index > -1) 
			{
				mod.Comment = entry.Substring(index + "Comment:".Length).Trim();
			}
		}

		internal virtual string ParseFolderName() 
		{
			string folderName = null;
			int checkinIndex = entry.IndexOf("Checked in");
			if (checkinIndex > -1) 
			{
				int startIndex = checkinIndex + "Checked in".Length;
				int length = entry.Length - startIndex;
				int commentIndex = entry.IndexOf("Comment:");
				if (commentIndex > 0)
				{
					length = commentIndex - startIndex;
				}
				folderName = entry.Substring(startIndex, length).Trim();
			}
			return folderName;
		}

		protected string ParseFileNameOther(string type) 
		{
			int timeIndex = entry.IndexOf("Time:");
			int newlineIndex = entry.IndexOf("\n", timeIndex);
			int addedIndex = entry.IndexOf(type, newlineIndex);
			string fileName = entry.Substring(newlineIndex, addedIndex - newlineIndex);
			return fileName.Trim();
		}

		internal string ParseFirstLineName() 
		{
			Match match = REGEX_FILE_NAME.Match(entry);

			return match.Groups[1].Value.Trim();
		}
	}

	internal class CheckInParser : VSSParser 
	{
		public CheckInParser(string entry, CultureInfo culture) : base(entry, culture) {}

		internal override void SetType(Modification mod) 
		{
			mod.Type = "checkin";
		}

		internal override string ParseFileName() 
		{
			return ParseFirstLineName();
		}
	}

	internal class AddedParser : VSSParser 
	{
		private readonly static string type = "added";

		public AddedParser(string entry, CultureInfo cultureInfo) : base(entry, cultureInfo) {}

		public override Modification Parse() 
		{
			Modification mod = base.Parse();
			if (mod.FileName.StartsWith("$"))
				return null;
			else
				return mod;
		}

		internal override void SetType(Modification mod) 
		{
			mod.Type = type;
		}

		internal override string ParseFileName() 
		{
			return ParseFileNameOther(type);
		}

		internal override string ParseFolderName() 
		{
			if (entry.StartsWith(DELIMITER_VERSIONED_START))
				return  "[projectRoot]";
			else
				return ParseFirstLineName();
		}
	}

	internal class DeletedParser : VSSParser 
	{
		private readonly static string type = "deleted";

		public DeletedParser(string entry, CultureInfo culture) : base(entry, culture) {}

		internal override void SetType(Modification mod) 
		{
			mod.Type = type;
		}

		internal override string ParseFileName() 
		{
			return ParseFileNameOther(type);
		}

		internal override string ParseFolderName() 
		{
			if (entry.StartsWith(DELIMITER_VERSIONED_START))
				return  "[projectRoot]";
			else
				return ParseFirstLineName();
		}
	}

	internal class DestroyedParser : VSSParser 
	{
		private readonly static string type = "destroyed";

		public DestroyedParser(string entry, CultureInfo culture) : base(entry, culture) {}

		internal override void SetType(Modification mod) 
		{
			mod.Type = type;
		}

		internal override string ParseFileName() 
		{
			return ParseFileNameOther(type);
		}

		internal override string ParseFolderName() 
		{
			if (entry.StartsWith(DELIMITER_VERSIONED_START))
				return "[projectRoot]";
			else
				return ParseFirstLineName();
		}
	}

	internal class NullParser : VSSParser 
	{
		public NullParser(string entry, CultureInfo culture) : base(entry, culture) {}

		public override Modification Parse() 
		{
			return null;
		}

		internal override void SetType(Modification mod) 
		{
		}

		internal override string ParseFileName() 
		{
			return null;
		}
	}
}
