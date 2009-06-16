/*
 * Copyright  2002-2004 The Apache Software Foundation
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

/*
 *  This is a direct port from the Apache Ant product.
 */
using System;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    ///<summary>
    /// This is a utility class containing static methods that match file patterns
    /// against file paths. This utility is uses the Ant project tools as a basis.
    /// 
    /// It supports the following wild cards.
    /// 
    /// ** for any directory matching
    /// *  for zero or more of any character
    /// ?  for one of any character
    /// 
    /// The patterns may be combined.
    ///</summary>
    public static class PathUtils
    {
        private static string defaultProgramDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), 
            Path.Combine("CruiseControl.NET", "Server"));

        /// <summary>
        /// Tests wheter or not a string matches against a pattern. The
        /// pattern can contain two special characters: <br/>
        /// '*' means zero or more of any characters<br/>
        /// '?' means one and only one of any character<br />
        /// All other characters are matched exactly.
        /// </summary>
        /// <param name="pattern">The pattern to match against.</param>
        /// <param name="str">the string that must match the pattern.</param>
        /// <param name="isCaseSensitive">Whether or not the match is
        /// case sensitive.</param>
        /// <returns><c>true</c> if the string matches the pattern.</returns>
        public static bool Match(string pattern, string str, bool isCaseSensitive)
        {
            /*
             * Short circuit a common *.* idiom in the Windows world to simply
             * mean everything by substituting with '*'
             */
            if (pattern.Equals("*.*"))
            {
                pattern = "*";
            }
            char[] patArr = pattern.ToCharArray();
            char[] strArr = str.ToCharArray();
            int patIdxStart = 0;
            int patIdxEnd = patArr.Length - 1;
            int strIdxStart = 0;
            int strIdxEnd = strArr.Length - 1;
            char ch;
            bool containsStar = false;
            for (int i = 0; i < patArr.Length; i++)
            {
                if (patArr[i] == '*')
                {
                    containsStar = true;
                    break;
                }
            }
            if (!containsStar)
            {
                // No '*'s, so we make a shortcut
                if (patIdxEnd != strIdxEnd)
                {
                    return false; // Pattern and string do not have the same size
                }
                for (int i = 0; i <= patIdxEnd; i++)
                {
                    ch = patArr[i];
                    if (ch != '?')
                    {
                        if (isCaseSensitive && ch != strArr[i])
                        {
                            return false; // Character mismatch
                        }
                        if (!isCaseSensitive && Char.ToUpper(ch) != Char.ToUpper(strArr[i]))
                        {
                            return false; // Character mismatch
                        }
                    }
                }
                return true; // string matches against pattern
            }
            if (patIdxEnd == 0)
            {
                return true; // Pattern contains only '*', which matches anything
            }
            // Process characters before first star
            while ((ch = patArr[patIdxStart]) != '*' && strIdxStart <= strIdxEnd)
            {
                if (ch != '?')
                {
                    if (isCaseSensitive && ch != strArr[strIdxStart])
                    {
                        return false; // Character mismatch
                    }
                    if (!isCaseSensitive && Char.ToUpper(ch) != Char.ToUpper(strArr[strIdxStart]))
                    {
                        return false; // Character mismatch
                    }
                }
                patIdxStart++;
                strIdxStart++;
            }
            if (strIdxStart > strIdxEnd)
            {
                // All characters in the string are used. Check if only '*'s are
                // left in the pattern. If so, we succeeded. Otherwise failure.
                for (int i = patIdxStart; i <= patIdxEnd; i++)
                {
                    if (patArr[i] != '*')
                    {
                        return false;
                    }
                }
                return true;
            }
            // Process characters after last star
            while ((ch = patArr[patIdxEnd]) != '*' && strIdxStart <= strIdxEnd)
            {
                if (ch != '?')
                {
                    if (isCaseSensitive && ch != strArr[strIdxEnd])
                    {
                        return false; // Character mismatch
                    }
                    if (!isCaseSensitive && Char.ToUpper(ch) != Char.ToUpper(strArr[strIdxEnd]))
                    {
                        return false; // Character mismatch
                    }
                }
                patIdxEnd--;
                strIdxEnd--;
            }
            if (strIdxStart > strIdxEnd)
            {
                // All characters in the string are used. Check if only '*'s are
                // left in the pattern. If so, we succeeded. Otherwise failure.
                for (int i = patIdxStart; i <= patIdxEnd; i++)
                {
                    if (patArr[i] != '*')
                    {
                        return false;
                    }
                }
                return true;
            }
            // process pattern between stars. padIdxStart and patIdxEnd point
            // always to a '*'.
            while (patIdxStart != patIdxEnd && strIdxStart <= strIdxEnd)
            {
                int patIdxTmp = -1;
                for (int i = patIdxStart + 1; i <= patIdxEnd; i++)
                {
                    if (patArr[i] == '*')
                    {
                        patIdxTmp = i;
                        break;
                    }
                }
                if (patIdxTmp == patIdxStart + 1)
                {
                    // Two stars next to each other, skip the first one.
                    patIdxStart++;
                    continue;
                }
                // Find the pattern between padIdxStart & padIdxTmp in str between
                // strIdxStart & strIdxEnd
                int patLength = (patIdxTmp - patIdxStart - 1);
                int strLength = (strIdxEnd - strIdxStart + 1);
                int foundIdx = -1;
                // was strLoop:
                for (int i = 0; i <= strLength - patLength; i++)
                {
                    bool mismatch = false;
                    for (int j = 0; j < patLength; j++)
                    {
                        ch = patArr[patIdxStart + j + 1];
                        if (ch != '?')
                        {
                            if (isCaseSensitive && ch != strArr[strIdxStart + i + j])
                            {
                                // was continue strLoop;
                                mismatch = true;
                                break;
                            }
                            if (!isCaseSensitive &&
                                Char.ToUpper(ch) != Char.ToUpper(strArr[strIdxStart + i + j]))
                            {
                                // was continue strLoop;
                                mismatch = true;
                                break;
                            }
                        }
                    }
                    if (!mismatch)
                    {
                        foundIdx = strIdxStart + i;
                        break;
                    }
                }
                if (foundIdx == -1)
                {
                    return false;
                }
                patIdxStart = patIdxTmp;
                strIdxStart = foundIdx + patLength;
            }
            // All characters in the string are used. Check if only '*'s are left
            // in the pattern. If so, we succeeded. Otherwise failure.
            for (int i = patIdxStart; i <= patIdxEnd; i++)
            {
                if (patArr[i] != '*')
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Tests whether or not a given path matches a given pattern. The
        /// special sequence '**' means zero or more directories.
        /// </summary>
        /// <param name="pattern">the pattern to match against.</param>
        /// <param name="str">The path to match.</param>
        /// <param name="isCaseSensitive">Wheter or not match should be performed
        /// case sisitively.</param>
        /// <returns><c>true</c>if the pattern matches.</returns>
        /// <remarks>If <paramref name="str"/> is null or an empty string, the match always fails.</remarks>
        public static bool MatchPath(string pattern, string str, bool isCaseSensitive)
        {
            if (string.IsNullOrEmpty(str))
                return false;               // A null string can't match anything
            // When str starts with a separator, pattern has to start with a
            // separator or the first pattern must be any directories.
            string[] patDirs = SplitPath(pattern);
            if (IsSeperator(Char.Parse(str.Substring(0, 1))))
            {
                if (!IsSeperator(Char.Parse(pattern.Substring(0, 1))))
                {
                    if (patDirs.Length == 0 || !patDirs[0].Equals("**"))
                    {
                        return false;
                    }
                }
            }
            string[] strDirs = SplitPath(str);
            int patIdxStart = 0;
            int patIdxEnd = patDirs.Length - 1;
            int strIdxStart = 0;
            int strIdxEnd = strDirs.Length - 1;
            // up to first '**'
            while (patIdxStart <= patIdxEnd && strIdxStart <= strIdxEnd)
            {
                string patDir = patDirs[patIdxStart];
                if (patDir.Equals("**"))
                {
                    break;
                }
                if (!Match(patDir, strDirs[strIdxStart], isCaseSensitive))
                {
                    return false;
                }
                patIdxStart++;
                strIdxStart++;
            }
            if (strIdxStart > strIdxEnd)
            {
                // string is exhausted
                for (int i = patIdxStart; i <= patIdxEnd; i++)
                {
                    if (!patDirs[i].Equals("**"))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                if (patIdxStart > patIdxEnd)
                {
                    // string not exhausted, but pattern is. Failure.
                    return false;
                }
            }
            // back to last '**'
            while (patIdxStart <= patIdxEnd && strIdxStart <= strIdxEnd)
            {
                string patDir = patDirs[patIdxEnd];
                if (patDir.Equals("**"))
                {
                    break;
                }
                if (!Match(patDir, strDirs[strIdxEnd], isCaseSensitive))
                {
                    return false;
                }
                patIdxEnd--;
                strIdxEnd--;
            }
            if (strIdxStart > strIdxEnd)
            {
                // string is exhausted
                for (int i = patIdxStart; i <= patIdxEnd; i++)
                {
                    if (!patDirs[i].Equals("**"))
                    {
                        return false;
                    }
                }
                return true;
            }
            // now check the pattern against the directories between
            // the **'s
            while (patIdxStart != patIdxEnd && strIdxStart <= strIdxEnd)
            {
                int patIdxTmp = -1;
                for (int i = patIdxStart + 1; i <= patIdxEnd; i++)
                {
                    if (patDirs[i].Equals("**"))
                    {
                        patIdxTmp = i;
                        break;
                    }
                }
                if (patIdxTmp == patIdxStart + 1)
                {
                    // '**/**' situation, so skip one
                    patIdxStart++;
                    continue;
                }
                // Find the pattern between padIdxStart & padIdxTmp in str between
                // strIdxStart & strIdxEnd
                int patLength = (patIdxTmp - patIdxStart - 1);
                int strLength = (strIdxEnd - strIdxStart + 1);
                int foundIdx = -1;
                // was strloop:
                for (int i = 0; i <= strLength - patLength; i++)
                {
                    bool mismatch = false;
                    for (int j = 0; j < patLength; j++)
                    {
                        string subPat = patDirs[patIdxStart + j + 1];
                        string subStr = strDirs[strIdxStart + i + j];
                        if (!Match(subPat, subStr, isCaseSensitive))
                        {
                            // was continue strLoop;
                            mismatch = true;
                            break;
                        }
                    }
                    if (!mismatch)
                    {
                        foundIdx = strIdxStart + i;
                        break;
                    }
                }
                if (foundIdx == -1)
                {
                    return false;
                }
                patIdxStart = patIdxTmp;
                strIdxStart = foundIdx + patLength;
            }
            for (int i = patIdxStart; i <= patIdxEnd; i++)
            {
                if (!patDirs[i].Equals("**"))
                {
                    return false;
                }
            }
            return true;
        }

        /**
 * Breaks up a target path based on known file seperators such
 * as / or \ or :
 * 
 * @param target
 * @return
 */


        /// <summary>
        /// Breaks up a target path based on know seperators such as
        /// / or \. This method also includes the platform seperator.
        /// </summary>
        /// <param name="target">This is the path to split.</param>
        /// <returns></returns>
        public static string[] SplitPath(string target)
        {
            int start = 0;
            int len = target.Length;
            int count = 0;
            char[] path = target.ToCharArray();
            for (int pos = 0; pos < len; pos++)
            {
                if (IsSeperator(path[pos]))
                {
                    if (pos != start)
                    {
                        count++;
                    }
                    start = pos + 1;
                }
            }
            if (len != start)
            {
                count++;
            }
            string[] l = new string[count];
            count = 0;
            start = 0;
            for (int pos = 0; pos < len; pos++)
            {
                if (IsSeperator(path[pos]))
                {
                    if (pos != start)
                    {
                        string tok = target.Substring(start, pos - start);
                        l[count++] = tok;
                    }
                    start = pos + 1;
                }
            }
            if (len != start)
            {
                string tok = target.Substring(start);
                l[count] = tok;
            }
            return l;
        }

        /// <summary>
        /// Determine if a character is a commonly used seperator, or the
        /// platform seperator.
        /// </summary>
        /// <param name="c">Character to examine.</param>
        /// <returns></returns>
        private static bool IsSeperator(char c)
        {
            char sep = Path.DirectorySeparatorChar;
            return (c == '\\' || c == '/' || c == sep);
        }

        /// <summary>
        /// Enstures the path is rooted.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string EnsurePathIsRooted(string path)
        {
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(
                    DefaultProgramDataFolder,
                    path);
            }
            return path;
        }

        /// <summary>
        /// The default program data folder to use.
        /// </summary>
        public static string DefaultProgramDataFolder
        {
            get { return defaultProgramDataFolder; }
            set { defaultProgramDataFolder = value; }
        }
    }
}
