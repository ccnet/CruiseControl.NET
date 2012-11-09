
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Tasks;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// Class with handy stirng routines
    /// </summary>
    public class StringUtil
    {
        private static readonly Regex NullStringRegex = new Regex("\0");
        private static readonly Regex urlEncodeRegex = new Regex("[^a-zA-Z0-9\\.\\-_~]", RegexOptions.Compiled);

        // public for testing only
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string DEFAULT_DELIMITER = ",";

        /// <summary>
        /// Checks if the 2 strings are equal, ignoring casing
        /// </summary>
        /// <param name="stringOne"></param>
        /// <param name="stringTwo"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(string stringOne, string stringTwo)
        {
            return CaseInsensitiveComparer.Default.Compare(stringOne, stringTwo) == 0;
        }

        /// <summary>
        /// generates a hashcode for the string array
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static int GenerateHashCode(params string[] values)
        {
            int hashcode = 0;
            foreach (string value in values)
            {
                if (value != null)
                    hashcode += value.GetHashCode();
            }
            return hashcode;
        }

        /// <summary>
        /// returns the last word of the string, using separators space ,;!?:
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string LastWord(string input)
        {
            return LastWord(input, " .,;!?:");
        }

        /// <summary>
        /// returns the last word of the string, using the specified separators 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="separators"></param>
        /// <returns></returns>
        public static string LastWord(string input, string separators)
        {
            if (input == null)
                return null;

            string[] tokens = input.Split(separators.ToCharArray());
            for (int i = tokens.Length - 1; i >= 0; i--)
            {
                if (IsWhitespace(tokens[i]) == false)
                    return tokens[i].Trim();
            }

            return String.Empty;
        }

        /// <summary>
        /// checks if a string is null or empty or is made only of spaces 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsWhitespace(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return true;
            }
            else
            {
                // Check each character to see if it is whitespace or not
                var isWhiteSpace = true;
                for (var loop = 0; loop < input.Length; loop++)
                {
                    if (!char.IsWhiteSpace(input[loop]))
                    {
                        isWhiteSpace = false;
                        break;
                    }
                }

                return isWhiteSpace;
            }
        }

        /// <summary>
        /// removes the specified strings in the string array from the input string
        /// </summary>
        /// <param name="input"></param>
        /// <param name="removals"></param>
        /// <returns></returns>
        public static string Strip(string input, params string[] removals)
        {
            string revised = input;
            foreach (string removal in removals)
            {
                int i;
                while ((i = revised.IndexOf(removal)) > -1)
                    revised = revised.Remove(i, removal.Length);
            }

            return revised;
        }


        /// <summary>
        /// joins the strings with the speficied separator.
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="strings"></param>
        /// <returns></returns>
        public static string Join(string separator, params string[] strings)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in strings)
            {
                if (string.IsNullOrEmpty(s))
                    continue;

                if (sb.Length > 0)
                    sb.Append(separator);

                sb.Append(s);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Removes the nulls.	
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string RemoveNulls(string input)
        {
            return NullStringRegex.Replace(input, string.Empty).TrimStart();
        }

        /// <summary>
        /// Removes leading and trailing quotes " 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string StripQuotes(string filename)
        {
            return filename == null ? null : filename.Trim('"');
        }

        /// <summary>
        /// removes invalid charactes from filenames, like the slash and backslash
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string RemoveInvalidCharactersFromFileName(string fileName)
        {
            return Strip(fileName, "\\", "/", ":", "*", "?", "\"", "<", ">", "|");
        }

        /// <summary>
        /// Add leading and trailing double quotes to the provided string if required.
        /// If the string contains a trailing backslash, that escape the added double quote,
        /// escape it also with another backslash.
        /// </summary>
        /// <param name="value">The string to double quote.</param>
        /// <returns>A double quoted string.</returns>
        public static string AutoDoubleQuoteString(string value)
        {
            if (!string.IsNullOrEmpty(value) && (value.IndexOf(' ') > -1) && (value.IndexOf('"') == -1))
            {
                if (value.EndsWith(@"\"))
                    value = string.Concat(value, @"\");

                return string.Concat('"', value, '"');
            }

            return value;
        }

        /// <summary>
        /// Encodes an argument for passing into a program, removes double quotes from begining and end, if it doesn't require them.
        /// </summary>
        /// <param name="original">The value that should be received by the program</param>
        /// <returns>The value which needs to be passed to the program for the original value to come through</returns>
        public static string StripThenEncodeParameterArgument(string original)
        {
            if (string.IsNullOrEmpty(original))
                return original;

            if (original == "\"\"")
            {
                return original;
            }
            else
            {
                original = StripQuotes(original);
            }

            return EncodeParameterArgument(original);
        }

        /// <summary>
        /// Encodes an argument for passing into a program
        /// </summary>
        /// <param name="original">The value that should be received by the program</param>
        /// <returns>The value which needs to be passed to the program for the original value to come through</returns>
        public static string EncodeParameterArgument(string original)
        {
            if (string.IsNullOrEmpty(original))
                return original;

            string value = Regex.Replace(original, @"(\\*)" + "\"", @"$1\$0");
            value = Regex.Replace(value, @"^(.*(?:\s|,).*?)(\\*)$", "\"$1$2$2\"");
            return value;
        }

        /// <summary>
        /// Removes the trailing path delimeter.	
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string RemoveTrailingPathDelimeter(string directory)
        {
            return string.IsNullOrEmpty(directory) ? string.Empty : directory.TrimEnd(new char[] { Path.DirectorySeparatorChar });
        }

        /// <summary>
        /// Integrations the property to string.	
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string IntegrationPropertyToString(object value)
        {
            return IntegrationPropertyToString(value, DEFAULT_DELIMITER);
        }

        /// <summary>
        /// Integrations the property to string.	
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string IntegrationPropertyToString(object value, string delimiter)
        {
            if (value == null)
                return null;

            if ((value is string) || (value is int) || (value is Enum))
                return value.ToString();

            if (value is Guid)
            {
                var x =  (Guid) value;
                return x.ToString("N");
            }

            var dummy = value as ArrayList;
            if (dummy != null)
            {
                string[] tmp = (string[])dummy.ToArray(typeof(string));
                if (tmp.Length <= 1)
                    return string.Join(string.Empty, tmp);

                return string.Format(System.Globalization.CultureInfo.CurrentCulture,"\"{0}\"", string.Join(delimiter, tmp));
            }

            throw new ArgumentException(
                string.Format(System.Globalization.CultureInfo.CurrentCulture,"The IntegrationProperty type {0} is not supported yet", value.GetType()), "value");
        }

        /// <summary>
        /// Convert a stream of text lines separated with newline sequences into an XML build result.
        /// </summary>
        /// <param name="input">the text stream</param>
        /// <param name="msgLevel">the message level, if any.  Values are "Error" and "Warning".</param>
        /// <param name="task">the task to serialize alongside the result, if any</param>
        /// <returns>the build result string</returns>
        /// <remarks>If there are any non-blank lines in the input, they are each wrapped in a
        /// <code>&lt;message&gt;</code> element and the entire set is wrapped in a
        /// <code>&lt;buildresults&gt;</code> element and returned.  Each line of the input is encoded
        /// as XML CDATA rules require.  If the input is empty or contains only whitspace, an 
        /// empty string is returned.
        /// Note: If we can't manage to understand the input, we just return it unchanged.
        /// </remarks>
        public static string MakeBuildResult(string input, string msgLevel, TaskBase task)
        {
            StringBuilder sb = new StringBuilder();

            // Pattern for capturing a line of text, exclusive of the line-ending sequence.
            // A "line" is an non-empty unbounded sequence of characters followed by some 
            // kind of line-ending sequence (CR, LF, or any combination thereof) or 
            // end-of-string.
            Regex linePattern = new Regex(@"([^\r\n]+)");

            MatchCollection lines = linePattern.Matches(input);
            if (lines.Count > 0)
            {
                sb.Append(Environment.NewLine);
                sb.Append("<buildresults>");
                sb.Append(Environment.NewLine);

                if (task != null)
                {
                    StringWriter buffer = new StringWriter();
                    new ReflectorTypeAttribute("task").Write(new XmlTextWriter(buffer), task);
                    sb.Append(buffer.ToString());
                    sb.Append(Environment.NewLine);
                }

                foreach (Match line in lines)
                {
                    sb.Append("  <message");
                    if (!(msgLevel != null && msgLevel.Length == 0))
                        sb.AppendFormat(CultureInfo.CurrentCulture, " level=\"{0}\"", msgLevel);
                    sb.Append(">");
                    sb.Append(XmlUtil.EncodePCDATA(line.ToString()));
                    sb.Append("</message>");
                    sb.Append(Environment.NewLine);
                }
                sb.Append("</buildresults>");
                sb.Append(Environment.NewLine);
            }
            else
                sb.Append(input); // All of that stuff failed, just return our input
            return sb.ToString();
        }

        /// <summary>
        /// Convert a stream of text lines separated with newline sequences into an XML build result.
        /// </summary>
        /// <param name="input">the text stream</param>
        /// <param name="msgLevel">the message level, if any.  Values are "Error" and "Warning".</param>
        /// <returns>the build result string</returns>
        /// <remarks>If there are any non-blank lines in the input, they are each wrapped in a
        /// <code>&lt;message&gt;</code> element and the entire set is wrapped in a
        /// <code>&lt;buildresults&gt;</code> element and returned.  Each line of the input is encoded
        /// as XML CDATA rules require.  If the input is empty or contains only whitspace, an 
        /// empty string is returned.
        /// Note: If we can't manage to understand the input, we just return it unchanged.
        /// </remarks>
        public static string MakeBuildResult(string input, string msgLevel)
        {
            return MakeBuildResult(input, msgLevel, null);
        }

        /// <summary>
        /// Arrays to new line separated string.	
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string ArrayToNewLineSeparatedString(string[] input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string file in input)
            {
                if (sb.Length > 0)
                    sb.Append(Environment.NewLine);
                sb.Append(file);
            }

            return sb.ToString();
        }

        /// <summary>
        /// News the line separated string to array.	
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string[] NewLineSeparatedStringToArray(string input)
        {
            if (string.IsNullOrEmpty(input))
                return new string[0];

            List<string> targets = new List<string>();
            using (StringReader reader = new StringReader(input))
            {
                while (reader.Peek() >= 0)
                {
                    targets.Add(reader.ReadLine());
                }
            }

            return targets.ToArray();
        }



        /// <summary>
        /// returns the elements of the array as a string, delimited with the default delimitor
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string GetArrayContents(Array array)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();

            foreach (object o in array)
            {
                result.AppendFormat("{0}{1} ", o.ToString(), DEFAULT_DELIMITER);
            }

            if (result.Length > 0)
            {
                result.Length -= 2;
            }

            return result.ToString();

        }

        /// <summary>
        /// Correctly encode a name for a URL.
        /// </summary>
        /// <param name="name">The name to encode.</param>
        /// <returns>The encoded name.</returns>
        /// <remarks>
        /// <para>
        /// HttpUtility.UrlEncode does not correctly encode for a URL, spaces get converted into 
        /// pluses, which can cause security errors.
        /// </para>
        /// <para>
        /// This method will encode characters according to RFC 3986. This means only the following 
        /// characters are allowed un-encoded:
        /// </para>
        /// <para>
        /// A B C D E F G H I J K L M N O P Q R S T U V W X Y Z a b c d e f g h i j k l m n o p q r s 
        /// t u v w x y z 0 1 2 3 4 5 6 7 8 9 - _ . ~
        /// </para>
        /// <para>
        /// However, since the encoding only uses two-hex digits, it is not possible to encode non-ASCII
        /// characters using this approach. Therefore we are using the RFC 3986 recommendation and assuming
        /// the string will be using UTF-8 encoding and leaving the characters as they are.
        /// </para>
        /// </remarks>
        public static string UrlEncodeName(string name)
        {
            var encodedName = urlEncodeRegex.Replace(name, (match) =>
            {
                var charValue = (int)match.Value[0];
                var value = charValue >= 255 ? match.Value : "%" + string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0:x2}", charValue);
                return value;
            });
            return encodedName;
        }
    }
}
