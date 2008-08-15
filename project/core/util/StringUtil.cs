using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class StringUtil
	{
		private static readonly Regex NullStringRegex = new Regex("\0");

		// public for testing only
		public const string DEFAULT_DELIMITER = ",";

		public static bool Contains(string text, string fragment)
		{
			return text.IndexOf(fragment) > -1;
		}

		public static bool EqualsIgnoreCase(string a, string b)
		{
			return CaseInsensitiveComparer.Default.Compare(a, b) == 0;
		}

		public static string JoinUnique(string delimiter, params string[][] fragmentArrays)
		{
			SortedList list = new SortedList();
			foreach (string[] fragmentArray in fragmentArrays)
			{
				foreach (string fragment in fragmentArray)
				{
					if (! list.Contains(fragment))
						list.Add(fragment, fragment);
				}
			}
			StringBuilder buffer = new StringBuilder();
			foreach (string value in list.Values)
			{
				if (buffer.Length > 0)
				{
					buffer.Append(delimiter);
				}
				buffer.Append(value);
			}
			return buffer.ToString();
		}

		public static int GenerateHashCode(params string[] values)
		{
			int hashcode = 0;
			foreach (string value in values)
			{
				if (value != null)
				{
					hashcode += value.GetHashCode();
				}
			}
			return hashcode;
		}

		public static string LastWord(string input)
		{
			return LastWord(input, " .,;!?:");
		}

		public static string LastWord(string input, string separators)
		{
			if (input == null)
			{
				return null;
			}
			string[] tokens = input.Split(separators.ToCharArray());
			for (int i = tokens.Length - 1; i >= 0; i--)
			{
				if (IsWhitespace(tokens[i]) == false)
				{
					return tokens[i].Trim();
				}
			}
			return String.Empty;
		}

		public static bool IsBlank(string input)
		{
			return (input == null || input.Length == 0);
		}

		public static bool IsWhitespace(string input)
		{
			return (input == null || input.Trim().Length == 0);
		}

		public static string Strip(string input, params string[] removals)
		{
			string revised = input;
			foreach (string removal in removals)
			{
				int i;
				while ((i = revised.IndexOf(removal)) > -1)
				{
					revised = revised.Remove(i, removal.Length);
				}
			}
			return revised;
		}

		public static string[] Insert(string[] input, string insert, int index)
		{
			ArrayList list = new ArrayList(input);
			list.Insert(index, insert);
			return (string[]) list.ToArray(typeof (string));
		}

		public static string Join(string separator, params string[] strings)
		{
			StringBuilder builder = new StringBuilder();
			foreach (string s in strings)
			{
				if (IsBlank(s)) continue;
				if (builder.Length > 0) builder.Append(separator);
				builder.Append(s);
			}
			return builder.ToString();
		}

		public static string RemoveNulls(string s)
		{
			return NullStringRegex.Replace(s, string.Empty).TrimStart();
		}

		public static string StripQuotes(string filename)
		{
			return filename == null ? null : filename.Trim('"');
		}

		public static string RemoveInvalidCharactersFromFileName(string fileName)
		{
			return Strip(fileName, "\\", "/", ":", "*", "?", "\"", "<", ">", "|");
		}

		public static string AutoDoubleQuoteString(string value)
		{
			if (!IsBlank(value) && (value.IndexOf(' ') > -1) && (value.IndexOf("\"") == -1))
			{
				return string.Format("\"{0}\"", value);
			}
			return value;
		}

		public static string RemoveTrailingPathDelimeter(string directory)
		{
			return IsBlank(directory) ? string.Empty : directory.TrimEnd(new char[] {Path.DirectorySeparatorChar});
		}

		public static string IntegrationPropertyToString(object value)
		{
			return IntegrationPropertyToString(value, DEFAULT_DELIMITER);
		}

		public static string IntegrationPropertyToString(object value, string delimiter)
		{
			if ((value is string) || (value is int) || (value is Enum))
			{
				return value.ToString();
			}
			else if (value is ArrayList)
			{
				string[] tmp = (string[]) ((ArrayList) value).ToArray(typeof (string));
				if (tmp.Length > 1)
				{
					return string.Format("\"{0}\"", string.Join(delimiter, tmp));
				}
				else
				{
					return string.Join(string.Empty, tmp);
				}
			}
			else
			{
				throw new ArgumentException(
					string.Format("The IntegrationProperty type {0} is not supported yet", value.GetType()));
			}
		}

		/// <summary>
		/// Convert a stream of text lines separated with newline sequences into an XML build result.
		/// </summary>
		/// <param name="input">the text stream</param>
		/// <param name="msgLevel">the message level, if any.  Values are "Error" and "Warning".</param>
		/// <returns>the build result string</returns>
		/// <remarks>If there are any non-blank lines in the input, they are each wrapped in a
		/// <code>&lt;message&gt</code> element and the entire set is wrapped in a
		/// <code>&lt;buildresults&gt;</code> element and returned.  Each line of the input is encoded
		/// as XML CDATA rules require.  If the input is empty or contains only whitspace, an 
		/// empty string is returned.
		/// Note: If we can't manage to understand the input, we just return it unchanged.
		/// </remarks>
		public static string MakeBuildResult(string input, string msgLevel)
		{
			StringBuilder output = new StringBuilder();

			// Pattern for capturing a line of text, exclusive of the line-ending sequence.
			// A "line" is an non-empty unbounded sequence of characters followed by some 
			// kind of line-ending sequence (CR, LF, or any combination thereof) or 
			// end-of-string.
			Regex linePattern = new Regex(@"([^\r\n]+)");

			MatchCollection lines = linePattern.Matches(input);
			if (lines.Count > 0)
			{
				output.Append(Environment.NewLine);
				output.Append("<buildresults>");
				output.Append(Environment.NewLine);
				foreach (Match line in lines)
				{
					output.Append("  <message");
					if (msgLevel != "")
						output.AppendFormat(" level=\"{0}\"", msgLevel);
					output.Append(">");
					output.Append(XmlUtil.EncodePCDATA(line.ToString()));
					output.Append("</message>");
					output.Append(Environment.NewLine);
				}
				output.Append("</buildresults>");
				output.Append(Environment.NewLine);
			}
			else
				output.Append(input); // All of that stuff failed, just return our input
			return output.ToString();
		}

		public static string ArrayToNewLineSeparatedString(string[] input)
		{
			StringBuilder combined = new StringBuilder();
			foreach (string file in input)
			{
				if (combined.Length > 0) combined.Append(Environment.NewLine);
				combined.Append(file);
			}
			return combined.ToString();
		}

		public static string[] NewLineSeparatedStringToArray(string input)
		{
			string[] array = new string[0];
			if (IsBlank(input)) return array;

			ArrayList targets = new ArrayList();
			using (StringReader reader = new StringReader(input))
			{
				while (reader.Peek() >= 0)
				{
					targets.Add(reader.ReadLine());
				}
			}
			array = (string[])targets.ToArray(typeof(string));
			return array;
		}
		
	}
}
