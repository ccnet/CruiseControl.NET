using System.Text.RegularExpressions;

namespace ThoughtWorks.CruiseControl.Core.Security
{
    public static class SecurityHelpers
    {
        public static bool IsWildCardMatch(string wildCard, string value)
        {
            Regex wildCardRegex = new Regex(wildCard.Replace("*", "[a-zA-Z0-9_.@-]*"), RegexOptions.IgnoreCase);
            return wildCardRegex.IsMatch(value);
        }
    }
}
