using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    /// <summary>
    /// This issue tracker allows a combination of the other issuetrackers.
    /// </summary>
    /// <title>Multi Issue Tracker URL Builder</title>
    /// <version>1.0</version>
    /// <example>
    /// <code>
    /// &lt;issueUrlBuilder type="multiIssueTracker"&gt;
    /// &lt;issueTrackers&gt;
    /// &lt;defaultIssueTracker&gt;
    /// &lt;url&gt;http://jira.public.thoughtworks.org/browse/CCNET-{0}&lt;/url&gt;
    /// &lt;/defaultIssueTracker&gt;
    /// &lt;regexIssueTracker&gt;
    /// &lt;find&gt;^.*(CCNET-\d*).*$&lt;/find&gt;
    /// &lt;replace&gt;http://jira.public.thoughtworks.org/browse/$1&lt;/replace&gt;
    /// &lt;/regexIssueTracker&gt;
    /// &lt;/issueTrackers&gt;
    /// &lt;/issueUrlBuilder&gt;
    /// </code>
    /// </example>
    [ReflectorType("multiIssueTracker")]
    public class MultiIssueTrackerUrlBuilder : IModificationUrlBuilder
    {

        private IModificationUrlBuilder[] _issueTrackers;

        /// <summary>
        /// The issue trackers to combine.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("issueTrackers", Required = true)]
        public IModificationUrlBuilder[] IssueTrackers
        {
            get
            {
                if (_issueTrackers == null)
                    _issueTrackers = new IModificationUrlBuilder[0];

                return _issueTrackers;
            }

            set { _issueTrackers = value; }
        }

        public void SetupModification(Modification[] modifications)
        {            
            foreach (IModificationUrlBuilder modificationUrlBuilder in _issueTrackers)
            {
                modificationUrlBuilder.SetupModification(modifications);          
            }            
        }

    }
}
