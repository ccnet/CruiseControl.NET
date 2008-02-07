using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class StringUtil
	{
		private static Regex NullStringRegex = new Regex("\0");

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
	}
}