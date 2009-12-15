using System.Text.RegularExpressions;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// This will use regular expressions to convert the comment into an url.
    /// </summary>
    /// <title>Regex Issue Tracker URL Builder</title>
    /// <version>1.0</version>
    /// <example>
    /// <code>
    /// &lt;issueUrlBuilder type="regexIssueTracker"&gt;
    /// &lt;find&gt;^.*(CCNET.?\d*).*$&lt;/find&gt;
    /// &lt;replace&gt;http://jira.public.thoughtworks.org/browse/$1&lt;/replace&gt;
    /// &lt;/issueUrlBuilder&gt;  
    /// </code>
    /// </example>
    /// <remarks>
    /// <heading>Find Statement</heading>
    /// <para>For people not familier with Regex, here are some examples:</para>
    /// <list type="1">
    /// <item>Assert position at the beginning of a line (at beginning of the string or after a line break character): «<b>^</b>»</item>
    /// <item>Match any single character that is not a line break character: «<b>.</b>»</item>
    /// <item>Between zero and unlimited times, as many times as possible, giving back as needed (greedy): «<b>*</b>»</item>
    /// <item>Match the regular expression below and capture its match into backreference number 1: «<b>(CCNET.?\d)</b>»</item>
    /// <item>Match the characters "CCNET" literally: «<b>CCNET</b>»</item>
    /// <item>Between zero and one times, as many times as possible, giving back as needed (greedy): «<b>?</b>»</item>
    /// <item>Match a single digit (i.e. 0..9): «<b>\d</b>»</item>
    /// <item>Assert position at the end of a line (at the end of the string or before a line break character): «<b>$</b>»</item>
    /// <item>Check without case sensitivity: «<b>(?i)</b>»</item>
    /// </list>
    /// <heading>Replace Statement</heading>
    /// <para>
    /// To reference a backreference use «<b>$1</b>».
    /// </para>
    /// </remarks>
    [ReflectorType("regexIssueTracker")]
    public class RegExIssueTrackerUrlBuilder : IModificationUrlBuilder
    {
        private string _find;
        private string _replace;

        /// <summary>
        /// The string to find.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("find")]
        public string Find
        {
            get { return _find; }
            set { _find = value; }
        }

        /// <summary>
        /// The replacement string.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("replace")]
        public string Replace
        {
            get { return _replace; }
            set { _replace = value; }
        }

        public void SetupModification(Modification[] modifications)
        {
            foreach (Modification mod in modifications)
            {
                if ((mod != null) && !string.IsNullOrEmpty(mod.Comment) && mod.Comment.Length > 0)
                {
                    if (Regex.IsMatch(mod.Comment, _find))
                    {
                        mod.IssueUrl = Regex.Replace(mod.Comment, _find, _replace);
                    }
                }

            }
        }
    }
}
