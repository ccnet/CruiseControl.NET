using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class StringUtil
	{
		private static Regex NullStringRegex = new Regex("\0");

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
                int i = 0;
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
				builder.Append(s.ToString());
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
            return Strip(fileName,"\\", "/", ":", "*", "?", "\"", "<", ">", "|");

        }

		public static string AutoDoubleQuoteString(string value)
		{
			if (!StringUtil.IsBlank(value) && (value.IndexOf(' ') > -1) && (value.IndexOf("\"") == -1))
			{
				return string.Format("\"{0}\"", value);
			}
			return value;
		}

		public static string RemoveTrailingPathDelimeter(string directory)
		{
			return StringUtil.IsBlank(directory) ? string.Empty : directory.TrimEnd(new char[] { Path.DirectorySeparatorChar });
		}

		public static string IntegrationPropertyToString(object value)
		{
			return StringUtil.IntegrationPropertyToString(value, DEFAULT_DELIMITER);
		}

		public static string IntegrationPropertyToString(object value, string delimiter)
		{
			if ((value is string) || (value is int) || (value is Enum))
			{
				return value.ToString();
			}
			else if (value is ArrayList)
			{
				string[] tmp = (string[])((ArrayList)value).ToArray(typeof(string));
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
				throw new ArgumentException(string.Format("The IntegrationProperty type {0} is not supported yet", value.GetType().ToString()));
			}			
		}		
	}
}