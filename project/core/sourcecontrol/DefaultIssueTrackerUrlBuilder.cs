using System;
using Exortech.NetReflector;
using System.Globalization;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// It contains the url of the involved project, with the issue number as a parameter.
    /// </summary>
    /// <title>Default Issue Tracker URL Builder</title>
    /// <version>1.0</version>
    /// <example>
    /// <code>
    /// &lt;issueUrlBuilder type="defaultIssueTracker"&gt;
    /// &lt;url&gt;http://jira.public.thoughtworks.org/browse/CCNET-{0}&lt;/url&gt;
    /// &lt;/issueUrlBuilder&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <para>
    /// Whenever a checkin is done, the following logic is applied :
    /// </para>
    /// <list type="1">
    /// <item>Split the comment into a string array; separator is space</item>
    /// <item>From the first part, take all the numeric parts, starting from the end of this part</item>
    /// <item>Paste this number into the parameter</item>
    /// </list>
    /// <para>
    /// For example, with this configuration:
    /// </para>
    /// <code>
    /// &lt;issueUrlBuilder type="defaultIssueTracker"&gt;
    /// &lt;url&gt;http://jira.public.thoughtworks.org/browse/CCNET-{0}&lt;/url&gt;
    /// &lt;/issueUrlBuilder&gt;
    /// </code>
    /// <para>
    /// The following comments would be converted into the URL http://jira.public.thoughtworks.org/browse/CCNET-1223:
    /// </para>
    /// <list type="1">
    /// <item>CCNET-1223: CCnet should foresee a way to transform the comments into hyperlinks for integration with issue tracking systems</item>
    /// <item>CCNET-1223 CCnet should foresee a way to transform the comments into hyperlinks for integration with issue tracking systems</item>
    /// <item>CCNET-1223</item>
    /// <item>1223</item>
    /// </list>
    /// </remarks>
    [ReflectorType("defaultIssueTracker")]
    public class DefaultIssueTrackerUrlBuilder : IModificationUrlBuilder
    {
        private string _url;

        /// <summary>
        /// The base URL to use.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        /// <remarks>
        /// The issue number is held in \{0\}.
        /// </remarks>
        [ReflectorProperty("url")]
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        /// <summary>
        /// Setups the modification.	
        /// </summary>
        /// <param name="modifications">The modifications.</param>
        /// <remarks></remarks>
        public void SetupModification(Modification[] modifications)
        {
            if (modifications == null) throw new ArgumentNullException("modifications");

            foreach (Modification mod in modifications)
            {
                //split the comment on a space, take the first part
                //this must be the issue ID
                //from the last position of this part, go back while the characters are numeric                

                if ((mod != null) && !string.IsNullOrEmpty(mod.Comment) && mod.Comment.Length > 0)
                {
                    if (mod.Comment.IndexOf(' ') != 0)
                    {
                        string searchingComment = mod.Comment.Split(' ')[0];
                        int endPosition = searchingComment.Length - 1;
                        char currentChar = searchingComment[endPosition];
                        string result = string.Empty;
                        bool numericPartFound = false;

                        //eliminate non numeric characters at the end (ex type  [ccnet-1500])
                        while (endPosition > 0 && !char.IsNumber(currentChar))
                        {
                            endPosition--;
                            currentChar = searchingComment[endPosition];
                        }


                        //while last position is numeric add to result
                        while (endPosition >= 0 && char.IsNumber(currentChar))
                        {
                            result = result.Insert(0, currentChar.ToString(CultureInfo.CurrentCulture));
                            endPosition--;
                            if (endPosition >= 0) currentChar = searchingComment[endPosition];

                            numericPartFound = true;
                        }

                        if (numericPartFound)
                        {
                            mod.IssueUrl = string.Format(CultureInfo.CurrentCulture, _url, result);
                        }
                    }
                }
            }
        }

    }
}
