using System.Text.RegularExpressions;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    /// <summary>
    /// 	
    /// </summary>
    public static class SecurityHelpers
    {
        /// <summary>
        /// Determines whether [is wild card match] [the specified wild card].	
        /// </summary>
        /// <param name="wildCard">The wild card.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool IsWildCardMatch(string wildCard, string value)
        {
            Regex wildCardRegex = new Regex(wildCard.Replace("*", "[a-zA-Z0-9_.@-]*"), RegexOptions.IgnoreCase);
            return wildCardRegex.IsMatch(value);
        }
    }
}
