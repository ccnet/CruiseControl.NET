using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	public class VssHistoryParser : IHistoryParser
	{
		private const string DELIMITER_VERSIONED_START = "*****************  ";
		private const string DELIMITER_VERSIONED_END = "  *****************";

		private const string DELIMITER_UNVERSIONED_START = "*****  ";
		private const string DELIMITER_UNVERSIONED_END = "  *****";

		private IVssLocale locale;

		public VssHistoryParser(IVssLocale locale)
		{
			this.locale = locale;
		}

		public Modification[] Parse(TextReader history, DateTime from, DateTime to)
		{
			string[] entries = this.ReadAllEntries(history);
			return ParseModifications(entries);
		}

		internal Modification[] ParseModifications(string[] entries)
		{
			// not every entry will yield a valid modification so we can't use
			// an array, but we can assume that most will so starting our 
			// arraylist to be at least as big as the array will save
			// some resizing
			ArrayList modifications = new ArrayList(entries.Length);

			foreach (string entry in entries)
			{
				VSSParser parser = VSSParserFactory.CreateParser(entry, locale);
				Modification mod = parser.Parse();
				if (mod != null)
					modifications.Add(mod);
			}

			return (Modification[]) modifications.ToArray(typeof (Modification));
		}

		internal string[] ReadAllEntries(TextReader history)
		{
			ArrayList entries = new ArrayList();
			string currentLine = history.ReadLine();
			while (IsEndOfFile(currentLine) == false)
			{
				if (IsEntryDelimiter(currentLine))
				{
					StringBuilder builder = new StringBuilder();
					builder.Append(currentLine).Append("\n");
					currentLine = history.ReadLine();
					while (!IsEntryDelimiter(currentLine))
					{
						builder.Append(currentLine).Append("\n");
						currentLine = history.ReadLine();
					}
					entries.Add(builder.ToString());
				}
				else
				{
					currentLine = history.ReadLine();
				}
			}
			return (string[]) entries.ToArray(typeof (string));
		}

		internal bool IsEntryDelimiter(string line)
		{
			return IsEndOfFile(line) || 
				(line.StartsWith(DELIMITER_UNVERSIONED_START) && line.EndsWith(DELIMITER_UNVERSIONED_END)) || 
				line.StartsWith(DELIMITER_VERSIONED_START) && line.EndsWith(DELIMITER_VERSIONED_END);
		}

		private bool IsEndOfFile(string line)
		{
			return line == null;
		}

		public IVssLocale Locale
		{
			get { return locale; }
			set { locale = value; }
		}
	}

	internal class VSSParserFactory
	{
		public static VSSParser CreateParser(string entry, IVssLocale locale)
		{
			string vssKeyworkdLine = ReadVSSKeywordLine(entry);
			if (vssKeyworkdLine.IndexOf(locale.CheckedInKeyword) > -1)
			{
				return new CheckInParser(entry, locale);
			}
			else if (vssKeyworkdLine.IndexOf(locale.AddedKeyword) > -1)
			{
				return new AddedParser(entry, locale);
			}
			else if (vssKeyworkdLine.IndexOf(locale.DeletedKeyword) > -1)
				return new DeletedParser(entry, locale);
			else if (vssKeyworkdLine.IndexOf(locale.DestroyedKeyword) > -1)
				return new DestroyedParser(entry, locale);

			return new NullParser(entry, locale);
		}

		private static string ReadVSSKeywordLine(string entry)
		{
			StringReader reader = new StringReader(entry);
			reader.ReadLine();
			reader.ReadLine();
			string nonCommentEntry = reader.ReadLine() + Environment.NewLine + reader.ReadLine();
			return nonCommentEntry;
		}
	}

	internal abstract class VSSParser
	{
		private Regex REGEX_USER_DATE_LINE;
		private static readonly Regex REGEX_FILE_NAME = new Regex(@"\*+([\w\s\.-]+)", RegexOptions.Multiline);

		protected string entry;
		protected IVssLocale locale;

		internal const string DELIMITER_VERSIONED_START = "*****************  ";

		public VSSParser(string entry, IVssLocale locale)
		{
			this.entry = entry;
			this.locale = locale;
			string regex = string.Format(@"{0}:(.+){1}:(.+){2}:(.+)$", locale.UserKeyword, locale.DateKeyword, locale.TimeKeyword);
			REGEX_USER_DATE_LINE = new Regex(regex, RegexOptions.Multiline);
			this.locale = locale;
		}

		public virtual Modification Parse()
		{
			Modification mod = new Modification();
			mod.Type = Keyword;
			ParseUsernameAndDate(mod);
			ParseComment(mod);
			mod.FileName = ParseFileName();
			mod.FolderName = ParseFolderName();
			return mod;
		}

		public abstract string Keyword { get; }

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

			mod.ModifiedTime = locale.ParseDateTime(date, time);
		}

		internal void ParseComment(Modification mod)
		{
			string comment = locale.CommentKeyword + ":";
			int index = entry.IndexOf(comment);
			if (index > -1)
			{
				mod.Comment = entry.Substring(index + comment.Length).Trim();
			}
		}

		internal virtual string ParseFolderName()
		{
			string checkedin = locale.CheckedInKeyword;
			string comment = locale.CommentKeyword + ":";

			string folderName = null;
			int checkinIndex = entry.IndexOf(checkedin);
			if (checkinIndex > -1)
			{
				int startIndex = checkinIndex + checkedin.Length;
				int length = entry.Length - startIndex;
				int commentIndex = entry.IndexOf(comment);
				if (commentIndex > 0)
				{
					length = commentIndex - startIndex;
				}
				folderName = entry.Substring(startIndex, length).Trim();
			}
			return folderName;
		}

		protected string ParseFileNameOther()
		{
			try
			{
				int timeIndex = entry.IndexOf(locale.TimeKeyword + ":");
				int newlineIndex = entry.IndexOf("\n", timeIndex);

				int addedIndex = entry.IndexOf(Keyword, newlineIndex);
				string fileName = entry.Substring(newlineIndex, addedIndex - newlineIndex);
				return fileName.Trim();
			}
			catch (Exception e)
			{
				throw new CruiseControlException(String.Format("ParseFileNameOther failed on string \"{0}\", type {1}", entry, Keyword), e);
			}
		}

		internal string ParseFirstLineName()
		{
			Match match = REGEX_FILE_NAME.Match(entry);
			return match.Groups[1].Value.Trim();
		}
	}

	internal class CheckInParser : VSSParser
	{
		public CheckInParser(string entry, IVssLocale locale) : base(entry, locale)
		{
		}

		public override string Keyword
		{
			get { return locale.CheckedInKeyword; }
		}

		internal override string ParseFileName()
		{
			return ParseFirstLineName();
		}
	}

	internal class AddedParser : VSSParser
	{
		public AddedParser(string entry, IVssLocale locale) : base(entry, locale)
		{
		}

		public override Modification Parse()
		{
			Modification mod = base.Parse();
			if (mod.FileName.StartsWith("$"))
				return null;
			else
				return mod;
		}

		public override string Keyword
		{
			get { return locale.AddedKeyword; }
		}

		internal override string ParseFileName()
		{
			return ParseFileNameOther();
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
		public DeletedParser(string entry, IVssLocale locale) : base(entry, locale)
		{
		}

		public override string Keyword
		{
			get { return locale.DeletedKeyword; }
		}

		internal override string ParseFileName()
		{
			return ParseFileNameOther();
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
		public DestroyedParser(string entry, IVssLocale locale) : base(entry, locale)
		{
		}

		public override string Keyword
		{
			get { return locale.DestroyedKeyword; }
		}

		internal override string ParseFileName()
		{
			return ParseFileNameOther();
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
		public NullParser(string entry, IVssLocale locale) : base(entry, locale)
		{
		}

		public override string Keyword
		{
			get { return null; }
		}

		public override Modification Parse()
		{
			return null;
		}

		internal override string ParseFileName()
		{
			return null;
		}
	}
}